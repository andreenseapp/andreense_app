using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public interface IFiscalRepositorio
{
    Task<CertificadoDigital?> ObterCertificadoPorEmpresaAsync(Guid empresaId);
    Task CadastrarCertificadoAsync(CertificadoDigital certificado);
    Task DeletarSeguroCertificadoAsync(Guid empresaId);
    Task<IEnumerable<ConfiguracaoEmissao>> ListarConfiguracoesEmissaoAsync(Guid empresaId);
    Task<ConfiguracaoEmissao?> ObterConfiguracaoEmissaoPorIdAsync(Guid empresaId, Guid id);
    Task CadastrarConfiguracaoEmissaoAsync(ConfiguracaoEmissao config);
    Task AtualizarConfiguracaoEmissaoAsync(ConfiguracaoEmissao config);
    Task DeletarSeguroConfiguracaoEmissaoAsync(Guid empresaId, Guid id);
}


