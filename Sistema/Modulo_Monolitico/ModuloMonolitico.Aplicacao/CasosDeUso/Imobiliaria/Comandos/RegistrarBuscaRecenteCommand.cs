using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;
using ModuloMonolitico.Dominio.Imobiliaria.DTOs;
using ModuloMonolitico.Dominio.Imobiliaria.Servicos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Imobiliaria;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Imobiliaria.Comandos;

public record RegistrarBuscaRecenteCommand(Guid UsuarioId, FiltroAnuncio Filtro) : IRequest<ResultadoDto>;

public class RegistrarBuscaRecenteCommandHandler : IRequestHandler<RegistrarBuscaRecenteCommand, ResultadoDto>
{
    private readonly IBuscaRecenteRepositorio _repositorio;
    private readonly ILogger<RegistrarBuscaRecenteCommandHandler> _logger;

    public RegistrarBuscaRecenteCommandHandler(
        IBuscaRecenteRepositorio repositorio,
        ILogger<RegistrarBuscaRecenteCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(RegistrarBuscaRecenteCommand comando, CancellationToken cancellationToken)
    {
        if (comando.UsuarioId == Guid.Empty || !BuscaRecenteFiltroHelper.TemCriterioSalvavel(comando.Filtro))
        {
            _logger.LogInformation("Busca recente ignorada para usuário {UsuarioId} (sem critério salvável).", comando.UsuarioId);
            return ResultadoDto.ComSucesso();
        }

        var agora = DateTime.UtcNow;
        var busca = new BuscaRecente
        {
            Id = Guid.NewGuid(),
            UsuarioId = comando.UsuarioId,
            ChaveFiltro = BuscaRecenteFiltroHelper.GerarChave(comando.Filtro),
            FiltroJson = BuscaRecenteFiltroHelper.SerializarJson(comando.Filtro),
            Rotulo = BuscaRecenteFiltroHelper.GerarRotulo(comando.Filtro),
            DataInclusao = agora,
            DataUltimaBusca = agora
        };

        await _repositorio.RegistrarAsync(busca, BuscaRecenteFiltroHelper.Limite);

        _logger.LogInformation(
            "Busca recente registrada para usuário {UsuarioId} com chave {ChaveFiltro}.",
            comando.UsuarioId,
            busca.ChaveFiltro);

        return ResultadoDto.ComSucesso(MensagensSistema.Geral.Sucesso);
    }
}
