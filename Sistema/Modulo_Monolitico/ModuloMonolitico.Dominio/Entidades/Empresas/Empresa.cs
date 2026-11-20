using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class Empresa
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? RazaoSocial { get; set; }
    public string? Cnpj { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? Segmento { get; set; }
    public string? CnaePrincipal { get; set; }
    public List<EmpresaCnaeSecundario> CnaesSecundarios { get; set; } = new();
    public string? Cep { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    public DateTime DataUltimaAtualizacao { get; set; } = DateTime.UtcNow;
}

