namespace ModuloMonolitico.Web.Servicos;

public interface IServicoCookieAutenticacao
{
    void DefinirTokensAutenticacao(string tokenAcesso, string tokenRenovacao, bool manterConectado = false);
    void DefinirTokenAcesso(string tokenAcesso, bool manterConectado = false);
    void DefinirEmpresaAtiva(Guid empresaId, string? nome = null);
    void DefinirThrottleEmailValidacao();
    void LimparSessao();
}
