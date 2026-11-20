using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuloMonolitico.Dominio.Compartilhado;

public class ResultadoPaginado<T>
{
    public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
    public int TotalItens { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }

    public ResultadoPaginado() { }

    public ResultadoPaginado(IEnumerable<T> itens, int totalItens, int paginaAtual, int tamanhoPagina)
    {
        Itens = itens;
        TotalItens = totalItens;
        PaginaAtual = paginaAtual;
        TotalPaginas = tamanhoPagina == 0 ? 0 : (int)Math.Ceiling((double)TotalItens / tamanhoPagina);
    }
}
