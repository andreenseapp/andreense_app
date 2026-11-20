using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public interface IRegistradorDesempenhoRequisicao
{
    bool EstaHabilitado { get; }
    void HabilitarParaRequisicao();
    void RegistrarEtapa(string nome, long duracaoMs);
    IReadOnlyList<MedicaoEtapaDesempenho> ObterEtapas();
    long ObterDuracaoTotalMs();
}
