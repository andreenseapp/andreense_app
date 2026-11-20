using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso
{
    public class GerarCodigoFidelidadeCommand : IRequest<ResultadoDto<string>>
    {
        public Guid CartaoId { get; set; }
        public Guid EmpresaId { get; set; }
    }

    public class GerarCodigoFidelidadeCommandHandler : IRequestHandler<GerarCodigoFidelidadeCommand, ResultadoDto<string>>
    {
        private readonly IFidelidadeRepositorio _fidelidadeRepositorio;

        public GerarCodigoFidelidadeCommandHandler(IFidelidadeRepositorio fidelidadeRepositorio)
        {
            _fidelidadeRepositorio = fidelidadeRepositorio;
        }

        public async Task<ResultadoDto<string>> Handle(GerarCodigoFidelidadeCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var tokenString = GerarCodigoAleatorio();
                var token = new FidelidadeToken
                {
                    Id = Guid.NewGuid(),
                    Codigo = tokenString,
                    CartaoId = comando.CartaoId,
                    EmpresaId = comando.EmpresaId,
                    Situacao = "disponivel",
                    DataExpiracao = DateTime.UtcNow.AddSeconds(90),
                    DataInclusao = DateTime.UtcNow
                };

                await _fidelidadeRepositorio.CadastrarCodigoAsync(token);

                return ResultadoDto<string>.ComSucesso(tokenString);
            }
            catch (Exception ex)
            {
                return ResultadoDto<string>.Falha(string.Format(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Fidelizacao.Cashback.ErroGerarToken, ex.Message));
            }
        }

        private string GerarCodigoAleatorio()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 16);
        }
    }
}
