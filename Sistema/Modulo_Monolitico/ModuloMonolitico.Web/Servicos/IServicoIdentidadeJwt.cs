using System.Security.Claims;

namespace ModuloMonolitico.Web.Servicos;

public interface IServicoIdentidadeJwt
{
    bool TentarResolverUsuarioId(ClaimsPrincipal user, out Guid usuarioId);
    string ObterEmail(ClaimsPrincipal user);
    bool EhUsuarioMaster(Guid usuarioId, string email);
    bool ClaimsSaoConsistentes(ClaimsPrincipal user);
    Task<bool> ValidarIdentidadeContraBancoAsync(Guid usuarioId, string emailJwt, CancellationToken cancellationToken = default);
    void EncerrarSessaoInvalida(HttpContext context);
    void InvalidarCacheIdentidade(Guid usuarioId);
}
