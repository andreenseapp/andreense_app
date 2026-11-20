using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface IFidelidadeRepositorio
    {
        Task<ResultadoPaginado<FidelidadeCartao>> ListarCartoesPorEmpresaAsync(Guid empresaId, string? ordenarPor = "ordem", string ordenarDirecao = "asc", int paginaAtual = 1, int tamanhoPagina = 30);
        Task<FidelidadeCartao?> ObterCartaoPorIdAsync(Guid id);
        Task<IEnumerable<FidelidadeCartao>> ObterCartoesAtivosPorEmpresaAsync(Guid empresaId);
        Task CadastrarCartaoAsync(FidelidadeCartao cartao);
        Task AtualizarCartaoAsync(FidelidadeCartao cartao);
        Task AtualizarUrlImagemCartaoAsync(Guid id, string? urlImagem);
        Task<FidelidadeSaldo?> ObterSaldoAsync(Guid cartaoId, Guid usuarioId);
        Task<FidelidadeSaldo?> ObterSaldoPorIdAsync(Guid saldoId);
        Task<IEnumerable<FidelidadeSaldo>> ListarSaldosPorUsuarioAsync(Guid usuarioId);
        Task ExecutarCarimboAsync(Guid cartaoId, Guid usuarioId, FidelidadeCarimbo carimbo, bool completouCartao);
        Task ExecutarResgateAsync(Guid saldoId);
        Task CadastrarCodigoAsync(FidelidadeToken token);
        Task<FidelidadeToken?> ObterCodigoPorValorAsync(string token);
        Task AtualizarSituacaoCodigoAsync(Guid id, string situacao);
        Task RegistrarLogAsync(FidelidadeLog log);
        Task<IEnumerable<FidelidadeLog>> ListarLogsPorSaldoAsync(Guid saldoId);
        Task<IEnumerable<FidelidadeCartaoComProgressoDto>> ListarCartoesComProgressoPorUsuarioAsync(Guid usuarioId);
    }

    public record FidelidadeCartaoComProgressoDto(
        FidelidadeSaldo Saldo,
        FidelidadeCartao Cartao,
        int PorcentagemProgresso
    );
}


