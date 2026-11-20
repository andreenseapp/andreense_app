using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public bool EmailValidado { get; set; }
    public string? UrlImagem { get; set; }
}

