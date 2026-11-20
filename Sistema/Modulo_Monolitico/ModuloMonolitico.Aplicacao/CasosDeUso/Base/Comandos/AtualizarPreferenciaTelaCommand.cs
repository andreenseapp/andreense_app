using MediatR;
using ModuloMonolitico.Dominio.Entidades.Base;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Base;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Base.Comandos;

public record AtualizarPreferenciaTelaCommand(Guid UsuarioId, Guid EmpresaId, string CaminhoTela, string TabelaId, string ColunasOcultasJson) : IRequest;

public class AtualizarPreferenciaTelaCommandHandler : IRequestHandler<AtualizarPreferenciaTelaCommand>
{
    private readonly IPreferenciaColunasTabelaRepositorio _repo;

    public AtualizarPreferenciaTelaCommandHandler(IPreferenciaColunasTabelaRepositorio repo)
    {
        _repo = repo;
    }

    public async Task Handle(AtualizarPreferenciaTelaCommand comando, CancellationToken cancellationToken)
    {
        var pref = new PreferenciaTelaUsuario
        {
            Id = Guid.NewGuid(),
            UsuarioId = comando.UsuarioId,
            EmpresaId = comando.EmpresaId,
            CaminhoTela = comando.CaminhoTela,
            TabelaId = comando.TabelaId,
            ColunasOcultasJson = comando.ColunasOcultasJson,
            DataAtualizacao = DateTime.UtcNow
        };

        await _repo.AtualizarAsync(pref);
    }
}
