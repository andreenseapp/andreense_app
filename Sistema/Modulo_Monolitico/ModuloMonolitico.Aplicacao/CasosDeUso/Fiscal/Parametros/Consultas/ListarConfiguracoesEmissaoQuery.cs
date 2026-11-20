using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarConfiguracoesEmissaoQuery(Guid EmpresaId) : IRequest<IEnumerable<ConfiguracaoEmissao>>;

public class ListarConfiguracoesEmissaoQueryHandler : IRequestHandler<ListarConfiguracoesEmissaoQuery, IEnumerable<ConfiguracaoEmissao>>
{
    private readonly IFiscalRepositorio _fiscalRepositorio;

    public ListarConfiguracoesEmissaoQueryHandler(IFiscalRepositorio fiscalRepositorio)
        => _fiscalRepositorio = fiscalRepositorio;

    public Task<IEnumerable<ConfiguracaoEmissao>> Handle(ListarConfiguracoesEmissaoQuery comando, CancellationToken cancellationToken)
        => _fiscalRepositorio.ListarConfiguracoesEmissaoAsync(comando.EmpresaId);
}
