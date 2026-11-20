using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class RegistrarAceiteCommand : IRequest<bool>
    {
        public Guid UsuarioId { get; set; }
        public Guid DocumentoLegalId { get; set; }
    }

    public class RegistrarAceiteCommandHandler : IRequestHandler<RegistrarAceiteCommand, bool>
    {
        private readonly IDocumentoRepositorio _documentoRepositorio;

        public RegistrarAceiteCommandHandler(IDocumentoRepositorio documentoRepositorio)
        {
            _documentoRepositorio = documentoRepositorio;
        }

        public async Task<bool> Handle(RegistrarAceiteCommand comando, CancellationToken cancellationToken)
        {
            var aceite = new DocumentoAceite
            {
                UsuarioId = comando.UsuarioId,
                DocumentoLegalId = comando.DocumentoLegalId
            };

            return await _documentoRepositorio.RegistrarAceiteAsync(aceite);
        }
    }
}


