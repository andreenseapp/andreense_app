namespace ModuloMonolitico.Dominio.InterfacesDeRepositorio;

public record MarcaDto(Guid Id, string Nome, string? Descricao, bool Ativo);
