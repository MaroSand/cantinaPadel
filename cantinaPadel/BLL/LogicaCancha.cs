using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaCancha
    {
        private readonly ICanchaRepository _repo;

        public LogicaCancha()
            : this(new CanchaRepository())
        {
        }

        public LogicaCancha(ICanchaRepository repo)
        {
            _repo = repo;
        }

        public List<Cancha> ObtenerTodas(bool? activa = true) => _repo.ObtenerTodas(activa);

        public Cancha? ObtenerPorId(int idCancha) => _repo.ObtenerPorId(idCancha);

        // Alta: además de la cancha, crea el producto "Hora Padel" asociado (categoría "Hora Padel", precio = precio por hora ingresado)
        public void Agregar(Cancha cancha)
        {
            Validar(cancha, esAlta: true);

            cancha.Producto = new Producto
            {
                Nombre = $"Hora Padel - {cancha.Nombre}",
                IdCategoria = _repo.ObtenerIdCategoriaHoraPadel(),
                PrecioVenta = cancha.PrecioHora,
                Activo = true
            };

            _repo.Agregar(cancha);
        }

        // El nombre y el precio por hora pueden cambiar; el precio impacta directo sobre el producto ya asociado
        public void Modificar(Cancha cancha)
        {
            Validar(cancha, esAlta: false);

            _repo.Modificar(cancha);
        }

        public void CambiarEstado(int idCancha, bool nuevoEstado) => _repo.CambiarEstado(idCancha, nuevoEstado);

        private void Validar(Cancha cancha, bool esAlta)
        {
            if (cancha == null)
                throw new ArgumentException("Los datos de la cancha son obligatorios.");

            // Valida nombre obligatorio/longitud y precio > 0 (ver Cancha.Validar)
            cancha.Validar();

            int? idExcluir = esAlta ? null : cancha.IdCancha;
            if (_repo.ExisteNombre(cancha.Nombre, idExcluir))
                throw new ArgumentException($"Ya existe una cancha activa con el nombre '{cancha.Nombre}'.");
        }
    }
}