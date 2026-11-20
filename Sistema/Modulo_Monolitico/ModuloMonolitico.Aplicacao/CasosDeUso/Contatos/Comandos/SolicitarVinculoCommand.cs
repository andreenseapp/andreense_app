using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record SolicitarVinculoCommand(
    Guid ContaSolicitanteId,
    string EmailAlvo,
    string TipoVinculo,
    string? Mensagem
) : IRequest<SolicitarVinculoResultado>;

public record SolicitarVinculoResultado(bool Sucesso, string? Erro, Guid? SolicitacaoId);

public class SolicitarVinculoCommandHandler : IRequestHandler<SolicitarVinculoCommand, SolicitarVinculoResultado>
{
    private readonly IVinculoRepositorio _vinculoRepositorio;

    public SolicitarVinculoCommandHandler(IVinculoRepositorio vinculoRepositorio)
        => _vinculoRepositorio = vinculoRepositorio;

    public async Task<SolicitarVinculoResultado> Handle(SolicitarVinculoCommand comando, CancellationToken cancellationToken)
    {
        var contaAlvoId = await _vinculoRepositorio.ObterContaIdPorEmailAsync(comando.EmailAlvo);

        var tipoValido = comando.TipoVinculo is "confianca" or "servico" or "comercial"
            ? comando.TipoVinculo : "confianca";

        var solicitacao = new SolicitacaoVinculo
        {
            Id = Guid.NewGuid(),
            ContaSolicitanteId = comando.ContaSolicitanteId,
            EmailAlvo = comando.EmailAlvo,
            ContaAlvoId = contaAlvoId,
            TipoVinculo = tipoValido,
            Mensagem = comando.Mensagem,
            Situacao = "pendente",
            DataInclusao = DateTime.UtcNow
        };

        await _vinculoRepositorio.CadastrarSolicitacaoAsync(solicitacao);
        return new SolicitarVinculoResultado(true, null, solicitacao.Id);
    }
}



