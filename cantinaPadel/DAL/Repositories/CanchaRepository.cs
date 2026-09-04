using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class CanchaRepository : ICanchaRepository
    {
        public List<Cancha> ObtenerTodas(bool? activa = true)
        {
            using var ctx = new AppDbContext();
            IQueryable<Cancha> query = ctx.Canchas.Include(c => c.Producto);

            if (activa.HasValue)
                query = query.Where(c => c.Activa == activa.Value);

            return query.OrderBy(c => c.Nombre).ToList();
        }

        public Cancha? ObtenerPorId(int idCancha)
        {
            using var ctx = new AppDbContext();
            return ctx.Canchas
                .Include(c => c.Producto)
                .FirstOrDefault(c => c.IdCancha == idCancha);
        }

        public List<Cancha> ObtenerActivas()
        {
            using var ctx = new AppDbContext();
            return ctx.Canchas
                .Include(c => c.Producto)
                .Where(c => c.Activa)
                .OrderBy(c => c.Nombre)
                .ToList();
        }

        // El nombre debe ser único entre todas las canchas (activas o no)
        public bool ExisteNombre(string nombre, int? idCanchaExcluir = null)
        {
            using var ctx = new AppDbContext();
            return ctx.Canchas.Any(c =>
                c.Nombre.ToLower() == nombre.ToLower() &&
                (!idCanchaExcluir.HasValue || c.IdCancha != idCanchaExcluir.Value));
        }

        public int ObtenerIdCategoriaHoraPadel()
        {
            using var ctx = new AppDbContext();
            var categoria = ctx.Categorias.FirstOrDefault(c => c.Nombre == "Hora Padel");

            if (categoria == null)
                throw new InvalidOperationException(
                    "No existe la categoría 'Hora Padel'. Debe crearse en la base antes de dar de alta una cancha.");

            return categoria.IdCategoria;
        }

        public void Agregar(Cancha cancha)
        {
            using var ctx = new AppDbContext();
            // El producto "Hora Padel" viaja embebido en cancha.Producto y EF lo inserta junto con la cancha en la misma transacción
            ctx.Canchas.Add(cancha);
            ctx.SaveChanges();
        }

        public void Modificar(Cancha cancha)
        {
            using var ctx = new AppDbContext();
            var existente = ctx.Canchas
                .Include(c => c.Producto)
                .FirstOrDefault(c => c.IdCancha == cancha.IdCancha);

            if (existente == null) return;

            existente.Nombre = cancha.Nombre;
            existente.Activa = cancha.Activa;

            ctx.SaveChanges();
        }

        public void CambiarEstado(int idCancha, bool nuevoEstado)
        {
            using var ctx = new AppDbContext();
            var cancha = ctx.Canchas.Find(idCancha);
            if (cancha != null)
            {
                cancha.Activa = nuevoEstado;
                ctx.SaveChanges();
            }
        }
    }
}