using cantinaPadel.Models;

namespace cantinaPadel.BLL;

/// <summary>Reglas de cálculo para el impacto de una compra en productos.</summary>
public static class CalculadorCompra
{
    // productos.precio_venta y compras/detalles_compra usan DECIMAL(12,2).
    public const decimal MontoMaximo = DetalleCompra.CostoMaximo;

    // El script base usa "Sin Categoria" como categoría genérica; se trata igual que "Varios" para preservar precios manuales.
    public static bool EsPrecioVentaManual(string? nombreCategoria)
        => string.Equals(nombreCategoria?.Trim(), "Varios", StringComparison.OrdinalIgnoreCase) ||
           string.Equals(nombreCategoria?.Trim(), "Sin Categoria", StringComparison.OrdinalIgnoreCase);

    public static decimal CalcularPrecioVentaBase(decimal costo, decimal porcentajeGanancia)
    {
        if (costo <= 0m)
            throw new ArgumentException("El costo debe ser mayor a cero.", nameof(costo));
        if (costo > MontoMaximo)
            throw new ArgumentException("El costo supera el máximo permitido por la base de datos.", nameof(costo));
        if (porcentajeGanancia < 0m)
            throw new ArgumentException("El porcentaje de ganancia no puede ser negativo.", nameof(porcentajeGanancia));

        var precio = decimal.Round(costo * (1m + porcentajeGanancia / 100m), 2);
        if (precio > Producto.PrecioVentaMaximo)
            throw new ArgumentException("El nuevo precio de venta supera el máximo permitido por la base de datos.");

        return precio;
    }

    public static int CalcularStockNuevo(int stockActual, int cantidadIngresada)
    {
        if (stockActual < 0)
            throw new ArgumentException("El stock actual no puede ser negativo.", nameof(stockActual));
        if (cantidadIngresada <= 0)
            throw new ArgumentException("La cantidad ingresada debe ser mayor a cero.", nameof(cantidadIngresada));

        try
        {
            return checked(stockActual + cantidadIngresada);
        }
        catch (OverflowException)
        {
            throw new ArgumentException("El stock resultante supera el máximo permitido.");
        }
    }

    public static int CalcularStockLuegoDeAnular(int stockActual, int cantidadComprada)
    {
        if (stockActual < 0)
            throw new InvalidOperationException("El stock actual no puede ser negativo.");
        if (cantidadComprada <= 0)
            throw new InvalidOperationException("La cantidad de la compra no es válida.");
        if (stockActual < cantidadComprada)
            throw new InvalidOperationException(
                "No se puede anular la compra porque parte del stock ya se vendió o se descontó. El stock quedaría negativo.");

        return stockActual - cantidadComprada;
    }
}
