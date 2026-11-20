using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IVendedorRepositorio
{
    Task CadastrarAsync(Vendedor vendedor);
    Task AtualizarAsync(Vendedor vendedor);
    Task<IEnumerable<Vendedor>> ListarPorContaIdAsync(Guid contaId);
    Task<ResultadoPaginado<Vendedor>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? ordenarPor = null, string? ordenarDirecao = null);
    Task<Vendedor?> ObterPorIdEContaAsync(Guid id, Guid contaId);
    Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId);
}

public interface ITipoVendedorRepositorio
{
    Task CadastrarAsync(TipoVendedor tipoVendedor);
    Task AtualizarAsync(TipoVendedor tipoVendedor);
    Task<IEnumerable<TipoVendedor>> ListarPorContaIdAsync(Guid contaId);
    Task<TipoVendedor?> ObterPorIdEContaAsync(Guid id, Guid contaId);
}



