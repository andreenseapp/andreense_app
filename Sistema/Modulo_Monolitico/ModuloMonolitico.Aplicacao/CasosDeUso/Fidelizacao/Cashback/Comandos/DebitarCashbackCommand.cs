using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class DebitarCashbackCommand : IRequest<ResultadoDto<decimal>>
    {
        public Guid EmpresaId { get; set; }
        public Guid UsuarioId { get; set; }
        public decimal ValorGasto { get; set; }
        public Guid? VendaId { get; set; }
    }

    public class DebitarCashbackCommandHandler : IRequestHandler<DebitarCashbackCommand, ResultadoDto<decimal>>
    {
        private readonly ICashbackRepositorio _cashbackRepositorio;

        public DebitarCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        {
            _cashbackRepositorio = cashbackRepositorio;
        }

        public async Task<ResultadoDto<decimal>> Handle(DebitarCashbackCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var saldo = await _cashbackRepositorio.ObterSaldoAsync(comando.EmpresaId, comando.UsuarioId);
                
                if (saldo == null || saldo.SaldoDisponivel < comando.ValorGasto)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.SaldoInsuficiente);
                var creditosDisponiveis = await _cashbackRepositorio.ListarCreditosDisponiveisAsync(comando.EmpresaId, comando.UsuarioId);
                
                var valorRestanteADebitar = comando.ValorGasto;
                var utilizacoes = new List<(Guid CreditoId, decimal ValorDebitado)>();

                foreach (var credito in creditosDisponiveis)
                {
                    if (valorRestanteADebitar <= 0) break;

                    var valorADebitarNesteCredito = Math.Min(credito.ValorDisponivel, valorRestanteADebitar);
                    
                    utilizacoes.Add((credito.Id, valorADebitarNesteCredito));
                    valorRestanteADebitar -= valorADebitarNesteCredito;
                }

                if (valorRestanteADebitar > 0)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ErroConsistenciaSaldo);
                var novoSaldo = saldo.SaldoDisponivel - comando.ValorGasto;
                await _cashbackRepositorio.ExecutarDebitoAsync(comando.EmpresaId, comando.UsuarioId, comando.ValorGasto, utilizacoes, novoSaldo);

                return ResultadoDto<decimal>.ComSucesso(novoSaldo, "Cashback utilizado com sucesso.");
            }
            catch (Exception ex)
            {
                return ResultadoDto<decimal>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ErroDebitar, ex.Message));
            }
        }
    }
}



