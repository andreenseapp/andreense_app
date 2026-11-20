using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record DeletarSeguroConfiguracaoEmissaoCommand(Guid EmpresaId, Guid Id) : IRequest<ResultadoDto>;

public class DeletarSeguroConfiguracaoEmissaoCommandHandler : IRequestHandler<DeletarSeguroConfiguracaoEmissaoCommand, ResultadoDto>
{
    private readonly IFiscalRepositorio _fiscalRepositorio;

    public DeletarSeguroConfiguracaoEmissaoCommandHandler(IFiscalRepositorio fiscalRepositorio)
        => _fiscalRepositorio = fiscalRepositorio;

    public async Task<ResultadoDto> Handle(DeletarSeguroConfiguracaoEmissaoCommand comando, CancellationToken cancellationToken)
    {
        await _fiscalRepositorio.DeletarSeguroConfiguracaoEmissaoAsync(comando.EmpresaId, comando.Id);
        return ResultadoDto.ComSucesso(MensagensSistema.Fiscal.SerieRemovida);
    }
}
