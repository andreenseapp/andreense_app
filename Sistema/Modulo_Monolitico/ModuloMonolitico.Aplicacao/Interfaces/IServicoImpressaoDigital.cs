using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.ObjetosDeValor.Base;

namespace ModuloMonolitico.Aplicacao.Interfaces;
public interface IServicoImpressaoDigital
{
    string GerarHashImpressaoDigital(InfoImpressaoDigital info);
}


