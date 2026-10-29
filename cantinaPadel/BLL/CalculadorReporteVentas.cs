using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Reglas del reporte de ventas (US-25). No toca la base: recibe los datos ya leídos y arma las filas del reporte
public static class CalculadorReporteVentas
{
    public const string TipoPadel = "Pádel";
    public const string TipoCantina = "Cantina";

    // Nombre de la categoría del producto "Hora Padel" que se crea para cada cancha
    public const string CategoriaHoraPadel = "Hora Padel";

    // Mismo IVA que usa LogicaVenta: los precios de venta se guardan CON IVA
    public const decimal TasaIva = 0.21m;

    // Un detalle es de pádel si está atado a un turno reservado o si es el producto "Hora Padel" de una cancha.
    // Todo lo demás es cantina
    public static bool EsDetallePadel(DetalleVenta detalle, IReadOnlySet<int> idsProductosPadel)
    {
        ArgumentNullException.ThrowIfNull(detalle);
        ArgumentNullException.ThrowIfNull(idsProductosPadel);

        return detalle.IdTurno.HasValue
            || (detalle.IdProducto.HasValue && idsProductosPadel.Contains(detalle.IdProducto.Value));
    }

    // Arma las filas del reporte: una por cada (venta, tipo) con su cantidad de unidades y su total.
    // - Solo cuentan las ventas Activas: las anuladas no son ventas.
    // - Las ventas "hijas" (IdVentaPadre != null) no cuentan: son los cobros de deuda de Cuenta Corriente, y la venta original
    //   ya se contó cuando se hizo. Contarlas también duplicaría el importe (mismo criterio que usa el cierre de caja)
    public static List<FilaReporteVentas> ArmarFilas(
        IEnumerable<Venta> ventas,
        IReadOnlySet<int> idsProductosPadel,
        IReadOnlyDictionary<int, decimal> costosProductos,
        IReadOnlyDictionary<int, string> nombresClientes,
        IReadOnlyDictionary<int, string> nombresEmpleados,
        TipoVentaReporte tipo)
    {
        ArgumentNullException.ThrowIfNull(ventas);
        ArgumentNullException.ThrowIfNull(idsProductosPadel);
        ArgumentNullException.ThrowIfNull(costosProductos);
        ArgumentNullException.ThrowIfNull(nombresClientes);
        ArgumentNullException.ThrowIfNull(nombresEmpleados);

        var filas = new List<FilaReporteVentas>();

        foreach (var venta in ventas)
        {
            if (venta.Estado != "Activa" || venta.IdVentaPadre != null)
                continue;

            var porTipo = venta.Detalles
                .GroupBy(d => EsDetallePadel(d, idsProductosPadel) ? TipoPadel : TipoCantina);

            foreach (var grupo in porTipo)
            {
                decimal total = Math.Round(grupo.Sum(d => d.Subtotal), 2);
                decimal neto = CalcularNeto(total);
                decimal costo = grupo.Key == TipoPadel ? 0m : CalcularCosto(grupo, costosProductos);

                filas.Add(new FilaReporteVentas
                {
                    IdVenta = venta.IdVenta,
                    Fecha = venta.FechaVenta,
                    Tipo = grupo.Key,
                    Cliente = nombresClientes.GetValueOrDefault(venta.IdCliente, "—"),
                    Empleado = nombresEmpleados.GetValueOrDefault(venta.IdEmpleado, "—"),
                    FormaPago = venta.FormaPago,
                    Pagado = venta.Pagado,
                    Unidades = grupo.Count(),
                    Total = total,
                    Neto = neto,
                    Costo = costo,
                    Ganancia = neto - costo
                });
            }
        }

        return filas
            .Where(f => CoincideConTipo(f.Tipo, tipo))
            .OrderByDescending(f => f.Fecha)
            .ThenByDescending(f => f.IdVenta)
            .ThenBy(f => f.Tipo, StringComparer.Ordinal)
            .ToList();
    }

    // Saca el IVA a un importe que lo incluye (misma cuenta que LogicaVenta al registrar la venta)
    public static decimal CalcularNeto(decimal totalConIva) => Math.Round(totalConIva / (1 + TasaIva), 2);

    // Costo de las unidades vendidas: cada fila de detalle es una unidad, y se valúa al costo actual del producto.
    // Un producto sin costo cargado cuenta como 0
    private static decimal CalcularCosto(IEnumerable<DetalleVenta> detalles, IReadOnlyDictionary<int, decimal> costos)
        => Math.Round(detalles
            .Where(d => d.IdProducto.HasValue)
            .Sum(d => costos.GetValueOrDefault(d.IdProducto!.Value, 0m)), 2);

    // Subtotales de pádel y de cantina más el total general, sobre las filas que se están mostrando
    public static ResumenReporteVentas CalcularResumen(IEnumerable<FilaReporteVentas> filas)
    {
        ArgumentNullException.ThrowIfNull(filas);
        var lista = filas.ToList();

        return new ResumenReporteVentas
        {
            Padel = Sumar("Pádel", lista.Where(f => f.Tipo == TipoPadel)),
            Cantina = Sumar("Cantina", lista.Where(f => f.Tipo == TipoCantina)),
            General = Sumar("Total", lista)
        };
    }

    private static SubtotalReporteVentas Sumar(string nombre, IEnumerable<FilaReporteVentas> filas)
    {
        var l = filas.ToList();
        return new SubtotalReporteVentas
        {
            Nombre = nombre,
            Unidades = l.Sum(f => f.Unidades),
            Total = Math.Round(l.Sum(f => f.Total), 2),
            Neto = Math.Round(l.Sum(f => f.Neto), 2),
            Costo = Math.Round(l.Sum(f => f.Costo), 2),
            Ganancia = Math.Round(l.Sum(f => f.Ganancia), 2)
        };
    }

    public static decimal CalcularTotal(IEnumerable<FilaReporteVentas> filas)
    {
        ArgumentNullException.ThrowIfNull(filas);
        return Math.Round(filas.Sum(f => f.Total), 2);
    }

    private static bool CoincideConTipo(string tipoFila, TipoVentaReporte filtro) => filtro switch
    {
        TipoVentaReporte.Padel => tipoFila == TipoPadel,
        TipoVentaReporte.Cantina => tipoFila == TipoCantina,
        _ => true
    };
}
