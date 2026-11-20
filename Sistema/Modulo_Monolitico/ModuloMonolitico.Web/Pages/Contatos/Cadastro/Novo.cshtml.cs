using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class NovoModel : ContatoFormularioPageModel
{
    protected override string? ChavePermissao => "CRM.Contatos.Cadastrar";

    public NovoModel(IMediator mediator) : base(mediator)
    {
    }

    [BindProperty(SupportsGet = true)]
    public string? Papel { get; set; }

    public async Task OnGetAsync()
    {
        await CarregarListaVendedoresAsync();

        if (!string.IsNullOrEmpty(Papel))
        {
            Form.EhLead = Papel == "lead";
            Form.EhProspect = Papel == "prospect";
            Form.EhCliente = Papel == "cliente";
            Form.EhFornecedor = Papel == "fornecedor";
            Form.EhFuncionario = Papel == "funcionario";
            Form.EhTransportador = Papel == "transportador";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var contatoId = Guid.NewGuid();
        Form.Id = contatoId;

        var command = new CadastrarContatoCommand
        {
            Id = contatoId,
            ContaId = ContaId,
            EmpresaId = EmpresaAtiva!.Value,
            TipoPessoa = Form.TipoPessoa,
            NomeCompleto = Form.NomeCompleto,
            Cpf = Form.Cpf,
            Rg = Form.Rg,
            DataNascimento = Form.DataNascimento,
            Genero = Form.Genero,
            RazaoSocial = Form.RazaoSocial,
            NomeFantasia = Form.NomeFantasia,
            Cnpj = Form.Cnpj,
            InscricaoEstadual = Form.InscricaoEstadual,
            InscricaoMunicipal = Form.InscricaoMunicipal,
            Crt = Form.Crt,
            Email = Form.Email,
            Telefone = Form.Telefone,
            EnviaEmailAgendamento = Form.EnviaEmailAgendamento,
            EnderecoEntregaId = Form.EnderecoEntregaId,
            EnderecoFaturamentoId = Form.EnderecoFaturamentoId,
            EnderecoFiscalId = Form.EnderecoFiscalId,
            EnderecoCobrancaId = Form.EnderecoCobrancaId,
            EnderecoRetiradaId = Form.EnderecoRetiradaId,
            Vendedor1Id = Form.Vendedor1Id,
            Vendedor2Id = Form.Vendedor2Id,
            Enderecos = Form.Enderecos.Select(e => new CadastrarContatoCommand.EnderecoModelo
            {
                Id = e.Id,
                Cep = e.Cep,
                Logradouro = e.Logradouro,
                Numero = e.Numero,
                Complemento = e.Complemento,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Uf = e.Uf
            }).ToList(),
            EhCliente = Form.EhCliente,
            EhFornecedor = Form.EhFornecedor,
            EhTransportador = Form.EhTransportador,
            EhLead = Form.EhLead,
            EhProspect = Form.EhProspect,
            EhFuncionario = Form.EhFuncionario,
            LimiteCreditoCliente = Form.LimiteCreditoCliente,
            DescontoPadraoCliente = Form.DescontoPadraoCliente,
            ObservacaoCliente = Form.ObservacaoCliente,
            ObservacaoFornecedor = Form.ObservacaoFornecedor,
            ObservacaoTransportador = Form.ObservacaoTransportador,
            ObservacaoLead = Form.ObservacaoLead,
            ObservacaoProspect = Form.ObservacaoProspect,
            ObservacaoFuncionario = Form.ObservacaoFuncionario
        };

        var resultado = await Mediator.Send(command);

        if (!resultado.Sucesso)
        {
            Logger.LogWarning(
                "Falha ao cadastrar contato na Empresa {EmpresaId}. Motivo: {Mensagem} | Usuário: '{Email}'.",
                EmpresaAtiva.Value, resultado.Mensagem, UsuarioEmail);
            MensagemErro = resultado.Mensagem;
            await CarregarListaVendedoresAsync();
            return Page();
        }

        LimparRascunho("instantaneo_Contatos_Novo");
        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;

        var nomeLog = !string.IsNullOrWhiteSpace(Form.NomeFantasia) ? Form.NomeFantasia : Form.NomeCompleto;
        Logger.LogInformation(
            "Novo contato '{Nome}' (ID: {Id}) criado na Empresa {EmpresaId} pelo usuário '{Email}'.",
            nomeLog, contatoId, EmpresaAtiva.Value, UsuarioEmail);

        await RegistrarAuditoriaAsync("contatos", contatoId.ToString(), "cadastrar", Form);
        return string.IsNullOrEmpty(Papel)
            ? ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro)
            : ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro, new { Papel });
    }

    public IActionResult OnGetFormularioFisica() => Partial("_FormularioFisica", Form);
    public IActionResult OnGetFormularioJuridica() => Partial("_FormularioJuridica", Form);
    public IActionResult OnGetFormularioPapeis() => Partial("_FormularioPapeis", Form);
}
