using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IEstoqueRepositorio
{

    Task<Guid> CadastrarAreaAsync(AreaEstoque area);
    Task AtualizarAreaAsync(AreaEstoque area);
    Task DeletarFisicoAreaAsync(Guid id, Guid contaId);
    Task<IEnumerable<AreaEstoque>> ListarAreasPorContaEEmpresaAsync(Guid contaId, Guid empresaId);


    Task<Guid> CadastrarLocalAsync(LocalEstoque local);
    Task AtualizarLocalAsync(LocalEstoque local);
    Task DeletarFisicoLocalAsync(Guid id, Guid contaId);
    Task<IEnumerable<LocalEstoque>> ListarLocaisPorAreaAsync(Guid areaEstoqueId, Guid contaId);
    Task<IEnumerable<LocalEstoque>> ListarLocaisPorContaEEmpresaAsync(Guid contaId, Guid empresaId);


    Task<EstoqueSaldo> CadastrarSaldoAsync(EstoqueSaldo saldo, EstoqueMovimentacao movimentacao);
    Task<EstoqueSaldo> RegistrarSaidaAsync(Guid saldoId, decimal quantidade, string? motivo, Guid contaId, Guid? usuarioId);
    Task<IEnumerable<EstoqueSaldo>> ListarSaldosPorVariacaoAsync(Guid produtoVariacaoId, Guid contaId);
    Task<IEnumerable<EstoqueSaldo>> ListarSaldosPorContaEEmpresaAsync(Guid contaId, Guid empresaId, string? termoBusca = null);
    Task<EstoqueSaldo?> ObterSaldoPorIdAsync(Guid id, Guid contaId);


    Task<IEnumerable<EstoqueMovimentacao>> ListarMovimentacoesPorSaldoAsync(Guid saldoId, Guid contaId);
}


