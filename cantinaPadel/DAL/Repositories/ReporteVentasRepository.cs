using cantinaPadel.BLL;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class ReporteVentasRepository : IReporteVentasRepository
{
    public DatosReporteVentas ObtenerDatos(DateTime desde, DateTime hasta, string? formaPago, int? idEmpleado)
    {
        using var ctx = new AppDbContext();

        // Rango inclusivo por día: de las 00:00 de "desde" hasta antes de las 00:00 del día siguiente a "hasta"
        DateTime inicio = desde.Date;
        DateTime finExclusivo = hasta.Date.AddDays(1);

        IQueryable<Venta> consulta = ctx.Ventas
            .AsNoTracking()
            .Include(v => v.Detalles)
            .Where(v => v.FechaVenta >= inicio && v.FechaVenta < finExclusivo);

        if (!string.IsNullOrWhiteSpace(formaPago))
            consulta = consulta.Where(v => v.FormaPago == formaPago);

        if (idEmpleado.HasValue)
            consulta = consulta.Where(v => v.IdEmpleado == idEmpleado.Value);

        var ventas = consulta.ToList();

        var idsPadel = ctx.Productos
            .Where(p => p.Categoria.Nombre == CalculadorReporteVentas.CategoriaHoraPadel)
            .Select(p => p.IdProducto)
            .ToList();

        // Costo actual de los productos vendidos (los detalles de turnos no tienen producto)
        var idsVendidos = ventas.SelectMany(v => v.Detalles)
            .Where(d => d.IdProducto.HasValue)
            .Select(d => d.IdProducto!.Value)
            .Distinct()
            .ToList();
        var costos = ctx.Productos
            .Where(p => idsVendidos.Contains(p.IdProducto))
            .Select(p => new { p.IdProducto, p.PrecioCosto })
            .ToList()
            .ToDictionary(p => p.IdProducto, p => p.PrecioCosto);

        var idsClientes = ventas.Select(v => v.IdCliente).Distinct().ToList();
        var clientes = ctx.Clientes
            .Where(c => idsClientes.Contains(c.IdCliente))
            .Select(c => new { c.IdCliente, c.Persona.Nombre, c.Persona.Apellido })
            .ToList()
            .ToDictionary(c => c.IdCliente, c => $"{c.Nombre} {c.Apellido}".Trim());

        var idsEmpleados = ventas.Select(v => v.IdEmpleado).Distinct().ToList();
        var empleados = ctx.Empleados
            .Where(e => idsEmpleados.Contains(e.IdEmpleado))
            .Select(e => new { e.IdEmpleado, e.Persona.Nombre, e.Persona.Apellido })
            .ToList()
            .ToDictionary(e => e.IdEmpleado, e => $"{e.Nombre} {e.Apellido}".Trim());

        return new DatosReporteVentas
        {
            Ventas = ventas,
            IdsProductosPadel = new HashSet<int>(idsPadel),
            CostosProductos = costos,
            Clientes = clientes,
            Empleados = empleados
        };
    }
}
