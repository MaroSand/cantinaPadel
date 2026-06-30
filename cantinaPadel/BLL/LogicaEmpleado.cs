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
            // Valida que el nombre de usuario no esté repetido
            // Se pasa IdEmpleado para que si es edición (id > 0) se excluya a sí mismo
            if (_empleadoRepository.ExisteUsuario(empleado.NombreUsuario, empleado.IdEmpleado))
            {
                throw new ArgumentException("El nombre de usuario ya está registrado en el sistema.");
            }

            // Se valida que el DNI no esté duplicado en el sistema
            int idPersonaExcluir = empleado.IdEmpleado;
            if (_empleadoRepository.ExisteDni(empleado.Persona.Dni, idPersonaExcluir))
            {
                throw new ArgumentException("El DNI ingresado ya pertenece a otra persona registrada.");
            }

            // Si pasa las validaciones, guarda (para Alta y Modificación)
            _empleadoRepository.Guardar(empleado);
        }
    }
}