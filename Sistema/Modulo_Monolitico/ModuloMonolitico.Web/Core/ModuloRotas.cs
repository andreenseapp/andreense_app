using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ModuloMonolitico.Web.Core;

public static partial class ModuloRotas
{
    public static class Raiz
    {
        public const string Index = "/";
    }

    public static class AutenticacaoPublicas
    {
        public static readonly string[] Rotas =
        [
            Autenticacao.Login,
            Autenticacao.Cadastro,
            Autenticacao.EsqueciSenha,
            Autenticacao.RedefinirSenha,
            Autenticacao.RenovarToken,
            Autenticacao.LoginGoogle,
            Autenticacao.ValidarEmail
        ];
    }

    public static IEnumerable<string> TodasRotasAutenticacao =>
        AutenticacaoPublicas.Rotas
            .Append(Autenticacao.Sair);

    public static readonly string[] RotasPortalPublicasImoveis =
    [
        Imoveis.Index,
        Imoveis.Detalhes,
        Imoveis.Anunciante,
        Imoveis.BuscasRecentes,
        Imoveis.Favoritos,
        Imoveis.Favoritar,
        Imoveis.RegistrarVisualizacao,
        Imoveis.ContextoUsuario
    ];

    public static readonly string[] RotasPortalPublicasVeiculos =
    [
        Veiculos.Index,
        Veiculos.Detalhes,
        Veiculos.Vendedor,
        Veiculos.BuscasRecentes,
        Veiculos.Favoritos,
        Veiculos.Favoritar,
        Veiculos.ContextoUsuario
    ];

    public const string ParametroUrlRetorno = "ReturnUrl";

    public static RedirectResult Redirecionar(string rota) => new(rota);

    public static RedirectResult Redirecionar(string rota, object routeValues)
    {
        var query = QueryString.Create(
            routeValues.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetValue(routeValues) is not null)
                .Select(p => new KeyValuePair<string, string?>(
                    p.Name,
                    p.GetValue(routeValues)?.ToString())));

        return new RedirectResult(rota + query);
    }

    public static RedirectResult RedirecionarSelecaoEmpresa(string? urlRetorno = null)
    {
        if (string.IsNullOrWhiteSpace(urlRetorno))
            return Redirecionar(Empresas.Empresas_Selecionar);

        return new RedirectResult(Empresas.Empresas_Selecionar + QueryString.Create(ParametroUrlRetorno, urlRetorno));
    }

    public static JsonResult JsonSucesso(string rotaRedirect, object? dadosExtras = null)
    {
        if (dadosExtras is null)
            return new JsonResult(new { sucesso = true, urlRedirecionamento = rotaRedirect });

        return new JsonResult(new { sucesso = true, urlRedirecionamento = rotaRedirect, dados = dadosExtras });
    }

    public static JsonResult JsonSucessoMensagem(string mensagem)
        => new JsonResult(new { sucesso = true, mensagem });

    public static JsonResult JsonErro(string mensagem)
        => new JsonResult(new { sucesso = false, mensagem });
}
