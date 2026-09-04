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

        public void Agregar(Cancha cancha)
        {
            Validar(cancha, esAlta: true);

            cancha.Producto = new Producto
            {
                Nombre = $"Hora Padel - {cancha.Nombre}",
                IdCategoria = _repo.ObtenerIdCategoriaHoraPadel(),
                PrecioVenta = 0m,
                Activo = true
            };

            _repo.Agregar(cancha);
        }

        // El nombre y el estado pueden cambiar acá; el precio se maneja aparte, desde Actualización de Precios
        public void Modificar(Cancha cancha)
        {
            Validar(cancha, esAlta: false);

            _repo.Modificar(cancha);
        }

        public void CambiarEstado(int idCancha, bool nuevoEstado)
        {
            if (nuevoEstado)
            {
                var cancha = _repo.ObtenerPorId(idCancha);
                if (cancha == null)
                    throw new ArgumentException("La cancha no existe.");

                if (_repo.ExisteNombre(cancha.Nombre, idCancha))
                    throw new ArgumentException(
                        $"No se puede activar: ya existe una cancha activa con el nombre '{cancha.Nombre}'.");
            }

            _repo.CambiarEstado(idCancha, nuevoEstado);
        }

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