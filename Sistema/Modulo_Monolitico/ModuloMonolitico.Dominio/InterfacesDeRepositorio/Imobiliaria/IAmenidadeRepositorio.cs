using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;

public interface IAmenidadeRepositorio
{
    Task<IReadOnlyList<AmenidadeDto>> ListarTodasAsync();
    Task<IReadOnlyList<AmenidadeDto>> ListarPorImovelAsync(Guid imovelId);
    Task SubstituirDoImovelAsync(Guid imovelId, IEnumerable<string> codigos);
}
