using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarEquipeQuery(Guid EmpresaId, int Pagina = 1, int TamanhoPagina = 10) : IRequest<(IEnumerable<MembroEquipeDto> Itens, int Total)>;

public class ListarEquipeQueryHandler : IRequestHandler<ListarEquipeQuery, (IEnumerable<MembroEquipeDto> Itens, int Total)>
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public ListarEquipeQueryHandler(IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task<(IEnumerable<MembroEquipeDto> Itens, int Total)> Handle(ListarEquipeQuery comando, CancellationToken cancellationToken)
    {
        return await _usuarioRepositorio.ListarEquipeDaEmpresaAsync(comando.EmpresaId, comando.Pagina, comando.TamanhoPagina);
    }
}



