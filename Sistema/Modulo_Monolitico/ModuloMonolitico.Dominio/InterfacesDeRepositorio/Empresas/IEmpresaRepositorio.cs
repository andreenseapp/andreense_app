using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IEmpresaRepositorio
{
    Task<IEnumerable<Empresa>> ListarPorContaIdAsync(Guid contaId);
    Task<IEnumerable<Empresa>> ListarTodasAcessiveisAoUsuarioAsync(Guid usuarioId, Guid contaPrincipalId);
    Task<Empresa?> ObterPorIdAsync(Guid id);
    Task<Empresa?> ObterPorIdEContaAsync(Guid id, Guid contaId);
    Task<Empresa?> ObterPorContaIdAsync(Guid contaId);
    Task CadastrarAsync(Empresa empresa);
    Task AtualizarAsync(Empresa empresa);
    Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId);
}
