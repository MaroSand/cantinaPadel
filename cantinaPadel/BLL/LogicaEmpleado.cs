using System;
using System.Collections.Generic;
using System.Linq;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaEmpleado
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public LogicaEmpleado()
        {
            _empleadoRepository = new EmpleadoRepository();
        }

        public List<Empleado> ObtenerTodos()
        {
            // Se recupera el listado completo desde el repositorio unificado
            return _empleadoRepository.ObtenerTodos();
        }

        public void RegistrarOGuardar(Empleado empleado)
        {
            // Se verifica que la entidad principal y su navegación no sean nulas
            if (empleado == null) throw new ArgumentException("Los datos del empleado no pueden estar vacíos.");
            if (empleado.Persona == null) throw new ArgumentException("Los datos personales del empleado no pueden estar vacíos.");

            // Se ejecutan las validaciones comunes de la tabla Persona diseñadas por Facu
            empleado.Persona.ValidarDatosComunes(dniObligatorio: true);

            // --- VALIDACIONES DE LA TABLA EMPLEADO (Con Trim) ---
            empleado.NombreUsuario = empleado.NombreUsuario?.Trim() ?? "";
            empleado.Contrasena = empleado.Contrasena?.Trim() ?? "";
            empleado.Rol = empleado.Rol?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(empleado.NombreUsuario))
                throw new ArgumentException("El campo Nombre de Usuario es obligatorio.");

            // Se adopta el límite de 50 caracteres definido en la actualización de Facu
            if (empleado.NombreUsuario.Length > 50)
                throw new ArgumentException("El Nombre de Usuario no puede exceder los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(empleado.Contrasena))
                throw new ArgumentException("El campo Contraseña es obligatorio.");

            if (empleado.Contrasena.Length > 9)
                throw new ArgumentException("La Contraseña no puede exceder los 9 caracteres.");

            if (string.IsNullOrWhiteSpace(empleado.Rol))
                throw new ArgumentException("Debe seleccionar un Rol válido para el empleado.");

            // --- VALIDACIONES DE UNICIDAD CONTRA LA BASE DE DATOS ---
            if (_empleadoRepository.ExisteUsuario(empleado.NombreUsuario, empleado.IdEmpleado))
            {
                throw new ArgumentException("El nombre de usuario ya está registrado en el sistema.");
            }

            // Se controla si el documento ya se encuentra asignado a otro empleado activo o inactivo usando el operador de supresión de nulos
            if (_empleadoRepository.ExisteDni(empleado.Persona.Dni!, empleado.IdEmpleado))
            {
                throw new ArgumentException("El DNI ingresado ya pertenece a otra persona registrada.");
            }

            // Si pasa todas las validaciones de negocio específicas, se envía al repositorio para impactar los cambios
            _empleadoRepository.Guardar(empleado);
        }
    }
}