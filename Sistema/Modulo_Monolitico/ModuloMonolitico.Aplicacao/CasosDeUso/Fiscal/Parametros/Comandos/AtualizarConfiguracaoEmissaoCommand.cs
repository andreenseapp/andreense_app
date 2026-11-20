using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarConfiguracaoEmissaoCommand(
    Guid EmpresaId,
    Guid Id,
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

public class AtualizarConfiguracaoEmissaoCommandHandler : IRequestHandler<AtualizarConfiguracaoEmissaoCommand, ResultadoDto>
{
    private readonly IFiscalRepositorio _fiscalRepositorio;

    public AtualizarConfiguracaoEmissaoCommandHandler(IFiscalRepositorio fiscalRepositorio)
        => _fiscalRepositorio = fiscalRepositorio;

    public async Task<ResultadoDto> Handle(AtualizarConfiguracaoEmissaoCommand comando, CancellationToken cancellationToken)
    {
        var config = await _fiscalRepositorio.ObterConfiguracaoEmissaoPorIdAsync(comando.EmpresaId, comando.Id);
        if (config == null)
            return ResultadoDto.Falha(MensagensSistema.Fiscal.ConfiguracaoNaoEncontrada);

        config.Ambiente = comando.Ambiente;
        config.Serie = comando.Serie;
        config.Modelo = comando.Modelo;
        config.ProximoNumeroNf = comando.ProximoNumeroNf;
        config.CscId = comando.CscId;
        config.CscToken = comando.CscToken;
        config.TipoEmissaoPadrao = comando.TipoEmissaoPadrao;
        config.TipoImpressaoPadrao = comando.TipoImpressaoPadrao;
        config.TratamentoDesconto = comando.TratamentoDesconto;
        config.Situacao = comando.Ativo ? "ativo" : "inativo";

        await _fiscalRepositorio.AtualizarConfiguracaoEmissaoAsync(config);
        return ResultadoDto.ComSucesso(MensagensSistema.Fiscal.SerieAtualizada);
    }
}
