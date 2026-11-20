using MediatR;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class AtualizarContextoPrecoCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid ContaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool EhPadrao { get; set; }
    public bool Ativo { get; set; }
    public int Ordem { get; set; }
}

public class AtualizarContextoPrecoCommandHandler : IRequestHandler<AtualizarContextoPrecoCommand>
{
    private readonly IContextoPrecoRepositorio _repo;

    public AtualizarContextoPrecoCommandHandler(IContextoPrecoRepositorio repo) => _repo = repo;

    public async Task Handle(AtualizarContextoPrecoCommand comando, CancellationToken cancellationToken)
    {
        var contexto = await _repo.ObterPorIdEContaAsync(comando.Id, comando.ContaId)
            ?? throw new InvalidOperationException(MensagensSistema.Precos.NaoEncontrado);

        contexto.Nome = comando.Nome;
        contexto.Descricao = comando.Descricao;
        contexto.EhPadrao = comando.EhPadrao;
        contexto.Ativo = comando.Ativo;
        contexto.Ordem = comando.Ordem;
        contexto.DataUltimaAtualizacao = DateTime.UtcNow;

        await _repo.AtualizarAsync(contexto);
    }
}
