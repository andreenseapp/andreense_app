using Dapper;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Imobiliaria;

public class AmenidadeRepositorio : RepositorioBase, IAmenidadeRepositorio
{
    private readonly IMemoryCache _cache;

    public AmenidadeRepositorio(string conexaoString, IMemoryCache cache) : base(conexaoString) 
    {
        _cache = cache;
    }

    public async Task<IReadOnlyList<AmenidadeDto>> ListarTodasAsync()
    {
        return await _cache.GetOrCreateAsync("Imobiliaria_Amenidades_Todas", async entrada =>
        {
            entrada.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            
            const string sql = """
                SELECT
                    id AS Id,
                    codigo AS Codigo,
                    nome AS Nome,
                    categoria AS Categoria,
                    ordem AS Ordem
                FROM imobiliaria_amenidades
                ORDER BY
                    CASE categoria
                        WHEN 'imovel' THEN 1
                        WHEN 'condominio' THEN 2
                        WHEN 'seguranca' THEN 3
                        ELSE 9
                    END,
                    ordem ASC,
                    nome ASC
                """;

            await using var conexao = CriarConexao();
            var itens = await conexao.QueryAsync<AmenidadeDto>(sql);
            return (IReadOnlyList<AmenidadeDto>)itens.ToList();
        }) ?? [];
    }

    public async Task<IReadOnlyList<AmenidadeDto>> ListarPorImovelAsync(Guid imovelId)
    {
        const string sql = """
            SELECT
                a.id AS Id,
                a.codigo AS Codigo,
                a.nome AS Nome,
                a.categoria AS Categoria,
                a.ordem AS Ordem
            FROM imobiliaria_imovel_amenidades ia
            INNER JOIN imobiliaria_amenidades a ON a.id = ia.amenidade_id
            WHERE ia.imovel_id = @ImovelId
            ORDER BY
                CASE a.categoria
                    WHEN 'imovel' THEN 1
                    WHEN 'condominio' THEN 2
                    WHEN 'seguranca' THEN 3
                    ELSE 9
                END,
                a.ordem ASC,
                a.nome ASC
            """;

        await using var conexao = CriarConexao();
        var itens = await conexao.QueryAsync<AmenidadeDto>(sql, new { ImovelId = imovelId });
        return itens.ToList();
    }

    public async Task SubstituirDoImovelAsync(Guid imovelId, IEnumerable<string> codigos)
    {
        var lista = (codigos ?? Enumerable.Empty<string>())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync(
                "DELETE FROM imobiliaria_imovel_amenidades WHERE imovel_id = @ImovelId",
                new { ImovelId = imovelId });

            if (lista.Length == 0)
                return;

            const string sqlInsert = """
                INSERT INTO imobiliaria_imovel_amenidades (imovel_id, amenidade_id)
                SELECT @ImovelId, a.id
                FROM imobiliaria_amenidades a
                WHERE a.codigo = ANY(@Codigos)
                ON CONFLICT DO NOTHING
                """;

            await conexao.ExecuteAsync(sqlInsert, new { ImovelId = imovelId, Codigos = lista });
        });
    }
}
