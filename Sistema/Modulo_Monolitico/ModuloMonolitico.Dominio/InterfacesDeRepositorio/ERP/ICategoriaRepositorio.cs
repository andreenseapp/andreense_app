namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface ICategoriaRepositorio
{
    Task<IEnumerable<CategoriaDto>> ListarPorContaAsync(Guid contaId);
}
