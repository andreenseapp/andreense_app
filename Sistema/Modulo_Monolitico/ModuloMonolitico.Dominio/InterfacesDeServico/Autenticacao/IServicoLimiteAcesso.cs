namespace ModuloMonolitico.Dominio.InterfacesDeServico;

public interface IServicoLimiteAcesso
{
    bool EstaBloqueado(string chave);
    void RegistrarTentativaFalha(string chave);
    void LimparTentativas(string chave);
}
