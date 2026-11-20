using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModuloMonolitico.Dominio.Entidades;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Contexto;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly ILogger<DbContext>? _logger;

    public DbContext(DbContextOptions<DbContext> options, ILogger<DbContext>? logger = null) : base(options)
    {
        _logger = logger;
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Conta> Contas { get; set; } = null!;
    public DbSet<Empresa> Empresas { get; set; } = null!;
    public DbSet<UsuarioConta> UsuariosContas { get; set; } = null!;
    public DbSet<UsuarioEmpresa> UsuariosEmpresas { get; set; } = null!;
    
    public DbSet<TipoVendedor> TiposVendedores { get; set; } = null!;
    public DbSet<ModuloMonolitico.Dominio.Entidades.Imobiliaria.ImovelFavorito> ImoveisFavoritos { get; set; } = null!;
    public DbSet<ModuloMonolitico.Dominio.Entidades.Imobiliaria.AnuncioVisualizado> AnunciosVisualizados { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AuditarModificacoes();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void AuditarModificacoes()
    {
        if (_logger == null) return;

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var entidadeNome = entry.Entity.GetType().Name;
            
            if (entry.State == EntityState.Modified)
            {
                var modifiedProperties = entry.Properties.Where(p => p.IsModified).ToList();
                foreach (var prop in modifiedProperties)
                {
                    _logger.LogInformation("Auditoria - Tabela: {Tabela} | Ação: MODIFICAR | Campo: {Campo} | De: {ValorAntigo} -> Para: {ValorNovo}", 
                        entidadeNome, prop.Metadata.Name, prop.OriginalValue, prop.CurrentValue);
                }
            }
            else
            {
                _logger.LogInformation("Auditoria - Tabela: {Tabela} | Ação: {Acao}", 
                    entidadeNome, entry.State.ToString().ToUpper());
            }
        }
    }
}

