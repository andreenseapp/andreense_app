using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class CreditarCashbackCommand : IRequest<ResultadoDto<decimal>>
    {
        public Guid EmpresaId { get; set; }
        public Guid UsuarioId { get; set; }
        public decimal ValorCompra { get; set; }
        public string? CodigoReferencia { get; set; }
        public string? Descricao { get; set; }
    }

    public class CreditarCashbackCommandHandler : IRequestHandler<CreditarCashbackCommand, ResultadoDto<decimal>>
    {
        private readonly ICashbackRepositorio _cashbackRepositorio;

        public CreditarCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        {
            _cashbackRepositorio = cashbackRepositorio;
        }

        public async Task<ResultadoDto<decimal>> Handle(CreditarCashbackCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var regras = await _cashbackRepositorio.ObterRegrasAtivasPorEmpresaAsync(comando.EmpresaId);
                var agora = DateTime.UtcNow;
                var regraAplicavel = regras.FirstOrDefault(r => 
                    (r.DataInicioVigencia == null || r.DataInicioVigencia <= agora) &&
                    (r.DataFimVigencia == null || r.DataFimVigencia >= agora)
                );

                if (regraAplicavel == null)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.NenhumaRegraAtiva);
                if (regraAplicavel.TemReferencia && string.IsNullOrWhiteSpace(comando.CodigoReferencia))
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ExigeCodigoReferencia);
                var valorCashback = Math.Round(comando.ValorCompra * (regraAplicavel.Percentual / 100), 2);

                if (valorCashback <= 0)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.NaoGeraMinimo);
                if (regraAplicavel.ValorMaximoUnidade.HasValue && valorCashback > regraAplicavel.ValorMaximoUnidade.Value)
                    valorCashback = regraAplicavel.ValorMaximoUnidade.Value;
                DateTime? dataExpiracao = regraAplicavel.DiasValidade.HasValue 
                    ? agora.AddDays(regraAplicavel.DiasValidade.Value) 
                    : null;

                var credito = new CashbackCredito
                {
                    Id = Guid.NewGuid(),
                    EmpresaId = comando.EmpresaId,
                    UsuarioId = comando.UsuarioId,
                    RegraId = regraAplicavel.Id,
                    ValorInicial = valorCashback,
                    ValorUtilizado = 0,
                    Descricao = comando.Descricao ?? string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.DescricaoPadrao, regraAplicavel.Percentual, comando.ValorCompra),
                    CodigoReferencia = comando.CodigoReferencia,
                    Situacao = "disponivel",
                    DataLiberacao = agora,
                    DataExpiracao = dataExpiracao,
                    DataInclusao = agora
                };
                var saldoAtual = await _cashbackRepositorio.ObterSaldoAsync(comando.EmpresaId, comando.UsuarioId);
                var novoSaldo = (saldoAtual?.SaldoDisponivel ?? 0) + valorCashback;

                await _cashbackRepositorio.ExecutarCreditoAsync(credito, novoSaldo);

                return ResultadoDto<decimal>.ComSucesso(novoSaldo, "Cashback creditado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResultadoDto<decimal>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ErroCreditar, ex.Message));
            }
        }
    }
}




