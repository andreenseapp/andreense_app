using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterMeuCashbackQuery(Guid UsuarioId, int Pagina = 1, int Limite = 10)
    : IRequest<(IReadOnlyList<SaldoCashbackPorEmpresaDto> Saldos, IReadOnlyList<CashbackCredito> Historico, decimal TotalDisponivel, int TotalPaginas)>;

public class ObterMeuCashbackQueryHandler
    : IRequestHandler<ObterMeuCashbackQuery, (IReadOnlyList<SaldoCashbackPorEmpresaDto>, IReadOnlyList<CashbackCredito>, decimal, int)>
{
    private readonly ICashbackRepositorio _cashbackRepositorio;
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public ObterMeuCashbackQueryHandler(ICashbackRepositorio cashbackRepositorio, IEmpresaRepositorio empresaRepositorio)
    {
        _cashbackRepositorio = cashbackRepositorio;
        _empresaRepositorio = empresaRepositorio;
    }

    public async Task<(IReadOnlyList<SaldoCashbackPorEmpresaDto>, IReadOnlyList<CashbackCredito>, decimal, int)> Handle(
        ObterMeuCashbackQuery comando, CancellationToken cancellationToken)
    {
        var saldos = (await _cashbackRepositorio.ListarSaldosComEmpresaPorUsuarioAsync(comando.UsuarioId)).ToList();
        var resultadoPaginado = await _cashbackRepositorio.ListarHistoricoCreditosPorUsuarioAsync(comando.UsuarioId, comando.Pagina, comando.Limite);
        var creditos = resultadoPaginado.Itens.ToList();

        var totalDisponivel = saldos.Sum(s => s.SaldoDisponivel);
        var totalPaginas = resultadoPaginado.TotalPaginas;

        return (saldos, creditos, totalDisponivel, totalPaginas);
    }
}
