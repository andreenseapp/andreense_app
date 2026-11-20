using System;
using System.Threading.Tasks;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IComunicadoSaaSRepositorio
{
    Task<ComunicadoSaaS?> ObterPrimeiroPendenteParaUsuarioAsync(Guid contaId, Guid usuarioId, string moduloAtual);
    Task RegistrarLeituraAsync(LeituraComunicadoSaaS leitura);
    Task<System.Collections.Generic.IEnumerable<ComunicadoSaaS>> ListarTodosAsync();
    Task<ComunicadoSaaS?> ObterPorIdAsync(Guid id);
    Task CadastrarComunicadoAsync(ComunicadoSaaS comunicado, System.Collections.Generic.List<OpcaoComunicadoSaaS> opcoes);
    Task DeletarFisicoComunicadoAsync(Guid id);
    Task<System.Collections.Generic.IEnumerable<ResultadoEnqueteDto>> ObterResultadosEnqueteAsync(Guid comunicadoId);
    Task<System.Collections.Generic.IEnumerable<ComunicadoSaaSComMetricasDto>> ListarTodosComMetricasAsync();
    Task<System.Collections.Generic.IEnumerable<InteracaoComunicadoDto>> ObterInteracoesAsync(Guid comunicadoId);
}


