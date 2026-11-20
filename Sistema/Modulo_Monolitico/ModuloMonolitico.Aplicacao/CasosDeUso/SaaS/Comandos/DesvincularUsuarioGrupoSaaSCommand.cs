using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class DesvincularUsuarioGrupoSaaSCommand : IRequest<ResultadoDto>
{
    public Guid UsuarioId { get; set; }
    public Guid GrupoSaaSId { get; set; }
}

public class DesvincularUsuarioGrupoSaaSCommandHandler : IRequestHandler<DesvincularUsuarioGrupoSaaSCommand, ResultadoDto>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public DesvincularUsuarioGrupoSaaSCommandHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<ResultadoDto> Handle(DesvincularUsuarioGrupoSaaSCommand comando, CancellationToken cancellationToken)
    {
        await _repo.DesvincularUsuarioDoGrupoAsync(comando.UsuarioId, comando.GrupoSaaSId);
        return ResultadoDto.ComSucesso("Usuário removido do grupo SaaS com sucesso.");
    }
}


