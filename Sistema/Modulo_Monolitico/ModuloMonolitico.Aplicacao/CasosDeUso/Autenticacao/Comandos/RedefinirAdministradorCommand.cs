using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class RedefinirAdministradorCommand : IRequest<ResultadoDto>
{
    public string EmailAdministrador { get; set; } = string.Empty;
}

public class RedefinirAdministradorCommandHandler : IRequestHandler<RedefinirAdministradorCommand, ResultadoDto>
{
    private readonly IUsuarioRepositorio _usuarioRepo;

    public RedefinirAdministradorCommandHandler(IUsuarioRepositorio usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public async Task<ResultadoDto> Handle(RedefinirAdministradorCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepo.ObterPorEmailAsync(comando.EmailAdministrador);
        
        if (usuario == null)
        {
            return ResultadoDto.Falha($"Usuário {comando.EmailAdministrador} não foi encontrado no banco de dados. Cadastre novamente.");
        }

        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456", 12);
        await _usuarioRepo.AtualizarAsync(usuario);

        try
        {
            await _usuarioRepo.RedefinirAdministradorAsync(usuario);

            return ResultadoDto.ComSucesso($"Usuário {comando.EmailAdministrador} (ID: {usuario.Id}). Senha redefinida e vínculos inseridos com sucesso.");
        }
        catch (Exception ex)
        {
            return ResultadoDto.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.ErroRecriarVinculos, ex.Message));
        }
    }
}



