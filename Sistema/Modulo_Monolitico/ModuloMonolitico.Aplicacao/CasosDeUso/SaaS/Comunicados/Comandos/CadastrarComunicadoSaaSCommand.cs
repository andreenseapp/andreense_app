using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarComunicadoSaaSCommand(ComunicadoSaaS Comunicado, List<OpcaoComunicadoSaaS> Opcoes) : IRequest;

public class CadastrarComunicadoSaaSCommandHandler : IRequestHandler<CadastrarComunicadoSaaSCommand>
{
    private readonly IComunicadoSaaSRepositorio _repositorio;

    public CadastrarComunicadoSaaSCommandHandler(IComunicadoSaaSRepositorio repositorio)
        => _repositorio = repositorio;

    public Task Handle(CadastrarComunicadoSaaSCommand comando, CancellationToken cancellationToken)
        => _repositorio.CadastrarComunicadoAsync(comando.Comunicado, comando.Opcoes);
}
