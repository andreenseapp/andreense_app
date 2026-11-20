using System.Security.Claims;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Aplicacao.DTOs;

namespace ModuloMonolitico.Aplicacao.Interfaces;
public interface IServicoToken
{
    string GerarTokenAcesso(Usuario usuario, Guid contaId, IList<Guid> idsEmpresas, string hashImpressaoDigital, ContaDireitosPlanoDto? direitosPlano = null);
    string GerarTokenRenovacao();
    string GerarHashTokenRenovacao(string tokenRenovacao);
    ClaimsPrincipal? ValidarTokenAcesso(string tokenAcesso);
}


