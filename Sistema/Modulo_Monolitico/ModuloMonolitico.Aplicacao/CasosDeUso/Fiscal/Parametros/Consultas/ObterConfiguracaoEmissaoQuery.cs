using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterConfiguracaoEmissaoQuery(Guid EmpresaId, Guid Id) : IRequest<ConfiguracaoEmissao?>;

public class ObterConfiguracaoEmissaoQueryHandler : IRequestHandler<ObterConfiguracaoEmissaoQuery, ConfiguracaoEmissao?>
{
    private readonly IFiscalRepositorio _fiscalRepositorio;

    public ObterConfiguracaoEmissaoQueryHandler(IFiscalRepositorio fiscalRepositorio)
        => _fiscalRepositorio = fiscalRepositorio;

    public Task<ConfiguracaoEmissao?> Handle(ObterConfiguracaoEmissaoQuery comando, CancellationToken cancellationToken)
        => _fiscalRepositorio.ObterConfiguracaoEmissaoPorIdAsync(comando.EmpresaId, comando.Id);
}
