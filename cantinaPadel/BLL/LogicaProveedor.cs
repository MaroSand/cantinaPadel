using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaProveedor
    {
        private readonly IProveedorRepositorio _repo;

        public LogicaProveedor()
        {
            _repo = new ProveedorRepositorio();
        }

        public List<Proveedor> ObtenerTodos() => _repo.ObtenerTodos();

        public List<Proveedor> Buscar(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return _repo.ObtenerTodos();

            return _repo.BuscarPorNombreOCuit(termino);
        }

        public Proveedor? ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        // Alta: valida campos obligatorios antes de persistir
        public (bool ok, string error) Agregar(Persona persona, Proveedor proveedor)
        {
            var validacion = Validar(persona);
            if (!validacion.ok) return validacion;

            _repo.Agregar(persona, proveedor);
            return (true, string.Empty);
        }

        // Modificación: mismas validaciones que el alta
        public (bool ok, string error) Modificar(Persona persona, Proveedor proveedor)
        {
            var validacion = Validar(persona);
            if (!validacion.ok) return validacion;

            _repo.Modificar(persona, proveedor);
            return (true, string.Empty);
        }

        public void BajaLogica(int idProveedor) => _repo.BajaLogica(idProveedor);

        // Validaciones de campos obligatorios (US-05)
        private (bool ok, string error) Validar(Persona persona)
        {
            if (string.IsNullOrWhiteSpace(persona.Nombre))
                return (false, "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(persona.Apellido))
                return (false, "El apellido es obligatorio.");

            // CUIT: si se ingresa, debe tener formato XX-XXXXXXXX-X (13 chars con guiones)
            if (!string.IsNullOrWhiteSpace(persona.Cuit) && persona.Cuit.Length != 13)
                return (false, "El CUIT debe tener el formato XX-XXXXXXXX-X.");

            return (true, string.Empty);
        }
    }
}