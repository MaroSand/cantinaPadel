using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("detalles_compra")]
public class DetalleCompra
{
    public const decimal CostoMaximo = 9_999_999_999.99m;

    // Tope razonable por línea: evita errores de tipeo (ej: 1000000 en vez de 100) y desbordes del INT / DECIMAL(12,2)
    public const int CantidadMaxima = 1_000_000;

    [Key]
    [Column("id_detalle_compra")]
    public int IdDetalleCompra { get; set; }

    [Column("id_compra")]
    public int IdCompra { get; set; }

    [Column("id_producto")]
    public int IdProducto { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; } = 1;

    // Precio de costo unitario (lo que se le pagó al proveedor por unidad)
    [Column("precio_unitario")]
    public decimal PrecioUnitario { get; set; }

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    // Estado anterior del producto, necesario para revertir la primera compra activa si se anula.
    // Es nullable para permitir la migración de compras históricas anteriores a US-27.
    [Column("costo_anterior")]
    public decimal? CostoAnterior { get; set; }

    [Column("id_proveedor_anterior")]
    public int? IdProveedorAnterior { get; set; }

    // Solo para mostrar (detalle de la compra). Al registrar una compra se completa únicamente IdProducto, para que EF no intente insertar el producto
    [ForeignKey("IdProducto")]
    public Producto? Producto { get; set; }

    // Reglas de cantidad y costo, compartidas con el carrito de la pantalla de alta para que ambos validen exactamente lo mismo
    public static void ValidarCantidad(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.");

        if (cantidad > CantidadMaxima)
            throw new ArgumentException($"La cantidad no puede superar {CantidadMaxima:N0} unidades por producto.");
    }

    public static void ValidarCosto(decimal costoUnitario)
    {
        if (costoUnitario <= 0)
            throw new ArgumentException("El costo unitario debe ser mayor a cero.");

        if (costoUnitario > CostoMaximo)
            throw new ArgumentException("El costo unitario supera el máximo permitido por la base de datos.");

        if (decimal.Round(costoUnitario, 2) != costoUnitario)
            throw new ArgumentException("El costo unitario admite como máximo 2 decimales.");
    }

    public static decimal CalcularSubtotal(int cantidad, decimal costoUnitario) => decimal.Round(cantidad * costoUnitario, 2);

    public void Validar()
    {
        if (IdProducto <= 0)
            throw new ArgumentException("Todas las líneas de la compra deben tener un producto.");

        ValidarCantidad(Cantidad);
        ValidarCosto(PrecioUnitario);

        if (Subtotal != CalcularSubtotal(Cantidad, PrecioUnitario))
            throw new ArgumentException("El subtotal no coincide con la cantidad y el costo unitario.");
    }
}
