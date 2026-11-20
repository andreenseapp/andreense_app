using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class DetalhesModel : AppBasePageModel
{
    protected override string? ChavePermissao => "CRM.Contatos.Detalhes";

    private readonly IMediator _mediator;
    private readonly ILogger<DetalhesModel> _logger;

    public DetalhesModel(IMediator mediator, ILogger<DetalhesModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public Contato? Contato { get; private set; }
    public EmpresaCliente? PapelCliente { get; private set; }
    public EmpresaFornecedor? PapelFornecedor { get; private set; }
    public EmpresaTransportador? PapelTransportador { get; private set; }
    public EmpresaLead? PapelLead { get; private set; }
    public EmpresaProspect? PapelProspect { get; private set; }
    public EmpresaFuncionario? PapelFuncionario { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Papel { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var dto = await _mediator.Send(new ObterContatoComPapeisQuery(id, ContaId, EmpresaAtiva!.Value));

        if (dto.Contato == null)
        {
            _logger.LogWarning(
                "Contato {ContatoId} não encontrado para visualização pelo usuário '{Email}'. ContaId: {ContaId}",
                id, UsuarioEmail, ContaId);
            MensagemErro = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Contatos.Cadastro.NaoEncontrado;
            return string.IsNullOrEmpty(Papel)
                ? ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro)
                : ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro, new { Papel });
        }

        Contato = dto.Contato;
        PapelCliente = dto.Cliente;
        PapelFornecedor = dto.Fornecedor;
        PapelTransportador = dto.Transportador;
        PapelLead = dto.Lead;
        PapelProspect = dto.Prospect;
        PapelFuncionario = dto.Funcionario;

        var nomeExibicao = Contato.TipoPessoa == "fisica" ? Contato.NomeCompleto : Contato.RazaoSocial;
        _logger.LogInformation(
            "Detalhes do contato '{Nome}' (ID: {Id}) visualizados pelo usuário '{Email}'.",
            nomeExibicao, Contato.Id, UsuarioEmail);

        return Page();
    }
}
