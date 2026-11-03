using cantinaPadel.BLL;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public sealed class ReporteCuentaCorrienteRepository : IReporteCuentaCorrienteRepository
{
    public List<ClienteCuentaCorrienteReporte> ObtenerClientesConActividad()
    {
        using var ctx = new AppDbContext();

        var clientes = ctx.Clientes.AsNoTracking()
            .Where(c => ctx.MovimientosCuentaCorriente.Any(m => m.IdCliente == c.IdCliente))
            .Select(c => new
            {
                c.IdCliente,
                NombreCompleto = c.Persona.Nombre + " " + c.Persona.Apellido,
                Dni = c.Persona.Dni,
                Credito = c.SaldoCuentaCorriente
            })
            .ToList();

        if (clientes.Count == 0) return new List<ClienteCuentaCorrienteReporte>();

        var deudaBrutaPorCliente = (from detalle in ctx.DetallesVenta.AsNoTracking()
                                    join venta in ctx.Ventas.AsNoTracking() on detalle.IdVenta equals venta.IdVenta
                                    join producto in ctx.Productos.AsNoTracking() on detalle.IdProducto equals (int?)producto.IdProducto
                                    where !detalle.Pagado
                                    group detalle by venta.IdCliente into pendientes
                                    select new
                                    {
                                        IdCliente = pendientes.Key,
                                        MontoPendiente = pendientes.Sum(d => d.Subtotal)
                                    })
            .ToDictionary(x => x.IdCliente, x => x.MontoPendiente);

        return clientes
            .Select(c => new ClienteCuentaCorrienteReporte
            {
                IdCliente = c.IdCliente,
                NombreCompleto = c.NombreCompleto.Trim(),
                Dni = c.Dni ?? string.Empty,
                MontoAdeudado = CalculadorCuentaCorriente.CalcularDeudaNeta(
                    deudaBrutaPorCliente.GetValueOrDefault(c.IdCliente), Math.Max(c.Credito, 0m))
            })
            .OrderBy(c => c.NombreCompleto)
            .ToList();
    }

    public List<MovimientoCuentaCorrienteReporte> ObtenerMovimientos(int idCliente)
    {
        using var ctx = new AppDbContext();

        var movimientos = (from movimiento in ctx.MovimientosCuentaCorriente.AsNoTracking()
                           join empleado in ctx.Empleados.AsNoTracking() on movimiento.IdEmpleado equals empleado.IdEmpleado
                           join persona in ctx.Personas.AsNoTracking() on empleado.IdPersona equals persona.IdPersona
                           where movimiento.IdCliente == idCliente
                           orderby movimiento.Fecha descending, movimiento.IdMovimiento descending
                           select new
                           {
                               movimiento.Fecha,
                               movimiento.Tipo,
                               movimiento.TipoDePago,
                               movimiento.IdVenta,
                               movimiento.IdInstanciaTurno,
                               movimiento.Monto,
                               movimiento.SaldoPosterior,
                               persona.Nombre,
                               persona.Apellido
                           })
            .ToList();

        return movimientos.Select(m => new MovimientoCuentaCorrienteReporte
        {
            Fecha = m.Fecha,
            Tipo = m.Tipo,
            Referencia = m.IdInstanciaTurno.HasValue
                ? $"Turno #{m.IdInstanciaTurno.Value}"
                : m.IdVenta.HasValue ? $"Venta #{m.IdVenta.Value}" : "Sin referencia",
            MedioPago = m.TipoDePago == "MercadoPago" ? "Mercado Pago" : m.TipoDePago ?? "—",
            Monto = m.Monto,
            CreditoPosterior = m.SaldoPosterior,
            Responsable = $"{m.Nombre} {m.Apellido}".Trim()
        }).ToList();
    }
}
