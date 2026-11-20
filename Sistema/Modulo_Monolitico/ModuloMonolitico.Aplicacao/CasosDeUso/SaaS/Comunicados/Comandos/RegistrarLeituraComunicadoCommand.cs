using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class RegistrarLeituraComunicadoCommand : IRequest<ResultadoDto>
{
    public Guid ComunicadoId { get; set; }
    public Guid ContaId { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid? OpcaoSelecionadaId { get; set; }
}

public class RegistrarLeituraComunicadoCommandHandler : IRequestHandler<RegistrarLeituraComunicadoCommand, ResultadoDto>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public RegistrarLeituraComunicadoCommandHandler(IComunicadoSaaSRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoDto> Handle(RegistrarLeituraComunicadoCommand comando, CancellationToken cancellationToken)
    {
        var leitura = new LeituraComunicadoSaaS(
            comando.ComunicadoId,
            comando.ContaId,
            comando.UsuarioId,
            comando.OpcaoSelecionadaId
        );

        await _repositorio.RegistrarLeituraAsync(leitura);

        return new ResultadoDto { Sucesso = true, Mensagem = "Interação registrada com sucesso." };
    }
}



