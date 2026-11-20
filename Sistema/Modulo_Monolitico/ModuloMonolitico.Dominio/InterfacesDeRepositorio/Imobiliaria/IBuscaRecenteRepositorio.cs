using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IBuscaRecenteRepositorio
{
    Task RegistrarAsync(BuscaRecente busca, int limite);
    Task<IReadOnlyList<BuscaRecente>> ListarPorUsuarioAsync(Guid usuarioId, int limite);
    Task<bool> DeletarFisicoAsync(Guid id, Guid usuarioId);
    Task DeletarFisicoTodasAsync(Guid usuarioId);
}
