using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ObterVendedorQuery(Guid Id, Guid ContaId) : IRequest<Vendedor?>;

public class ObterVendedorQueryHandler : IRequestHandler<ObterVendedorQuery, Vendedor?>
{
    private readonly IVendedorRepositorio _vendedorRepo;

    public ObterVendedorQueryHandler(IVendedorRepositorio vendedorRepo)
    {
        _vendedorRepo = vendedorRepo;
    }

    public async Task<Vendedor?> Handle(ObterVendedorQuery comando, CancellationToken cancellationToken)
        => await _vendedorRepo.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}



