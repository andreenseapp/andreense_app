using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Geral.PreferenciaTela;

public record ObterPreferenciaTelaQuery(Guid ContaId, Guid UsuarioId, string TelaIdentificador) : IRequest<string?>;

public class ObterPreferenciaTelaQueryHandler : IRequestHandler<ObterPreferenciaTelaQuery, string?>
{
    private readonly IPreferenciaTelaUsuarioRepositorio _repositorio;

    public ObterPreferenciaTelaQueryHandler(IPreferenciaTelaUsuarioRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<string?> Handle(ObterPreferenciaTelaQuery comando, CancellationToken cancellationToken)
    {
        var preferencia = await _repositorio.ObterPreferenciaAsync(comando.ContaId, comando.UsuarioId, comando.TelaIdentificador);
        return preferencia?.ColunasVisiveisJson;
    }
}
