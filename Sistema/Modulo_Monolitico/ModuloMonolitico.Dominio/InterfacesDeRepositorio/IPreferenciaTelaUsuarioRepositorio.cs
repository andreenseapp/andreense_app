using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IPreferenciaTelaUsuarioRepositorio
{
    Task<PreferenciaTelaUsuario?> ObterPreferenciaAsync(Guid contaId, Guid usuarioId, string telaIdentificador);
    Task AtualizarPreferenciaAsync(PreferenciaTelaUsuario preferencia);
}
