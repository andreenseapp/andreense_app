using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterConfiguracoesHorariosQuery(
    Guid EmpresaId,
    Guid? ProfissionalUsuarioId = null,
    Guid? ProfissionalContatoId = null
) : IRequest<IEnumerable<ConfiguracaoHorario>>;

public class ObterConfiguracoesHorariosQueryHandler : IRequestHandler<ObterConfiguracoesHorariosQuery, IEnumerable<ConfiguracaoHorario>>
{
    private readonly IAgendamentoRepositorio _repo;
    public ObterConfiguracoesHorariosQueryHandler(IAgendamentoRepositorio repo) => _repo = repo;

    public async Task<IEnumerable<ConfiguracaoHorario>> Handle(ObterConfiguracoesHorariosQuery q, CancellationToken ct)
    {
        var horarios = await _repo.ObterConfiguracoesHorariosAsync(
            q.EmpresaId, q.ProfissionalUsuarioId, q.ProfissionalContatoId);

        if (!horarios.Any() && q.ProfissionalUsuarioId == null && q.ProfissionalContatoId == null && q.EmpresaId != Guid.Empty)
        {
            var masterHorarios = await _repo.ObterConfiguracoesHorariosAsync(Guid.Empty);
            
            if (masterHorarios.Any())
            {
                var novosHorarios = masterHorarios.Select(h => new ConfiguracaoHorario
                {
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFim = h.HoraFim,
                    IntervaloMinutos = h.IntervaloMinutos,
                    Ativo = h.Ativo
                }).ToList();

                await _repo.AtualizarConfiguracoesHorariosAsync(q.EmpresaId, novosHorarios);
                return novosHorarios;
            }
            else
            {
                var defaultHorarios = new List<ConfiguracaoHorario>();
                for (int i = 1; i <= 5; i++)
                {
                    defaultHorarios.Add(new ConfiguracaoHorario 
                    { 
                        DiaSemana = i, 
                        HoraInicio = new TimeOnly(8, 0), 
                        HoraFim = new TimeOnly(18, 0), 
                        IntervaloMinutos = 30, 
                        Ativo = true 
                    });
                }
                await _repo.AtualizarConfiguracoesHorariosAsync(q.EmpresaId, defaultHorarios);
                return defaultHorarios;
            }
        }

        return horarios;
    }
}



