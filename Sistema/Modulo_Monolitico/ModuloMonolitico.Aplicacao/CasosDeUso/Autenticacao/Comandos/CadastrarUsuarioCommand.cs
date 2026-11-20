using MediatR;
using ModuloMonolitico.Aplicacao.DTOs;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;

namespace ModuloMonolitico.Aplicacao.CasosDeUso;
public class CadastrarUsuarioCommand : IRequest<ResultadoDto>
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string ConfirmarSenha { get; set; } = string.Empty;
}

public class CadastrarUsuarioCommandHandler : IRequestHandler<CadastrarUsuarioCommand, ResultadoDto>
{
    private readonly IUsuarioRepositorio _usuarioRepo;

    public CadastrarUsuarioCommandHandler(IUsuarioRepositorio usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public async Task<ResultadoDto> Handle(CadastrarUsuarioCommand comando, CancellationToken cancellationToken)
    {
        if (comando.Senha != comando.ConfirmarSenha)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.SenhasNaoCoincidem);

        var jaExiste = await _usuarioRepo.EmailJaExisteAsync(comando.Email);
        if (jaExiste)
            return ResultadoDto.Falha(ModuloMonolitico.Dominio.Configuracoes.MensagensSistema.Autenticacao.EmailJaCadastrado);

        var senhaHash = BCrypt.Net.BCrypt.HashPassword(comando.Senha);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = comando.Nome,
            Email = comando.Email,
            SenhaHash = senhaHash,
            Ativo = true,
            EmailValidado = false
        };

        var conta = new Conta
        {
            Id = Guid.NewGuid(),
            Nome = comando.Nome
        };

        var empresa = new Empresa
        {
            Id = Guid.NewGuid(),
            ContaId = conta.Id,
            Nome = comando.Nome,
            RazaoSocial = comando.Nome,
            Cnpj = "00000000000000",
            Ativo = true
        };

        await _usuarioRepo.CadastrarComContaEEmpresaAsync(usuario, conta, empresa);

        return ResultadoDto.ComSucesso("Conta criada com sucesso! Bem-vindo ao sistema.");
    }
}



