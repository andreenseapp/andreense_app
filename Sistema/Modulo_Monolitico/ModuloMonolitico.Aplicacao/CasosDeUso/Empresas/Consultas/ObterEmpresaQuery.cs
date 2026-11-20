using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterEmpresaQuery(Guid Id, Guid ContaId) : IRequest<Empresa?>;

public class ObterEmpresaQueryHandler : IRequestHandler<ObterEmpresaQuery, Empresa?>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public ObterEmpresaQueryHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public Task<Empresa?> Handle(ObterEmpresaQuery comando, CancellationToken cancellationToken)
        => _empresaRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}
