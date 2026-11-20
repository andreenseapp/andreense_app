using ModuloMonolitico.Dominio.Entidades.Base;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Base;

public interface IPreferenciaColunasTabelaRepositorio
{
    Task AtualizarAsync(PreferenciaTelaUsuario preferencia);
    Task<PreferenciaTelaUsuario?> ObterPreferenciaAsync(Guid usuarioId, Guid empresaId, string caminhoTela, string tabelaId);
}
