using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ValidarCodigoEmailCommand(string Email, string Codigo) : IRequest<bool>;

public class ValidarCodigoEmailCommandHandler : IRequestHandler<ValidarCodigoEmailCommand, bool>
{
    private readonly ICodigoValidacaoRepositorio _codigoValidacaoRepositorio;

    public ValidarCodigoEmailCommandHandler(ICodigoValidacaoRepositorio codigoValidacaoRepositorio)
        => _codigoValidacaoRepositorio = codigoValidacaoRepositorio;

    public async Task<bool> Handle(ValidarCodigoEmailCommand comando, CancellationToken cancellationToken)
    {
        var valido = await _codigoValidacaoRepositorio.ValidarAsync(comando.Email, comando.Codigo, "validacao_email");
        if (valido)
            await _codigoValidacaoRepositorio.MarcarComoValidadoAsync(comando.Email);
        return valido;
    }
}
