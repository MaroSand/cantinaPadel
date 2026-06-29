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

        // Listar todo: Trae los empleados con su persona asociada (Para el FrmListadoEmpleados)
        public List<Empleado> ObtenerTodos()
        {
            using var context = new AppDbContext();
            return context.Empleados
                .Include(e => e.Persona)
                .ToList();
        }

        // Guardar alta y modificación
        public void Guardar(Empleado empleado)
        {
            using var context = new AppDbContext();

            if (empleado.IdEmpleado == 0)
            {
                // Alta: Si el id es 0, es un empleado nuevo
                context.Empleados.Add(empleado);
            }
            else
            {
                // Modificación: Si ya tiene id, se adjunta y marcam como modificado
                context.Entry(empleado).State = EntityState.Modified;
                context.Entry(empleado.Persona).State = EntityState.Modified;
            }

            context.SaveChanges();
        }

        // Validad unicidad de usuario
        // Verifica si el nombre de usuario ya existe (excluyendo al empleado actual si es una edición)
        public bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir)
        {
            using var context = new AppDbContext();
            return context.Empleados
                .Any(e => e.NombreUsuario == nombreUsuario && e.IdEmpleado != idEmpleadoExcluir);
        }

        // Validad unicidad de DNI
        public bool ExisteDni(string dni, int idPersonaExcluir)
        {
            using var context = new AppDbContext();
            return context.Personas
                .Any(p => p.Dni == dni && p.IdPersona != idPersonaExcluir);
        }
    }
}