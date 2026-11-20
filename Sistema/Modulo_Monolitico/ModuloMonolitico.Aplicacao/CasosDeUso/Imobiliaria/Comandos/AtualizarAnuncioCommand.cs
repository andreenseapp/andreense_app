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

public record AtualizarAnuncioCommand(
    Guid AnuncioId,
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
) : IRequest<ResultadoDto>;

public class AtualizarAnuncioCommandValidator : AbstractValidator<AtualizarAnuncioCommand>
{
    public AtualizarAnuncioCommandValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().WithMessage("O título é obrigatório.");
        RuleFor(x => x.Cep).NotEmpty().WithMessage(MensagensSistema.Imobiliaria.CepObrigatorio);
        RuleFor(x => x.Logradouro).NotEmpty().WithMessage(MensagensSistema.Imobiliaria.LogradouroObrigatorio);
        RuleFor(x => x.Numero).NotEmpty().WithMessage(MensagensSistema.Imobiliaria.NumeroObrigatorio);
        RuleFor(x => x.Bairro).NotEmpty().WithMessage(MensagensSistema.Imobiliaria.BairroObrigatorio);
        RuleFor(x => x.TipoImovel).NotEmpty().WithMessage("O tipo de imóvel é obrigatório.");
        RuleFor(x => x.TipoNegocio).NotEmpty().WithMessage("O tipo de negócio é obrigatório.");
        RuleFor(x => x.ValorPrincipal).GreaterThan(0).WithMessage("O valor deve ser maior que zero.");
    }
}

public class AtualizarAnuncioCommandHandler : IRequestHandler<AtualizarAnuncioCommand, ResultadoDto>
{
    private readonly IAnuncioRepositorio _repositorio;
    private readonly ILogger<AtualizarAnuncioCommandHandler> _logger;

    public AtualizarAnuncioCommandHandler(IAnuncioRepositorio repositorio, ILogger<AtualizarAnuncioCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AtualizarAnuncioCommand comando, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização do anúncio {AnuncioId} pelo usuário {UsuarioId}", comando.AnuncioId, comando.UsuarioId);

        var anuncio = await _repositorio.ObterAnuncioPorIdAsync(comando.AnuncioId);
        if (anuncio is null)
        {
            _logger.LogWarning("Anúncio {AnuncioId} não encontrado para atualização", comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioNaoEncontrado);
        }

        if (anuncio.UsuarioId != comando.UsuarioId)
        {
            _logger.LogWarning("Usuário {UsuarioId} tentou editar anúncio {AnuncioId} sem permissão", comando.UsuarioId, comando.AnuncioId);
            return ResultadoDto.Falha(MensagensSistema.Imobiliaria.AnuncioSemPermissao);
        }

        anuncio.TipoNegocio = comando.TipoNegocio;
        anuncio.ValorPrincipal = comando.ValorPrincipal;
        anuncio.ValorCondominio = comando.ValorCondominio;
        anuncio.ValorIptu = comando.ValorIptu;
        anuncio.Titulo = comando.Titulo;
        anuncio.Descricao = comando.Descricao;
        anuncio.ContatoWhatsapp = comando.ContatoWhatsapp;
        anuncio.MostrarLogradouro = comando.MostrarLogradouro;
        anuncio.MostrarNumero = comando.MostrarNumero;
        anuncio.DataUltimaAtualizacao = DateTime.UtcNow;

        var imovel = new Imovel
        {
            Id = anuncio.ImovelId,
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
            Cep = comando.Cep?.Replace("-", ""),
            Latitude = comando.Latitude,
            Longitude = comando.Longitude
        };

        await _repositorio.AtualizarAsync(imovel, anuncio);

        _logger.LogInformation("Anúncio {AnuncioId} e Imóvel {ImovelId} atualizados com sucesso", comando.AnuncioId, imovel.Id);
        return ResultadoDto.ComSucesso(MensagensSistema.Geral.Sucesso);
    }
}
