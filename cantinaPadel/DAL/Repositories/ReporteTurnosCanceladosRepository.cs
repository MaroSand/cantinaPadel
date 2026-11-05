using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public sealed class ReporteTurnosCanceladosRepository
{
    public List<InstanciaTurno> ObtenerCancelaciones()
    {
        using var ctx = new AppDbContext();
        return ctx.InstanciasTurno.AsNoTracking()
            .Where(i => i.Estado == InstanciaTurno.EstadoCancelada && i.FechaHoraCancelacion != null)
            .Include(i => i.TurnoReservado).ThenInclude(t => t.Cliente).ThenInclude(c => c.Persona)
            .Include(i => i.HorarioCancha).ThenInclude(h => h!.Cancha)
            .Include(i => i.EmpleadoCancelo).ThenInclude(e => e!.Persona)
            .OrderByDescending(i => i.FechaHoraCancelacion)
            .ToList();
    }
}
