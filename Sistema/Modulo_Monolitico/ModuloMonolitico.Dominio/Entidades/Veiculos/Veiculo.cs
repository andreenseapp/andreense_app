using System;

namespace ModuloMonolitico.Dominio.Entidades.Veiculos;

public class Veiculo
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string? Versao { get; set; }
    public int AnoFabricacao { get; set; }
    public int AnoModelo { get; set; }
    public int Quilometragem { get; set; }
    public string Combustivel { get; set; } = string.Empty;
    public string Cambio { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public string? Placa { get; set; }
    public string TipoVeiculo { get; set; } = "carro";
    public short? NumeroPortas { get; set; }
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
