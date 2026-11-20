using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Veiculos.MeusAnuncios;

public class PerfilModel : AppBasePageModel
{
    private readonly IPerfilVendedorRepositorio _repositorio;

    public PerfilModel(IPerfilVendedorRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    [BindProperty] public string NomeExibicao { get; set; } = string.Empty;
    [BindProperty] public string? Descricao { get; set; }
    [BindProperty] public string? UrlFotoLogo { get; set; }
    [BindProperty] public string? Whatsapp { get; set; }
    [BindProperty] public string? UrlSite { get; set; }
    [BindProperty] public string? Instagram { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var perfil = await _repositorio.ObterEntidadePorContaAsync(ContaId);
        if (perfil is not null)
        {
            NomeExibicao = perfil.NomeExibicao;
            Descricao = perfil.Descricao;
            UrlFotoLogo = perfil.UrlFotoLogo;
            Whatsapp = perfil.Whatsapp;
            UrlSite = perfil.UrlSite;
            Instagram = perfil.Instagram;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var perfilExistente = await _repositorio.ObterEntidadePorContaAsync(ContaId);

        if (perfilExistente is null)
        {
            await _repositorio.CadastrarAsync(new PerfilVendedor
            {
                Id = System.Guid.NewGuid(),
                ContaId = ContaId,
                NomeExibicao = NomeExibicao,
                Descricao = Descricao,
                UrlFotoLogo = UrlFotoLogo,
                Whatsapp = Whatsapp,
                UrlSite = UrlSite,
                Instagram = Instagram
            });
        }
        else
        {
            perfilExistente.NomeExibicao = NomeExibicao;
            perfilExistente.Descricao = Descricao;
            perfilExistente.UrlFotoLogo = UrlFotoLogo;
            perfilExistente.Whatsapp = Whatsapp;
            perfilExistente.UrlSite = UrlSite;
            perfilExistente.Instagram = Instagram;
            await _repositorio.AtualizarAsync(perfilExistente);
        }

        NotificarSucesso(MensagensSistema.Geral.Sucesso);
        return RedirectToPage();
    }
}
