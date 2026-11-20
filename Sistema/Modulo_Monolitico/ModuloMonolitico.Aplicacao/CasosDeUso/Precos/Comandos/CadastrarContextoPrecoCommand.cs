using FluentValidation;
using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class CadastrarContextoPrecoCommand : IRequest<Guid>
{
    public Guid ContaId { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool EhPadrao { get; set; } = false;
    public int Ordem { get; set; } = 0;
}

public class CadastrarContextoPrecoCommandValidador : AbstractValidator<CadastrarContextoPrecoCommand>
{
    public CadastrarContextoPrecoCommandValidador()
    {
        RuleFor(x => x.ContaId).NotEmpty().WithMessage("A conta é obrigatória.");
        RuleFor(x => x.EmpresaId).NotEmpty().WithMessage("A empresa é obrigatória.");
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100).WithMessage("O nome do contexto é obrigatório (máx. 100 caracteres).");
    }
}

public class CadastrarContextoPrecoCommandHandler : IRequestHandler<CadastrarContextoPrecoCommand, Guid>
{
    private readonly IContextoPrecoRepositorio _repo;

    public CadastrarContextoPrecoCommandHandler(IContextoPrecoRepositorio repo) => _repo = repo;

    public async Task<Guid> Handle(CadastrarContextoPrecoCommand comando, CancellationToken cancellationToken)
    {
        var contexto = new Dominio.Entidades.ContextoPreco
        {
            Id = Guid.NewGuid(),
            ContaId = comando.ContaId,
            EmpresaId = comando.EmpresaId,
            Nome = comando.Nome,
            Descricao = comando.Descricao,
            EhPadrao = comando.EhPadrao,
            Ativo = true,
            Ordem = comando.Ordem,
            DataInclusao = DateTime.UtcNow,
            DataUltimaAtualizacao = DateTime.UtcNow
        };
        return await _repo.CadastrarAsync(contexto);
    }
}





