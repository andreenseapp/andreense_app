using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterDocumentoLegalQuery(Guid Id) : IRequest<DocumentoLegal?>;

public class ObterDocumentoLegalQueryHandler : IRequestHandler<ObterDocumentoLegalQuery, DocumentoLegal?>
{
    private readonly IDocumentoRepositorio _documentoRepositorio;

    public ObterDocumentoLegalQueryHandler(IDocumentoRepositorio documentoRepositorio)
        => _documentoRepositorio = documentoRepositorio;

    public Task<DocumentoLegal?> Handle(ObterDocumentoLegalQuery comando, CancellationToken cancellationToken)
        => _documentoRepositorio.ObterPorIdAsync(comando.Id);
}
