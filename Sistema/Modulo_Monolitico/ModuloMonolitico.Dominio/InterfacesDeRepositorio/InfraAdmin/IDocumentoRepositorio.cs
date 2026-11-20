using ModuloMonolitico.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio
{
    public interface IDocumentoRepositorio
    {
        Task<IEnumerable<DocumentoLegal>> ListarDocumentosAsync();
        Task<DocumentoLegal?> ObterPorIdAsync(Guid id);
        Task<DocumentoLegal?> ObterUltimaVersaoAtivaAsync(string tipo);
        Task<Guid> CadastrarAsync(DocumentoLegal documento);
        Task<bool> AtualizarAsync(DocumentoLegal documento);
        Task<bool> RegistrarAceiteAsync(DocumentoAceite aceite);
        Task<bool> UsuarioJaAceitouVersaoAtualAsync(Guid usuarioId, string tipo);
    }
}



