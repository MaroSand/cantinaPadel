using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaProveedor
    {
        private readonly IProveedorRepository _repo;

        public LogicaProveedor() : this(new ProveedorRepository())
        {
        }

        public LogicaProveedor(IProveedorRepository repo)
        {
            _repo = repo;
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
        public void Agregar(Persona persona, Proveedor proveedor)
        {
            Validar(persona, proveedor);

            _repo.Agregar(persona, proveedor);
        }

        // Modificación: mismas validaciones que el alta
        public void Modificar(Persona persona, Proveedor proveedor)
        {
            Validar(persona, proveedor);

            _repo.Modificar(persona, proveedor);
        }

        // Activa/Desactiva (toggle) el proveedor según su estado actual
        public void BajaLogica(int idProveedor) => _repo.BajaLogica(idProveedor);

        // Validaciones de negocio para alta/modificación de Proveedor.
        // Los guards de null se quedan acá (no se le puede pedir a una referencia null
        // que se autovalide); el formato de cada objeto lo resuelve cada uno por su cuenta.
        public void Validar(Persona persona, Proveedor proveedor)
        {
            if (persona == null)
                throw new ArgumentException("Los datos de la persona son obligatorios.");

            if (proveedor == null)
                throw new ArgumentException("Los datos del proveedor son obligatorios.");

            persona.ValidarDatosComunes();
            proveedor.ValidarNombreEmpresa();
        }
    }
}