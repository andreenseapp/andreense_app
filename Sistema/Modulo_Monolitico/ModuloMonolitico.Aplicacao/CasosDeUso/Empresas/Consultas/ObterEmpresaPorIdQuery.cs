using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterEmpresaPorIdQuery(Guid Id) : IRequest<Empresa?>;

public class ObterEmpresaPorIdQueryHandler : IRequestHandler<ObterEmpresaPorIdQuery, Empresa?>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public ObterEmpresaPorIdQueryHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public Task<Empresa?> Handle(ObterEmpresaPorIdQuery comando, CancellationToken cancellationToken)
        => _empresaRepositorio.ObterPorIdAsync(comando.Id);
}
