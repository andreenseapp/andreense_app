using System.Security.Cryptography;
using System.Text;
using ModuloMonolitico.Aplicacao.Interfaces;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;

namespace ModuloMonolitico.Infraestrutura.Servicos.Autenticacao;

public class ServicoImpressaoDigital : IServicoImpressaoDigital
{
    public string GerarHashImpressaoDigital(InfoImpressaoDigital info)
    {
        var textoBase = $"{info.AgenteUsuario}|{info.IdiomaAceito}|{info.Plataforma}|{info.EnderecoIp}";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoBase));
        return Convert.ToBase64String(hashBytes);
    }
}



