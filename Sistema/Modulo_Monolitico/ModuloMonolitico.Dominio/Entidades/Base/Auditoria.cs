using System;

namespace ModuloMonolitico.Dominio.Entidades
{
    public class Auditoria
    {
        public Guid Id { get; set; }
        public string Tabela { get; set; } = string.Empty;
        public string RegistroId { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public string Acao { get; set; } = string.Empty;
        public string? Detalhes { get; set; }
        public DateTime DataHora { get; set; }
    }
}


