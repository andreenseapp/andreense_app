using MediatR;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record ObterContatoQuery(Guid Id, Guid ContaId) : IRequest<Contato?>;

public class ObterContatoQueryHandler : IRequestHandler<ObterContatoQuery, Contato?>
{
    private readonly IContatoRepositorio _contatoRepositorio;

    public ObterContatoQueryHandler(IContatoRepositorio contatoRepositorio)
        => _contatoRepositorio = contatoRepositorio;

    public Task<Contato?> Handle(ObterContatoQuery comando, CancellationToken cancellationToken)
        => _contatoRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
}
