using System.Linq;
using System.Text.RegularExpressions;
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
            var validacion = Validar(persona, proveedor);
            if (!validacion.ok) return validacion;

            _repo.Agregar(persona, proveedor);
            return (true, string.Empty);
        }

        // Modificación: mismas validaciones que el alta
        public (bool ok, string error) Modificar(Persona persona, Proveedor proveedor)
        {
            var validacion = Validar(persona, proveedor);
            if (!validacion.ok) return validacion;

            _repo.Modificar(persona, proveedor);
            return (true, string.Empty);
        }

        // Activa/Desactiva (toggle) el proveedor según su estado actual
        public void BajaLogica(int idProveedor) => _repo.BajaLogica(idProveedor);

        // Validaciones de negocio para alta/modificación de Proveedor.
        // Público y estático en sus reglas para poder testearse de forma aislada con MSTest,
        // sin necesitar conexión a base de datos.
        public (bool ok, string error) Validar(Persona persona, Proveedor proveedor)
        {
            if (persona == null)
                return (false, "Los datos de la persona son obligatorios.");

            if (string.IsNullOrWhiteSpace(persona.Nombre))
                return (false, "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(persona.Apellido))
                return (false, "El apellido es obligatorio.");

            // DNI: si se ingresa, debe ser numérico de 7 u 8 dígitos
            if (!string.IsNullOrWhiteSpace(persona.Dni) && !Regex.IsMatch(persona.Dni, @"^\d{7,8}$"))
                return (false, "El DNI debe contener entre 7 y 8 dígitos numéricos.");

            // CUIT: si se ingresa, debe tener formato XX-XXXXXXXX-X
            if (!string.IsNullOrWhiteSpace(persona.Cuit) && !Regex.IsMatch(persona.Cuit, @"^\d{2}-\d{8}-\d{1}$"))
                return (false, "El CUIT debe tener el formato XX-XXXXXXXX-X.");

            // Teléfono: si se ingresa, solo dígitos, espacios, guiones o el símbolo +
            if (!string.IsNullOrWhiteSpace(persona.Telefono) && !Regex.IsMatch(persona.Telefono, @"^[\d\s\-\+]{6,20}$"))
                return (false, "El teléfono ingresado no es válido.");

            // Condición IVA: si se ingresa, debe ser una de las opciones válidas
            string[] condicionesValidas = { "Responsable Inscripto", "Monotributista", "Exento", "Consumidor Final" };
            if (!string.IsNullOrWhiteSpace(persona.CondicionIva) && !condicionesValidas.Contains(persona.CondicionIva))
                return (false, "La condición de IVA ingresada no es válida.");

            // Nombre de empresa: si se ingresa, no puede ser solo espacios en blanco
            if (proveedor != null && proveedor.NombreEmpresa != null && proveedor.NombreEmpresa.Trim().Length == 0)
                return (false, "El nombre de la empresa no puede estar vacío si se especifica.");

            return (true, string.Empty);
        }
    }
}   