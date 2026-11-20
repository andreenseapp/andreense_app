using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.Interfaces.Consultas;

public interface IVinculoQueries
{
    Task<IEnumerable<ResultadoSeletorUsuario>> BuscarContatosInternosAsync(Guid contaId, Guid empresaId, string papel, string? termo);
    Task<IEnumerable<ResultadoSeletorUsuario>> BuscarContatosVinculadosAsync(Guid contaId, string papel, string? termo);
}
