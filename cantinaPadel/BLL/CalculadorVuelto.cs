using System.Globalization;

namespace cantinaPadel.BLL;

// Cálculo del vuelto cuando se cobra en efectivo. Lo usan el Punto de Venta y la pantalla de Cuenta Corriente.
// "Paga con" es opcional: si el cajero no lo informa (0), no se calcula ni se valida nada
public static class CalculadorVuelto
{
    // Interpreta el texto que escribió el cajero. Vacío = no informado (devuelve true con monto 0).
    // Devuelve false si lo escrito no es un número válido o es negativo
    public static bool TryParsearMonto(string? texto, out decimal monto)
    {
        monto = 0m;
        var limpio = (texto ?? string.Empty).Replace("$", string.Empty).Trim();
        if (limpio.Length == 0)
            return true;

        return decimal.TryParse(limpio, NumberStyles.Number, CultureInfo.CurrentCulture, out monto) && monto >= 0m;
    }

    // Devuelve un mensaje de error si el cliente entrega menos de lo que hay que cobrar; null si está bien o no se informó el monto
    public static string? Validar(decimal totalACobrar, decimal pagaCon)
    {
        if (pagaCon > 0m && pagaCon < totalACobrar)
            return $"El cliente paga con {pagaCon:C2}, que es menos que el monto a cobrar ({totalACobrar:C2}).";
        return null;
    }

    // Vuelto a entregar (0 si no se informó con cuánto paga o si pagó justo)
    public static decimal Calcular(decimal totalACobrar, decimal pagaCon)
        => pagaCon > totalACobrar ? Math.Round(pagaCon - totalACobrar, 2) : 0m;
}