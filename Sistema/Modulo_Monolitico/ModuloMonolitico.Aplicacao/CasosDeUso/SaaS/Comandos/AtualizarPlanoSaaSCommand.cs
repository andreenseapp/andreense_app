using FluentValidation;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class AtualizarPlanoSaaSCommand : IRequest<ResultadoDto>
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensal { get; set; }
    public bool Ativo { get; set; } = true;
    public List<string> RecursosSelecionados { get; set; } = [];
    public Dictionary<string, string> ValoresRecurso { get; set; } = new();
}

public class AtualizarPlanoSaaSCommandValidador : AbstractValidator<AtualizarPlanoSaaSCommand>
{
    public AtualizarPlanoSaaSCommandValidador()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do plano é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do plano é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.ValorMensal)
            .GreaterThanOrEqualTo(0).WithMessage("O valor mensal não pode ser negativo.");
    }
}

public class AtualizarPlanoSaaSCommandHandler : IRequestHandler<AtualizarPlanoSaaSCommand, ResultadoDto>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public AtualizarPlanoSaaSCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public async Task<ResultadoDto> Handle(AtualizarPlanoSaaSCommand comando, CancellationToken cancellationToken)
    {
        var existente = await _repositorio.ObterPlanoPorIdAsync(comando.Id);
        if (existente is null)
            return ResultadoDto.Falha("Plano SaaS não encontrado.");

        var plano = new PlanoSaaS
        {
            Id = comando.Id,
            Nome = comando.Nome,
            Descricao = comando.Descricao,
            ValorMensal = comando.ValorMensal,
            Ativo = comando.Ativo,
            DataInclusao = existente.DataInclusao
        };

        await _repositorio.AtualizarPlanoAsync(plano, comando.RecursosSelecionados, comando.ValoresRecurso);
        return ResultadoDto.ComSucesso("Plano SaaS atualizado com sucesso.");
    }
}
