using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class ObterDocumentosAtivosParaAceiteQuery : IRequest<IEnumerable<DocumentoLegal>>
    {
    }

    public class ObterDocumentosAtivosParaAceiteQueryHandler : IRequestHandler<ObterDocumentosAtivosParaAceiteQuery, IEnumerable<DocumentoLegal>>
    {
        private readonly IDocumentoRepositorio _documentoRepositorio;

        public ObterDocumentosAtivosParaAceiteQueryHandler(IDocumentoRepositorio documentoRepositorio)
        {
            _documentoRepositorio = documentoRepositorio;
        }

        public async Task<IEnumerable<DocumentoLegal>> Handle(ObterDocumentosAtivosParaAceiteQuery comando, CancellationToken cancellationToken)
        {
            var documentos = await _documentoRepositorio.ListarDocumentosAsync();
            var ativos = documentos
                .Where(d => d.Situacao == "ativo")
                .GroupBy(d => d.Tipo)
                .Select(g => g.First())
                .ToList();

            return ativos;
        }
    }
}


