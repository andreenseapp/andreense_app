using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class ConsumirCodigoCashbackCommand : IRequest<ResultadoDto<decimal>>
    {
        public string Codigo { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public decimal? ValorCompraInformadoPeloCliente { get; set; } 
    }

    public class ConsumirCodigoCashbackCommandHandler : IRequestHandler<ConsumirCodigoCashbackCommand, ResultadoDto<decimal>>
    {
        private readonly ICashbackRepositorio _cashbackRepositorio;
        private readonly IMediator _mediator;

        public ConsumirCodigoCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio, IMediator mediator)
        {
            _cashbackRepositorio = cashbackRepositorio;
            _mediator = mediator;
        }

        public async Task<ResultadoDto<decimal>> Handle(ConsumirCodigoCashbackCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _cashbackRepositorio.ObterCodigoPorValorAsync(comando.Codigo);

                if (token == null)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.TokenInvalido);

                if (token.Situacao != "disponivel")
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.QrCodeUtilizado);

                if (token.EstaExpirado)
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.QrCodeExpirou);
                if (token.Tipo == "credito")
                {
                    var valorCompra = token.ValorCompra ?? comando.ValorCompraInformadoPeloCliente ?? 0;
                    if (valorCompra <= 0)
                        return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ValorNaoInformado);

                    var resultado = await _mediator.Send(new CreditarCashbackCommand
                    {
                        EmpresaId = token.EmpresaId,
                        UsuarioId = comando.UsuarioId,
                        ValorCompra = valorCompra,
                        Descricao = "Crédito via QR Code"
                    });

                    if (resultado.Sucesso)
                    {
                        await _cashbackRepositorio.AtualizarSituacaoCodigoAsync(token.Id, "usado");
                        return resultado;
                    }
                    return ResultadoDto<decimal>.Falha(resultado.Mensagem);
                }
                else if (token.Tipo == "debito")
                {
                    return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.DebitoNaoImplementado);
                }

                return ResultadoDto<decimal>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.TipoTokenDesconhecido);
            }
            catch (Exception ex)
            {
                return ResultadoDto<decimal>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ErroConsumirToken, ex.Message));
            }
        }
    }
}
