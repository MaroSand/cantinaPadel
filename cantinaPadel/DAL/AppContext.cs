using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace cantinaPadel.DAL
{
    public class AppDbContext : DbContext
    {
        // Cada DbSet es una tabla.
        public DbSet<Persona>   Personas   { get; set; }
        public DbSet<Empleado>  Empleados  { get; set; }
        public DbSet<Cliente>   Clientes   { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string connStr = ConfigurationManager
                .ConnectionStrings["cantinaPadel"]
                .ConnectionString;

            options.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 0)));
        }
        
        //TODO ver bien esto para cada model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación Persona → Empleado (1 a 0..1)
            // HasForeignKey le dice a EF qué columna es la FK
            modelBuilder.Entity<Empleado>()
                .HasOne(e => e.Persona)
                .WithOne(p => p.Empleado)
                .HasForeignKey<Empleado>(e => e.IdPersona);

            // Relación Persona → Cliente (1 a 0..1)
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Persona)
                .WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(c => c.IdPersona);

            // Relación Persona → Proveedor (1 a 0..1)
            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.Persona)
                .WithOne(per => per.Proveedor)
                .HasForeignKey<Proveedor>(p => p.IdPersona);
        }
    }
}