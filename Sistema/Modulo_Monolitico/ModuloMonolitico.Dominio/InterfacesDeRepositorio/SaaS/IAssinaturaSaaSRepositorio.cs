using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;



public interface IAssinaturaSaaSRepositorio
{
    Task<IEnumerable<PlanoSaaS>> ListarPlanosSaaSAsync();
    Task<IEnumerable<PlanoSaaS>> ListarPlanosAtivosAsync();
    Task<PlanoSaaS?> ObterPlanoPorIdAsync(Guid id);
    Task CadastrarPlanoAsync(PlanoSaaS plano, List<string> idsRecurso, Dictionary<string, string> valoresRecurso);
    Task AtualizarPlanoAsync(PlanoSaaS plano, List<string> idsRecurso, Dictionary<string, string> valoresRecurso);
    Task<IEnumerable<RecursoPlanoSaaS>> ListarRecursosAsync();
    Task<ContaAssinatura?> ObterAssinaturaDaContaAsync(Guid contaId);
    Task CadastrarAssinaturaAsync(ContaAssinatura assinatura);
    Task AtualizarAssinaturaAsync(ContaAssinatura assinatura);
    Task<IEnumerable<ContaComplementoSaaS>> ListarComplementosDaContaAsync(Guid contaId);
    Task CadastrarComplementoAsync(ContaComplementoSaaS addon);
    Task DesativarComplementoAsync(Guid addonId);
    Task<IEnumerable<FaturaSaaS>> ListarFaturasDaContaAsync(Guid contaId);
    Task<FaturaSaaS?> ObterFaturaPorIdAsync(Guid id);
    Task CadastrarFaturaAsync(FaturaSaaS fatura);
    Task DarBaixaFaturaAsync(Guid faturaId, Guid operadorId);


}


