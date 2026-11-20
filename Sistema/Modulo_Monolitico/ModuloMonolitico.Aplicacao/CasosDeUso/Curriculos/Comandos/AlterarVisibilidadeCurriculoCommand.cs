using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record AlterarVisibilidadeCurriculoCommand(Guid ContaId, Guid UsuarioId, bool Visivel) : IRequest<ResultadoDto>;

public class AlterarVisibilidadeCurriculoCommandHandler : IRequestHandler<AlterarVisibilidadeCurriculoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<AlterarVisibilidadeCurriculoCommandHandler> _logger;

    public AlterarVisibilidadeCurriculoCommandHandler(ICurriculoRepositorio repositorio, ILogger<AlterarVisibilidadeCurriculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(AlterarVisibilidadeCurriculoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Alterando visibilidade do currículo do usuário {UsuarioId} para {Visivel}", request.UsuarioId, request.Visivel);

        try
        {
            await _repositorio.CadastrarOuObterIdAsync(request.ContaId, request.UsuarioId);
            await _repositorio.AlterarVisibilidadeAsync(request.ContaId, request.UsuarioId, request.Visivel);
            _logger.LogInformation("Visibilidade do currículo do usuário {UsuarioId} alterada para {Visivel}", request.UsuarioId, request.Visivel);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar visibilidade do currículo do usuário {UsuarioId}", request.UsuarioId);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroAlterarVisibilidade);
        }
    }
}
