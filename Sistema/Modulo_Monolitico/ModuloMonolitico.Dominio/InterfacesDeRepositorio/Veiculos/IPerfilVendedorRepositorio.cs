using ModuloMonolitico.Dominio.Entidades.Veiculos;
using ModuloMonolitico.Dominio.Veiculos.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio.Veiculos;

public interface IPerfilVendedorRepositorio
{
    Task<PerfilVendedorDto?> ObterPorContaAsync(Guid contaId);
    Task CadastrarAsync(PerfilVendedor perfil);
    Task AtualizarAsync(PerfilVendedor perfil);
    Task<PerfilVendedor?> ObterEntidadePorContaAsync(Guid contaId);
}
