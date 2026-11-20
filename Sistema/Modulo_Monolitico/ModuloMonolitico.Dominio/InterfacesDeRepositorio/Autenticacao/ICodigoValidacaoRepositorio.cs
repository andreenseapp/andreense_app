namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface ICodigoValidacaoRepositorio
    {
        Task CadastrarAsync(string email, string codigo, string tipo, DateTime expiraEm);
        Task<bool> ValidarAsync(string email, string codigo, string tipo);
        Task<bool> ExisteCodigoValidoAsync(string email, string codigo, string tipo);
        Task<bool> PossuiCodigoPendenteAsync(string email, string tipo);
        Task<string?> ObterCodigoPendenteMaisRecenteAsync(string email, string tipo);
        Task MarcarComoValidadoAsync(string email);
    }
}
