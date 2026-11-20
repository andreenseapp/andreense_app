using MediatR;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarCodigoValidacaoCommand(string Email, string Codigo, string Tipo, DateTime ExpiraEm) : IRequest;

public class CadastrarCodigoValidacaoCommandHandler : IRequestHandler<CadastrarCodigoValidacaoCommand>
{
    private readonly ICodigoValidacaoRepositorio _codigoValidacaoRepositorio;

    public CadastrarCodigoValidacaoCommandHandler(ICodigoValidacaoRepositorio codigoValidacaoRepositorio)
        => _codigoValidacaoRepositorio = codigoValidacaoRepositorio;

    public Task Handle(CadastrarCodigoValidacaoCommand comando, CancellationToken cancellationToken)
        => _codigoValidacaoRepositorio.CadastrarAsync(comando.Email, comando.Codigo, comando.Tipo, comando.ExpiraEm);
}
