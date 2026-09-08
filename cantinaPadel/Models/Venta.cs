using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("ventas")]
public class Venta
{
    [Key]
    [Column("id_venta")]
    public int IdVenta { get; set; }

    [Column("id_caja")]
    public int IdCaja { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("fecha_venta")]
    public DateTime FechaVenta { get; set; } = DateTime.Now;

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [Column("iva")]
    public decimal Iva { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    // Ticket se registra como B hasta que se implemente la tabla de comprobantes.
    [Column("tipo_comprobante")]
    public string TipoComprobante { get; set; } = "B";

    [Column("forma_pago")]
    public string FormaPago { get; set; } = string.Empty;

    [Column("estado")]
    public string Estado { get; set; } = "Activa";

    public List<DetalleVenta> Detalles { get; set; } = new();
}
