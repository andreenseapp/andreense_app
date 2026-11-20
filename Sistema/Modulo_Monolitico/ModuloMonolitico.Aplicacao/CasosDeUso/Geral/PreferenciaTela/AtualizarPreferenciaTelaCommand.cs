using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Geral.PreferenciaTela;

public record AtualizarPreferenciaTelaCommand(Guid ContaId, Guid UsuarioId, string TelaIdentificador, string ColunasVisiveisJson) : IRequest;

public class AtualizarPreferenciaTelaCommandHandler : IRequestHandler<AtualizarPreferenciaTelaCommand>
{
    private readonly IPreferenciaTelaUsuarioRepositorio _repositorio;

    public AtualizarPreferenciaTelaCommandHandler(IPreferenciaTelaUsuarioRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task Handle(AtualizarPreferenciaTelaCommand comando, CancellationToken cancellationToken)
    {
        var preferencia = new PreferenciaTelaUsuario
        {
            ContaId = comando.ContaId,
            UsuarioId = comando.UsuarioId,
            TelaIdentificador = comando.TelaIdentificador,
            ColunasVisiveisJson = comando.ColunasVisiveisJson,
            DataUltimaAtualizacao = DateTime.UtcNow
        };

        await _repositorio.AtualizarPreferenciaAsync(preferencia);
    }
}
