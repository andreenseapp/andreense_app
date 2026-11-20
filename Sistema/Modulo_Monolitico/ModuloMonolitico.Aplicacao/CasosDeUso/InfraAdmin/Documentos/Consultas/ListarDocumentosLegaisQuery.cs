using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ListarDocumentosLegaisQuery() : IRequest<IEnumerable<DocumentoLegal>>;

public class ListarDocumentosLegaisQueryHandler : IRequestHandler<ListarDocumentosLegaisQuery, IEnumerable<DocumentoLegal>>
{
    private readonly IDocumentoRepositorio _documentoRepositorio;

    public ListarDocumentosLegaisQueryHandler(IDocumentoRepositorio documentoRepositorio)
        => _documentoRepositorio = documentoRepositorio;

    public Task<IEnumerable<DocumentoLegal>> Handle(ListarDocumentosLegaisQuery comando, CancellationToken cancellationToken)
        => _documentoRepositorio.ListarDocumentosAsync();
}
