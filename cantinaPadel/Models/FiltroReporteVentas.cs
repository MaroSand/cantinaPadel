namespace cantinaPadel.Models;

// Qué parte de las ventas se quiere ver en el reporte
public enum TipoVentaReporte
{
    Todas,
    Padel,
    Cantina
}

// Filtros de FrmReporteVentas (US-25). Desde y Hasta son fechas inclusivas: solo importa el día, no la hora
public sealed class FiltroReporteVentas
{
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    public TipoVentaReporte Tipo { get; set; } = TipoVentaReporte.Todas;

    // null = todos los métodos de pago
    public string? FormaPago { get; set; }

    // null = todos los empleados
    public int? IdEmpleado { get; set; }
}
