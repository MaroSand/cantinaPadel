using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class CajaRepository : ICajaRepository
{
    public Caja? ObtenerCajaAbierta(int idEmpleado)
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.Where(c => c.IdEmpleado == idEmpleado && c.Estado == Caja.EstadoAbierta)
            .OrderByDescending(c => c.FechaApertura).FirstOrDefault();
    }

    public Caja? ObtenerCajaAbiertaGeneral()
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.Where(c => c.Estado == Caja.EstadoAbierta).OrderByDescending(c => c.FechaApertura).FirstOrDefault();
    }

    public Caja? ObtenerUltimaCajaCerrada()
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.Where(c => c.Estado == Caja.EstadoCerrada && c.FechaCierre != null)
            .OrderByDescending(c => c.FechaCierre).FirstOrDefault();
    }

    public List<Caja> ObtenerHistorial(int idEmpleado)
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.Where(c => c.IdEmpleado == idEmpleado).OrderByDescending(c => c.FechaApertura).ToList();
    }

    public List<Caja> ObtenerTodoHistorial()
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.OrderByDescending(c => c.FechaApertura).ToList();
    }

    public List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int? idEmpleado)
    {
        using var ctx = new AppDbContext();
        var ingresos = (from i in ctx.IngresosCaja
                        join c in ctx.Cajas on i.IdCaja equals c.IdCaja
                        join e in ctx.Empleados.Include(e => e.Persona) on i.IdAdmin equals e.IdEmpleado
                        where idEmpleado == null || c.IdEmpleado == idEmpleado
                        select new MovimientoEfectivoHistorial
                        {
                            IdCaja = i.IdCaja,
                            Fecha = i.FechaIngreso,
                            Tipo = "Ingreso",
                            Monto = i.Monto,
                            Usuario = e.Persona.Nombre + " " + e.Persona.Apellido
                        }).ToList();

        var retiros = (from r in ctx.RetirosCaja
                       join c in ctx.Cajas on r.IdCaja equals c.IdCaja
                       join e in ctx.Empleados.Include(e => e.Persona) on r.IdEmpleado equals e.IdEmpleado
                       where idEmpleado == null || c.IdEmpleado == idEmpleado
                       select new MovimientoEfectivoHistorial
                       {
                           IdCaja = r.IdCaja,
                           Fecha = r.FechaRetiro,
                           Tipo = "Retiro",
                           Monto = r.Monto,
                           Usuario = e.Persona.Nombre + " " + e.Persona.Apellido
                       }).ToList();

        return ingresos.Concat(retiros).OrderByDescending(m => m.Fecha).ToList();
    }

    public Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado)
    {
        var abierta = ObtenerCajaAbierta(idEmpleado);
        return abierta ?? Abrir(idEmpleado, 0m);
    }

    public Caja Abrir(int idEmpleado, decimal montoInicial)
    {
        using var ctx = new AppDbContext();
        if (ctx.Cajas.Any(c => c.Estado == Caja.EstadoAbierta))
            throw new InvalidOperationException("La caja física ya está abierta por otro usuario. Cerrala antes de iniciar el siguiente turno.");
        var caja = new Caja { IdEmpleado = idEmpleado, FechaApertura = DateTime.Now, MontoApertura = montoInicial, Estado = Caja.EstadoAbierta };
        ctx.Cajas.Add(caja);
        ctx.SaveChanges();
        return caja;
    }

    public Caja Obtener(int idCaja)
    {
        using var ctx = new AppDbContext();
        return ctx.Cajas.FirstOrDefault(c => c.IdCaja == idCaja) ?? throw new InvalidOperationException("No se encontró la caja.");
    }

    public List<MovimientoCajaDato> ObtenerMovimientosResumen(int idCaja)
    {
        using var ctx = new AppDbContext();
        var result = new List<MovimientoCajaDato>();
        var ventas = ctx.Ventas
            .Where(v => v.IdCaja == idCaja && v.Estado == "Activa" && v.IdVentaPadre == null).ToList();
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
            .Where(m => m.IdCaja == idCaja && m.Tipo == MovimientoCuentaCorriente.TipoPago)
            .Select(m => new MovimientoCajaDato(m.TipoDePago == "MercadoPago" ? "Transferencia" : m.TipoDePago ?? "", m.Monto, true)));
        return result;
    }

    public EfectivoCajaDatos ObtenerDatosEfectivo(int idCaja)
    {
        using var ctx = new AppDbContext();
        var caja = ctx.Cajas.FirstOrDefault(c => c.IdCaja == idCaja) ?? throw new InvalidOperationException("No se encontró la caja.");
        var ventas = ctx.Ventas.Where(v => v.IdCaja == idCaja && v.Estado == "Activa" && v.IdVentaPadre == null && v.FormaPago == "Efectivo").Sum(v => (decimal?)v.Total) ?? 0m;
        var pagos = ctx.MovimientosCuentaCorriente.Where(m => m.IdCaja == idCaja && m.Tipo == MovimientoCuentaCorriente.TipoPago && m.TipoDePago == "Efectivo").Sum(m => (decimal?)m.Monto) ?? 0m;
        var ingresos = ctx.IngresosCaja.Where(i => i.IdCaja == idCaja).Sum(i => (decimal?)i.Monto) ?? 0m;
        var retiros = ctx.RetirosCaja.Where(x => x.IdCaja == idCaja).Sum(x => (decimal?)x.Monto) ?? 0m;
        return new EfectivoCajaDatos(caja.MontoApertura + ingresos, ventas, pagos, retiros);
    }

    public void Cerrar(int idCaja, CajaResumenDatos resumen, decimal efectivoFinal)
    {
        using var ctx = new AppDbContext();
        var caja = ctx.Cajas.FirstOrDefault(c => c.IdCaja == idCaja) ?? throw new InvalidOperationException("No se encontró la caja.");
        if (caja.Estado != Caja.EstadoAbierta) throw new InvalidOperationException("La caja ya está cerrada.");
        caja.FechaCierre = DateTime.Now;
        caja.CierreEfectivo = resumen.Efectivo;
        caja.CierreTarjeta = resumen.Tarjeta;
        caja.CierreTransferencia = resumen.Transferencia;
        caja.EfectivoFinal = efectivoFinal;
        caja.Estado = Caja.EstadoCerrada;
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
