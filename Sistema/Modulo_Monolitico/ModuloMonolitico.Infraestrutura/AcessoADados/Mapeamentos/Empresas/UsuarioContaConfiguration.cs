using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Empresas;

public class UsuarioContaConfiguration : IEntityTypeConfiguration<UsuarioConta>
{
    public void Configure(EntityTypeBuilder<UsuarioConta> builder)
    {
        builder.ToTable("usuarios_contas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.ContaId).HasColumnName("conta_id");
        builder.Property(x => x.EhTitular).HasColumnName("is_titular");
        builder.Property(x => x.Ativo).HasColumnName("ativo");
    }
}

