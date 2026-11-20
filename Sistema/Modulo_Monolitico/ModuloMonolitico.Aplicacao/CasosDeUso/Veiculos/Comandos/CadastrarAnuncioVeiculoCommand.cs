using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record CadastrarAnuncioVeiculoCommand(
    Guid ContaId,
    Guid UsuarioId,
    string Marca,
    string Modelo,
    string? Versao,
    int AnoFabricacao,
    int AnoModelo,
    int Quilometragem,
    string Combustivel,
    string Cambio,
    string Cor,
    string? Placa,
    string TipoVeiculo,
    short? NumeroPortas,
    string Titulo,
    string? Descricao,
    decimal Preco,
    bool AceitaTroca,
    string? ContatoWhatsapp
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarAnuncioVeiculoCommandValidator : AbstractValidator<CadastrarAnuncioVeiculoCommand>
{
    public CadastrarAnuncioVeiculoCommandValidator()
    {
        RuleFor(x => x.Marca)
            .NotEmpty().WithMessage("A marca do veículo é obrigatória.");

        RuleFor(x => x.Modelo)
            .NotEmpty().WithMessage("O modelo do veículo é obrigatório.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título do anúncio é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.AnoFabricacao)
            .InclusiveBetween(1950, DateTime.Now.Year + 1).WithMessage("Ano de fabricação inválido.");

        RuleFor(x => x.AnoModelo)
            .InclusiveBetween(1950, DateTime.Now.Year + 2).WithMessage("Ano do modelo inválido.");

        RuleFor(x => x.Quilometragem)
            .GreaterThanOrEqualTo(0).WithMessage("A quilometragem não pode ser negativa.")
            .LessThanOrEqualTo(9999999).WithMessage("Quilometragem inválida.");

        RuleFor(x => x.Preco)
            .GreaterThan(0m).WithMessage("O preço do veículo é obrigatório.")
            .LessThanOrEqualTo(50000000m).WithMessage("O preço informado excede o limite permitido.");

        RuleFor(x => x.Combustivel)
            .NotEmpty().WithMessage("O tipo de combustível é obrigatório.");

        RuleFor(x => x.Cambio)
            .NotEmpty().WithMessage("O tipo de câmbio é obrigatório.");

        RuleFor(x => x.Cor)
            .NotEmpty().WithMessage("A cor do veículo é obrigatória.");
    }
}

public class CadastrarAnuncioVeiculoCommandHandler : IRequestHandler<CadastrarAnuncioVeiculoCommand, ResultadoDto<Guid>>
{
    private readonly IAnuncioVeiculoRepositorio _repositorio;
    private readonly ILogger<CadastrarAnuncioVeiculoCommandHandler> _logger;

    public CadastrarAnuncioVeiculoCommandHandler(
        IAnuncioVeiculoRepositorio repositorio,
        ILogger<CadastrarAnuncioVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarAnuncioVeiculoCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando cadastro de anúncio de veículo pelo usuário {UsuarioId}", comando.UsuarioId);

        var veiculoId = Guid.NewGuid();
        var anuncioId = Guid.NewGuid();

        var veiculo = new Veiculo
        {
            Id = veiculoId,
            ContaId = comando.ContaId,
            UsuarioId = comando.UsuarioId,
            Marca = comando.Marca,
            Modelo = comando.Modelo,
            Versao = comando.Versao,
            AnoFabricacao = comando.AnoFabricacao,
            AnoModelo = comando.AnoModelo,
            Quilometragem = comando.Quilometragem,
            Combustivel = comando.Combustivel,
            Cambio = comando.Cambio,
            Cor = comando.Cor,
            Placa = comando.Placa,
            TipoVeiculo = string.IsNullOrWhiteSpace(comando.TipoVeiculo) ? "carro" : comando.TipoVeiculo,
            NumeroPortas = comando.NumeroPortas
        };

        var anuncio = new AnuncioVeiculo
        {
            Id = anuncioId,
            VeiculoId = veiculoId,
            ContaId = comando.ContaId,
            UsuarioId = comando.UsuarioId,
            Titulo = comando.Titulo,
            Descricao = comando.Descricao,
            Preco = comando.Preco,
            AceitaTroca = comando.AceitaTroca,
            ContatoWhatsapp = comando.ContatoWhatsapp,
            Situacao = "ativo",
            CodigoPublico = GerarCodigoPublico(anuncioId),
            DataExpiracao = DateTime.UtcNow.AddDays(45)
        };

        await _repositorio.CadastrarAsync(veiculo, anuncio);

        _logger.LogInformation("Anúncio de veículo {AnuncioId} cadastrado com sucesso pelo usuário {UsuarioId}", anuncioId, comando.UsuarioId);

        return ResultadoDto<Guid>.ComSucesso(anuncioId, MensagensSistema.Geral.Sucesso);
    }

    private static string GerarCodigoPublico(Guid anuncioId)
    {
        var trecho = anuncioId.ToString("N")[..8].ToUpperInvariant();
        return $"VE-{trecho}";
    }
}
