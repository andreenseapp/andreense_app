using MediatR;
using ModuloMonolitico.Dominio.Configuracoes;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public class RegistrarSaidaEstoqueCommand : IRequest<EstoqueSaldo>
{
    public Guid SaldoId { get; set; }
    public Guid ContaId { get; set; }
    public decimal Quantidade { get; set; }
    public string? Motivo { get; set; }
    public Guid? UsuarioResponsavelId { get; set; }
}

public class RegistrarSaidaEstoqueCommandHandler : IRequestHandler<RegistrarSaidaEstoqueCommand, EstoqueSaldo>
{
    private readonly IEstoqueRepositorio _repo;

    public RegistrarSaidaEstoqueCommandHandler(IEstoqueRepositorio repo) => _repo = repo;

    public async Task<EstoqueSaldo> Handle(RegistrarSaidaEstoqueCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Quantidade <= 0)
            throw new InvalidOperationException(MensagensSistema.Estoque.QuantidadeSaidaInvalida);

        return await _repo.RegistrarSaidaAsync(
            comando.SaldoId, comando.Quantidade, comando.Motivo,
            comando.ContaId, comando.UsuarioResponsavelId);
    }
}
