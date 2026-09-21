using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("detalles_venta")]
public class DetalleVenta
{
    [Key]
    [Column("id_detalle")]
    public int IdDetalle { get; set; }

    [Column("id_venta")]
    public int IdVenta { get; set; }

    [Column("id_producto")]
    public int? IdProducto { get; set; }

    [Column("id_turno")]
    public int? IdTurno { get; set; }

    // ya no existe "cantidad". Cada fila representa una única unidad
    // vendida (si se venden 5 cocas, se insertan 5 filas), para poder llevar
    // el estado de pago producto por producto en cuenta corriente.
    [Column("precio_unitario")]
    public decimal PrecioUnitario { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [Column("pagado")]
    public bool Pagado { get; set; } = true;
}
