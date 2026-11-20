using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarAchadoPerdidoCommand(
    Guid UsuarioId,
    string Tipo,
    string Titulo,
    string? Descricao,
    string? LocalVisto,
    DateTime? DataEvento,
    string? ContatoInfo,
    bool EsconderContato,
    byte[]? FotoBytes,
    string? FotoFileName,
    string? FotoContentType
) : IRequest<ResultadoDto<AchadoPerdido>>;

public class CadastrarAchadoPerdidoCommandHandler : IRequestHandler<CadastrarAchadoPerdidoCommand, ResultadoDto<AchadoPerdido>>
{
    private readonly IAchadosPerdidosRepositorio _repositorio;
    private readonly IServicoArmazenamento _servicoArmazenamento;

    public CadastrarAchadoPerdidoCommandHandler(
        IAchadosPerdidosRepositorio repositorio,
        IServicoArmazenamento servicoArmazenamento)
    {
        _repositorio = repositorio;
        _servicoArmazenamento = servicoArmazenamento;
    }

    public async Task<ResultadoDto<AchadoPerdido>> Handle(CadastrarAchadoPerdidoCommand comando, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var item = new AchadoPerdido
        {
            Id = id,
            UsuarioId = comando.UsuarioId,
            Tipo = comando.Tipo,
            Titulo = comando.Titulo,
            Descricao = comando.Descricao,
            LocalVisto = comando.LocalVisto,
            DataEvento = comando.DataEvento,
            ContatoInfo = comando.ContatoInfo,
            EsconderContato = comando.EsconderContato,
            Situacao = "ativo",
            DataInclusao = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow
        };

        if (comando.FotoBytes is { Length: > 0 } && !string.IsNullOrEmpty(comando.FotoFileName))
        {
            var ext = Path.GetExtension(comando.FotoFileName);
            var nomeArquivo = ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.StorageAchadosPerdidos(id, ext);
            using var stream = new MemoryStream(comando.FotoBytes);
            item.UrlImagem = await _servicoArmazenamento.EnviarArquivoAsync(
                stream, nomeArquivo, comando.FotoContentType ?? "application/octet-stream");
        }

        await _repositorio.CadastrarAsync(item);
        return ResultadoDto<AchadoPerdido>.ComSucesso(item, MensagensSistema.Cidadania.PublicacaoRealizadaComSucesso);
    }
}
