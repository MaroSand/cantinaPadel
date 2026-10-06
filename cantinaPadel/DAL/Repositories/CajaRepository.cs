using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class CajaRepository : ICajaRepository
{
    public TurnoCaja? ObtenerCajaAbierta(int idEmpleado)
    {
        using var ctx = new AppDbContext();
        return CompletarNombres(ctx, ctx.TurnosCaja.Where(c => c.IdEmpleado == idEmpleado && c.Estado == TurnoCaja.EstadoAbierta)
            .OrderByDescending(c => c.FechaApertura).FirstOrDefault());
    }

    // Completa NombreEmpleado (nombre y apellido) a partir de IdEmpleado, con una sola consulta para toda la lista
    private static List<TurnoCaja> CompletarNombres(AppDbContext ctx, List<TurnoCaja> turnos)
    {
        var ids = turnos.Select(t => t.IdEmpleado).Distinct().ToList();
        var nombres = ctx.Empleados.Include(e => e.Persona)
            .Where(e => ids.Contains(e.IdEmpleado))
            .ToDictionary(e => e.IdEmpleado, e => $"{e.Persona.Nombre} {e.Persona.Apellido}");
        foreach (var turno in turnos)
            turno.NombreEmpleado = nombres.GetValueOrDefault(turno.IdEmpleado, string.Empty);
        return turnos;
    }

    private static TurnoCaja? CompletarNombres(AppDbContext ctx, TurnoCaja? turno)
    {
        if (turno != null) CompletarNombres(ctx, new List<TurnoCaja> { turno });
        return turno;
    }

    public TurnoCaja? ObtenerCajaAbiertaGeneral()
    {
        using var ctx = new AppDbContext();
        return CompletarNombres(ctx, ctx.TurnosCaja.Where(c => c.Estado == TurnoCaja.EstadoAbierta).OrderByDescending(c => c.FechaApertura).FirstOrDefault());
    }

    public TurnoCaja? ObtenerUltimaCajaCerrada()
    {
        using var ctx = new AppDbContext();
        return ctx.TurnosCaja.Where(c => c.Estado == TurnoCaja.EstadoCerrada && c.FechaCierre != null)
            .OrderByDescending(c => c.FechaCierre).FirstOrDefault();
    }

    public List<TurnoCaja> ObtenerHistorial(int idEmpleado)
    {
        using var ctx = new AppDbContext();
        return CompletarNombres(ctx, ctx.TurnosCaja.Where(c => c.IdEmpleado == idEmpleado).OrderByDescending(c => c.FechaApertura).ToList());
    }

    public List<TurnoCaja> ObtenerTodoHistorial()
    {
        using var ctx = new AppDbContext();
        return CompletarNombres(ctx, ctx.TurnosCaja.OrderByDescending(c => c.FechaApertura).ToList());
    }

    public List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int? idEmpleado)
    {
        using var ctx = new AppDbContext();
        var ingresos = (from i in ctx.IngresosCaja
                        join c in ctx.TurnosCaja on i.IdTurnoCaja equals c.IdTurnoCaja
                        join e in ctx.Empleados.Include(e => e.Persona) on i.IdAdmin equals e.IdEmpleado
                        where idEmpleado == null || c.IdEmpleado == idEmpleado
                        select new MovimientoEfectivoHistorial
                        {
                            IdTurnoCaja = i.IdTurnoCaja,
                            Fecha = i.FechaIngreso,
                            Tipo = "Ingreso",
                            Monto = i.Monto,
                            Usuario = e.Persona.Nombre + " " + e.Persona.Apellido
                        }).ToList();

        var retiros = (from r in ctx.RetirosCaja
                       join c in ctx.TurnosCaja on r.IdTurnoCaja equals c.IdTurnoCaja
                       join e in ctx.Empleados.Include(e => e.Persona) on r.IdEmpleado equals e.IdEmpleado
                       where idEmpleado == null || c.IdEmpleado == idEmpleado
                       select new MovimientoEfectivoHistorial
                       {
                           IdTurnoCaja = r.IdTurnoCaja,
                           Fecha = r.FechaRetiro,
                           Tipo = "Retiro",
                           Monto = r.Monto,
                           Usuario = e.Persona.Nombre + " " + e.Persona.Apellido
                       }).ToList();

        return ingresos.Concat(retiros).OrderByDescending(m => m.Fecha).ToList();
    }

    public TurnoCaja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado)
    {
        var abierta = ObtenerCajaAbierta(idEmpleado);
        return abierta ?? Abrir(idEmpleado, 0m);
    }

    public TurnoCaja Abrir(int idEmpleado, decimal montoInicial)
    {
        using var ctx = new AppDbContext();
        if (ctx.TurnosCaja.Any(c => c.Estado == TurnoCaja.EstadoAbierta))
            throw new InvalidOperationException("La caja física ya está abierta por otro usuario. Cerrala antes de iniciar el siguiente turno.");
        var caja = ctx.Cajas.FirstOrDefault(c => c.IdEmpleado == idEmpleado);
        if (caja == null)
        {
            caja = new Caja { IdEmpleado = idEmpleado };
            ctx.Cajas.Add(caja);
            ctx.SaveChanges();
        }
        var turno = new TurnoCaja { IdCaja = caja.IdCaja, IdEmpleado = idEmpleado, FechaApertura = DateTime.Now, MontoApertura = montoInicial, Estado = TurnoCaja.EstadoAbierta };
        ctx.TurnosCaja.Add(turno);
        ctx.SaveChanges();
        return turno;
    }

    public TurnoCaja Obtener(int idTurnoCaja)
    {
        using var ctx = new AppDbContext();
        return ctx.TurnosCaja.FirstOrDefault(c => c.IdTurnoCaja == idTurnoCaja) ?? throw new InvalidOperationException("No se encontró el turno de caja.");
    }

    public List<MovimientoCajaDato> ObtenerMovimientosResumen(int idTurnoCaja)
    {
        using var ctx = new AppDbContext();
        var result = new List<MovimientoCajaDato>();
        var ventas = ctx.Ventas
            .Where(v => v.IdTurnoCaja == idTurnoCaja && v.Estado == "Activa" && v.IdVentaPadre == null).ToList();
        foreach (var venta in ventas)
        {
            if (venta.FormaPago == "Cuenta Corriente")
            {
                continue;
            }
            var metodo = venta.FormaPago is "Transferencia" or "MercadoPago" ? "Transferencia" : venta.FormaPago;
            result.Add(new MovimientoCajaDato(metodo, venta.Total));
        }
        result.AddRange(ctx.MovimientosCuentaCorriente
            .Where(m => m.IdTurnoCaja == idTurnoCaja && m.Tipo == MovimientoCuentaCorriente.TipoPago)
            .Select(m => new MovimientoCajaDato(m.TipoDePago == "MercadoPago" ? "Transferencia" : m.TipoDePago ?? "", m.Monto, true)));
        return result;
    }

    public EfectivoCajaDatos ObtenerDatosEfectivo(int idTurnoCaja)
    {
        using var ctx = new AppDbContext();
        var turno = ctx.TurnosCaja.FirstOrDefault(c => c.IdTurnoCaja == idTurnoCaja) ?? throw new InvalidOperationException("No se encontró el turno de caja.");
        var ventas = ctx.Ventas.Where(v => v.IdTurnoCaja == idTurnoCaja && v.Estado == "Activa" && v.IdVentaPadre == null && v.FormaPago == "Efectivo").Sum(v => (decimal?)v.Total) ?? 0m;
        var pagos = ctx.MovimientosCuentaCorriente.Where(m => m.IdTurnoCaja == idTurnoCaja && m.Tipo == MovimientoCuentaCorriente.TipoPago && m.TipoDePago == "Efectivo").Sum(m => (decimal?)m.Monto) ?? 0m;
        var ingresos = ctx.IngresosCaja.Where(i => i.IdTurnoCaja == idTurnoCaja).Sum(i => (decimal?)i.Monto) ?? 0m;
        var retiros = ctx.RetirosCaja.Where(x => x.IdTurnoCaja == idTurnoCaja).Sum(x => (decimal?)x.Monto) ?? 0m;
        return new EfectivoCajaDatos(turno.MontoApertura + ingresos, ventas, pagos, retiros);
    }

    public void Cerrar(int idTurnoCaja, CajaResumenDatos resumen, decimal efectivoEsperado, decimal efectivoContado, decimal diferencia, string? motivoDiferencia)
    {
        using var ctx = new AppDbContext();
        var caja = ctx.TurnosCaja.FirstOrDefault(c => c.IdTurnoCaja == idTurnoCaja) ?? throw new InvalidOperationException("No se encontró el turno de caja.");
        if (caja.Estado != TurnoCaja.EstadoAbierta) throw new InvalidOperationException("La caja ya está cerrada.");
        caja.FechaCierre = DateTime.Now;
        caja.CierreEfectivo = resumen.Efectivo;
        caja.CierreTarjeta = resumen.Tarjeta;
        caja.CierreTransferencia = resumen.Transferencia;
        caja.EfectivoEsperado = efectivoEsperado;
        caja.EfectivoFinal = efectivoContado;
        caja.DiferenciaEfectivo = diferencia;
        caja.MotivoDiferencia = motivoDiferencia;
        caja.Estado = TurnoCaja.EstadoCerrada;
        ctx.SaveChanges();
    }

    public void RegistrarRetiro(RetiroCaja retiro)
    {
        using var ctx = new AppDbContext();
        ctx.RetirosCaja.Add(retiro);
        ctx.SaveChanges();
    }

    public void RegistrarIngreso(IngresoCaja ingreso)
    {
        using var ctx = new AppDbContext();
        ctx.IngresosCaja.Add(ingreso);
        ctx.SaveChanges();
    }
}