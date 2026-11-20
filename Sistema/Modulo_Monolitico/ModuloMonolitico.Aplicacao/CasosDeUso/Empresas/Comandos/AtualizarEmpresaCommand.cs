using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record AtualizarEmpresaCommand : IRequest<ResultadoDto>
{
    public Guid Id { get; init; }
    public Guid ContaId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? RazaoSocial { get; init; }
    public string? Cnpj { get; init; }
    public string? InscricaoEstadual { get; init; }
    public string? Segmento { get; init; }
    public string? CnaePrincipal { get; init; }
    public string? CnaesSecundarios { get; init; }
    public string? Cep { get; init; }
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Bairro { get; init; }
    public string? Cidade { get; init; }
    public string? Uf { get; init; }
}

public class AtualizarEmpresaCommandHandler : IRequestHandler<AtualizarEmpresaCommand, ResultadoDto>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public AtualizarEmpresaCommandHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public async Task<ResultadoDto> Handle(AtualizarEmpresaCommand comando, CancellationToken cancellationToken)
    {
        var empresa = await _empresaRepositorio.ObterPorIdEContaAsync(comando.Id, comando.ContaId);
        if (empresa is null)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Empresas.NaoEncontrada);

        AuxiliarComandoEmpresa.AplicarDados(empresa, comando.Nome, comando.RazaoSocial, comando.Cnpj, comando.InscricaoEstadual, comando.Segmento, comando.CnaePrincipal, comando.CnaesSecundarios, comando.Cep, comando.Logradouro, comando.Numero, comando.Bairro, comando.Cidade, comando.Uf);
        await _empresaRepositorio.AtualizarAsync(empresa);
        return ResultadoDto.ComSucesso(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso);
    }
}
