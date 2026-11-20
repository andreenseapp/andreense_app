using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades.Imobiliaria;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Imobiliaria;

public class AnuncioVisualizadoMapping : IEntityTypeConfiguration<AnuncioVisualizado>
{
    public void Configure(EntityTypeBuilder<AnuncioVisualizado> builder)
    {
        builder.ToTable("imoveis_visualizados");

        builder.HasKey(v => new { v.UsuarioId, v.AnuncioId });

        builder.Property(v => v.UsuarioId).HasColumnName("usuario_id").IsRequired();
        builder.Property(v => v.AnuncioId).HasColumnName("anuncio_id").IsRequired();
        builder.Property(v => v.DataVisualizacao).HasColumnName("data_visualizacao").IsRequired();
    }
}
