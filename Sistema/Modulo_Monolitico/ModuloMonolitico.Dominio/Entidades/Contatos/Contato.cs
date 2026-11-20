using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class Contato
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public string TipoPessoa { get; set; } = "fisica";
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool EnviaEmailAgendamento { get; set; } = true;
    public string? NomeCompleto { get; set; }
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Genero { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? InscricaoMunicipal { get; set; }
    public string? Crt { get; set; }
    public Guid? Vendedor1Id { get; set; }
    public Guid? Vendedor2Id { get; set; }
    public List<Endereco> Enderecos { get; set; } = new();
    public Guid? EnderecoEntregaId { get; set; }
    public Guid? EnderecoFaturamentoId { get; set; }
    public Guid? EnderecoFiscalId { get; set; }
    public Guid? EnderecoCobrancaId { get; set; }
    public Guid? EnderecoRetiradaId { get; set; }
}

