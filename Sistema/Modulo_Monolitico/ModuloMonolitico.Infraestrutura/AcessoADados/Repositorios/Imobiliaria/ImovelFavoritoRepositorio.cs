using Microsoft.EntityFrameworkCore;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class ImovelFavoritoRepositorio : IImovelFavoritoRepositorio
{
    private readonly Contexto.DbContext _dbContext;

    public ImovelFavoritoRepositorio(Contexto.DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ImovelFavorito?> ObterAsync(Guid usuarioId, Guid imovelId)
    {
        return await _dbContext.ImoveisFavoritos
            .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.ImovelId == imovelId);
    }

    public async Task CadastrarAsync(ImovelFavorito favorito)
    {
        _dbContext.ImoveisFavoritos.Add(favorito);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoverAsync(ImovelFavorito favorito)
    {
        _dbContext.ImoveisFavoritos.Remove(favorito);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Guid>> ObterIdsFavoritosDoUsuarioAsync(Guid usuarioId)
    {
        return await _dbContext.ImoveisFavoritos
            .AsNoTracking()
            .Where(f => f.UsuarioId == usuarioId)
            .Select(f => f.ImovelId)
            .ToListAsync();
    }
}
