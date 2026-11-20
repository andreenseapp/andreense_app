using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public class ServicoLimiteAcesso(IConfiguration configuration) : IServicoLimiteAcesso
{
    private readonly ConcurrentDictionary<string, TentativaAcesso> _tentativas = new();
    private readonly int _maximoTentativas = int.TryParse(configuration["ConfiguracoesLogin:MaximoTentativasLogin"], out int maxTentativas) ? maxTentativas : 10;
    private static readonly TimeSpan PeriodoBloqueio = TimeSpan.FromMinutes(15);

    public bool EstaBloqueado(string chave)
    {
        if (!_tentativas.TryGetValue(chave, out var tentativa) || !tentativa.EstaBloqueada)
            return false;

        if (DateTime.UtcNow < tentativa.BloqueadoAte)
            return true;
        _tentativas.TryRemove(chave, out _);
        return false;
    }

    public void RegistrarTentativaFalha(string chave)
    {
        _tentativas.AddOrUpdate(
            chave,
            addValueFactory: _ => new TentativaAcesso(Quantidade: 1, UltimaTentativa: DateTime.UtcNow),
            updateValueFactory: (_, existente) =>
            {
                var novaQuantidade = existente.Quantidade + 1;
                var bloqueada      = novaQuantidade >= _maximoTentativas;

                return existente with
                {
                    Quantidade      = novaQuantidade,
                    UltimaTentativa = DateTime.UtcNow,
                    EstaBloqueada   = bloqueada,
                    BloqueadoAte    = bloqueada ? DateTime.UtcNow.Add(PeriodoBloqueio) : existente.BloqueadoAte
                };
            });
    }

    public void LimparTentativas(string chave) =>
        _tentativas.TryRemove(chave, out _);
    private sealed record TentativaAcesso(
        int      Quantidade,
        DateTime UltimaTentativa,
        bool     EstaBloqueada = false,
        DateTime BloqueadoAte  = default);
}
