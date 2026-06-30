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

            // valida tabla Persona
            empleado.Persona.Dni = empleado.Persona.Dni?.Trim() ?? "";
            empleado.Persona.Apellido = empleado.Persona.Apellido?.Trim() ?? "";
            empleado.Persona.Nombre = empleado.Persona.Nombre?.Trim() ?? "";
            empleado.Persona.Telefono = empleado.Persona.Telefono?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(empleado.Persona.Dni))
                throw new ArgumentException("El campo DNI es obligatorio y no puede contener solo espacios.");

            if (empleado.Persona.Dni.Length < 7 || empleado.Persona.Dni.Length > 8)
                throw new ArgumentException("El DNI debe tener entre 7 y 8 dígitos numéricos.");

            if (!empleado.Persona.Dni.All(char.IsDigit))
                throw new ArgumentException("El DNI debe contener únicamente números.");

            if (string.IsNullOrWhiteSpace(empleado.Persona.Apellido))
                throw new ArgumentException("El campo Apellido es obligatorio y no puede contener solo espacios.");

            if (empleado.Persona.Apellido.Length > 100)
                throw new ArgumentException("El Apellido no puede exceder los 30 caracteres.");

            if (string.IsNullOrWhiteSpace(empleado.Persona.Nombre))
                throw new ArgumentException("El campo Nombre es obligatorio y no puede contener solo espacios.");

            if (empleado.Persona.Nombre.Length > 100)
                throw new ArgumentException("El Nombre no puede exceder los 30 caracteres.");

            if (!string.IsNullOrEmpty(empleado.Persona.Telefono))
            {
                if (empleado.Persona.Telefono.Length > 20)
                    throw new ArgumentException("El Teléfono no puede exceder los 10 caracteres.");

                if (!empleado.Persona.Telefono.All(char.IsDigit))
                    throw new ArgumentException("El campo Teléfono debe contener únicamente números.");
            }

            if (empleado.Persona.Apellido.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
                throw new ArgumentException("El Apellido no puede contener números ni caracteres especiales.");

            if (empleado.Persona.Nombre.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
                throw new ArgumentException("El Nombre no puede contener números ni caracteres especiales.");

            // Validaciones de la tabla Empleado
            empleado.NombreUsuario = empleado.NombreUsuario?.Trim() ?? "";
            empleado.Contrasena = empleado.Contrasena?.Trim() ?? "";
            empleado.Rol = empleado.Rol?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(empleado.NombreUsuario))
                throw new ArgumentException("El campo Nombre de Usuario es obligatorio.");

            if (empleado.NombreUsuario.Length > 50)
                throw new ArgumentException("El Nombre de Usuario no puede exceder los 30 caracteres.");

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

            if (_empleadoRepository.ExisteDni(empleado.Persona.Dni, empleado.IdEmpleado))
            {
                throw new ArgumentException("El DNI ingresado ya pertenece a otra persona registrada.");
            }

            // Si pasa todas las validaciones de negocio, se envía al repositorio
            _empleadoRepository.Guardar(empleado);
        }
    }
}