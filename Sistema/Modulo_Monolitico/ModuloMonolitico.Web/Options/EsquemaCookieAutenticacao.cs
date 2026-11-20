using ModuloMonolitico.Web.Core;

namespace ModuloMonolitico.Web.Options;

public static class EsquemaCookieAutenticacao
{
    public const string TokenAcesso = "auth_token_v2";
    public const string TokenRenovacao = "refresh_token";
    public const string EmpresaAtivaId = "empresa_ativa_v2";
    public const string EmpresaAtivaNome = "empresa_ativa_nome_v2";
    public const string EmailCodigoEnviado = "EmailCodigoEnviado";

    public const string PathRaiz = "/";
    public const string CaminhoTokenRenovacao = ModuloRotas.Autenticacao.RenovarToken;

    public static readonly TimeSpan DuracaoTokenAcesso = TimeSpan.FromMinutes(15);
    public static readonly TimeSpan DuracaoTokenRenovacao = TimeSpan.FromDays(7);
    public static readonly TimeSpan EmpresaAtivaDuracao = TimeSpan.FromDays(7);
    public static readonly TimeSpan EmailCodigoThrottleDuracao = TimeSpan.FromMinutes(2);
}
