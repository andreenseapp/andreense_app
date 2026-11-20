using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Veiculos.Comandos;

public record RegistrarBuscaRecenteVeiculoCommand(Guid UsuarioId, string ChaveFiltro, string FiltroJson, string Rotulo) : IRequest;

public class RegistrarBuscaRecenteVeiculoCommandHandler : IRequestHandler<RegistrarBuscaRecenteVeiculoCommand>
{
    private readonly IBuscaRecenteVeiculoRepositorio _repositorio;
    private readonly ILogger<RegistrarBuscaRecenteVeiculoCommandHandler> _logger;

    public RegistrarBuscaRecenteVeiculoCommandHandler(
        IBuscaRecenteVeiculoRepositorio repositorio,
        ILogger<RegistrarBuscaRecenteVeiculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task Handle(RegistrarBuscaRecenteVeiculoCommand comando, CancellationToken cancellationToken)
    {
        await _repositorio.CadastrarOuAtualizarAsync(new BuscaRecenteVeiculo
        {
            Id = Guid.NewGuid(),
            UsuarioId = comando.UsuarioId,
            ChaveFiltro = comando.ChaveFiltro,
            FiltroJson = comando.FiltroJson,
            Rotulo = comando.Rotulo
        });

        _logger.LogInformation("Busca recente de veículo registrada para o usuário {UsuarioId}", comando.UsuarioId);
    }
}
