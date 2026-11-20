using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Mapeamentos.Empresas;

public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("empresas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ContaId).HasColumnName("conta_id");
        builder.Property(x => x.Nome).HasColumnName("nome");
        builder.Property(x => x.RazaoSocial).HasColumnName("razao_social");
        builder.Property(x => x.Cnpj).HasColumnName("cnpj");
        builder.Property(x => x.InscricaoEstadual).HasColumnName("inscricao_estadual");
        builder.Property(x => x.Segmento).HasColumnName("segmento");
        builder.Property(x => x.CnaePrincipal).HasColumnName("cnae_principal");
        builder.Property(x => x.Cep).HasColumnName("cep");
        builder.Property(x => x.Logradouro).HasColumnName("logradouro");
        builder.Property(x => x.Numero).HasColumnName("numero");
        builder.Property(x => x.Bairro).HasColumnName("bairro");
        builder.Property(x => x.Cidade).HasColumnName("cidade");
        builder.Property(x => x.Uf).HasColumnName("uf");
        builder.Property(x => x.Ativo).HasColumnName("ativo");
        builder.Property(x => x.DataInclusao).HasColumnName("data_inclusao");
        builder.Property(x => x.DataUltimaAtualizacao).HasColumnName("data_ultima_atualizacao");
        builder.Ignore(x => x.CnaesSecundarios);
    }
}

