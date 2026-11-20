using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record CadastrarAnuncioCommand(
    Guid ContaId,
    Guid UsuarioId,
    string TipoImovel,
    decimal? AreaTotal,
    short Quartos,
    short Banheiros,
    short VagasGaragem,
    short? Andar,
    bool TemElevador,
    bool TemPiscina,
    bool AceitaPet,
    bool TemChurrasqueira,
    bool TemSalaoFestas,
    bool TemQuadra,
    bool TemAcademia,
    bool TemPortaria24h,
    bool TemVaranda,
    bool TemPlayground,
    bool TemAguaSeparada,
    bool TemLuzSeparada,
    bool TemEntradaSeparada,
    bool TemCobertura,
    string SituacaoObra,
    string? Logradouro,
    string? Numero,
    string? Complemento,
    string Bairro,
    string? Cep,
    decimal? Latitude,
    decimal? Longitude,
    bool MostrarLogradouro,
    bool MostrarNumero,
    string TipoNegocio,
    decimal ValorPrincipal,
    decimal ValorCondominio,
    decimal ValorIptu,
    string Titulo,
    string? Descricao,
    string? ContatoWhatsapp
) : IRequest<ResultadoDto<Guid>>;

public class CadastrarAnuncioCommandValidator : AbstractValidator<CadastrarAnuncioCommand>
{
    public CadastrarAnuncioCommandValidator()
    {
        RuleFor(x => x.TipoNegocio)
            .NotEmpty().WithMessage("A finalidade do negócio (Venda ou Aluguel) é obrigatória.")
            .Must(t => t == "venda" || t == "aluguel").WithMessage("Finalidade do imóvel inválida.");

        RuleFor(x => x.TipoImovel)
            .NotEmpty().WithMessage("O tipo do imóvel é obrigatório.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título do anúncio é obrigatório.")
            .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Quartos)
            .InclusiveBetween((short)0, (short)15).WithMessage("A quantidade de quartos deve estar entre 0 e 15.");

        RuleFor(x => x.Banheiros)
            .InclusiveBetween((short)0, (short)15).WithMessage("A quantidade de banheiros deve estar entre 0 e 15.");

        RuleFor(x => x.VagasGaragem)
            .InclusiveBetween((short)0, (short)20).WithMessage("A quantidade de vagas de garagem deve estar entre 0 e 20.");

        RuleFor(x => x.Andar)
            .InclusiveBetween((short)0, (short)100).When(x => x.Andar.HasValue).WithMessage("O andar informado deve estar entre 0 e 100.");

        RuleFor(x => x.AreaTotal)
            .InclusiveBetween(0m, 1000000m).When(x => x.AreaTotal.HasValue).WithMessage("A área total informada é inválida.");

        RuleFor(x => x.ValorPrincipal)
            .GreaterThanOrEqualTo(0m).WithMessage("O valor do imóvel não pode ser negativo.")
            .LessThanOrEqualTo(500000000m).WithMessage("O valor principal informado excede o limite permitido.");

        RuleFor(x => x.ValorCondominio)
            .InclusiveBetween(0m, 500000m).WithMessage("O valor de condomínio informado é inválido.");

        RuleFor(x => x.ValorIptu)
            .InclusiveBetween(0m, 500000m).WithMessage("O valor de IPTU informado é inválido.");

        RuleFor(x => x.Cep)
            .NotEmpty().WithMessage(MensagensSistema.Imobiliaria.CepObrigatorio);

        RuleFor(x => x.Logradouro)
            .NotEmpty().WithMessage(MensagensSistema.Imobiliaria.LogradouroObrigatorio);

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage(MensagensSistema.Imobiliaria.NumeroObrigatorio);

        RuleFor(x => x.Bairro)
            .NotEmpty().WithMessage(MensagensSistema.Imobiliaria.BairroObrigatorio);
    }
}

public class CadastrarAnuncioCommandHandler : IRequestHandler<CadastrarAnuncioCommand, ResultadoDto<Guid>>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<CadastrarAnuncioCommandHandler> _logger;

    public CadastrarAnuncioCommandHandler(IAnuncioRepositorio repositorio, ILogger<CadastrarAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto<Guid>> Handle(CadastrarAnuncioCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando cadastro de anúncio imobiliário pelo usuário {UsuarioId}", comando.UsuarioId);

        var imovelId = Guid.NewGuid();
        var anuncioId = Guid.NewGuid();

        var imovel = new Imovel
        {
            Id = imovelId,
            ContaId = comando.ContaId,
            UsuarioId = comando.UsuarioId,
            TipoImovel = comando.TipoImovel,
            AreaTotal = comando.AreaTotal,
            Quartos = comando.Quartos,
            Banheiros = comando.Banheiros,
            VagasGaragem = comando.VagasGaragem,
            Andar = comando.Andar,
            TemElevador = comando.TemElevador,
            TemPiscina = comando.TemPiscina,
            AceitaPet = comando.AceitaPet,
            TemChurrasqueira = comando.TemChurrasqueira,
            TemSalaoFestas = comando.TemSalaoFestas,
            TemQuadra = comando.TemQuadra,
            TemAcademia = comando.TemAcademia,
            TemPortaria24h = comando.TemPortaria24h,
            TemVaranda = comando.TemVaranda,
            TemPlayground = comando.TemPlayground,
            TemAguaSeparada = comando.TemAguaSeparada,
            TemLuzSeparada = comando.TemLuzSeparada,
            TemEntradaSeparada = comando.TemEntradaSeparada,
            TemCobertura = comando.TemCobertura,
            SituacaoObra = string.IsNullOrWhiteSpace(comando.SituacaoObra) ? "pronto" : comando.SituacaoObra,
            Logradouro = comando.Logradouro,
            Numero = comando.Numero,
            Complemento = comando.Complemento,
            Bairro = comando.Bairro,
            Cidade = "Santo André",
            Cep = comando.Cep,
            Latitude = comando.Latitude,
            Longitude = comando.Longitude
        };

        var dataExpiracao = DateTime.UtcNow.AddDays(45);

        var anuncio = new Anuncio
        {
            Id = anuncioId,
            ImovelId = imovelId,
            ContaId = comando.ContaId,
            UsuarioId = comando.UsuarioId,
            TipoNegocio = comando.TipoNegocio,
            ValorPrincipal = comando.ValorPrincipal,
            ValorCondominio = comando.ValorCondominio,
            ValorIptu = comando.ValorIptu,
            Titulo = comando.Titulo,
            Descricao = comando.Descricao,
            ContatoWhatsapp = comando.ContatoWhatsapp,
            MostrarLogradouro = comando.MostrarLogradouro,
            MostrarNumero = comando.MostrarNumero,
            Situacao = "ativo",
            CodigoPublico = GerarCodigoPublico(anuncioId),
            DataExpiracao = dataExpiracao
        };

        await _repositorio.CadastrarAsync(imovel, anuncio);

        _logger.LogInformation("Anúncio {AnuncioId} cadastrado com sucesso pelo usuário {UsuarioId}", anuncioId, comando.UsuarioId);

        return ResultadoDto<Guid>.ComSucesso(anuncioId, MensagensSistema.Geral.Sucesso);
    }

    private static string GerarCodigoPublico(Guid anuncioId)
    {
        var trecho = anuncioId.ToString("N")[..8].ToUpperInvariant();
        return $"SA-{trecho}";
    }
}
