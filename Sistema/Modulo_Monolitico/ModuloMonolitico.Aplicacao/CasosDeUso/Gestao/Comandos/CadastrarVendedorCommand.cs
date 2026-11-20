using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public class CadastrarVendedorCommand : IRequest<ResultadoDto>
{
    public Guid? Id { get; set; }
    public Guid ContaId { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid TipoVendedorId { get; set; }
    public string NomeFantasia { get; set; } = string.Empty;
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
}

public class CadastrarVendedorCommandHandler : IRequestHandler<CadastrarVendedorCommand, ResultadoDto>
{
    private readonly IVendedorRepositorio _vendedorRepo;

    public CadastrarVendedorCommandHandler(IVendedorRepositorio vendedorRepo)
    {
        _vendedorRepo = vendedorRepo;
    }

    public async Task<ResultadoDto> Handle(CadastrarVendedorCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Id.HasValue && comando.Id.Value != Guid.Empty)
        {
            var existente = await _vendedorRepo.ObterPorIdEContaAsync(comando.Id.Value, comando.ContaId);
            if (existente == null)
                return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Gestao.Vendedores.NaoEncontrado);

            existente.UsuarioId = comando.UsuarioId;
            existente.TipoVendedorId = comando.TipoVendedorId;
            existente.NomeFantasia = comando.NomeFantasia;
            existente.Observacao = comando.Observacao;
            existente.Ativo = comando.Ativo;
            existente.DataUltimaAtualizacao = DateTime.UtcNow;

            await _vendedorRepo.AtualizarAsync(existente);
            return ResultadoDto.ComSucesso("Vendedor atualizado com sucesso.");
        }
        else
        {
            var vendedor = new Vendedor
            {
                Id = Guid.NewGuid(),
                ContaId = comando.ContaId,
                UsuarioId = comando.UsuarioId,
                TipoVendedorId = comando.TipoVendedorId,
                NomeFantasia = comando.NomeFantasia,
                Observacao = comando.Observacao,
                Ativo = comando.Ativo,
                DataInclusao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow
            };

            await _vendedorRepo.CadastrarAsync(vendedor);
            return ResultadoDto.ComSucesso("Vendedor cadastrado com sucesso.");
        }
    }
}




