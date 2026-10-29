namespace cantinaPadel.Models;

// Una fila del reporte de ventas: es la parte de UNA venta que corresponde a UN tipo (Pádel o Cantina).
// Una venta que mezcla turnos y productos genera dos filas (con el mismo IdVenta), así cada tipo se puede
// filtrar y subtotalizar por separado sin repartir importes a ojo
public sealed class FilaReporteVentas
{
    public int IdVenta { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Empleado { get; set; } = string.Empty;
    public string FormaPago { get; set; } = string.Empty;

    // false solo en las ventas de Cuenta Corriente que todavía tienen deuda sin cobrar
    public bool Pagado { get; set; }

    // Cada fila de detalles_venta es una unidad (o una hora de pádel)
    public int Unidades { get; set; }

    // Importe CON IVA de las unidades de este tipo en la venta
    public decimal Total { get; set; }

    // Importe SIN IVA (el IVA no es ganancia: se le entrega al fisco)
    public decimal Neto { get; set; }

    // Costo de la mercadería vendida en esta fila. Las horas de pádel no tienen costo de mercadería
    public decimal Costo { get; set; }

    // Neto - Costo
    public decimal Ganancia { get; set; }

    // Para mostrar en la grilla
    public string EstadoCobro => Pagado ? "Cobrada" : "Pendiente";
}
