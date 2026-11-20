using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class EmpresaCnaeSecundario
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string Cnae { get; set; } = string.Empty;
}

