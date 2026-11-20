using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarFaturaSaaSCommand(FaturaSaaS Fatura) : IRequest;

public class CadastrarFaturaSaaSCommandHandler : IRequestHandler<CadastrarFaturaSaaSCommand>
{
    private readonly IAssinaturaSaaSRepositorio _repositorio;

    public CadastrarFaturaSaaSCommandHandler(IAssinaturaSaaSRepositorio repositorio) => _repositorio = repositorio;

    public Task Handle(CadastrarFaturaSaaSCommand comando, CancellationToken cancellationToken)
        => _repositorio.CadastrarFaturaAsync(comando.Fatura);
}
