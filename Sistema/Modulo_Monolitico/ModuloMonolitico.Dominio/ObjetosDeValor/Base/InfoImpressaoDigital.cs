namespace ModuloMonolitico.Dominio.ObjetosDeValor.Base;

public class InfoImpressaoDigital
{
    public string AgenteUsuario { get; init; } = string.Empty;
    public string IdiomaAceito { get; init; } = string.Empty;
    public string Plataforma { get; init; } = string.Empty;
    public string EnderecoIp { get; init; } = string.Empty;
    public int CalcularScore(InfoImpressaoDigital outro)
    {
        var score = 0;
        if (!string.IsNullOrEmpty(AgenteUsuario) && AgenteUsuario == outro.AgenteUsuario)
            score += 40;
        if (!string.IsNullOrEmpty(Plataforma) && Plataforma == outro.Plataforma)
            score += 20;
        if (!string.IsNullOrEmpty(IdiomaAceito) && IdiomaAceito == outro.IdiomaAceito)
            score += 20;
        if (!string.IsNullOrEmpty(EnderecoIp) && EnderecoIp == outro.EnderecoIp)
            score += 20;

        return score;
    }
}
