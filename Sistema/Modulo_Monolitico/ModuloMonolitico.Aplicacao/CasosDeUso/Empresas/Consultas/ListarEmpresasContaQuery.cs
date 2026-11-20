using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarEmpresasContaQuery(Guid ContaId, string? Busca = null) : IRequest<IEnumerable<Empresa>>;

public class ListarEmpresasContaQueryHandler : IRequestHandler<ListarEmpresasContaQuery, IEnumerable<Empresa>>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public ListarEmpresasContaQueryHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public async Task<IEnumerable<Empresa>> Handle(ListarEmpresasContaQuery comando, CancellationToken cancellationToken)
    {
        var empresas = await _empresaRepositorio.ListarPorContaIdAsync(comando.ContaId);

        if (string.IsNullOrWhiteSpace(comando.Busca))
            return empresas;

        var termo = comando.Busca.Trim().ToLower();
        return empresas.Where(e =>
            (e.Nome != null && e.Nome.ToLower().Contains(termo)) ||
            (e.RazaoSocial != null && e.RazaoSocial.ToLower().Contains(termo)) ||
            (e.Cnpj != null && e.Cnpj.Contains(termo)));
    }
}
