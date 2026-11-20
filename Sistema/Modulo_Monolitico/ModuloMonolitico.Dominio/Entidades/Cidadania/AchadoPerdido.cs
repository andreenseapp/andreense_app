using System;

namespace ModuloMonolitico.Dominio.Entidades
{
    public class AchadoPerdido
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Tipo { get; set; } = "perdido";
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? LocalVisto { get; set; }
        public DateTime? DataEvento { get; set; }
        public string? ContatoInfo { get; set; }
        public bool EsconderContato { get; set; } = true;
        public string? UrlImagem { get; set; }
        public string Situacao { get; set; } = "ativo";
        public DateTime DataInclusao { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }
    }
}


