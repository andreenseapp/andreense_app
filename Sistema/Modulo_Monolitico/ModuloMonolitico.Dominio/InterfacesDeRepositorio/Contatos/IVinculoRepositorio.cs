using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IVinculoRepositorio
{
    Task CadastrarSolicitacaoAsync(SolicitacaoVinculo solicitacao);
    Task<SolicitacaoVinculo?> ObterSolicitacaoPorIdAsync(Guid id);
    Task<IEnumerable<SolicitacaoVinculo>> ListarSolicitacoesEnviadasAsync(Guid contaId);
    Task<IEnumerable<SolicitacaoVinculo>> ListarSolicitacoesRecebidasAsync(Guid contaId);
    Task AtualizarSituacaoSolicitacaoAsync(Guid id, string novaSituacao, Guid? contaAlvoId);
    Task CadastrarVinculoAsync(VinculoConta vinculo);
    Task<IEnumerable<VinculoConta>> ListarVinculosAtivosPorContaAsync(Guid contaId);
    Task RevogarVinculoAsync(Guid vinculoId, Guid contaId);
    Task<Guid?> ObterContaIdPorEmailAsync(string email);
}
