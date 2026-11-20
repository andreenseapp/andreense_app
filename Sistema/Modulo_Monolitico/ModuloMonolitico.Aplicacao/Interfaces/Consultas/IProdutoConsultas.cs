using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.Interfaces.Consultas;

public interface IProdutoConsultas
{
    Task<IEnumerable<ResultadoSeletorProduto>> BuscarParaSeletorAsync(Guid contaId, string? termo, bool apenasServicos);
}
