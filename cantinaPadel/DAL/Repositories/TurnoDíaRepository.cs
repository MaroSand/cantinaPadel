using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories
{
    public class TurnoDiaRepository : ITurnoDiaRepository
    {
        public HorarioCancha? ObtenerHorarioPorFranja(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
        {
            using var ctx = new AppDbContext();
            return ctx.HorariosCancha
                .Include(h => h.Cancha)
                    .ThenInclude(c => c!.Producto)
                .FirstOrDefault(h => h.IdCancha == idCancha
                                  && h.DiaSemana == diaSemana
                                  && h.HoraInicio == horaInicio
                                  && h.HoraFin == horaFin
                                  && h.Activo);
        }

        public HorarioCancha? ObtenerHorarioPorId(int idHorario)
        {
            using var ctx = new AppDbContext();
            return ctx.HorariosCancha
                .Include(h => h.Cancha)
                .FirstOrDefault(h => h.IdHorario == idHorario);
        }

        public bool EstaDisponible(int idCancha, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
        {
            using var ctx = new AppDbContext();
            return !ctx.InstanciasTurno.Any(i =>
                i.HorarioCancha.IdCancha == idCancha &&
                i.Fecha == fecha.Date &&
                i.Estado == InstanciaTurno.EstadoActiva &&
                i.HorarioCancha.HoraInicio < horaFin &&
                horaInicio < i.HorarioCancha.HoraFin);
        }

        public void RegistrarAlquiler(TurnoReservado turno, HorarioCancha horario, InstanciaTurno instancia)
        {
            RegistrarAlquiler(turno, new List<HorarioCancha> { horario }, new List<InstanciaTurno> { instancia });
        }

        public void RegistrarAlquiler(TurnoReservado turno, List<HorarioCancha> horarios, List<InstanciaTurno> instancias)
        {
            using var ctx = new AppDbContext();
            using var tx = ctx.Database.BeginTransaction();

            ctx.TurnosReservados.Add(turno);

            for (int i = 0; i < instancias.Count; i++)
            {
                var horario = horarios[i];
                var instancia = instancias[i];

                var horarioPersistido = ctx.HorariosCancha.FirstOrDefault(h =>
                    h.IdCancha == horario.IdCancha &&
                    h.DiaSemana == horario.DiaSemana &&
                    h.HoraInicio == horario.HoraInicio &&
                    h.HoraFin == horario.HoraFin &&
                    h.Activo);

                if (horarioPersistido == null)
                {
                    horarioPersistido = horario;
                    ctx.HorariosCancha.Add(horarioPersistido);
                }

                instancia.TurnoReservado = turno;
                instancia.HorarioCancha = horarioPersistido;
                ctx.InstanciasTurno.Add(instancia);
            }

            ctx.SaveChanges();
            tx.Commit();
        }

        public List<InstanciaTurno> ObtenerInstanciasPorFecha(DateTime fecha, int? idCancha = null)
        {
            using var ctx = new AppDbContext();

            var query = ctx.InstanciasTurno
                .Include(i => i.HorarioCancha)
                    .ThenInclude(h => h.Cancha)
                        .ThenInclude(c => c!.Producto)
                .Include(i => i.TurnoReservado)
                    .ThenInclude(t => t.Cliente)
                        .ThenInclude(c => c.Persona)
                .Where(i => i.Fecha == fecha.Date
                         && i.Estado == InstanciaTurno.EstadoActiva
                         && i.TurnoReservado.Estado == TurnoReservado.EstadoActivo);

            if (idCancha.HasValue && idCancha.Value > 0)
                query = query.Where(i => i.HorarioCancha.IdCancha == idCancha.Value);

            return query
                .OrderBy(i => i.HorarioCancha.Cancha!.Nombre)
                .ThenBy(i => i.HorarioCancha.HoraInicio)
                .ToList();
        }

        public List<InstanciaTurno> ObtenerInstanciasPorCliente(int idCliente)
        {
            using var ctx = new AppDbContext();

            return ctx.InstanciasTurno
                .Include(i => i.HorarioCancha)
                    .ThenInclude(h => h.Cancha)
                        .ThenInclude(c => c!.Producto)
                .Include(i => i.TurnoReservado)
                    .ThenInclude(t => t.Cliente)
                        .ThenInclude(c => c.Persona)
                .Where(i => i.TurnoReservado.IdCliente == idCliente
                         && i.Estado == InstanciaTurno.EstadoActiva
                         && i.TurnoReservado.Estado == TurnoReservado.EstadoActivo)
                .OrderBy(i => i.Fecha)
                    .ThenBy(i => i.HorarioCancha.HoraInicio)
                .ToList();
        }

        public InstanciaTurno? ObtenerInstanciaPorId(int idInstancia)
        {
            using var ctx = new AppDbContext();
            return ctx.InstanciasTurno
                .Include(i => i.HorarioCancha)
                    .ThenInclude(h => h.Cancha)
                .Include(i => i.TurnoReservado)
                    .ThenInclude(t => t.Cliente)
                        .ThenInclude(c => c.Persona)
                .FirstOrDefault(i => i.IdInstancia == idInstancia);
        }

        public void CancelarInstancia(int idInstancia)
        {
            using var ctx = new AppDbContext();
            var instancia = ctx.InstanciasTurno
                .Include(i => i.TurnoReservado)
                .FirstOrDefault(i => i.IdInstancia == idInstancia);

            if (instancia == null) return;

            instancia.Estado = InstanciaTurno.EstadoCancelada;

            // "Por dia" tiene una sola instancia. Si se cancela, no se deja la cabecera del turno como "Activo"
            if (instancia.TurnoReservado.Modalidad == TurnoReservado.ModalidadPorDia)
                instancia.TurnoReservado.Estado = TurnoReservado.EstadoCancelado;

            ctx.SaveChanges();
        }

        public void CancelarTurnoDesdeInstancia(int idInstancia, DateTime fechaDesde)
        {
            using var ctx = new AppDbContext();
            var instancia = ctx.InstanciasTurno
                .Include(i => i.TurnoReservado)
                    .ThenInclude(t => t.Instancias)
                .FirstOrDefault(i => i.IdInstancia == idInstancia);

            if (instancia == null) return;

            if (instancia.TurnoReservado.Modalidad == TurnoReservado.ModalidadPorDia)
            {
                instancia.Estado = InstanciaTurno.EstadoCancelada;
                instancia.TurnoReservado.Estado = TurnoReservado.EstadoCancelado;
            }
            else
            {
                instancia.TurnoReservado.Estado = TurnoReservado.EstadoCancelado;
                foreach (var futura in instancia.TurnoReservado.Instancias.Where(i =>
                    i.Fecha >= fechaDesde.Date &&
                    i.Estado == InstanciaTurno.EstadoActiva))
                {
                    futura.Estado = InstanciaTurno.EstadoCancelada;
                }
            }

            ctx.SaveChanges();
        }
    }
}