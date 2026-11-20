using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.Entidades.Servicos;
using ModuloMonolitico.Dominio.Compartilhado;
namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IAgendamentoRepositorio
{
    Task<IEnumerable<ServicoAgendavel>> ListarServicosDaContaAsync(Guid contaId, bool apenasAtivos = true);
    Task<IEnumerable<Prestador>> ListarPrestadoresAsync(Guid empresaId, Guid contaId, bool apenasAtivos = true, Guid? produtoVariacaoId = null);
    Task VincularServicosAoPrestadorAsync(Guid empresaId, Guid? usuarioId, Guid? contatoId, IEnumerable<Guid> produtoVariacaoIds);
    Task<IEnumerable<ConfiguracaoHorario>> ObterConfiguracoesHorariosAsync(Guid empresaId, Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null);
    Task AtualizarConfiguracoesHorariosAsync(Guid empresaId, IEnumerable<ConfiguracaoHorario> horarios, Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null);
    Task<IEnumerable<HorarioBloqueado>> ListarHorariosBloqueadosAsync(Guid empresaId, DateTime? de = null, DateTime? ate = null);
    Task CadastrarHorarioBloqueadoAsync(HorarioBloqueado bloqueio);
    Task DeletarFisicoHorarioBloqueadoAsync(Guid id, Guid empresaId);
    Task<Agendamento?> ObterPorIdAsync(Guid id, Guid empresaId);
    Task<Agendamento?> ObterPorIdEClienteAsync(Guid id, Guid clienteUsuarioId);
    Task<IEnumerable<Agendamento>> ListarPorPeriodoAsync(Guid empresaId, DateTime de, DateTime ate, Guid? profissionalUsuarioId = null, Guid? profissionalContatoId = null);
    Task<bool> ExisteConflitoHorarioAsync(Guid empresaId, Guid? profissionalUsuarioId, Guid? profissionalContatoId, DateTime dataHoraInicio, DateTime dataHoraFim);
    Task<IEnumerable<Agendamento>> ListarProximosPorClienteAsync(Guid clienteUsuarioId);
    Task<ResultadoPaginado<Agendamento>> ListarHistoricoPorClienteAsync(Guid clienteUsuarioId, int pagina = 1, int tamanhoPagina = 20);
    Task<ResultadoPaginado<Agendamento>> ListarPorClienteAsync(Guid clienteUsuarioId, int pagina = 1, int tamanhoPagina = 50);
    Task CadastrarAsync(Agendamento agendamento);
    Task AtualizarSituacaoAsync(Guid id, Guid empresaId, string novaSituacao, string? motivo = null, string? canceladoPor = null);
}





