using ModuloMonolitico.Web.Core.Constantes;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using ModuloMonolitico.Web.Core;
using ModuloMonolitico.Web.Pages.Shared;
using System;
using System.Threading.Tasks;

namespace ModuloMonolitico.Web.Pages.Imobiliaria.MeusAnuncios;

[LimiteAcesso]
public class PerfilModel : AppBasePageModel
{
    private readonly IPerfilAnuncianteRepositorio _perfilRepositorio;

    public PerfilModel(IPerfilAnuncianteRepositorio perfilRepositorio)
    {
        _perfilRepositorio = perfilRepositorio;
    }

    [BindProperty]
    public PerfilAnunciante Perfil { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var perfilDto = await _perfilRepositorio.ObterPorContaIdAsync(ContaId);
        
        if (perfilDto != null)
        {
            Perfil = new PerfilAnunciante
            {
                ContaId = perfilDto.ContaId,
                NomeExibicao = perfilDto.NomeExibicao,
                Descricao = perfilDto.Descricao,
                UrlFotoLogo = perfilDto.UrlFotoLogo,
                Whatsapp = perfilDto.Whatsapp,
                UrlSite = perfilDto.UrlSite,
                Instagram = perfilDto.Instagram,
                Creci = perfilDto.Creci
            };
        }
        else
        {
            Perfil = new PerfilAnunciante
            {
                NomeExibicao = UsuarioEmail
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var perfilExistente = await _perfilRepositorio.ObterPorContaIdAsync(ContaId);
        
        Perfil.ContaId = ContaId;
        Perfil.DataAtualizacao = DateTime.UtcNow;

        if (perfilExistente == null)
        {
            Perfil.Id = Guid.NewGuid();
            Perfil.DataInclusao = DateTime.UtcNow;
            await _perfilRepositorio.CadastrarAsync(Perfil);
        }
        else
        {
            await _perfilRepositorio.AtualizarAsync(Perfil);
        }

        NotificarSucesso();
        return RedirectToPage(ModuloRotas.Imobiliaria.MeusAnuncios);
    }
}
