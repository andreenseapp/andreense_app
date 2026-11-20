using ModuloMonolitico.Web.Core.Constantes;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Web.Pages.Shared;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.CasosDeUso;
namespace ModuloMonolitico.Web.Pages;

public class MeuPerfilModel : AppBasePageModel
{
    [TempData] public string? MensagemErro { get; set; }
    [TempData] public string? MensagemSucesso { get; set; }



  protected override bool ExigeEmpresaAtiva => false;

  private readonly IMediator _mediator;
  private readonly ILogger<MeuPerfilModel> _logger;

  public MeuPerfilModel(IMediator mediator, ILogger<MeuPerfilModel> logger)
  {
    _mediator = mediator;
    _logger = logger;
  }

  [BindProperty]
  public PerfilFormulario Formulario { get; set; } = new();

  [BindProperty]
  public AlterarSenhaFormulario FormularioSenha { get; set; } = new();

  public string? LetraAvatar { get; private set; }
  public string? EmailAtual { get; private set; }
  public string? NomeAtual { get; private set; }
  public string? UrlFoto { get; private set; }
  public string PortalOrigem { get; private set; } = string.Empty;

  public IEnumerable<UsuarioProfissao> Profissoes { get; private set; } = new List<UsuarioProfissao>();

  
  [BindProperty]
  [Display(Name = "Profissão ou habilidade")]
  [MaxLength(100)]
  public string? NovaProfissao { get; set; }



  public async Task<IActionResult> OnGetAsync()
  {
    var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));
    if (usuario is null) return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

    var host = Request.Host.Value.ToLower();
    var hostParts = host.Split('.');
    
    if (hostParts.Length > 0 && hostParts[0] != "www" && hostParts[0] != "localhost" && hostParts[0] != "andreense" && hostParts[0] != "127")
    {
        PortalOrigem = hostParts[0];
    }
    else if (Request.Cookies.TryGetValue("portal_origem", out var cookieOrigem))
    {
        if (cookieOrigem.Contains("/Imoveis", StringComparison.OrdinalIgnoreCase))
            PortalOrigem = "imoveis";
        else if (cookieOrigem.Contains("/Curriculos", StringComparison.OrdinalIgnoreCase))
            PortalOrigem = "curriculos";
        else if (cookieOrigem.Contains("/Contextos/Cidadania", StringComparison.OrdinalIgnoreCase))
            PortalOrigem = "cidade";
    }

    Formulario.Nome = usuario.Nome;
    Formulario.Email = usuario.Email;
    NomeAtual = usuario.Nome;
    EmailAtual = usuario.Email;
    UrlFoto = usuario.UrlImagem;
    LetraAvatar = usuario.Nome.Length > 0 ? usuario.Nome.Substring(0, 1).ToUpper() : "U";

    Profissoes = await _mediator.Send(new ListarProfissoesUsuarioQuery(usuario.Id));

    return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));
    if (usuario is null) return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

    foreach (var key in ModelState.Keys)
    {
      if (key.StartsWith("FormularioSenha."))
      {
        ModelState.ClearValidationState(key);
        ModelState.MarkFieldValid(key);
      }
    }

    if (!ModelState.IsValid)
    {
      _logger.LogWarning("Tentativa de atualização de perfil do usuário '{UsuarioId}' bloqueada devido a erros de validação.", UsuarioId);
      AtualizarDadosExibicao(usuario);
      Profissoes = await _mediator.Send(new ListarProfissoesUsuarioQuery(usuario.Id));
      return Page();
    }

    await _mediator.Send(new AtualizarNomeUsuarioCommand(UsuarioId, Formulario.Nome));
    _logger.LogInformation("Perfil do usuário '{UsuarioId}' atualizado com sucesso. Novo nome: '{Nome}'.", UsuarioId, Formulario.Nome);

    MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
    return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil);
  }

  public async Task<IActionResult> OnPostAlterarSenhaAsync()
  {
    var usuario = await _mediator.Send(new ObterUsuarioPorIdQuery(UsuarioId));
    if (usuario is null) return ModuloRotas.Redirecionar(ModuloRotas.Autenticacao.Login);

    foreach (var key in ModelState.Keys)
    {
      if (key.StartsWith("Formulario."))
      {
        ModelState.ClearValidationState(key);
        ModelState.MarkFieldValid(key);
      }
    }

    if (!ModelState.IsValid)
    {
      _logger.LogWarning("Tentativa de alteração de senha do usuário '{UsuarioId}' bloqueada devido a erros de validação.", UsuarioId);
      AtualizarDadosExibicao(usuario);
      Profissoes = await _mediator.Send(new ListarProfissoesUsuarioQuery(usuario.Id));
      ViewData[ViewDataKeys.ExibirModalSenha] = true;
      return Page();
    }

    var (sucesso, erro) = await _mediator.Send(new AtualizarSenhaUsuarioCommand(UsuarioId, FormularioSenha.SenhaAtual, FormularioSenha.NovaSenha));
    if (!sucesso)
    {
      _logger.LogWarning("Falha ao alterar senha do usuário '{UsuarioId}': {Erro}", UsuarioId, erro);
      MensagemErro = erro;
      AtualizarDadosExibicao(usuario);
      Profissoes = await _mediator.Send(new ListarProfissoesUsuarioQuery(usuario.Id));
      ViewData[ViewDataKeys.ExibirModalSenha] = true;
      return Page();
    }

    _logger.LogInformation("Senha do usuário '{UsuarioId}' alterada com sucesso.", UsuarioId);
    MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
    return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil);
  }

  public async Task<IActionResult> OnPostCadastrarProfissaoAsync()
  {
    if (string.IsNullOrWhiteSpace(NovaProfissao))
    {
      _logger.LogWarning("Tentativa de adicionar profissão vazia para o usuário '{UsuarioId}' rejeitada.", UsuarioId);
      MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Perfil.ProfissaoVazia;
      return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil + "#carreira");
    }

    await _mediator.Send(new CadastrarProfissaoUsuarioCommand(UsuarioId, NovaProfissao.Trim()));
    _logger.LogInformation("Profissão '{NovaProfissao}' adicionada com sucesso para o usuário '{UsuarioId}'.", NovaProfissao, UsuarioId);
    MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
    return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil + "#carreira");
  }

  public async Task<IActionResult> OnPostDeletarFisicoProfissaoAsync(Guid profissaoId)
  {
    await _mediator.Send(new DeletarFisicoProfissaoUsuarioCommand(profissaoId, UsuarioId));
    _logger.LogInformation("Profissão '{ProfissaoId}' removida com sucesso para o usuário '{UsuarioId}'.", profissaoId, UsuarioId);
    MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
    return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil + "#carreira");
  }



  public async Task<IActionResult> OnPostEnviarFotoAsync(IFormFile arquivo)
  {
    if (arquivo == null || arquivo.Length == 0)
    {
      _logger.LogWarning("Tentativa de upload de foto vazia ou nula pelo usuário '{UsuarioId}'.", UsuarioId);
      MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Perfil.ArquivoInvalido;
      return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil);
    }

    using var stream = arquivo.OpenReadStream();
    var command = new ModuloMonolitico.Aplicacao.CasosDeUso.AtualizarFotoPerfilCommand
    {
      UsuarioId = UsuarioId,
      ArquivoStream = stream,
      NomeArquivo = arquivo.FileName,
      TipoConteudo = arquivo.ContentType
    };

    var resultado = await _mediator.Send(command);

    if (resultado.Sucesso)
    {
      _logger.LogInformation("Foto de perfil do usuário '{UsuarioId}' atualizada com sucesso.", UsuarioId);
      MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;
    }
    else
    {
      _logger.LogWarning("Falha ao atualizar foto de perfil do usuário '{UsuarioId}': {Mensagem}", UsuarioId, resultado.Mensagem);
      MensagemErro = resultado.Mensagem;
    }

    return ModuloRotas.Redirecionar(ModuloRotas.Perfil.MeuPerfil);
  }

  private void AtualizarDadosExibicao(Usuario usuario)
  {
    NomeAtual = usuario.Nome;
    EmailAtual = usuario.Email;
    UrlFoto = usuario.UrlImagem;
    LetraAvatar = usuario.Nome.Length > 0 ? usuario.Nome.Substring(0, 1).ToUpper() : "U";
  }

  public class PerfilFormulario
  {
    [Display(Name = "Nome completo")]
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "E-mail")]
    public string? Email { get; set; }
  }

  public class AlterarSenhaFormulario
  {
    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
      ErrorMessage = "A senha deve ter no mínimo 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório.")]
    [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nova senha")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
  }
}



