using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        // Trae todos los proveedores (activos e inactivos); el filtro de estado se hace en la UI
        public List<Proveedor> ObtenerTodos()
        {
            using var ctx = new AppDbContext();
            return ctx.Proveedores
                .Include(p => p.Persona)
                .OrderBy(p => p.Persona.Apellido)
                .ThenBy(p => p.Persona.Nombre)
                .ToList();
        }

        // Búsqueda por nombre, apellido, nombre de empresa o CUIT
        public List<Proveedor> BuscarPorNombreOCuit(string termino)
        {
            using var ctx = new AppDbContext();
            termino = termino.Trim().ToLower();

            return ctx.Proveedores
                .Include(p => p.Persona)
                .Where(p =>
                    (p.Persona.Nombre   + " " + p.Persona.Apellido).ToLower().Contains(termino) ||
                    (p.Persona.Apellido + " " + p.Persona.Nombre).ToLower().Contains(termino)   ||
                    (p.Persona.Cuit     != null && p.Persona.Cuit.Contains(termino))             ||
                    (p.NombreEmpresa    != null && p.NombreEmpresa.ToLower().Contains(termino))
                )
                .OrderBy(p => p.Persona.Apellido)
                .ToList();
        }

        public Proveedor? ObtenerPorId(int idProveedor)
        {
            using var ctx = new AppDbContext();
            return ctx.Proveedores
                .Include(p => p.Persona)
                .FirstOrDefault(p => p.IdProveedor == idProveedor);
        }

        // Alta: inserta Persona primero, luego Proveedor (TPT).
        // Transacción para que si falla alguno no queden filas huérfanas.
        public void Agregar(Persona persona, Proveedor proveedor)
        {
            using var ctx = new AppDbContext();
            using var tx  = ctx.Database.BeginTransaction();

            persona.EsProveedor = true;
            ctx.Personas.Add(persona);
            ctx.SaveChanges();              // genera id_persona

            proveedor.IdPersona = persona.IdPersona;
            ctx.Proveedores.Add(proveedor);
            ctx.SaveChanges();

            tx.Commit();
        }

        // Modificación: actualiza Persona y Proveedor en una sola transacción
        public void Modificar(Persona persona, Proveedor proveedor)
        {
            using var ctx = new AppDbContext();
            using var tx  = ctx.Database.BeginTransaction();

            ctx.Personas.Update(persona);
            ctx.Proveedores.Update(proveedor);
            ctx.SaveChanges();

            tx.Commit();
        }

        // Baja/Alta lógica: no borra filas, alterna Persona.Activo (false <-> true)
        public void BajaLogica(int idProveedor)
        {
            using var ctx = new AppDbContext();

            var proveedor = ctx.Proveedores
                .Include(p => p.Persona)
                .FirstOrDefault(p => p.IdProveedor == idProveedor);

            if (proveedor == null) return;

            // Alterna el estado: si está activo lo da de baja, si está inactivo lo reactiva
            proveedor.Persona.Activo = !proveedor.Persona.Activo;
            ctx.SaveChanges();
        }
    }
}