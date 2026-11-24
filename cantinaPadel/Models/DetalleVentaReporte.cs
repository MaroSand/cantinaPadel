namespace cantinaPadel.Models;

// Línea individual de una venta para mostrar el detalle del reporte.
public sealed class DetalleVentaReporte
{
    public string Articulo { get; init; } = string.Empty;
    public decimal PrecioUnitario { get; init; }
    public decimal Subtotal { get; init; }
    public bool Pagado { get; init; }
    public string EstadoCobro => Pagado ? "Pagado" : "Pendiente";
}
