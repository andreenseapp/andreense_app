using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Gestao;

public class TipoVendedorConfiguration : IEntityTypeConfiguration<TipoVendedor>
{
    public void Configure(EntityTypeBuilder<TipoVendedor> builder)
    {
        builder.ToTable("tipos_vendedor");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.ContaId)
            .HasColumnName("conta_id")
            .IsRequired();

        builder.Property(t => t.Nome)
            .HasColumnName("nome")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        builder.Property(t => t.Ativo)
            .HasColumnName("ativo")
            .IsRequired();

        builder.Property(t => t.DataInclusao)
            .HasColumnName("data_inclusao")
            .IsRequired();

        builder.Property(t => t.OperadorInclusao)
            .HasColumnName("operador_inclusao");

        builder.Property(t => t.DataUltimaAtualizacao)
            .HasColumnName("data_ultima_atualizacao")
            .IsRequired();
            
        builder.HasIndex(t => t.ContaId);
    }
}
