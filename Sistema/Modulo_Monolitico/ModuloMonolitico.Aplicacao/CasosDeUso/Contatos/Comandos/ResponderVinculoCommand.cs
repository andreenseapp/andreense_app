using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ResponderVinculoCommand(
    Guid SolicitacaoId,
    Guid ContaRespondedoraId,
    bool Aceito
) : IRequest<ResponderVinculoResultado>;

public record ResponderVinculoResultado(bool Sucesso, string? Erro);

public class ResponderVinculoCommandHandler : IRequestHandler<ResponderVinculoCommand, ResponderVinculoResultado>
{
    private readonly IVinculoRepositorio _vinculoRepositorio;

    public ResponderVinculoCommandHandler(IVinculoRepositorio vinculoRepositorio)
        => _vinculoRepositorio = vinculoRepositorio;

    public async Task<ResponderVinculoResultado> Handle(ResponderVinculoCommand comando, CancellationToken cancellationToken)
    {
        var solicitacao = await _vinculoRepositorio.ObterSolicitacaoPorIdAsync(comando.SolicitacaoId);
        if (solicitacao is null)
            return new ResponderVinculoResultado(false, "Solicitação não encontrada.");

        if (solicitacao.Situacao != "pendente")
            return new ResponderVinculoResultado(false, "Esta solicitação já foi respondida.");

        var novaSituacao = comando.Aceito ? "aceito" : "recusado";
        await _vinculoRepositorio.AtualizarSituacaoSolicitacaoAsync(
            comando.SolicitacaoId, novaSituacao, comando.ContaRespondedoraId);

        if (comando.Aceito)
        {
            var vinculo = new VinculoConta
            {
                Id = Guid.NewGuid(),
                ContaAId = solicitacao.ContaSolicitanteId,
                ContaBId = comando.ContaRespondedoraId,
                Tipo = solicitacao.TipoVinculo,
                Situacao = "ativo",
                SolicitacaoId = comando.SolicitacaoId,
                DataAceite = DateTime.UtcNow,
                DataInclusao = DateTime.UtcNow
            };
            await _vinculoRepositorio.CadastrarVinculoAsync(vinculo);
        }

        return new ResponderVinculoResultado(true, null);
    }
}

public record RevogarVinculoCommand(
    Guid VinculoId,
    Guid ContaId
) : IRequest<RevogarVinculoResultado>;

public record RevogarVinculoResultado(bool Sucesso, string? Erro);

public class RevogarVinculoCommandHandler : IRequestHandler<RevogarVinculoCommand, RevogarVinculoResultado>
{
    private readonly IVinculoRepositorio _vinculoRepositorio;

    public RevogarVinculoCommandHandler(IVinculoRepositorio vinculoRepositorio)
        => _vinculoRepositorio = vinculoRepositorio;

    public async Task<RevogarVinculoResultado> Handle(RevogarVinculoCommand comando, CancellationToken cancellationToken)
    {
        await _vinculoRepositorio.RevogarVinculoAsync(comando.VinculoId, comando.ContaId);
        return new RevogarVinculoResultado(true, null);
    }
}



