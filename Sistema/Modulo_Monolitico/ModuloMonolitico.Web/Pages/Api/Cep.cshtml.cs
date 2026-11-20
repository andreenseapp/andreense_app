using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ModuloMonolitico.Dominio.Servicos;

namespace ModuloMonolitico.Web.Pages.Api;

[IgnoreAntiforgeryToken]
public class CepModel : PageModel
{
  private readonly IServicoEndereco _servicoEndereco;

  public CepModel(IServicoEndereco servicoEndereco)
  {
    _servicoEndereco = servicoEndereco;
  }

  public async Task<IActionResult> OnGetAsync(string cep)
  {
    if (string.IsNullOrWhiteSpace(cep))
      return new JsonResult(new { encontrado = false });

    var resultado = await _servicoEndereco.BuscarPorCepAsync(cep);

    return new JsonResult(new
    {
      encontrado  = resultado.Encontrado,
      logradouro  = resultado.Logradouro,
      complemento = resultado.Complemento,
      bairro    = resultado.Bairro,
      cidade    = resultado.Cidade,
      uf      = resultado.Uf
    });
  }
}

