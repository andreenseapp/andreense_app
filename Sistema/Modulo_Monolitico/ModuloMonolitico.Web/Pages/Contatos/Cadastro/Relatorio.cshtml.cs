using System.ComponentModel.DataAnnotations;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class RelatorioModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.Contatos.Relatorio";

    protected override bool ExigeEmpresaAtiva => false;

    private readonly IMediator _mediator;

    public RelatorioModel(IMediator mediator) => _mediator = mediator;

    public IEnumerable<Contato> Contatos { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Nome")]
    public string? FiltroNome { get; set; }

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Tipo")]
    public string? FiltroTipo { get; set; }

    public async Task OnGetAsync()
    {
        Contatos = await CarregarContatosAsync();

        Logger.LogInformation(
            "Relatório de contatos carregado por '{Email}'. Total: {Total}, Nome: {Nome}, Tipo: {Tipo}.",
            UsuarioEmail, Contatos.Count(), FiltroNome ?? "(vazio)", FiltroTipo ?? "(todos)");
    }

    public async Task<IActionResult> OnPostExportarCsvAsync()
    {
        Contatos = await CarregarContatosAsync();

        var builder = new StringBuilder();
        builder.AppendLine("Nome,Tipo,CPF/CNPJ,Email,Telefone");

        foreach (var c in Contatos)
        {
            var tipo = c.TipoPessoa == "fisica" ? "Pessoa Física" : "Pessoa Jurídica";
            var documento = !string.IsNullOrEmpty(c.Cpf) ? c.Cpf : c.Cnpj;
            var nome = $"\"{(c.NomeCompleto ?? c.RazaoSocial)?.Replace("\"", "\"\"")}\"";
            var email = $"\"{c.Email?.Replace("\"", "\"\"")}\"";
            var telefone = $"\"{c.Telefone?.Replace("\"", "\"\"")}\"";

            builder.AppendLine($"{nome},{tipo},{documento},{email},{telefone}");
        }

        Logger.LogInformation(
            "Exportação CSV de contatos gerada por '{Email}'. Registros: {Total}.",
            UsuarioEmail, Contatos.Count());

        return File(
            Encoding.UTF8.GetBytes(builder.ToString()),
            "text/csv",
            ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.ArquivoRelatorioContatos(DateTime.Now));
    }

    private async Task<IEnumerable<Contato>> CarregarContatosAsync()
    {
        return await _mediator.Send(new ListarContatosPorContaQuery(ContaId, FiltroNome, FiltroTipo));
    }
}
