using System;
using System.Collections.Generic;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.Entidades.Servicos;

public class Prestador
{
    public Guid? UsuarioId { get; set; }
    public Guid? ContatoId { get; set; }
    public string NomeExibicao { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Tipo => UsuarioId.HasValue ? "usuario" : "contato";
    public List<ServicoAgendavel> Servicos { get; set; } = new();
}


