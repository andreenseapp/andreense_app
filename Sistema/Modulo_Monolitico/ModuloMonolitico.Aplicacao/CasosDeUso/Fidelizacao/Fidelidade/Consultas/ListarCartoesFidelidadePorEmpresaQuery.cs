using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarCartoesFidelidadePorEmpresaQuery(Guid EmpresaId, string? OrdenarPor = "ordem", string OrdenarDirecao = "asc", int PaginaAtual = 1, int TamanhoPagina = 30) : IRequest<ResultadoPaginado<FidelidadeCartao>>;

public class ListarCartoesFidelidadePorEmpresaQueryHandler
    : IRequestHandler<ListarCartoesFidelidadePorEmpresaQuery, ResultadoPaginado<FidelidadeCartao>>
{
    private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

    public ListarCartoesFidelidadePorEmpresaQueryHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        => _fidelidadeRepositorio = fidelidadeRepositorio;

    public Task<ResultadoPaginado<FidelidadeCartao>> Handle(ListarCartoesFidelidadePorEmpresaQuery comando, CancellationToken cancellationToken)
        => _fidelidadeRepositorio.ListarCartoesPorEmpresaAsync(comando.EmpresaId, comando.OrdenarPor, comando.OrdenarDirecao, comando.PaginaAtual, comando.TamanhoPagina);
}
