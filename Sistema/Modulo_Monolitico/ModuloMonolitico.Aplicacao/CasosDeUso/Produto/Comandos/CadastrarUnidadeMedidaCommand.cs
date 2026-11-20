using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarUnidadeMedidaCommand(Guid ContaId, string Sigla, string Nome) : IRequest<ResultadoDto>;

public class CadastrarUnidadeMedidaCommandHandler : IRequestHandler<CadastrarUnidadeMedidaCommand, ResultadoDto>
{
    private readonly IProdutoRepositorio _repositorio;

    public CadastrarUnidadeMedidaCommandHandler(IProdutoRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<ResultadoDto> Handle(CadastrarUnidadeMedidaCommand comando, CancellationToken cancellationToken)
    {
        var sigla = comando.Sigla.Trim().ToUpper();
        if (await _repositorio.UnidadeMedidaExisteAsync(comando.ContaId, sigla))
        {
            return ResultadoDto.Falha(MensagensSistema.Estoque.UnidadeJaCadastrada);
        }

        await _repositorio.CadastrarUnidadeMedidaAsync(comando.ContaId, sigla, comando.Nome.Trim());
        return ResultadoDto.ComSucesso(MensagensSistema.Estoque.UnidadeCadastrada);
    }
}
