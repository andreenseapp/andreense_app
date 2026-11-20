using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarCategoriaProdutoCommand(Guid ContaId, string Nome) : IRequest<ResultadoDto>;

public class CadastrarCategoriaProdutoCommandHandler : IRequestHandler<CadastrarCategoriaProdutoCommand, ResultadoDto>
{
    private readonly IProdutoRepositorio _repositorio;

    public CadastrarCategoriaProdutoCommandHandler(IProdutoRepositorio repositorio)
        => _repositorio = repositorio;

    public async Task<ResultadoDto> Handle(CadastrarCategoriaProdutoCommand comando, CancellationToken cancellationToken)
    {
        await _repositorio.CadastrarCategoriaAsync(comando.ContaId, comando.Nome.Trim());
        return ResultadoDto.ComSucesso(MensagensSistema.Produto.CategoriaCadastrada);
    }
}
