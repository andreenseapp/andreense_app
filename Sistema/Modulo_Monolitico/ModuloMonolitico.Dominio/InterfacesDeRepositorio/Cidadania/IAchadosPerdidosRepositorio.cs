using ModuloMonolitico.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface IAchadosPerdidosRepositorio
    {
        Task<IEnumerable<AchadoPerdido>> ListarAtivosAsync();
        Task<AchadoPerdido?> ObterPorIdAsync(Guid id);
        Task CadastrarAsync(AchadoPerdido item);
        Task AtualizarAsync(AchadoPerdido item);
        Task<IEnumerable<AchadoPerdido>> ListarPorUsuarioAsync(Guid usuarioId);
    }
}




