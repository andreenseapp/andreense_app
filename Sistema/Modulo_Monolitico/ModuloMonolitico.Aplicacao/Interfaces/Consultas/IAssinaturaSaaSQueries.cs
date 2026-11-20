using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.Interfaces.Consultas;

public interface IAssinaturaSaaSQueries
{
    Task<ContaDireitosPlanoDto> ObterDireitosPlanoDaContaAsync(Guid contaId);
    Task<IEnumerable<ContaResumoDto>> ListarContasComAssinaturaAsync();
}
