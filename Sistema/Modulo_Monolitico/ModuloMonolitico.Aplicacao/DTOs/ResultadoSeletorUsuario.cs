using System;

namespace ModuloMonolitico.Aplicacao.DTOs;
public class ResultadoSeletorUsuario
{
    public Guid Id { get; set; }
    public string Fonte { get; set; } = "contato";
    public string TipoPessoa { get; set; } = "fisica";
    public string NomeExibicao { get; set; } = string.Empty;
    public string? Documento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool EnviaEmailAgendamento { get; set; } = true;
    public Guid? ContaVinculadaId { get; set; }
}
