using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.InterfacesDeServico;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public class AtualizarFotoPerfilCommand : IRequest<ResultadoDto>
{
    public Guid UsuarioId { get; set; }
    public Stream ArquivoStream { get; set; } = Stream.Null;
    public string NomeArquivo { get; set; } = string.Empty;
    public string TipoConteudo { get; set; } = string.Empty;
}

public class AtualizarFotoPerfilCommandHandler : IRequestHandler<AtualizarFotoPerfilCommand, ResultadoDto>
{
    private readonly IUsuarioRepositorio _usuarioRepo;
    private readonly IServicoArmazenamento _servicoArmazenamento;

    public AtualizarFotoPerfilCommandHandler(IUsuarioRepositorio usuarioRepo, IServicoArmazenamento servicoArmazenamento)
    {
        _usuarioRepo = usuarioRepo;
        _servicoArmazenamento = servicoArmazenamento;
    }

    public async Task<ResultadoDto> Handle(AtualizarFotoPerfilCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepo.ObterPorIdAsync(comando.UsuarioId);
        if (usuario == null)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.UsuarioNaoEncontrado);

        var extensao = Path.GetExtension(comando.NomeArquivo);
        var chave = ModuloMonolitico.Dominio.Configuracoes.ChavesSistema.StoragePerfil(comando.UsuarioId, extensao);

        var url = await _servicoArmazenamento.EnviarArquivoAsync(comando.ArquivoStream, chave, comando.TipoConteudo);

        usuario.UrlImagem = url;
        await _usuarioRepo.AtualizarAsync(usuario);

        return ResultadoDto.ComSucesso("Foto de perfil atualizada com sucesso.");
    }
}


