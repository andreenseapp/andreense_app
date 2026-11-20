using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.Servicos;

public class ServicoLimitacaoRequisicoes : IServicoLimitacaoRequisicoes
{
    private readonly ITentativaLoginRepositorio _repositorio;

    public ServicoLimitacaoRequisicoes(ITentativaLoginRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> EstaBloqueadoAsync(
        PoliticaLimitacaoRequisicoes politica,
        string chaveEmail,
        string ip,
        CancellationToken cancellationToken = default)
    {
        int porEmailIp;
        int porIp;

        if (politica.Modo == ModoLimitacaoRequisicoes.Falhas)
        {
            if (string.IsNullOrEmpty(politica.Provedor))
            {
                porEmailIp = await _repositorio.ContarFalhasRecentesAsync(chaveEmail, ip, politica.Janela);
                porIp = await _repositorio.ContarFalhasRecentesPorIpAsync(ip, politica.Janela);
            }
            else
            {
                porEmailIp = await _repositorio.ContarFalhasRecentesPorProvedorAsync(
                    chaveEmail, ip, politica.Provedor, politica.Janela);
                porIp = await _repositorio.ContarFalhasRecentesPorIpEProvedorAsync(
                    ip, politica.Provedor, politica.Janela);
            }
        }
        else
        {
            var provedor = politica.Provedor
                ?? throw new InvalidOperationException($"Política '{politica.Nome}' exige Provedor para modo Solicitacoes.");

            porEmailIp = await _repositorio.ContarSolicitacoesRecentesAsync(
                chaveEmail, ip, provedor, politica.Janela);
            porIp = await _repositorio.ContarSolicitacoesRecentesPorIpAsync(ip, provedor, politica.Janela);
        }

        return porEmailIp >= politica.MaximoPorEmailIp || porIp >= politica.MaximoPorIp;
    }

    public Task RegistrarAsync(RegistroTentativaLogin registro, CancellationToken cancellationToken = default) =>
        _repositorio.RegistrarAsync(registro);

    public Task AplicarAtrasoRespostaAsync(CancellationToken cancellationToken = default) =>
        Task.Delay(Random.Shared.Next(400, 800), cancellationToken);
}
