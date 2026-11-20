using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;
public sealed class ServicoRastreadorSessao(IMemoryCache cache)
{
    private static readonly TimeSpan InatividadeMaxima = TimeSpan.FromMinutes(30);

    private readonly IMemoryCache _cache = cache;
    private readonly ConcurrentDictionary<string, byte> _chavesAtivas = new();
    public void AtualizarSessao(string idSessao, Guid usuarioId, string nome, string login,
        string grupoCodigo, string grupoDescricao, string enderecoIp, string navegador)
    {
        var sessao = new SessaoAtiva
        {
            IdSessao    = idSessao,
            UsuarioId    = usuarioId,
            Nome         = nome,
            Login        = login,
            GrupoCodigo  = grupoCodigo,
            GrupoDescricao = grupoDescricao,
            UltimoAcesso = DateTime.UtcNow, 
            EnderecoIp   = enderecoIp,
            Navegador    = navegador
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(InatividadeMaxima)
            .RegisterPostEvictionCallback((key, _, reason, _) =>
            {
                if (reason == EvictionReason.Replaced) return;

                if (key is string strKey)
                    _chavesAtivas.TryRemove(strKey, out _);
            });

        _cache.Set(idSessao, sessao, cacheEntryOptions);
        _chavesAtivas.TryAdd(idSessao, 0); 
    }
    public SessaoAtiva? ObterSessao(string idSessao)
    {
        _cache.TryGetValue(idSessao, out SessaoAtiva? sessao);
        return sessao;
    }
    public void RevogarSessao(string idSessao)
    {
        if (_cache.TryGetValue(idSessao, out SessaoAtiva? sessao) && sessao != null)
        {
            sessao.Revogada = true;
            _cache.Set(idSessao, sessao);
        }
    }
    public void RemoverSessao(string idSessao)
    {
        _cache.Remove(idSessao);
        _chavesAtivas.TryRemove(idSessao, out _);
    }
    public List<SessaoAtiva> ObterSessoesAtivas()
    {
        List<SessaoAtiva> sessoes = [];
        foreach (var key in _chavesAtivas.Keys)
        {
            if (_cache.TryGetValue(key, out SessaoAtiva? sessao) && sessao?.Revogada is false)
            {
                sessoes.Add(sessao);
            }
        }
        return [.. sessoes.OrderByDescending(s => s.UltimoAcesso)];
    }
}
