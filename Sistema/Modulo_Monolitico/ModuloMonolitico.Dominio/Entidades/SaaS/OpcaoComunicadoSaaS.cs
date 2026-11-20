using System;

namespace ModuloMonolitico.Dominio.Entidades;

public class OpcaoComunicadoSaaS
{
    public Guid Id { get; private set; }
    public Guid ComunicadoId { get; private set; }
    public string Texto { get; private set; } = string.Empty;
    public int Ordem { get; private set; }

    protected OpcaoComunicadoSaaS() { }

    public OpcaoComunicadoSaaS(Guid comunicadoId, string texto, int ordem)
    {
        Id = Guid.NewGuid();
        ComunicadoId = comunicadoId;
        Texto = texto;
        Ordem = ordem;
    }
}

