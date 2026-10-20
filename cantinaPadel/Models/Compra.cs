using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace cantinaPadel.Models;

[Table("compras")]
public class Compra
{
    // Límite de la columna DECIMAL(12,2)
    public const decimal TotalMaximo = 9_999_999_999.99m;
    public const int LargoMaximoComprobante = 30;

    [Key]
    [Column("id_compra")]
    public int IdCompra { get; set; }

    [Column("id_proveedor")]
    public int IdProveedor { get; set; }

    // Empleado (usuario activo) que registró la compra
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("fecha_compra")]
    public DateTime FechaCompra { get; set; } = DateTime.Now;

    // Número de comprobante (factura / remito) que entrega el proveedor. Es opcional
    [Column("numero_comprobante")]
    public string? NumeroComprobante { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    // Baja lógica: una compra nunca se borra ni se modifica, si hubo un error se anula y se carga de nuevo (ver US-27)
    [Column("anulada")]
    public bool Anulada { get; set; } = false;

    [ForeignKey("IdProveedor")]
    public Proveedor? Proveedor { get; set; }

    [ForeignKey("IdEmpleado")]
    public Empleado? Empleado { get; set; }

    public List<DetalleCompra> Detalles { get; set; } = new();

    // Deja el número de comprobante prolijo: sin espacios en los bordes, y null si quedó vacío (porque es opcional)
    public void Normalizar()
    {
        NumeroComprobante = string.IsNullOrWhiteSpace(NumeroComprobante) ? null : NumeroComprobante.Trim();
    }

    public void Validar()
    {
        Normalizar();

        if (IdProveedor <= 0)
            throw new ArgumentException("Seleccione un proveedor.");

        if (IdEmpleado <= 0)
            throw new ArgumentException("No hay un empleado autenticado para registrar la compra.");

        if (FechaCompra.Date > DateTime.Today)
            throw new ArgumentException("La fecha de la compra no puede ser posterior a hoy.");

        if (NumeroComprobante != null)
        {
            if (NumeroComprobante.Length > LargoMaximoComprobante)
                throw new ArgumentException($"El número de comprobante no puede superar los {LargoMaximoComprobante} caracteres.");

            if (!Regex.IsMatch(NumeroComprobante, @"^[A-Za-z0-9][A-Za-z0-9\-/\. ]*$"))
                throw new ArgumentException("El número de comprobante solo puede tener letras, números, guiones, barras y puntos. Por ejemplo: 0001-00001234");
        }

        if (Detalles == null || Detalles.Count == 0)
            throw new ArgumentException("La compra debe tener al menos un producto.");

        foreach (var detalle in Detalles)
            detalle.Validar();

        var repetido = Detalles.GroupBy(d => d.IdProducto).FirstOrDefault(g => g.Count() > 1);
        if (repetido != null)
            throw new ArgumentException("No puede haber productos repetidos en la misma compra: se debe sumar la cantidad en una sola línea.");

        decimal totalCalculado = decimal.Round(Detalles.Sum(d => d.Subtotal), 2);
        if (Total != totalCalculado)
            throw new ArgumentException("El total no coincide con la suma de los subtotales.");

        if (Total > TotalMaximo)
            throw new ArgumentException("El total de la compra es demasiado grande. Revise las cantidades y los costos.");
    }
}