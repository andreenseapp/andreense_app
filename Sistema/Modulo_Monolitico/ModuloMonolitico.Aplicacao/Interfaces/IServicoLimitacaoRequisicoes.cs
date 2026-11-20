using ModuloMonolitico.Aplicacao.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.Interfaces;

public interface IServicoLimitacaoRequisicoes
{
    Task<bool> EstaBloqueadoAsync(
        PoliticaLimitacaoRequisicoes politica,
        string chaveEmail,
        string ip,
        CancellationToken cancellationToken = default);

    Task RegistrarAsync(
        RegistroTentativaLogin registro,
        CancellationToken cancellationToken = default);

    Task AplicarAtrasoRespostaAsync(CancellationToken cancellationToken = default);
}
