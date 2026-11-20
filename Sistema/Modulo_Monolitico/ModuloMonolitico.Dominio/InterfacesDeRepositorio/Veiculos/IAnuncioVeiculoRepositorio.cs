using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;

public interface IAnuncioVeiculoRepositorio
{
    Task<ResultadoPaginado<AnuncioVeiculoResumoDto>> ListarComFiltrosAsync(FiltroAnuncioVeiculo filtro);
    Task<AnuncioVeiculoDetalheDto?> ObterDetalheAsync(Guid anuncioVeiculoId);
    Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarDestaquesAsync(int limite = 6);
    Task<ResultadoPaginado<AnuncioVeiculoResumoDto>> ListarPorFavoritosDoUsuarioAsync(Guid usuarioId, int pagina, int tamanhoPagina);
    Task CadastrarAsync(Veiculo veiculo, AnuncioVeiculo anuncio);
    Task AtualizarAsync(Veiculo veiculo, AnuncioVeiculo anuncio);
    Task AtualizarSituacaoAsync(Guid anuncioVeiculoId, string novaSituacao);
    Task AlternarDestaqueAsync(Guid anuncioVeiculoId, bool ehDestaque);
    Task<AnuncioVeiculo?> ObterAnuncioPorIdAsync(Guid anuncioVeiculoId);
    Task CadastrarFotoAsync(AnuncioVeiculoFoto foto);
    Task DeletarFisicoFotoAsync(Guid fotoId);
    Task<AnuncioVeiculoFoto?> ObterFotoPorIdAsync(Guid fotoId);
    Task<int> ContarFotosAsync(Guid anuncioVeiculoId);
    Task<IEnumerable<AnuncioVeiculoFoto>> ListarFotosPorAnuncioAsync(Guid anuncioVeiculoId);
    Task<IEnumerable<AnuncioVeiculoResumoDto>> ListarPorVendedorAsync(Guid contaId, int pagina, int tamanhoPagina);
}
