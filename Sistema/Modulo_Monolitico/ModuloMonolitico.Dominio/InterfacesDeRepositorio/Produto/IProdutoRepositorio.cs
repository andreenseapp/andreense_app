using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IProdutoRepositorio
{
    Task CadastrarProdutoComVariacoesAsync(Produto produto, IEnumerable<ProdutoVariacao> variacoes);
    Task AtualizarProdutoComVariacoesAsync(Produto produto, IEnumerable<ProdutoVariacao> variacoes);
    Task<IEnumerable<Produto>> ListarPorContaIdAsync(Guid contaId);
    Task<ResultadoPaginado<Produto>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? tipo = null, string? ordenarPor = null, string? ordenarDirecao = null);
    Task<Produto?> ObterPorIdEContaAsync(Guid id, Guid contaId);
    Task DeletarSeguroAsync(Guid id, Guid contaId);


    Task<IEnumerable<string>> ListarCategoriasAtivasAsync(Guid contaId);
    Task CadastrarCategoriaAsync(Guid contaId, string nome);

    Task<IEnumerable<string>> ListarMarcasAtivasAsync(Guid contaId);
    Task CadastrarMarcaAsync(Guid contaId, string nome);

    Task<IEnumerable<(string Sigla, string Nome)>> ListarUnidadesMedidaAtivasAsync(Guid contaId);
    Task<bool> UnidadeMedidaExisteAsync(Guid contaId, string sigla);
    Task CadastrarUnidadeMedidaAsync(Guid contaId, string sigla, string nome);
}



