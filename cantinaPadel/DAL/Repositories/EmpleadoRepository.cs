using System;
using System.Collections.Generic;
using System.Linq;
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
                    e.NombreUsuario == nombreUsuario &&
                    e.Contrasena == contrasena &&
                    e.Activo == true);
        }

        // Listar todo: Trae los empleados con su persona asociada
        public List<Empleado> ObtenerTodos()
        {
            using var context = new AppDbContext();
            return context.Empleados
                .Include(e => e.Persona)
                .ToList();
        }

        // Se guarda el alta y modificación controlando entidades desconectadas
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
                // Modificación y Baja: se busca el registro real trackeado por este contexto
                var empleadoDb = context.Empleados
                    .Include(e => e.Persona)
                    .FirstOrDefault(e => e.IdEmpleado == empleado.IdEmpleado);

                if (empleadoDb != null)
                {
                    // Se actualizan los datos del Empleado (incluyendo el estado Activo)
                    empleadoDb.NombreUsuario = empleado.NombreUsuario;
                    empleadoDb.Contrasena = empleado.Contrasena;
                    empleadoDb.Rol = empleado.Rol;
                    empleadoDb.Activo = empleado.Activo; // Acá se guarda la Baja/Alta lógica

                    // Se actualizan los datos de la Persona asociada
                    if (empleadoDb.Persona != null && empleado.Persona != null)
                    {
                        empleadoDb.Persona.Apellido = empleado.Persona.Apellido;
                        empleadoDb.Persona.Nombre = empleado.Persona.Nombre;
                        empleadoDb.Persona.Telefono = empleado.Persona.Telefono;
                    }
                }
            }

            // Se impactan los cambios de forma segura
            context.SaveChanges();
        }

        // Validar unicidad de usuario
        // Se verifica si el nombre de usuario ya existe (excluyendo al empleado actual si es una edición)
        public bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir)
        {
            using var context = new AppDbContext();
            return context.Empleados
                .Any(e => e.NombreUsuario == nombreUsuario && e.IdEmpleado != idEmpleadoExcluir);
        }

        // Se valida la unicidad del DNI buscando por la relación de Empleado
        public bool ExisteDni(string dni, int idEmpleadoExcluir)
        {
            using var context = new AppDbContext();

            // Se busca si el DNI ya existe en el sistema, excluyendo al empleado que se está editando mediante su navegación
            return context.Empleados
                .Include(e => e.Persona)
                .Any(e => e.Persona.Dni == dni && e.IdEmpleado != idEmpleadoExcluir);
        }
    }
}