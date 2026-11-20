using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class FidelidadeSaldo
{
    public Guid Id { get; set; }
    public Guid CartaoId { get; set; }
    public Guid UsuarioId { get; set; }
    public int PontosAcumulados { get; set; }
    public int RecompensasAcumuladas { get; set; }
    public string Situacao { get; set; } = "ativo";
    public DateTime? DataResgateUltima { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
}

