using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarVendedoresQuery(Guid ContaId) : IRequest<IEnumerable<Vendedor>>;

public class ListarVendedoresQueryHandler : IRequestHandler<ListarVendedoresQuery, IEnumerable<Vendedor>>
{
    private readonly IVendedorRepositorio _vendedorRepo;

    public ListarVendedoresQueryHandler(IVendedorRepositorio vendedorRepo)
    {
        _vendedorRepo = vendedorRepo;
    }

    public async Task<IEnumerable<Vendedor>> Handle(ListarVendedoresQuery comando, CancellationToken cancellationToken)
        => await _vendedorRepo.ListarPorContaIdAsync(comando.ContaId);
}



