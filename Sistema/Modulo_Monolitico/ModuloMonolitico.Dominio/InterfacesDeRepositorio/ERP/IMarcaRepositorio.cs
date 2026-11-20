namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IMarcaRepositorio
{
    Task<IEnumerable<MarcaDto>> ListarPorContaAsync(Guid contaId);
}
