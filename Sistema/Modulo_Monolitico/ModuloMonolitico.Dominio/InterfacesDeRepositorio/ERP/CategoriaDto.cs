namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public record CategoriaDto(Guid Id, string Nome, string? Descricao, bool Ativo);
