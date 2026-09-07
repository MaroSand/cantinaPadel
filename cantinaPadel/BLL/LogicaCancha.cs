using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaCancha
    {
        private readonly ICanchaRepository _repo;
        private readonly IHorarioCanchaRepository _horarioRepo;

        public LogicaCancha()
            : this(new CanchaRepository(), new HorarioCanchaRepository())
        {
        }

        public LogicaCancha(ICanchaRepository repo)
            : this(repo, new HorarioCanchaRepository())
        {
        }

        public LogicaCancha(ICanchaRepository repo, IHorarioCanchaRepository horarioRepo)
        {
            _repo = repo;
            _horarioRepo = horarioRepo;
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

            // Al dar de baja la cancha, sus horarios fijos dejan de tener sentido como "vigentes":
            // se desactivan en cascada para no dejar horarios activos de una cancha inactiva
            // No se hace lo inverso al reactivar: los horarios quedan para que se revisen y reactiven a mano
            // (podrían solaparse con otros que se hayan cargado mientras tanto)
            if (!nuevoEstado)
                _horarioRepo.DesactivarTodosPorCancha(idCancha);
        }

        private void Validar(Cancha cancha, bool esAlta)
        {
            if (cancha == null)
                throw new ArgumentException("Los datos de la cancha son obligatorios.");

            // Valida nombre obligatorio y longitud (ver Cancha.Validar). El precio se maneja aparte, desde Actualización de Precios
            cancha.Validar();

            int? idExcluir = esAlta ? null : cancha.IdCancha;
            if (_repo.ExisteNombre(cancha.Nombre, idExcluir))
                throw new ArgumentException($"Ya existe una cancha activa con el nombre '{cancha.Nombre}'.");
        }
    }
}