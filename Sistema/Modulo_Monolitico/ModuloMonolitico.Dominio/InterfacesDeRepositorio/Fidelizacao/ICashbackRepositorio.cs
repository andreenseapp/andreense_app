using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface ICashbackRepositorio
    {
        Task<IEnumerable<CashbackRegra>> ListarRegrasPorEmpresaAsync(Guid empresaId);
        Task<CashbackRegra?> ObterRegraPorIdAsync(Guid id);
        Task<IEnumerable<CashbackRegra>> ObterRegrasAtivasPorEmpresaAsync(Guid empresaId);
        Task CadastrarRegraAsync(CashbackRegra regra);
        Task AtualizarRegraAsync(CashbackRegra regra);
        Task AtualizarUrlImagemRegraAsync(Guid id, string? urlImagem);
        Task<CashbackSaldo?> ObterSaldoAsync(Guid empresaId, Guid usuarioId);
        Task<IEnumerable<CashbackSaldo>> ListarSaldosPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<CashbackCredito>> ListarCreditosDisponiveisAsync(Guid empresaId, Guid usuarioId);
        Task<ResultadoPaginado<CashbackCredito>> ListarHistoricoCreditosPorUsuarioAsync(Guid usuarioId, int paginaAtual = 1, int tamanhoPagina = 50);
        Task ExecutarCreditoAsync(CashbackCredito credito, decimal novoSaldo);
        Task ExecutarDebitoAsync(Guid empresaId, Guid usuarioId, decimal valorGasto, List<(Guid CreditoId, decimal ValorDebitado)> utilizacoes, decimal novoSaldo);
        Task CadastrarCodigoAsync(CashbackToken token);
        Task<CashbackToken?> ObterCodigoPorValorAsync(string token);
        Task AtualizarSituacaoCodigoAsync(Guid id, string situacao);
        Task<IEnumerable<SaldoCashbackPorEmpresaDto>> ListarSaldosComEmpresaPorUsuarioAsync(Guid usuarioId);
    }

    public record SaldoCashbackPorEmpresaDto(
        Guid EmpresaId,
        string NomeEmpresa,
        decimal SaldoDisponivel,
        DateTime DataUltimaAtualizacao
    );
}


