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
    public class GerarCodigoCashbackCommand : IRequest<ResultadoDto<string>>
    {
        public Guid EmpresaId { get; set; }
        public decimal? ValorCompra { get; set; }
        public string Tipo { get; set; } = "credito"; 
    }

    public class GerarCodigoCashbackCommandHandler : IRequestHandler<GerarCodigoCashbackCommand, ResultadoDto<string>>
    {
        private readonly ICashbackRepositorio _cashbackRepositorio;

        public GerarCodigoCashbackCommandHandler(ICashbackRepositorio cashbackRepositorio)
        {
            _cashbackRepositorio = cashbackRepositorio;
        }

        public async Task<ResultadoDto<string>> Handle(GerarCodigoCashbackCommand comando, CancellationToken cancellationToken)
        {
            try
            {
                var tokenString = GerarCodigoAleatorio();
                var token = new CashbackToken
                {
                    Id = Guid.NewGuid(),
                    Codigo = tokenString,
                    EmpresaId = comando.EmpresaId,
                    ValorCompra = comando.ValorCompra,
                    Tipo = comando.Tipo,
                    Situacao = "disponivel",
                    DataExpiracao = DateTime.UtcNow.AddSeconds(90), 
                    DataInclusao = DateTime.UtcNow
                };

                await _cashbackRepositorio.CadastrarCodigoAsync(token);

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
