using FluentValidation;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public class CadastrarGrupoSaaSCommand : IRequest<ResultadoDto>
{
    public Guid? Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;
    public Guid OperadorId { get; set; }
    public List<Guid> RecursosSelecionados { get; set; } = new();
}

public class CadastrarGrupoSaaSCommandValidador : AbstractValidator<CadastrarGrupoSaaSCommand>
{
    public CadastrarGrupoSaaSCommandValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do grupo é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");
    }
}

public class CadastrarGrupoSaaSCommandHandler : IRequestHandler<CadastrarGrupoSaaSCommand, ResultadoDto>
{
    private readonly IGrupoUsuarioSaaSRepositorio _repo;

    public CadastrarGrupoSaaSCommandHandler(IGrupoUsuarioSaaSRepositorio repo) => _repo = repo;

    public async Task<ResultadoDto> Handle(CadastrarGrupoSaaSCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Id.HasValue && comando.Id.Value != Guid.Empty)
        {
            var existente = await _repo.ObterPorIdAsync(comando.Id.Value);
            if (existente is null)
                return ResultadoDto.Falha("Grupo SaaS não encontrado.");

            existente.Nome = comando.Nome;
            existente.Descricao = comando.Descricao;
            existente.Ativo = comando.Ativo;
            existente.DataUltimaAtualizacao = DateTime.UtcNow;

            await _repo.AtualizarAsync(existente);
            await _repo.VincularPermissoesAsync(existente.Id, comando.RecursosSelecionados, comando.OperadorId);

            return ResultadoDto.ComSucesso("Grupo SaaS atualizado com sucesso.");
        }
        else
        {
            var grupo = new GrupoUsuarioSaaS
            {
                Id = Guid.NewGuid(),
                Nome = comando.Nome,
                Descricao = comando.Descricao,
                Ativo = comando.Ativo,
                OperadorInclusao = comando.OperadorId,
                DataInclusao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow
            };

            await _repo.CadastrarAsync(grupo);
            await _repo.VincularPermissoesAsync(grupo.Id, comando.RecursosSelecionados, comando.OperadorId);

            return ResultadoDto.ComSucesso("Grupo SaaS criado com sucesso.");
        }
    }
}




