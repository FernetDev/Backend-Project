using WebAPI_Log.Entities;
using Microsoft.EntityFrameworkCore;


namespace WebAPI_Log.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Perfil> Perfiles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            //Defino como va a estar estructurada mi tabla

            modelBuilder.Entity<Perfil>(tb => {
                tb.HasKey(col => col.IdPerfil);
                tb.Property(col => col.IdPerfil).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.ToTable("Perfil");
                tb.HasData(
                       new Perfil { IdPerfil = 1, Nombre = "Alumno Normal" },
                       new Perfil { IdPerfil = 2, Nombre = "Alumno Personalizado" },
                       new Perfil { IdPerfil = 3, Nombre = "Alumno PowerLifter" }
                   );
            });

            modelBuilder.Entity<Cliente>(tb => {
                tb.HasKey(col => col.IdCliente);
                tb.Property(col => col.IdCliente).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.NombreCompleto).HasMaxLength(50);
                tb.HasOne(col => col.PerfilReferencia).WithMany(p => p.ClientesReferencia)
                .HasForeignKey(col => col.IdPerfil);
                tb.ToTable("Cliente");
            });

        }
    }
}
