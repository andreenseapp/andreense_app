using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IAnuncioRepositorio
{
    Task<ResultadoPaginado<AnuncioResumoDto>> ListarComFiltrosAsync(FiltroAnuncio filtro);
    Task<AnuncioDetalheDto?> ObterDetalheAsync(Guid anuncioId);
    Task<IEnumerable<AnuncioResumoDto>> ListarPorUsuarioAsync(Guid usuarioId);
    Task CadastrarAsync(Imovel imovel, Anuncio anuncio);
    Task AtualizarAsync(Imovel imovel, Anuncio anuncio);
    Task AtualizarAnuncioAsync(Anuncio anuncio);
    Task AtualizarSituacaoAsync(Guid anuncioId, string novaSituacao);
    Task AlternarDestaqueAsync(Guid anuncioId, bool ehDestaque);
    Task<IEnumerable<AnuncioResumoDto>> ListarDestaquesAsync(string? tipoNegocio, int limite = 6);
    Task<ResultadoPaginado<AnuncioResumoDto>> ListarPorFavoritosDoUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina);
    Task CadastrarFotoAsync(AnuncioFoto foto);
    Task DeletarFisicoFotoAsync(Guid fotoId);
    Task<AnuncioFoto?> ObterFotoPorIdAsync(Guid fotoId);
    Task<Anuncio?> ObterAnuncioPorIdAsync(Guid anuncioId);
    Task<int> ContarFotosAsync(Guid anuncioId);
    Task<IEnumerable<AnuncioFoto>> ListarFotosPorAnuncioAsync(Guid anuncioId);
}
