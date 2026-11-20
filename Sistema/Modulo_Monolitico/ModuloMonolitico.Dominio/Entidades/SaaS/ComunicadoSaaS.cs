using System;
using System.Collections.Generic;
using System.Collections.Generic;

namespace ModuloMonolitico.Dominio.Entidades;

public class ComunicadoSaaS
{
    public Guid Id { get; private set; }
    public DateTime DataInclusao { get; private set; }

    public string Titulo { get; private set; } = string.Empty;
    public string Mensagem { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "aviso";
    public string? ModuloAlvo { get; private set; }

    public DateTime? DataExpiracao { get; private set; }
    public bool Ativo { get; private set; } = true;
    public Guid? OperadorInclusao { get; private set; }

    public IReadOnlyCollection<OpcaoComunicadoSaaS> Opcoes { get; private set; } = new List<OpcaoComunicadoSaaS>();

    protected ComunicadoSaaS() { }

    public ComunicadoSaaS(string titulo, string mensagem, string tipo, string? moduloAlvo, DateTime? dataExpiracao, Guid? operadorInclusao)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Mensagem = mensagem;
        Tipo = tipo;
        ModuloAlvo = moduloAlvo;
        DataExpiracao = dataExpiracao;
        Ativo = true;
        OperadorInclusao = operadorInclusao;
        DataInclusao = DateTime.UtcNow;
    }
}

public class ComunicadoSaaSComMetricasDto
{
    public Guid Id { get; set; }
    public DateTime DataInclusao { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = "aviso";
    public string? ModuloAlvo { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public bool Ativo { get; set; }
    public int TotalLidos { get; set; }
    public int TotalRespostas { get; set; }
}

public class InteracaoComunicadoDto
{
    public string NomeConta { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public DateTime DataLeitura { get; set; }
    public string? OpcaoEscolhida { get; set; }
}

public class ResultadoEnqueteDto
{
    public Guid OpcaoId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int Votos { get; set; }
}


