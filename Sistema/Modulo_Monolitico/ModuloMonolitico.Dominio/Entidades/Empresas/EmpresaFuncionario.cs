using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EmpresaFuncionario : IContatoPapel
{
    public Guid ContatoId { get; set; }
    public Guid ContaId { get; set; }
    public string? Observacao { get; set; }
}

