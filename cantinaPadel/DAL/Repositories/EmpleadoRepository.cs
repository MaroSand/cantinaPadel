using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        public Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena)
        {
            // Cada operación crea su propio contexto y lo cierra al terminar.
            using var context = new AppDbContext();

            // .Include() = JOIN FETCH
            // Sin Include(), Persona quedaría null (no hay lazy loading por defecto)
            return context.Empleados
                .Include(e => e.Persona)
                .FirstOrDefault(e =>
                    e.NombreUsuario  == nombreUsuario  &&
                    e.Contrasena == contrasena &&
                    e.Activo         == true);
        }
    }
}