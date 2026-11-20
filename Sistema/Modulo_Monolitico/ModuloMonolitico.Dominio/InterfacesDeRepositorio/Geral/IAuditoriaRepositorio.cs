using ModuloMonolitico.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface IAuditoriaRepositorio
    {
        Task RegistrarLogAsync(Auditoria log);
        Task<IEnumerable<Auditoria>> ListarLogsPorRegistroAsync(string tabela, string registroId);
    }
}


