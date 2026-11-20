using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Autenticacao;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Nome).HasColumnName("nome");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.SenhaHash).HasColumnName("senha_hash");
        builder.Property(x => x.Ativo).HasColumnName("ativo");
        builder.Property(x => x.EmailValidado).HasColumnName("email_validado");
        builder.Property(x => x.UrlImagem).HasColumnName("url_imagem");
    }
}

