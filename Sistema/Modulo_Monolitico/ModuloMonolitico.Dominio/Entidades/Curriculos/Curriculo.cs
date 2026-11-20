using System;

namespace ModuloMonolitico.Dominio.Entidades.Curriculos;

public class Curriculo
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public string CargoDesejado { get; set; } = string.Empty;
    public string AreaAtuacao { get; set; } = string.Empty;
    public string Nivel { get; set; } = "pleno";
    public string Modalidade { get; set; } = "clt";
    public decimal? PretensaoSalarial { get; set; }
    public string Disponibilidade { get; set; } = "imediata";
    public bool AceitaViagem { get; set; }
    public bool AceitaMudanca { get; set; }
    public string? ResumoProfissional { get; set; }
    public string? CnhCategorias { get; set; }
    public bool Pcd { get; set; }
    public string? TipoDeficiencia { get; set; }
    public bool Visivel { get; set; } = true;
    public string Status { get; set; } = "ativo";
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}
