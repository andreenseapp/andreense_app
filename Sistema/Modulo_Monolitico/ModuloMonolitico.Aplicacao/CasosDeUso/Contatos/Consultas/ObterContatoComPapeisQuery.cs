using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ContatoComPapeisResultado(
    Contato? Contato,
    EmpresaCliente? Cliente,
    EmpresaFornecedor? Fornecedor,
    EmpresaTransportador? Transportador,
    EmpresaLead? Lead,
    EmpresaProspect? Prospect,
    EmpresaFuncionario? Funcionario);

public record ObterContatoComPapeisQuery(Guid Id, Guid ContaId, Guid EmpresaId) : IRequest<ContatoComPapeisResultado>;

public class ObterContatoComPapeisQueryHandler : IRequestHandler<ObterContatoComPapeisQuery, ContatoComPapeisResultado>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public ObterContatoComPapeisQueryHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public async Task<ContatoComPapeisResultado> Handle(ObterContatoComPapeisQuery comando, CancellationToken cancellationToken)
    {
        var contato = await _contatoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
        if (contato == null)
            return new ContatoComPapeisResultado(null, null, null, null, null, null, null);

        return new ContatoComPapeisResultado(
            contato,
            await _contatoRepositorio.ObterPapelClienteAsync(comando.Id, comando.EmpresaId),
            await _contatoRepositorio.ObterPapelFornecedorAsync(comando.Id, comando.EmpresaId),
            await _contatoRepositorio.ObterPapelTransportadorAsync(comando.Id, comando.EmpresaId),
            await _contatoRepositorio.ObterPapelLeadAsync(comando.Id, comando.EmpresaId),
            await _contatoRepositorio.ObterPapelProspectAsync(comando.Id, comando.EmpresaId),
            await _contatoRepositorio.ObterPapelFuncionarioAsync(comando.Id, comando.EmpresaId));
    }
}
