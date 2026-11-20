using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class ConsumirCodigoFidelidadeCommand : IRequest<ResultadoDto<FidelidadeSaldoDto>>
    {
        public string Codigo { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
    }

    public class ConsumirCodigoFidelidadeCommandHandler : IRequestHandler<ConsumirCodigoFidelidadeCommand, ResultadoDto<FidelidadeSaldoDto>>
    {
        private readonly IFidelidadeRepositorio _fidelidadeRepositorio;
        private readonly IMediator _mediator;

        public ConsumirCodigoFidelidadeCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio, IMediator mediator)
        {
            _fidelidadeRepositorio = fidelidadeRepositorio;
            _mediator = mediator;
        }

        public async Task<ResultadoDto<FidelidadeSaldoDto>> Handle(ConsumirCodigoFidelidadeCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _fidelidadeRepositorio.ObterCodigoPorValorAsync(comando.Codigo);

                if (token == null)
                    return ResultadoDto<FidelidadeSaldoDto>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.TokenInvalido);

                if (token.Situacao != "disponivel")
                    return ResultadoDto<FidelidadeSaldoDto>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.QrCodeUtilizado);

                if (token.EstaExpirado)
                    return ResultadoDto<FidelidadeSaldoDto>.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.QrCodeExpirou);
                var resultado = await _mediator.Send(new RegistrarCarimboCommand
                {
                    CartaoId = token.CartaoId,
                    UsuarioId = comando.UsuarioId,
                    CodigoReferencia = "Scan QR Code"
                });

                if (resultado.Sucesso)
                {
                    await _fidelidadeRepositorio.AtualizarSituacaoCodigoAsync(token.Id, "usado");
                    return resultado;
                }
                return ResultadoDto<FidelidadeSaldoDto>.Falha(resultado.Mensagem);
            }
            catch (Exception ex)
            {
                return ResultadoDto<FidelidadeSaldoDto>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Fidelidade.ErroConsumirToken, ex.Message));
            }
        }
    }
}
