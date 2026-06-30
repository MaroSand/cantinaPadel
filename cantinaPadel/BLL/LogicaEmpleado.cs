using System;
using System.Collections.Generic;
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
            return _empleadoRepository.ObtenerTodos();
        }

        public void RegistrarOGuardar(Empleado empleado)
        {
            // Se verifica que la entidad principal y su navegación no sean nulas
            if (empleado == null) throw new ArgumentException("Los datos del empleado no pueden estar vacíos.");
            if (empleado.Persona == null) throw new ArgumentException("Los datos personales del empleado no pueden estar vacíos.");

            // Validaciones de la tabla Persona
            empleado.Persona.ValidarDatosComunes(dniObligatorio: true);

            // Validaciones de la tabla Empleado
            empleado.NombreUsuario = empleado.NombreUsuario?.Trim() ?? "";
            empleado.Contrasena = empleado.Contrasena?.Trim() ?? "";
            empleado.Rol = empleado.Rol?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(empleado.NombreUsuario))
                throw new ArgumentException("El campo Nombre de Usuario es obligatorio.");

            if (empleado.NombreUsuario.Length > 50)
                throw new ArgumentException("El Nombre de Usuario no puede exceder los 50 caracteres.");

            if (string.IsNullOrWhiteSpace(empleado.Contrasena))
                throw new ArgumentException("El campo Contraseña es obligatorio.");

            if (empleado.Contrasena.Length > 255)
                throw new ArgumentException("La Contraseña no puede exceder los 9 caracteres.");

            if (string.IsNullOrWhiteSpace(empleado.Rol))
                throw new ArgumentException("Debe seleccionar un Rol válido para el empleado.");


            // Validaciones de unicidad con la bd
            if (_empleadoRepository.ExisteUsuario(empleado.NombreUsuario, empleado.IdEmpleado))
            {
                throw new ArgumentException("El nombre de usuario ya está registrado en el sistema.");
            }

            if (_empleadoRepository.ExisteDni(empleado.Persona.Dni!, empleado.IdEmpleado))
            {
                throw new ArgumentException("El DNI ingresado ya pertenece a otra persona registrada.");
            }

            // Si pasa todas las validaciones de negocio, se envía al repositorio
            _empleadoRepository.Guardar(empleado);
        }
    }
}
