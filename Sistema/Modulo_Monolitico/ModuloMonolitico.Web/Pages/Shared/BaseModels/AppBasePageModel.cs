using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Web.Servicos;
using Microsoft.Extensions.Logging;


namespace ModuloMonolitico.Web.Pages.Shared;

[Authorize]
public abstract class AppBasePageModel : PageModel
{
  public Guid UsuarioId { get; private set; }
  public string UsuarioEmail { get; private set; } = string.Empty;
  public bool EmailValidado { get; private set; }
  public bool UsuarioEhMaster { get; private set; }

  private ILogger? _logger;
  protected ILogger Logger => 
      _logger ??= HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

  public async Task<bool> UsuarioPossuiPermissaoSaaSAsync(string chavePermissao)
  {
      if (UsuarioEhMaster) return true;
      try
      {
          var repo = HttpContext.RequestServices.GetRequiredService<IGrupoUsuarioSaaSRepositorio>();
          return await repo.UsuarioPossuiPermissaoAsync(UsuarioId, chavePermissao);
      }
      catch { return false; }
  }

  public async Task<bool> UsuarioPossuiPermissaoTenantAsync(string chavePermissao)
  {
      if (UsuarioEhMaster) return true;
      if (!EmpresaAtiva.HasValue) return false;
      try
      {
          var repo = HttpContext.RequestServices.GetRequiredService<IGrupoUsuarioRepositorio>();
          return await repo.UsuarioPossuiPermissaoAsync(UsuarioId, EmpresaAtiva.Value, chavePermissao);
      }
      catch { return false; }
  }

  public Guid ContaId { get; private set; }

  public string SaaSSituacao { get; private set; } = SituacaoAssinatura.Trial;

  public IReadOnlyDictionary<string, string> SaaSRecursos { get; private set; } = new Dictionary<string, string>();

  public bool ContaPossuiRecurso(string chaveRecurso)
  {
      if (UsuarioEhMaster) return true;
      if (SaaSSituacao == SituacaoAssinatura.Trial) return true;
      if (SaaSSituacao == SituacaoAssinatura.Bloqueado || SaaSSituacao == SituacaoAssinatura.Cancelado) return false;
      return SaaSRecursos.ContainsKey(chaveRecurso) && SaaSRecursos[chaveRecurso] != "0";
  }

  public int ContaLimiteRecurso(string chaveRecurso)
  {
      if (UsuarioEhMaster || SaaSSituacao == SituacaoAssinatura.Trial) return int.MaxValue;
      if (!SaaSRecursos.TryGetValue(chaveRecurso, out var valor)) return 0;
      return int.TryParse(valor, out var n) ? n : 0;
  }

  public bool ContaInadimplente => SaaSSituacao == SituacaoAssinatura.InadimpleteCarencia;

  public bool ContaBloqueada => SaaSSituacao == SituacaoAssinatura.Bloqueado || SaaSSituacao == SituacaoAssinatura.Cancelado;

  public IReadOnlyList<Guid> EmpresasAutorizadas { get; private set; } = new List<Guid>();

  public Guid? EmpresaAtiva { get; private set; }

  protected virtual bool ExigeEmpresaAtiva => true;

  protected virtual string? ChavePermissao => null;

  private bool _contextoResolvido;

  public string ColunasPreferidasJson { get; private set; } = string.Empty;

  public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
  {
    if (!TentarResolverContextoDoUsuario(context))
      return;

    base.OnPageHandlerExecuting(context);
  }

  public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
  {
      if (!TentarResolverContextoDoUsuario(context))
          return;

      if (!UsuarioEhMaster && ChavePermissao != null && EmpresaAtiva.HasValue)
      {
          var repo = context.HttpContext.RequestServices.GetRequiredService<IGrupoUsuarioRepositorio>();
          var temAcesso = await repo.UsuarioPossuiPermissaoAsync(UsuarioId, EmpresaAtiva.Value, ChavePermissao);
          if (!temAcesso)
          {
              Logger.LogWarning("Acesso negado (RBAC). Usuário '{Email}' tentou acessar '{Path}' sem a permissão '{Permissao}'.", UsuarioEmail, context.HttpContext.Request.Path, ChavePermissao);
              context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
              return;
          }
      }

      var resultContext = await next();

      if (context.HttpContext.Request.Method == "POST")
      {
          if (resultContext.Exception != null)
          {
              Logger.LogError(resultContext.Exception, "Erro fatal durante execução do POST em '{Path}' pelo usuário '{Email}'.", context.HttpContext.Request.Path, UsuarioEmail);
          }
          else if (!context.ModelState.IsValid)
          {
              Logger.LogWarning("Validação de formulário falhou no POST para '{Path}' pelo usuário '{Email}'.", context.HttpContext.Request.Path, UsuarioEmail);
          }
          else
          {
              Logger.LogInformation("Ação executada com sucesso (POST) em '{Path}' pelo usuário '{Email}'.", context.HttpContext.Request.Path, UsuarioEmail);
          }
      }
  }

  private bool TentarResolverContextoDoUsuario(PageHandlerExecutingContext context)
  {
    if (_contextoResolvido)
      return context.Result == null;

    var usuarioClaim = context.HttpContext.User;
    var identidade = context.HttpContext.RequestServices.GetRequiredService<IServicoIdentidadeJwt>();

    if (!identidade.TentarResolverUsuarioId(usuarioClaim, out var uId))
    {
      context.Result = ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
      return false;
    }

    UsuarioId = uId;
    UsuarioEmail = identidade.ObterEmail(usuarioClaim);
    UsuarioEhMaster = identidade.EhUsuarioMaster(UsuarioId, UsuarioEmail);

    if (bool.TryParse(usuarioClaim.FindFirst("email_validado")?.Value, out var ev))
        EmailValidado = ev;

    if (!Guid.TryParse(usuarioClaim.FindFirst("conta_id")?.Value, out var cId))
    {
      context.Result = ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);
      return false;
    }
    ContaId = cId;

    SaaSSituacao = usuarioClaim.FindFirst("saas_status")?.Value ?? SituacaoAssinatura.Trial;
    var saasJson = usuarioClaim.FindFirst("saas_features")?.Value;
    if (!string.IsNullOrEmpty(saasJson))
    {
        try
        {
            var features = JsonSerializer.Deserialize<Dictionary<string, string>>(saasJson);
            if (features != null) SaaSRecursos = features;
        }
        catch
        {
        }
    }

    var empresasJson = usuarioClaim.FindFirst("empresas")?.Value;
    if (!string.IsNullOrEmpty(empresasJson))
    {
      try
      {
        var empresasArray = JsonSerializer.Deserialize<List<Guid>>(empresasJson);
        if (empresasArray != null)
          EmpresasAutorizadas = empresasArray;
      }
      catch
      {
      }
    }

    var empresaAtivaCookie = context.HttpContext.Request.Cookies[EsquemaCookieAutenticacao.EmpresaAtivaId];
    if (!string.IsNullOrEmpty(empresaAtivaCookie) && Guid.TryParse(empresaAtivaCookie, out var empId))
    {
      if (!EmpresasAutorizadas.Contains(empId))
      {
        context.HttpContext.Response.Cookies.Delete(EsquemaCookieAutenticacao.EmpresaAtivaId);
      }
      else
      {
        EmpresaAtiva = empId;
      }
    }

    if (ExigeEmpresaAtiva && !EmpresaAtiva.HasValue)
    {
      if (EmpresasAutorizadas.Count == 1)
      {
        EmpresaAtiva = EmpresasAutorizadas[0];
        context.HttpContext.RequestServices.GetRequiredService<IServicoCookieAutenticacao>()
            .DefinirEmpresaAtiva(EmpresaAtiva.Value);
      }
      else
      {
        if (context.HttpContext.Request.Path.StartsWithSegments("/Api", StringComparison.OrdinalIgnoreCase))
        {
          context.Result = new JsonResult(ModuloRotas.JsonErro("Selecione uma empresa (workspace) para continuar.").Value) { StatusCode = 400 };
          return false;
        }
        context.Result = ModuloRotas.RedirecionarSelecaoEmpresa(
            context.HttpContext.Request.Path + context.HttpContext.Request.QueryString);
        return false;
      }
    }

    if (EmpresaAtiva.HasValue)
    {
        try
        {
            var mediator = context.HttpContext.RequestServices.GetService<MediatR.IMediator>();
            if (mediator != null)
            {
                var telaId = context.ActionDescriptor.DisplayName?.Split(' ').FirstOrDefault() ?? context.HttpContext.Request.Path.Value ?? "";
                var prefQuery = new ModuloMonolitico.Aplicacao.CasosDeUso.Geral.PreferenciaTela.ObterPreferenciaTelaQuery(EmpresaAtiva.Value, UsuarioId, telaId);
                var json = mediator.Send(prefQuery).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(json))
                {
                    ColunasPreferidasJson = json;
                }
            }
        }
        catch
        {
        }
    }

    _contextoResolvido = true;
    return true;
  }

  public async Task RegistrarAuditoriaAsync(string tabela, string registroId, string acao, object? detalhesObj = null)
  {
    try
    {
      var repo = HttpContext.RequestServices.GetRequiredService<IAuditoriaRepositorio>();
      var detalhesJson = detalhesObj != null ? JsonSerializer.Serialize(detalhesObj) : null;

      await repo.RegistrarLogAsync(new Auditoria
      {
        Tabela = tabela,
        RegistroId = registroId,
        UsuarioId = UsuarioId,
        UsuarioEmail = UsuarioEmail,
        Acao = acao,
        Detalhes = detalhesJson,
        DataHora = DateTime.UtcNow
      });
    }
    catch (Exception ex)
    {
      Logger.LogError(ex, "Falha ao registrar auditoria para {Tabela} ID {RegistroId}", tabela, registroId);
    }
  }
  
  protected void DefinirMetaTags(string titulo, string? descricao = null)
  {
      ViewData[ModuloMonolitico.Web.Core.Constantes.ViewDataKeys.Title] = titulo;
      if (!string.IsNullOrWhiteSpace(descricao))
      {
          ViewData[ModuloMonolitico.Web.Core.Constantes.ViewDataKeys.Description] = descricao;
      }
  }

  protected void NotificarSucesso(string mensagem = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso)
  {
      TempData[ModuloMonolitico.Web.Core.Constantes.TempDataKeys.MensagemSucesso] = mensagem;
  }

  protected void NotificarErro(string mensagem)
  {
      TempData[ModuloMonolitico.Web.Core.Constantes.TempDataKeys.MensagemErro] = mensagem;
  }
}







