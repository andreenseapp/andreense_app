using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

public record CadastrarEmpresaCommand : IRequest<ResultadoDto>
{
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

public class CadastrarEmpresaCommandHandler : IRequestHandler<CadastrarEmpresaCommand, ResultadoDto>
{
    private readonly IEmpresaRepositorio _empresaRepositorio;

    public CadastrarEmpresaCommandHandler(IEmpresaRepositorio empresaRepositorio)
        => _empresaRepositorio = empresaRepositorio;

    public async Task<ResultadoDto> Handle(CadastrarEmpresaCommand comando, CancellationToken cancellationToken)
    {
        var novaEmpresa = new Empresa
        {
            Id = Guid.NewGuid(),
            ContaId = comando.ContaId
        };
        AuxiliarComandoEmpresa.AplicarDados(novaEmpresa, comando.Nome, comando.RazaoSocial, comando.Cnpj, comando.InscricaoEstadual, comando.Segmento, comando.CnaePrincipal, comando.CnaesSecundarios, comando.Cep, comando.Logradouro, comando.Numero, comando.Bairro, comando.Cidade, comando.Uf);
        await _empresaRepositorio.CadastrarAsync(novaEmpresa);
        return ResultadoDto.ComSucesso(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Geral.Sucesso);
    }
}
