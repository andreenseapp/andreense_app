using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class FidelidadeSaldoDto
    {
        public Guid CartaoId { get; set; }
        public string TituloCartao { get; set; } = string.Empty;
        public int PontosAcumulados { get; set; }
        public int PontosNecessarios { get; set; }
        public int RecompensasAcumuladas { get; set; }
        public string Recompensa { get; set; } = string.Empty;
    }


    public class RegistrarCarimboCommand : IRequest<ResultadoDto<FidelidadeSaldoDto>>
    {
        public Guid CartaoId { get; set; }
        public Guid UsuarioId { get; set; }
        public string? CodigoReferencia { get; set; }
    }

    public class RegistrarCarimboCommandHandler : IRequestHandler<RegistrarCarimboCommand, ResultadoDto<FidelidadeSaldoDto>>
    {
        private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

        public RegistrarCarimboCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        {
            _fidelidadeRepositorio = fidelidadeRepositorio;
        }

        public async Task<ResultadoDto<FidelidadeSaldoDto>> Handle(RegistrarCarimboCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var cartao = await _fidelidadeRepositorio.ObterCartaoPorIdAsync(comando.CartaoId);
                if (cartao == null || cartao.Situacao != "ativo")
                    return ResultadoDto<FidelidadeSaldoDto>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.CartaoNaoEncontradoOuInativo);
                if (cartao.TemReferencia && string.IsNullOrWhiteSpace(comando.CodigoReferencia))
                    return ResultadoDto<FidelidadeSaldoDto>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.ExigeCodigoReferencia);
                var saldo = await _fidelidadeRepositorio.ObterSaldoAsync(comando.CartaoId, comando.UsuarioId);
                bool completouCartao = (saldo?.PontosAcumulados ?? 0) + 1 >= cartao.PontosNecessarios;
                var agora = DateTime.UtcNow;
                DateTime? dataExpiracao = cartao.DiasValidadeCarimbo.HasValue 
                    ? agora.AddDays(cartao.DiasValidadeCarimbo.Value) 
                    : null;

                var carimbo = new FidelidadeCarimbo
                {
                    Id = Guid.NewGuid(),
                    Situacao = "valido",
                    CodigoReferencia = comando.CodigoReferencia,
                    DataExpiracao = dataExpiracao,
                    DataInclusao = agora
                };
                await _fidelidadeRepositorio.ExecutarCarimboAsync(comando.CartaoId, comando.UsuarioId, carimbo, completouCartao);
                var saldoAtualizado = await _fidelidadeRepositorio.ObterSaldoAsync(comando.CartaoId, comando.UsuarioId);
                
                return ResultadoDto<FidelidadeSaldoDto>.ComSucesso(new FidelidadeSaldoDto
                {
                    CartaoId = cartao.Id,
                    TituloCartao = cartao.Titulo,
                    PontosAcumulados = saldoAtualizado?.PontosAcumulados ?? 0,
                    PontosNecessarios = cartao.PontosNecessarios,
                    RecompensasAcumuladas = saldoAtualizado?.RecompensasAcumuladas ?? 0,
                    Recompensa = cartao.Recompensa
                }, completouCartao ? "Parabéns! Você completou um cartão e ganhou uma recompensa!" : "Carimbo registrado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResultadoDto<FidelidadeSaldoDto>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.ErroRegistrarCarimbo, ex.Message));
            }
        }
    }
}




