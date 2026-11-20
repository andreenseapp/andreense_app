using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarDocumentoLegalCommand(DocumentoLegal Documento) : IRequest<Guid>;

public class CadastrarDocumentoLegalCommandHandler : IRequestHandler<CadastrarDocumentoLegalCommand, Guid>
{
    private readonly IDocumentoRepositorio _documentoRepositorio;

    public CadastrarDocumentoLegalCommandHandler(IDocumentoRepositorio documentoRepositorio)
        => _documentoRepositorio = documentoRepositorio;

    public Task<Guid> Handle(CadastrarDocumentoLegalCommand comando, CancellationToken cancellationToken)
        => _documentoRepositorio.CadastrarAsync(comando.Documento);
}
