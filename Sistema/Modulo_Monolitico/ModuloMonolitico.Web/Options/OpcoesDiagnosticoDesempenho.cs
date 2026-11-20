namespace ModuloMonolitico.Web.Options;

public class OpcoesDiagnosticoDesempenho
{
    public bool HabilitadoEmProducao { get; set; }
    public int LimiteEtapas { get; set; } = 80;
    public int LimiteLogLentidaoMs { get; set; } = 800;
}
