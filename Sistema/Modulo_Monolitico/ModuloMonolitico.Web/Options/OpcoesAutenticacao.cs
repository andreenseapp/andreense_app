namespace ModuloMonolitico.Web.Options;

public class OpcoesAutenticacao
{
    public const string Secao = "Auth";

    public int TokenAcessoMinutos { get; set; } = 15;
    public int TokenRenovacaoDias { get; set; } = 7;
    public int SessaoPersistenteHoras { get; set; } = 8;
    public bool ExpiracaoDeslizante { get; set; } = true;
    public int IdentidadeCacheSegundos { get; set; } = 300;

    public TimeSpan DuracaoTokenAcesso => TimeSpan.FromMinutes(TokenAcessoMinutos);
    public TimeSpan DuracaoTokenRenovacao => TimeSpan.FromDays(TokenRenovacaoDias);
    public TimeSpan SessaoPersistenteDuracao => TimeSpan.FromHours(SessaoPersistenteHoras);
    public TimeSpan IdentidadeCacheDuracao => TimeSpan.FromSeconds(IdentidadeCacheSegundos);
}
