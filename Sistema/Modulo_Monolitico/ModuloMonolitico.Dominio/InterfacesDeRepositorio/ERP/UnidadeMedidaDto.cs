namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public record UnidadeMedidaDto(string Sigla, string Nome, Guid? ContaId, bool Ativo);
