using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public record ObterTipoVendedorQuery(Guid Id, Guid ContaId) : IRequest<TipoVendedor?>;

public class ObterTipoVendedorQueryHandler : IRequestHandler<ObterTipoVendedorQuery, TipoVendedor?>
{
    private readonly ITipoVendedorRepositorio _tipoRepo;

    public ObterTipoVendedorQueryHandler(ITipoVendedorRepositorio tipoRepo)
    {
        _tipoRepo = tipoRepo;
    }

    public async Task<TipoVendedor?> Handle(ObterTipoVendedorQuery comando, CancellationToken cancellationToken)
        => await _tipoRepo.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}



