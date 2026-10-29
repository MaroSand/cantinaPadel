namespace cantinaPadel.Models;

// Totales de un grupo de filas del reporte (un tipo de venta, o todas juntas)
public sealed class SubtotalReporteVentas
{
    public string Nombre { get; set; } = string.Empty;
    public int Unidades { get; set; }
    public decimal Total { get; set; }
    public decimal Neto { get; set; }
    public decimal Costo { get; set; }
    public decimal Ganancia { get; set; }
}

// Subtotales de ganancias de pádel vs. cantina, más el total general (US-25)
public sealed class ResumenReporteVentas
{
    public SubtotalReporteVentas Padel { get; init; } = new();
    public SubtotalReporteVentas Cantina { get; init; } = new();
    public SubtotalReporteVentas General { get; init; } = new();

    // Para mostrar en la grilla de resumen, en este orden
    public IReadOnlyList<SubtotalReporteVentas> Filas => new[] { Padel, Cantina, General };
}
