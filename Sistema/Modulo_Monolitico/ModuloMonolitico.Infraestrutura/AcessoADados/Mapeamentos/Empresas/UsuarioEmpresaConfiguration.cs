using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Empresas;

public class UsuarioEmpresaConfiguration : IEntityTypeConfiguration<UsuarioEmpresa>
{
    public void Configure(EntityTypeBuilder<UsuarioEmpresa> builder)
    {
        builder.ToTable("usuarios_empresas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.UsuarioId).HasColumnName("usuario_id");
        builder.Property(x => x.EmpresaId).HasColumnName("empresa_id");
        builder.Property(x => x.GrupoId).HasColumnName("grupo_id");
        builder.Property(x => x.Perfil).HasColumnName("perfil");
        builder.Property(x => x.Ativo).HasColumnName("ativo");
    }
}

