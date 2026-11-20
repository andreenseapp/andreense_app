using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Web.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class LimiteAcessoAttribute(string prefixo = "rate-limit") : Attribute, IAsyncPageFilter
{
    public string Prefixo { get; } = prefixo;
    public string? CampoUsuario { get; set; }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var servicoLimiteAcesso = context.HttpContext.RequestServices.GetRequiredService<IServicoLimiteAcesso>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
        var chave = ConstruirChave(context);

        if (servicoLimiteAcesso.EstaBloqueado(chave))
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP Desconhecido";
            var navegador = context.HttpContext.Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(navegador)) navegador = "Navegador Desconhecido";

            logger.LogWarning("Acesso bloqueado por excesso de tentativas. Chave: {Chave}. IP: {Ip}, Navegador: {Navegador}", chave, ip, navegador);

            context.Result = new JsonResult(new { success = false, message = "Muitas tentativas falharam. Tente novamente mais tarde." })
            {
                StatusCode = 429
            };
            return;
        }

        var contextoResultado = await next();

        if (contextoResultado.Result is JsonResult resultadoJson)
        {
            var tentativaFalhou = FoiTentativaFalha(resultadoJson.Value);
            if (tentativaFalhou is true)
            {
                servicoLimiteAcesso.RegistrarTentativaFalha(chave);
            }
            else if (tentativaFalhou is false)
            {
                servicoLimiteAcesso.LimparTentativas(chave);
            }
        }
        else if (contextoResultado.Result is PageResult)
        {
            if (!context.ModelState.IsValid)
            {
                servicoLimiteAcesso.RegistrarTentativaFalha(chave);
            }
            else
            {
                var model = context.HandlerInstance;
                var propMsg = model.GetType().GetProperty("MensagemErro", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propMsg != null && propMsg.GetValue(model) is string msg && !string.IsNullOrEmpty(msg))
                {
                    servicoLimiteAcesso.RegistrarTentativaFalha(chave);
                }
            }
        }
        else if (contextoResultado.Result is LocalRedirectResult || contextoResultado.Result is RedirectResult || contextoResultado.Result is RedirectToPageResult)
        {
            servicoLimiteAcesso.LimparTentativas(chave);
        }
    }

    private string ConstruirChave(PageHandlerExecutingContext context)
    {
        var ipRemoto = context.HttpContext.Connection.RemoteIpAddress;
        var enderecoIp = "desconhecido";

        if (ipRemoto != null)
        {
            if (ipRemoto.IsIPv4MappedToIPv6)
            {
                enderecoIp = ipRemoto.MapToIPv4().ToString();
            }
            else if (System.Net.IPAddress.IPv6Loopback.Equals(ipRemoto))
            {
                enderecoIp = "127.0.0.1";
            }
            else
            {
                enderecoIp = ipRemoto.ToString();
            }
        }

        var valorUsuario = "desconhecido";

        if (!string.IsNullOrWhiteSpace(CampoUsuario) && context.HandlerInstance is PageModel model)
        {
            var partes = CampoUsuario.Split('.');
            object? objetoAtual = model;
            foreach (var parte in partes)
            {
                if (objetoAtual == null) break;
                var propriedade = objetoAtual.GetType().GetProperty(parte, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
                objetoAtual = propriedade?.GetValue(objetoAtual);
            }
            var valorBruto = objetoAtual?.ToString();
            if (!string.IsNullOrWhiteSpace(valorBruto))
            {
                valorUsuario = valorBruto.Trim().ToLowerInvariant();
            }
        }

        return $"{Prefixo}|{enderecoIp}|{valorUsuario}";
    }

    private static bool? FoiTentativaFalha(object? valorResultado)
    {
        if (valorResultado == null)
            return null;

        var tipoValor = valorResultado.GetType();

        var propriedadeSuccess = tipoValor.GetProperty("success", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propriedadeSuccess?.PropertyType == typeof(bool))
            return (bool?)propriedadeSuccess.GetValue(valorResultado) == false;

        var propriedadeSucesso = tipoValor.GetProperty("sucesso", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propriedadeSucesso?.PropertyType == typeof(bool))
            return (bool?)propriedadeSucesso.GetValue(valorResultado) == false;

        return null;
    }
}
