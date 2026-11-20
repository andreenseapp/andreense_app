using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;

public interface IBuscaRecenteVeiculoRepositorio
{
    Task<IEnumerable<BuscaRecenteVeiculoDto>> ListarPorUsuarioAsync(Guid usuarioId, int limite = 10);
    Task CadastrarOuAtualizarAsync(BuscaRecenteVeiculo busca);
    Task DeletarFisicoAsync(Guid buscaId, Guid usuarioId);
    Task DeletarFisicoTodasAsync(Guid usuarioId);
}
