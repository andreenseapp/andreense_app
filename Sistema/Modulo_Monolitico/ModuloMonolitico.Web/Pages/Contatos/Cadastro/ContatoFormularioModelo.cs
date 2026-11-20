using System.ComponentModel.DataAnnotations;

namespace ModuloMonolitico.Web.Pages.Contatos.Cadastro;

public class ContatoFormularioModelo
{
  public Guid? Id { get; set; }

  [Display(Name = "Tipo de pessoa")]
  public string TipoPessoa { get; set; } = "fisica";

  [Display(Name = "Nome completo")]
  public string? NomeCompleto { get; set; }

  [Display(Name = "CPF")]
  public string? Cpf { get; set; }

  [Display(Name = "RG")]
  public string? Rg { get; set; }

  [Display(Name = "Data de nascimento")]
  public DateTime? DataNascimento { get; set; }

  [Display(Name = "Gênero")]
  public string? Genero { get; set; }

  [Display(Name = "Razão social")]
  public string? RazaoSocial { get; set; }

  [Display(Name = "Nome fantasia")]
  public string? NomeFantasia { get; set; }

  [Display(Name = "CNPJ")]
  public string? Cnpj { get; set; }

  [Display(Name = "Inscrição estadual")]
  public string? InscricaoEstadual { get; set; }

  [Display(Name = "Inscrição municipal")]
  public string? InscricaoMunicipal { get; set; }

  [Display(Name = "Regime tributário (CRT)")]
  public string? Crt { get; set; }

  [Display(Name = "Endereço de entrega")]
  public Guid? EnderecoEntregaId { get; set; }

  [Display(Name = "Endereço de faturamento")]
  public Guid? EnderecoFaturamentoId { get; set; }

  [Display(Name = "Endereço fiscal")]
  public Guid? EnderecoFiscalId { get; set; }

  [Display(Name = "Endereço de cobrança")]
  public Guid? EnderecoCobrancaId { get; set; }

  [Display(Name = "Endereço de retirada")]
  public Guid? EnderecoRetiradaId { get; set; }

  public List<EnderecoModelo> Enderecos { get; set; } = new();

  public class EnderecoModelo
  {
    public Guid Id { get; set; } = Guid.NewGuid();

    [Display(Name = "CEP")]
    public string? Cep { get; set; }

    [Display(Name = "Logradouro")]
    public string? Logradouro { get; set; }

    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

    [Display(Name = "Cidade")]
    public string? Cidade { get; set; }

    [Display(Name = "UF")]
    public string? Uf { get; set; }
  }

  [Display(Name = "E-mail")]
  [EmailAddress]
  public string? Email { get; set; }

  [Display(Name = "Telefone")]
  public string? Telefone { get; set; }

  [Display(Name = "Enviar e-mails de agendamento")]
  public bool EnviaEmailAgendamento { get; set; } = true;

  [Display(Name = "Vendedor principal")]
  public Guid? Vendedor1Id { get; set; }

  [Display(Name = "Vendedor secundário")]
  public Guid? Vendedor2Id { get; set; }

  [Display(Name = "Cliente")]
  public bool EhCliente { get; set; }

  [Display(Name = "Fornecedor")]
  public bool EhFornecedor { get; set; }

  [Display(Name = "Transportador")]
  public bool EhTransportador { get; set; }

  [Display(Name = "Lead")]
  public bool EhLead { get; set; }

  [Display(Name = "Prospect")]
  public bool EhProspect { get; set; }

  [Display(Name = "Funcionário")]
  public bool EhFuncionario { get; set; }

  [Display(Name = "Limite de crédito")]
  public decimal LimiteCreditoCliente { get; set; }

  [Display(Name = "Desconto padrão")]
  public decimal DescontoPadraoCliente { get; set; }

  [Display(Name = "Observação do cliente")]
  public string? ObservacaoCliente { get; set; }

  [Display(Name = "Observação do fornecedor")]
  public string? ObservacaoFornecedor { get; set; }

  [Display(Name = "Observação do transportador")]
  public string? ObservacaoTransportador { get; set; }

  [Display(Name = "Observação do lead")]
  public string? ObservacaoLead { get; set; }

  [Display(Name = "Observação do prospect")]
  public string? ObservacaoProspect { get; set; }

  [Display(Name = "Observação do funcionário")]
  public string? ObservacaoFuncionario { get; set; }
}
