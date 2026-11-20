using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarGruposQuery(Guid EmpresaId, string? OrdenarPor = "nome", bool Descendente = false, int Limite = 50, int Pagina = 1) : IRequest<ResultadoPaginado<GrupoUsuario>>;

public class ListarGruposQueryHandler : IRequestHandler<ListarGruposQuery, ResultadoPaginado<GrupoUsuario>>
{
    private readonly IGrupoUsuarioRepositorio _grupoRepo;

    public ListarGruposQueryHandler(IGrupoUsuarioRepositorio grupoRepo)
    {
        _grupoRepo = grupoRepo;
    }

    public async Task<ResultadoPaginado<GrupoUsuario>> Handle(ListarGruposQuery comando, CancellationToken cancellationToken)
        => await _grupoRepo.ListarPorEmpresaIdAsync(comando.EmpresaId, comando.OrdenarPor, comando.Descendente, comando.Limite, comando.Pagina);
}
