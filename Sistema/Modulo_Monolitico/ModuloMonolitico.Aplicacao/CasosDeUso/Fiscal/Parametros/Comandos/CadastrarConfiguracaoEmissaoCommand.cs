using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarConfiguracaoEmissaoCommand(
    Guid EmpresaId,
    int Ambiente,
    int Serie,
    int Modelo,
    int ProximoNumeroNf,
    string? CscId,
    string? CscToken,
    int TipoEmissaoPadrao,
    int TipoImpressaoPadrao,
    string TratamentoDesconto,
    bool Ativo
) : IRequest<ResultadoDto>;

public class CadastrarConfiguracaoEmissaoCommandHandler : IRequestHandler<CadastrarConfiguracaoEmissaoCommand, ResultadoDto>
{
    private readonly IFiscalRepositorio _fiscalRepositorio;

    public CadastrarConfiguracaoEmissaoCommandHandler(IFiscalRepositorio fiscalRepositorio)
        => _fiscalRepositorio = fiscalRepositorio;

    public async Task<ResultadoDto> Handle(CadastrarConfiguracaoEmissaoCommand comando, CancellationToken cancellationToken)
    {
        var certAtivo = await _fiscalRepositorio.ObterCertificadoPorEmpresaAsync(comando.EmpresaId);

        var config = new ConfiguracaoEmissao
        {
            Id = Guid.Empty,
            EmpresaId = comando.EmpresaId,
            CertificadoId = certAtivo?.Id,
            Ambiente = comando.Ambiente,
            Serie = comando.Serie,
            Modelo = comando.Modelo,
            ProximoNumeroNf = comando.ProximoNumeroNf,
            CscId = comando.CscId,
            CscToken = comando.CscToken,
            TipoEmissaoPadrao = comando.TipoEmissaoPadrao,
            TipoImpressaoPadrao = comando.TipoImpressaoPadrao,
            TratamentoDesconto = comando.TratamentoDesconto,
            Situacao = comando.Ativo ? "ativo" : "inativo"
        };

        await _fiscalRepositorio.CadastrarConfiguracaoEmissaoAsync(config);
        return ResultadoDto.ComSucesso(MensagensSistema.Fiscal.SerieCadastrada);
    }
}
