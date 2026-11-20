using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarMarcaProdutoCommand(Guid ContaId, string Nome) : IRequest<ResultadoDto>;

public class CadastrarMarcaProdutoCommandHandler : IRequestHandler<CadastrarMarcaProdutoCommand, ResultadoDto>
{
    private readonly IProdutoRepositorio _repositorio;

    public CadastrarMarcaProdutoCommandHandler(IProdutoRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<ResultadoDto> Handle(CadastrarMarcaProdutoCommand comando, CancellationToken cancellationToken)
    {
        await _repositorio.CadastrarMarcaAsync(comando.ContaId, comando.Nome.Trim());
        return ResultadoDto.ComSucesso(MensagensSistema.Produto.MarcaCadastrada);
    }
}
