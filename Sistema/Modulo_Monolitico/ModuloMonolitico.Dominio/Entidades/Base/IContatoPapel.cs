using System;

namespace ModuloMonolitico.Dominio.Entidades;

public interface IContatoPapel
{
    Guid ContatoId { get; set; }
    Guid ContaId { get; set; }
}

