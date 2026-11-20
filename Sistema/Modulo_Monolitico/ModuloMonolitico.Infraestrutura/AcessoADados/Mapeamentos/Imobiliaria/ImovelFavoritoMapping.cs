using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Imobiliaria;

public class ImovelFavoritoMapping : IEntityTypeConfiguration<ImovelFavorito>
{
    public void Configure(EntityTypeBuilder<ImovelFavorito> builder)
    {
        builder.ToTable("imoveis_favoritos");
        
        builder.HasKey(c => new { c.UsuarioId, c.ImovelId });

        builder.Property(c => c.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(c => c.ImovelId).HasColumnName("imovel_id").IsRequired();
        builder.Property(c => c.DataCriacao).HasColumnName("data_criacao").IsRequired();
        builder.Ignore(c => c.Imovel);
    }
}
