using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class DeletarGrupoCommand : IRequest<ResultadoDto>
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
}

public class DeletarGrupoCommandHandler : IRequestHandler<DeletarGrupoCommand, ResultadoDto>
{
    private readonly IGrupoUsuarioRepositorio _grupoRepo;

    public DeletarGrupoCommandHandler(IGrupoUsuarioRepositorio grupoRepo)
    {
        _grupoRepo = grupoRepo;
    }

    public async Task<ResultadoDto> Handle(DeletarGrupoCommand comando, CancellationToken cancellationToken)
    {
        var grupoExistente = await _grupoRepo.ObterPorIdEEmpresaAsync(comando.Id, comando.EmpresaId);
        if (grupoExistente == null)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoEncontrado);

        if (grupoExistente.EhAdministrador)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoPodeExcluirPrincipal);

        var qtdUsuarios = await _grupoRepo.ContarUsuariosVinculadosAsync(comando.Id);
        if (qtdUsuarios > 0)
            return ResultadoDto.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoPodeExcluirComUsuarios, qtdUsuarios));

        await _grupoRepo.DeletarFisicoAsync(comando.Id, comando.EmpresaId);

        return ResultadoDto.ComSucesso("Grupo excluído com sucesso.");
    }
}




