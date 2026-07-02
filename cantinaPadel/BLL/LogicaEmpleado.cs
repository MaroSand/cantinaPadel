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
            // Guard de entrada: no se le puede pedir a una referencia null que se autovalide
            if (empleado == null) throw new ArgumentException("Los datos del empleado no pueden estar vacíos.");

            // Formato/estructura (Persona + campos propios de Empleado) → responsabilidad del modelo
            empleado.ValidarFormato(dniObligatorio: true);

            // Validaciones de unicidad con la bd
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