using FluentValidation;
using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class CadastrarPlanoSaaSCommand : IRequest<ResultadoDto>
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal ValorMensal { get; set; }
    public List<string> RecursosSelecionados { get; set; } = [];
    public Dictionary<string, string> ValoresRecurso { get; set; } = new();
}

public class CadastrarPlanoSaaSCommandValidador : AbstractValidator<CadastrarPlanoSaaSCommand>
{
    public CadastrarPlanoSaaSCommandValidador()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do plano é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.ValorMensal)
            .GreaterThanOrEqualTo(0).WithMessage("O valor mensal não pode ser negativo.");
    }
}

public class CadastrarPlanoSaaSCommandHandler : IRequestHandler<CadastrarPlanoSaaSCommand, ResultadoDto>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public CadastrarPlanoSaaSCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public async Task<ResultadoDto> Handle(CadastrarPlanoSaaSCommand comando, CancellationToken cancellationToken)
    {
        var plano = new PlanoSaaS
        {
            Id = Guid.NewGuid(),
            Nome = comando.Nome,
            Descricao = comando.Descricao,
            ValorMensal = comando.ValorMensal,
            Ativo = true,
            DataInclusao = DateTime.UtcNow
        };

        await _repositorio.CadastrarPlanoAsync(plano, comando.RecursosSelecionados, comando.ValoresRecurso);
        return ResultadoDto.ComSucesso("Plano SaaS criado com sucesso.");
    }
}
