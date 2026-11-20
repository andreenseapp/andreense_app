namespace ModuloMonolitico.Aplicacao.DTOs;

public class ResultadoDto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;

    public static ResultadoDto ComSucesso(string mensagem = "") => new() { Sucesso = true, Mensagem = mensagem };
    public static ResultadoDto Falha(string mensagem) => new() { Sucesso = false, Mensagem = mensagem };
}

public class ResultadoDto<T> : ResultadoDto
{
    public T? Dados { get; set; }

    public static ResultadoDto<T> ComSucesso(T dados, string mensagem = "") => new() { Sucesso = true, Dados = dados, Mensagem = mensagem };
    public new static ResultadoDto<T> Falha(string mensagem) => new() { Sucesso = false, Mensagem = mensagem };
}

public class ResultadoAutenticacaoDto : ResultadoDto
{
    public string? TokenAcesso { get; set; }
    public string? TokenRenovacao { get; set; }

    public static ResultadoAutenticacaoDto ComSucesso(string token, string tokenRenovacao, string mensagem = "") =>
        new() { Sucesso = true, TokenAcesso = token, TokenRenovacao = tokenRenovacao, Mensagem = mensagem };

    public new static ResultadoAutenticacaoDto Falha(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem };
}
