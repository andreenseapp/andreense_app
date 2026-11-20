using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarDocumentoLegalCommand(DocumentoLegal Documento) : IRequest<bool>;

public class AtualizarDocumentoLegalCommandHandler : IRequestHandler<AtualizarDocumentoLegalCommand, bool>
{
    private readonly IDocumentoRepositorio _documentoRepositorio;

    public AtualizarDocumentoLegalCommandHandler(IDocumentoRepositorio documentoRepositorio)
        => _documentoRepositorio = documentoRepositorio;

    public Task<bool> Handle(AtualizarDocumentoLegalCommand comando, CancellationToken cancellationToken)
        => _documentoRepositorio.AtualizarAsync(comando.Documento);
}
