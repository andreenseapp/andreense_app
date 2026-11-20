using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;

internal static class AuxiliarComandoEmpresa
{
    public static void AplicarDados(Empresa empresa, string nome, string? razaoSocial, string? cnpj, string? inscricaoEstadual, string? segmento, string? cnaePrincipal, string? cnaesSecundarios, string? cep, string? logradouro, string? numero, string? bairro, string? cidade, string? uf)
    {
        empresa.Nome = nome;
        empresa.RazaoSocial = razaoSocial;
        empresa.Cnpj = cnpj;
        empresa.InscricaoEstadual = inscricaoEstadual;
        empresa.Segmento = segmento;
        empresa.CnaePrincipal = cnaePrincipal;
        empresa.CnaesSecundarios = ConverterCnaesSecundarios(cnaesSecundarios, empresa.Id);
        empresa.Cep = cep;
        empresa.Logradouro = logradouro;
        empresa.Numero = numero;
        empresa.Bairro = bairro;
        empresa.Cidade = cidade;
        empresa.Uf = uf;
    }

    private static List<EmpresaCnaeSecundario> ConverterCnaesSecundarios(string? cnaesString, Guid empresaId)
    {
        if (string.IsNullOrWhiteSpace(cnaesString))
            return new List<EmpresaCnaeSecundario>();

        return cnaesString
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .Select(cnae => new EmpresaCnaeSecundario
            {
                Id = Guid.NewGuid(),
                EmpresaId = empresaId,
                Cnae = cnae
            })
            .ToList();
    }
}
