using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class ResgatarRecompensaCommand : IRequest<ResultadoDto<bool>>
    {
        public Guid SaldoId { get; set; }
    }

    public class ResgatarRecompensaCommandHandler : IRequestHandler<ResgatarRecompensaCommand, ResultadoDto<bool>>
    {
        private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

        public ResgatarRecompensaCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        {
            _fidelidadeRepositorio = fidelidadeRepositorio;
        }

        public async Task<ResultadoDto<bool>> Handle(ResgatarRecompensaCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var saldo = await _fidelidadeRepositorio.ObterSaldoPorIdAsync(comando.SaldoId);
                if (saldo == null || saldo.RecompensasAcumuladas <= 0)
                    return ResultadoDto<bool>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.NenhumaRecompensa);

                await _fidelidadeRepositorio.ExecutarResgateAsync(comando.SaldoId);

                return ResultadoDto<bool>.ComSucesso(true, "Recompensa resgatada com sucesso!");
            }
            catch (Exception ex)
            {
                return ResultadoDto<bool>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.ErroResgatar, ex.Message));
            }
        }
    }
}



