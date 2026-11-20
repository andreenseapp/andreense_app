using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.Interfaces.Consultas;

public interface IAgendamentoQueries
{
    Task<IEnumerable<HorarioDisponivel>> ObterHorariosDisponiveisAsync(
        Guid empresaId,
        Guid produtoVariacaoId,
        DateTime data,
        Guid? profissionalUsuarioId = null,
        Guid? profissionalContatoId = null);
}
