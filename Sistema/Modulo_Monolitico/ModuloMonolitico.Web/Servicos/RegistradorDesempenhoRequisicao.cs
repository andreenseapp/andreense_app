using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Web.Options;
using Microsoft.Extensions.Options;

namespace ModuloMonolitico.Web.Servicos;

public class RegistradorDesempenhoRequisicao : IRegistradorDesempenhoRequisicao
{
    private const string ChaveHabilitado = "diag_desempenho_habilitado";
    private const string ChaveEtapas = "diag_desempenho_etapas";
    private const string ChaveContador = "diag_desempenho_contador";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OpcoesDiagnosticoDesempenho _opcoes;

    public RegistradorDesempenhoRequisicao(
        IHttpContextAccessor httpContextAccessor,
        IOptions<OpcoesDiagnosticoDesempenho> opcoes)
    {
        _httpContextAccessor = httpContextAccessor;
        _opcoes = opcoes.Value;
    }

    public bool EstaHabilitado =>
        _httpContextAccessor.HttpContext?.Items[ChaveHabilitado] as bool? == true;

    public void HabilitarParaRequisicao()
    {
        var contexto = _httpContextAccessor.HttpContext;
        if (contexto is null) return;

        contexto.Items[ChaveHabilitado] = true;
        contexto.Items[ChaveEtapas] = new List<MedicaoEtapaDesempenho>();
        contexto.Items[ChaveContador] = 0;
    }

    public void RegistrarEtapa(string nome, long duracaoMs)
    {
        if (!EstaHabilitado || string.IsNullOrWhiteSpace(nome)) return;

        var contexto = _httpContextAccessor.HttpContext;
        if (contexto?.Items[ChaveEtapas] is not List<MedicaoEtapaDesempenho> etapas) return;
        if (etapas.Count >= _opcoes.LimiteEtapas) return;

        var ordem = contexto.Items[ChaveContador] as int? ?? 0;
        contexto.Items[ChaveContador] = ordem + 1;

        etapas.Add(new MedicaoEtapaDesempenho
        {
            Nome = nome.Trim(),
            DuracaoMs = duracaoMs,
            Ordem = ordem
        });
    }

    public IReadOnlyList<MedicaoEtapaDesempenho> ObterEtapas()
    {
        if (!EstaHabilitado) return [];

        return _httpContextAccessor.HttpContext?.Items[ChaveEtapas] as List<MedicaoEtapaDesempenho> ?? [];
    }

    public long ObterDuracaoTotalMs() =>
        ObterEtapas()
            .Where(e => e.Nome.Equals("requisicao_total", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.DuracaoMs)
            .FirstOrDefault();
}
