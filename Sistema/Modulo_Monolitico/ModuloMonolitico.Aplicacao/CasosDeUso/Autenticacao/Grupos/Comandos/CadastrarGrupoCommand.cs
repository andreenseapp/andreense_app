using System.ComponentModel.DataAnnotations;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class CadastrarGrupoCommand : IRequest<ResultadoDto>
{
    public Guid? Id { get; set; }
    public Guid EmpresaId { get; set; }

    [Display(Name = "Nome do grupo")]
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    [MaxLength(500)]
    public string? Descricao { get; set; }

    [Display(Name = "Administrador")]
    public bool EhAdministrador { get; set; } = false;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Display(Name = "Recursos")]
    public List<Guid> RecursosSelecionados { get; set; } = new();

    [Display(Name = "Atalhos do dashboard")]
    public List<Guid> RecursosAtalhos { get; set; } = new();
}

public class CadastrarGrupoCommandHandler : IRequestHandler<CadastrarGrupoCommand, ResultadoDto>
{
    private readonly IGrupoUsuarioRepositorio _grupoRepo;

    public CadastrarGrupoCommandHandler(IGrupoUsuarioRepositorio grupoRepo)
    {
        _grupoRepo = grupoRepo;
    }

    public async Task<ResultadoDto> Handle(CadastrarGrupoCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Id.HasValue && comando.Id.Value != Guid.Empty)
        {
            var grupoExistente = await _grupoRepo.ObterPorIdEEmpresaAsync(comando.Id.Value, comando.EmpresaId);
            if (grupoExistente == null)
                return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoEncontrado);

            if (!comando.Ativo && grupoExistente.Ativo)
            {
                var qtdUsuarios = await _grupoRepo.ContarUsuariosVinculadosAsync(grupoExistente.Id);
                if (qtdUsuarios > 0)
                    return ResultadoDto.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.Grupos.NaoPodeInativarComUsuarios, qtdUsuarios));
            }

            grupoExistente.Nome = comando.Nome;
            grupoExistente.Descricao = comando.Descricao;
            grupoExistente.EhAdministrador = comando.EhAdministrador;
            grupoExistente.Ativo = comando.Ativo;
            grupoExistente.DataUltimaAtualizacao = DateTime.UtcNow;

            await _grupoRepo.AtualizarAsync(grupoExistente);
            await _grupoRepo.VincularPermissoesAsync(grupoExistente.Id, comando.RecursosSelecionados, comando.RecursosAtalhos);

            return ResultadoDto.ComSucesso("Grupo atualizado com sucesso.");
        }
        else
        {
            var grupo = new GrupoUsuario
            {
                Id = Guid.NewGuid(),
                EmpresaId = comando.EmpresaId,
                Nome = comando.Nome,
                Descricao = comando.Descricao,
                EhAdministrador = comando.EhAdministrador,
                Ativo = comando.Ativo,
                DataInclusao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow
            };

            await _grupoRepo.CadastrarAsync(grupo);
            await _grupoRepo.VincularPermissoesAsync(grupo.Id, comando.RecursosSelecionados, comando.RecursosAtalhos);

            return ResultadoDto.ComSucesso("Grupo criado com sucesso.");
        }
    }
}



