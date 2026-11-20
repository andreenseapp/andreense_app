using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IContextoPrecoRepositorio
{
    Task<Guid> CadastrarAsync(ContextoPreco contexto);
    Task AtualizarAsync(ContextoPreco contexto);
    Task DeletarSeguroAsync(Guid id, Guid contaId);
    Task<ContextoPreco?> ObterPorIdEContaAsync(Guid id, Guid contaId);
    Task<IEnumerable<ContextoPreco>> ListarPorContaEEmpresaAsync(Guid contaId, Guid empresaId);


    Task DefinirPrecoAsync(Preco preco);
    Task<IEnumerable<Preco>> ListarPrecosPorVariacaoAsync(Guid produtoVariacaoId, Guid contaId);
    Task<IEnumerable<Preco>> ListarPrecosPorContextoAsync(Guid contextoPrecoId, Guid contaId);
}



