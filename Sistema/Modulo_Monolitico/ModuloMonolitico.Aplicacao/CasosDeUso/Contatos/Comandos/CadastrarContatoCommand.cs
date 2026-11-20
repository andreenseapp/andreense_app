using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public class CadastrarContatoCommand : IRequest<ResultadoDto>
{
    public Guid? Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public string TipoPessoa { get; set; } = "fisica";
    public string? NomeCompleto { get; set; }
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Genero { get; set; }
    public string? RazaoSocial { get; set; }
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    public string? InscricaoEstadual { get; set; }
    public string? InscricaoMunicipal { get; set; }
    public string? Crt { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public bool EnviaEmailAgendamento { get; set; } = true;
    public Guid? Vendedor1Id { get; set; }
    public Guid? Vendedor2Id { get; set; }
    public Guid? EnderecoEntregaId { get; set; }
    public Guid? EnderecoFaturamentoId { get; set; }
    public Guid? EnderecoFiscalId { get; set; }
    public Guid? EnderecoCobrancaId { get; set; }
    public Guid? EnderecoRetiradaId { get; set; }
    public List<EnderecoModelo> Enderecos { get; set; } = new();
    public bool EhCliente { get; set; }
    public decimal LimiteCreditoCliente { get; set; }
    public decimal DescontoPadraoCliente { get; set; }
    public string? ObservacaoCliente { get; set; }
    public bool EhFornecedor { get; set; }
    public string? ObservacaoFornecedor { get; set; }
    public bool EhTransportador { get; set; }
    public string? ObservacaoTransportador { get; set; }
    public bool EhLead { get; set; }
    public string? ObservacaoLead { get; set; }
    public bool EhProspect { get; set; }
    public string? ObservacaoProspect { get; set; }
    public bool EhFuncionario { get; set; }
    public string? ObservacaoFuncionario { get; set; }

    public class EnderecoModelo
    {
        public Guid? Id { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
    }
}

public class CadastrarContatoCommandHandler : IRequestHandler<CadastrarContatoCommand, ResultadoDto>
{
    private readonly IContatoRepositorio _contatoRepo;

    public CadastrarContatoCommandHandler(IContatoRepositorio contatoRepo)
    {
        _contatoRepo = contatoRepo;
    }

    public async Task<ResultadoDto> Handle(CadastrarContatoCommand comando, CancellationToken cancellationToken)
    {
        var contatoId = comando.Id ?? Guid.NewGuid();

        var contato = new Contato
        {
            Id = contatoId,
            ContaId = comando.ContaId,
            EmpresaId = comando.EmpresaId,
            TipoPessoa = comando.TipoPessoa,
            NomeCompleto = comando.NomeCompleto,
            Cpf = comando.Cpf,
            Rg = comando.Rg,
            DataNascimento = comando.DataNascimento,
            Genero = comando.Genero,
            RazaoSocial = comando.RazaoSocial,
            NomeFantasia = comando.NomeFantasia,
            Cnpj = comando.Cnpj,
            InscricaoEstadual = comando.InscricaoEstadual,
            InscricaoMunicipal = comando.InscricaoMunicipal,
            Crt = comando.Crt,
            Email = comando.Email,
            Telefone = comando.Telefone,
            EnviaEmailAgendamento = comando.EnviaEmailAgendamento,
            Vendedor1Id = comando.Vendedor1Id,
            Vendedor2Id = comando.Vendedor2Id,
            EnderecoEntregaId = comando.EnderecoEntregaId,
            EnderecoFaturamentoId = comando.EnderecoFaturamentoId,
            EnderecoFiscalId = comando.EnderecoFiscalId,
            EnderecoCobrancaId = comando.EnderecoCobrancaId,
            EnderecoRetiradaId = comando.EnderecoRetiradaId,
            Enderecos = comando.Enderecos.Select(e => new Endereco
            {
                Id = e.Id ?? Guid.NewGuid(),
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Cep = e.Cep,
                Logradouro = e.Logradouro,
                Numero = e.Numero,
                Complemento = e.Complemento,
                Bairro = e.Bairro,
                Cidade = e.Cidade,
                Uf = e.Uf
            }).ToList()
        };

        var papeis = new List<IContatoPapel>();

        if (comando.EhCliente)
            papeis.Add(new EmpresaCliente
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                LimiteCredito = comando.LimiteCreditoCliente,
                DescontoPadrao = comando.DescontoPadraoCliente,
                Observacao = comando.ObservacaoCliente
            });

        if (comando.EhFornecedor)
            papeis.Add(new EmpresaFornecedor
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Observacao = comando.ObservacaoFornecedor
            });

        if (comando.EhTransportador)
            papeis.Add(new EmpresaTransportador
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Observacao = comando.ObservacaoTransportador
            });

        if (comando.EhLead)
            papeis.Add(new EmpresaLead
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Observacao = comando.ObservacaoLead
            });

        if (comando.EhProspect)
            papeis.Add(new EmpresaProspect
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Observacao = comando.ObservacaoProspect
            });

        if (comando.EhFuncionario)
            papeis.Add(new EmpresaFuncionario
            {
                ContatoId = contatoId,
                ContaId = comando.ContaId,
                Observacao = comando.ObservacaoFuncionario
            });

        await _contatoRepo.CadastrarContatoComPapeisAsync(contato, papeis);

        return ResultadoDto.ComSucesso("Contato cadastrado com sucesso.");
    }
}



