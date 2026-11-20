namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IUnidadeMedidaRepositorio
{
    Task<IEnumerable<UnidadeMedidaDto>> ListarPorContaAsync(Guid contaId);
}
