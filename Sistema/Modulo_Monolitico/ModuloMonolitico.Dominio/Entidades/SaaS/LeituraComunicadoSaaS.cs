using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class LeituraComunicadoSaaS
{
    public Guid Id { get; private set; }
    public Guid ComunicadoId { get; private set; }
    public Guid ContaId { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid? OpcaoSelecionadaId { get; private set; }
    public DateTime DataLeitura { get; private set; }

    protected LeituraComunicadoSaaS() { }

    public LeituraComunicadoSaaS(Guid comunicadoId, Guid contaId, Guid usuarioId, Guid? opcaoSelecionadaId)
    {
        Id = Guid.NewGuid();
        ComunicadoId = comunicadoId;
        ContaId = contaId;
        UsuarioId = usuarioId;
        OpcaoSelecionadaId = opcaoSelecionadaId;
        DataLeitura = DateTime.UtcNow;
    }
}

