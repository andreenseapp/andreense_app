using ModuloMonolitico.Dominio.Compartilhado;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IPerfilAnuncianteRepositorio
{
    Task<PerfilAnuncianteDto?> ObterPorContaIdAsync(Guid contaId);
    Task CadastrarAsync(PerfilAnunciante perfil);
    Task AtualizarAsync(PerfilAnunciante perfil);
    Task<ResultadoPaginado<AnuncioResumoDto>> ListarAnunciosPorContaAsync(Guid contaId, FiltroAnuncio filtro);
}
