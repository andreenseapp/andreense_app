using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IAnuncioVisualizadoRepositorio
{
    Task<AnuncioVisualizado?> ObterAsync(Guid usuarioId, Guid anuncioId);
    Task RegistrarAsync(AnuncioVisualizado visualizacao);
    Task<List<Guid>> ObterIdsVisualizadosDoUsuarioAsync(Guid usuarioId);
    Task<bool> FoiVisualizadoAsync(Guid usuarioId, Guid anuncioId);
}
