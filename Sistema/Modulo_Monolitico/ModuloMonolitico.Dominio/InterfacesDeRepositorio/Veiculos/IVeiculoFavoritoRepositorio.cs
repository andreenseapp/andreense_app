using ModuloMonolitico.Dominio.Entidades.Veiculos;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;

public interface IVeiculoFavoritoRepositorio
{
    Task<bool> EhFavoritadoAsync(Guid usuarioId, Guid anuncioVeiculoId);
    Task FavoritarAsync(VeiculoFavorito favorito);
    Task DesfavoritarAsync(Guid usuarioId, Guid anuncioVeiculoId);
}
