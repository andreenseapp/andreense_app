using Microsoft.EntityFrameworkCore;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class AnuncioVisualizadoRepositorio : IAnuncioVisualizadoRepositorio
{
    private readonly Contexto.DbContext _dbContext;

    public AnuncioVisualizadoRepositorio(Contexto.DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnuncioVisualizado?> ObterAsync(Guid usuarioId, Guid anuncioId)
    {
        return await _dbContext.AnunciosVisualizados
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId && v.AnuncioId == anuncioId);
    }

    public async Task RegistrarAsync(AnuncioVisualizado visualizacao)
    {
        var existente = await ObterAsync(visualizacao.UsuarioId, visualizacao.AnuncioId);
        if (existente != null)
        {
            existente.DataVisualizacao = visualizacao.DataVisualizacao;
            await _dbContext.SaveChangesAsync();
            return;
        }

        _dbContext.AnunciosVisualizados.Add(visualizacao);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Guid>> ObterIdsVisualizadosDoUsuarioAsync(Guid usuarioId)
    {
        return await _dbContext.AnunciosVisualizados
            .AsNoTracking()
            .Where(v => v.UsuarioId == usuarioId)
            .Select(v => v.AnuncioId)
            .ToListAsync();
    }

    public async Task<bool> FoiVisualizadoAsync(Guid usuarioId, Guid anuncioId)
    {
        return await _dbContext.AnunciosVisualizados
            .AsNoTracking()
            .AnyAsync(v => v.UsuarioId == usuarioId && v.AnuncioId == anuncioId);
    }
}
