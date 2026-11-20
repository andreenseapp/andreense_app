using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IImovelFavoritoRepositorio
{
    Task<ImovelFavorito?> ObterAsync(Guid usuarioId, Guid imovelId);
    Task CadastrarAsync(ImovelFavorito favorito);
    Task RemoverAsync(ImovelFavorito favorito);
    Task<List<Guid>> ObterIdsFavoritosDoUsuarioAsync(Guid usuarioId);
}
