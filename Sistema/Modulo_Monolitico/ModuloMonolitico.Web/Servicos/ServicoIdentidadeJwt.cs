using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Web.Options;
using ModuloMonolitico.Web.Extensions;
using ModuloMonolitico.Dominio.Configuracoes;

namespace ModuloMonolitico.Web.Servicos;

public class ServicoIdentidadeJwt : IServicoIdentidadeJwt
{
    private readonly OpcoesUsuarioMaster _master;
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly ILogger<ServicoIdentidadeJwt> _logger;
    private readonly IMemoryCache _cache;
    private readonly OpcoesAutenticacao _auth;

    public ServicoIdentidadeJwt(
        IOptions<OpcoesUsuarioMaster> masterOptions,
        IOptions<OpcoesAutenticacao> authOptions,
        IUsuarioRepositorio usuarioRepositorio,
        ILogger<ServicoIdentidadeJwt> logger,
        IMemoryCache cache)
    {
        _master = masterOptions.Value;
        _auth = authOptions.Value;
        _usuarioRepositorio = usuarioRepositorio;
        _logger = logger;
        _cache = cache;
    }

    public string ObterEmail(ClaimsPrincipal user) =>
        user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
        ?? user.FindFirst("email")?.Value
        ?? user.FindFirst(ClaimTypes.Email)?.Value
        ?? string.Empty;

    public bool EhUsuarioMaster(Guid usuarioId, string email) =>
        usuarioId == _master.Id
        && !string.IsNullOrWhiteSpace(_master.Email)
        && email.Equals(_master.Email, StringComparison.OrdinalIgnoreCase);

    public bool ClaimsSaoConsistentes(ClaimsPrincipal user)
    {
        var subRaw = ObterClaimSub(user);
        var nameIdRaw = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = ObterEmail(user);

        var temSub = Guid.TryParse(subRaw, out var subId);
        var temNameId = Guid.TryParse(nameIdRaw, out var nameId);

        if (temSub && temNameId
            && subId != Guid.Empty && nameId != Guid.Empty
            && subId != nameId)
        {
            _logger.LogWarning(
                "JWT rejeitado: claim sub ({SubId}) difere de NameIdentifier ({NameId})",
                subId, nameId);
            return false;
        }

        if (temSub && subId == Guid.Empty && !EhUsuarioMaster(Guid.Empty, email))
        {
            _logger.LogWarning(
                "JWT rejeitado: sub=Guid.Empty para e-mail não-master ({Email})",
                email);
            return false;
        }

        return true;
    }

    public bool TentarResolverUsuarioId(ClaimsPrincipal user, out Guid usuarioId)
    {
        usuarioId = default;

        if (!ClaimsSaoConsistentes(user))
            return false;

        var email = ObterEmail(user);

        foreach (var claimType in new[] { JwtRegisteredClaimNames.Sub, "sub" })
        {
            var raw = user.FindFirst(claimType)?.Value;
            if (!Guid.TryParse(raw, out var id))
                continue;

            if (id != Guid.Empty)
            {
                usuarioId = id;
                return true;
            }

            if (EhUsuarioMaster(Guid.Empty, email))
            {
                usuarioId = Guid.Empty;
                return true;
            }
        }

        var legacyRaw = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(legacyRaw, out var legacyId) && legacyId != Guid.Empty)
        {
            usuarioId = legacyId;
            return true;
        }

        return false;
    }

    public async Task<bool> ValidarIdentidadeContraBancoAsync(
        Guid usuarioId,
        string emailJwt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailJwt))
            return false;

        var cacheKey = ChavesSistema.CacheIdentidade(usuarioId);

        if (_cache.TryGetValue(cacheKey, out string? emailCached))
        {
            if (emailCached!.Equals(emailJwt, StringComparison.OrdinalIgnoreCase))
                return true;

            _cache.Remove(cacheKey);
        }

        var usuario = await _usuarioRepositorio.ObterPorIdAsync(usuarioId);
        if (usuario is null || !usuario.Ativo)
        {
            _logger.LogWarning(
                "Identidade rejeitada: usuário {UsuarioId} inexistente ou inativo",
                usuarioId);
            return false;
        }

        if (!usuario.Email.Equals(emailJwt, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Identidade rejeitada: JWT email ({EmailJwt}) ? banco ({EmailBanco}) para id {UsuarioId}",
                emailJwt, usuario.Email, usuarioId);
            return false;
        }

        _cache.Set(cacheKey, usuario.Email, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _auth.IdentidadeCacheDuracao,

            Priority = CacheItemPriority.Low
        });

        return true;
    }

    public void EncerrarSessaoInvalida(HttpContext context)
    {

        if (TentarResolverUsuarioId(context.User, out var usuarioId))
            _cache.Remove(ChavesSistema.CacheIdentidade(usuarioId));

        context.RequestServices.GetRequiredService<IServicoCookieAutenticacao>().LimparSessao();
    }

    public void InvalidarCacheIdentidade(Guid usuarioId)
    {
        _cache.Remove(ChavesSistema.CacheIdentidade(usuarioId));
        _logger.LogInformation("Cache de identidade invalidado para o usuário {UsuarioId}.", usuarioId);
    }

    private static string? ObterClaimSub(ClaimsPrincipal user) =>
        user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? user.FindFirst("sub")?.Value;
}
