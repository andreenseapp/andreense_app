using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class VincularUsuarioGrupoSaaSCommand : IRequest<ResultadoDto>
{
    public Guid UsuarioId { get; set; }
    public Guid GrupoSaaSId { get; set; }
    public Guid OperadorId { get; set; }
}

public class VincularUsuarioGrupoSaaSCommandHandler : IRequestHandler<VincularUsuarioGrupoSaaSCommand, ResultadoDto>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;
    public VincularUsuarioGrupoSaaSCommandHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<ResultadoDto> Handle(VincularUsuarioGrupoSaaSCommand comando, CancellationToken cancellationToken)
    {
        await _repo.VincularUsuarioAoGrupoAsync(comando.UsuarioId, comando.GrupoSaaSId, comando.OperadorId);
        return ResultadoDto.ComSucesso("Usuário vinculado ao grupo SaaS com sucesso.");
    }
}


