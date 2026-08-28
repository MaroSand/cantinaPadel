using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class HorarioCanchaRepository : IHorarioCanchaRepository
    {
        public List<HorarioCancha> ObtenerTodos(bool? activo = true)
        {
            using var ctx = new AppDbContext();
            IQueryable<HorarioCancha> query = ctx.HorariosCancha.Include(h => h.Cancha);

            if (activo.HasValue)
                query = query.Where(h => h.Activo == activo.Value);

            return query
                .OrderBy(h => h.Cancha!.Nombre)
                .ThenBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .ToList();
        }

        public HorarioCancha? ObtenerPorId(int idHorario)
        {
            using var ctx = new AppDbContext();
            return ctx.HorariosCancha
                .Include(h => h.Cancha)
                .FirstOrDefault(h => h.IdHorario == idHorario);
        }

        public bool ExisteSolapamiento(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idHorarioExcluir = null)
        {
            using var ctx = new AppDbContext();
            return ctx.HorariosCancha.Any(h =>
                h.IdCancha == idCancha &&
                h.DiaSemana == diaSemana &&
                h.Activo &&
                (!idHorarioExcluir.HasValue || h.IdHorario != idHorarioExcluir.Value) &&
                h.HoraInicio < horaFin &&
                horaInicio < h.HoraFin);
        }

        public void Agregar(HorarioCancha horario)
        {
            using var ctx = new AppDbContext();
            ctx.HorariosCancha.Add(horario);
            ctx.SaveChanges();
        }

        public void Modificar(HorarioCancha horario)
        {
            using var ctx = new AppDbContext();
            ctx.HorariosCancha.Update(horario);
            ctx.SaveChanges();
        }

        public void CambiarEstado(int idHorario, bool nuevoEstado)
        {
            using var ctx = new AppDbContext();
            var horario = ctx.HorariosCancha.Find(idHorario);
            if (horario != null)
            {
                horario.Activo = nuevoEstado;
                ctx.SaveChanges();
            }
        }
    }
}