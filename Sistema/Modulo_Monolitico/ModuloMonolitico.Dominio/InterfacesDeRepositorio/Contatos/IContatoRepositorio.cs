using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IContatoRepositorio
{
    Task CadastrarContatoComPapeisAsync(Contato pessoaBase, IEnumerable<IContatoPapel> papeis);
    Task<IEnumerable<Contato>> ListarPorContaIdAsync(Guid contaId, string? filtroNome = null, string? filtroTipo = null);
    Task<ResultadoPaginado<Contato>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? tipo = null, DateTime? desde = null, string? ordenarPor = null, string? ordenarDirecao = null, string? papel = null);
    Task<Contato?> ObterPorIdEContaAsync(Guid id, Guid contaId);
    Task AtualizarContatoComPapeisAsync(Contato pessoaBase, IEnumerable<IContatoPapel> papeis);
    Task DeletarSeguroAsync(Guid id, Guid contaId);
    Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId);

    Task<EmpresaCliente?> ObterPapelClienteAsync(Guid pessoaId, Guid empresaId);
    Task<EmpresaFornecedor?> ObterPapelFornecedorAsync(Guid pessoaId, Guid empresaId);
    Task<EmpresaTransportador?> ObterPapelTransportadorAsync(Guid pessoaId, Guid empresaId);
    Task<EmpresaLead?> ObterPapelLeadAsync(Guid pessoaId, Guid empresaId);
    Task<EmpresaProspect?> ObterPapelProspectAsync(Guid pessoaId, Guid empresaId);
    Task<EmpresaFuncionario?> ObterPapelFuncionarioAsync(Guid pessoaId, Guid empresaId);
}



