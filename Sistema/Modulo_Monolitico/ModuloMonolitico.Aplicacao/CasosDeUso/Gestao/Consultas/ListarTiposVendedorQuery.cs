using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ListarTiposVendedorQuery(Guid ContaId) : IRequest<IEnumerable<TipoVendedor>>;

public class ListarTiposVendedorQueryHandler : IRequestHandler<ListarTiposVendedorQuery, IEnumerable<TipoVendedor>>
{
    private readonly ITipoVendedorRepositorio _tipoRepo;

    public ListarTiposVendedorQueryHandler(ITipoVendedorRepositorio tipoRepo)
    {
        _tipoRepo = tipoRepo;
    }

    public async Task<IEnumerable<TipoVendedor>> Handle(ListarTiposVendedorQuery comando, CancellationToken cancellationToken)
        => await _tipoRepo.ListarPorContaIdAsync(comando.ContaId);
}



