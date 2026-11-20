using MediatR;
using Microsoft.AspNetCore.Mvc;
using ModuloMonolitico.Aplicacao.CasosDeUso;
using ModuloMonolitico.Web.Pages.Shared;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class EditarModel : ContatoFormularioPageModel
{
    protected override string? ChavePermissao => "CRM.Contatos.Editar";

    public EditarModel(IMediator mediator) : base(mediator)
    {
    }

    [BindProperty(SupportsGet = true)]
    public string? Papel { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        await CarregarForm(id);
        await CarregarListaVendedoresAsync();

        if (Form.Id == null || Form.Id == Guid.Empty)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var command = new AtualizarContatoCommand
        {
            Id = Form.Id ?? Guid.Empty,
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
                "Falha ao atualizar contato (ID: {Id}) na Empresa {EmpresaId}. Motivo: {Mensagem} | Usuário: '{Email}'.",
                Form.Id, EmpresaAtiva.Value, resultado.Mensagem, UsuarioEmail);
            MensagemErro = resultado.Mensagem;
            await CarregarListaVendedoresAsync();
            return Page();
        }

        MensagemSucesso = ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso;

        var nomeLog = !string.IsNullOrWhiteSpace(Form.NomeFantasia) ? Form.NomeFantasia : Form.NomeCompleto;
        Logger.LogInformation(
            "Contato '{Nome}' (ID: {Id}) atualizado na Empresa {EmpresaId} pelo usuário '{Email}'.",
            nomeLog, Form.Id, EmpresaAtiva.Value, UsuarioEmail);

        await RegistrarAuditoriaAsync("contatos", Form.Id.ToString()!, "editar", Form);
        return string.IsNullOrEmpty(Papel)
            ? ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro)
            : ModuloRotas.Redirecionar(ModuloRotas.Contatos.Cadastro, new { Papel });
    }

    private async Task CarregarForm(Guid id)
    {
        var dto = await Mediator.Send(new ObterContatoComPapeisQuery(id, ContaId, EmpresaAtiva!.Value));
        var contato = dto.Contato;

        if (contato == null) return;

        Form = new ContatoFormularioModelo
        {
            Id = contato.Id,
            TipoPessoa = contato.TipoPessoa,
            Email = contato.Email,
            Telefone = contato.Telefone,
            EnviaEmailAgendamento = contato.EnviaEmailAgendamento,
            EnderecoEntregaId = contato.EnderecoEntregaId,
            EnderecoFaturamentoId = contato.EnderecoFaturamentoId,
            EnderecoFiscalId = contato.EnderecoFiscalId,
            EnderecoCobrancaId = contato.EnderecoCobrancaId,
            EnderecoRetiradaId = contato.EnderecoRetiradaId,
            Vendedor1Id = contato.Vendedor1Id,
            Vendedor2Id = contato.Vendedor2Id,
            Enderecos = contato.Enderecos.Select(e => new ContatoFormularioModelo.EnderecoModelo
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
            NomeCompleto = contato.NomeCompleto,
            Cpf = contato.Cpf,
            Rg = contato.Rg,
            DataNascimento = contato.DataNascimento,
            Genero = contato.Genero,
            RazaoSocial = contato.RazaoSocial,
            NomeFantasia = contato.NomeFantasia,
            Cnpj = contato.Cnpj,
            InscricaoEstadual = contato.InscricaoEstadual,
            InscricaoMunicipal = contato.InscricaoMunicipal,
            Crt = contato.Crt
        };

        if (dto.Cliente != null)
        {
            Form.EhCliente = true;
            Form.LimiteCreditoCliente = dto.Cliente.LimiteCredito;
            Form.DescontoPadraoCliente = dto.Cliente.DescontoPadrao;
            Form.ObservacaoCliente = dto.Cliente.Observacao;
        }

        if (dto.Fornecedor != null)
        {
            Form.EhFornecedor = true;
            Form.ObservacaoFornecedor = dto.Fornecedor.Observacao;
        }

        if (dto.Transportador != null)
        {
            Form.EhTransportador = true;
            Form.ObservacaoTransportador = dto.Transportador.Observacao;
        }

        if (dto.Lead != null)
        {
            Form.EhLead = true;
            Form.ObservacaoLead = dto.Lead.Observacao;
        }

        if (dto.Prospect != null)
        {
            Form.EhProspect = true;
            Form.ObservacaoProspect = dto.Prospect.Observacao;
        }

        if (dto.Funcionario != null)
        {
            Form.EhFuncionario = true;
            Form.ObservacaoFuncionario = dto.Funcionario.Observacao;
        }
    }
}
