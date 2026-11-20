using System;
using System.Collections.Generic;

namespace ModuloMonolitico.Aplicacao.DTOs;
public class ContaDireitosPlanoDto
{
    public string SituacaoAssinatura { get; set; } = "Trial";
    public DateTime? Vencimento { get; set; }
    public Dictionary<string, string> Recursos { get; set; } = new();
}
