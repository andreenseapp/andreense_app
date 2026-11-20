using MediatR;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades.Curriculos;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio.Curriculos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso.Curriculos.Comandos;

public record SalvarCurriculoCommand(
    Guid ContaId,
    Guid UsuarioId,
    string CargoDesejado,
    string AreaAtuacao,
    string Nivel,
    string Modalidade,
    decimal? PretensaoSalarial,
    string Disponibilidade,
    bool AceitaViagem,
    bool AceitaMudanca,
    string? ResumoProfissional,
    string? CnhCategorias,
    bool Pcd,
    string? TipoDeficiencia
) : IRequest<ResultadoDto>;

public class SalvarCurriculoCommandHandler : IRequestHandler<SalvarCurriculoCommand, ResultadoDto>
{
    private readonly ICurriculoRepositorio _repositorio;
    private readonly ILogger<SalvarCurriculoCommandHandler> _logger;

    public SalvarCurriculoCommandHandler(ICurriculoRepositorio repositorio, ILogger<SalvarCurriculoCommandHandler> logger)
    {
        _repositorio = repositorio;
        _logger = logger;
    }

    public async Task<ResultadoDto> Handle(SalvarCurriculoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Salvando dados principais do currículo do usuário {UsuarioId}", request.UsuarioId);

        try
        {
            await _repositorio.CadastrarOuObterIdAsync(request.ContaId, request.UsuarioId);

            var curriculo = new Curriculo
            {
                ContaId = request.ContaId,
                UsuarioId = request.UsuarioId,
                CargoDesejado = request.CargoDesejado,
                AreaAtuacao = request.AreaAtuacao,
                Nivel = request.Nivel,
                Modalidade = request.Modalidade,
                PretensaoSalarial = request.PretensaoSalarial,
                Disponibilidade = request.Disponibilidade,
                AceitaViagem = request.AceitaViagem,
                AceitaMudanca = request.AceitaMudanca,
                ResumoProfissional = request.ResumoProfissional,
                CnhCategorias = request.CnhCategorias,
                Pcd = request.Pcd,
                TipoDeficiencia = request.TipoDeficiencia
            };

            await _repositorio.AtualizarDadosPrincipaisAsync(curriculo);
            _logger.LogInformation("Currículo do usuário {UsuarioId} salvo com sucesso", request.UsuarioId);
            return ResultadoDto.ComSucesso();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar currículo do usuário {UsuarioId}", request.UsuarioId);
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Curriculos.ErroSalvarCurriculo);
        }
    }
}
