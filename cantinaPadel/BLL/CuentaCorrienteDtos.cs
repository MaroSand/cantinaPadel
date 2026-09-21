namespace cantinaPadel.BLL;

// Una unidad de producto pendiente de pago para un cliente (una fila de
// detalles_venta con Pagado = false). El Monto ya refleja el precio actual
// del producto, porque ProductoRepository lo actualiza cuando cambia el
// precio de venta mientras la unidad sigue pendiente.
public class ItemDeudaCliente
{
    public int IdDetalle { get; set; }
    public int IdVenta { get; set; }
    public DateTime FechaVenta { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}

// Resultado de aplicar un pago (parcial o total) a la cuenta corriente de
// un cliente.
public class ResultadoPagoCuentaCorriente
{
    public List<ItemDeudaCliente> ItemsPagados { get; set; } = new();
    public List<ItemDeudaCliente> ItemsPendientes { get; set; } = new();
    public decimal MontoRecibido { get; set; }
    public decimal MontoAplicado => ItemsPagados.Sum(i => i.Monto);
    public decimal DeudaPendiente => ItemsPendientes.Sum(i => i.Monto);

    // Lo que sobra del pago (más el saldo a favor que ya tuviera el
    // cliente) una vez cubiertos todos los productos completos que
    // alcanzó a pagar. Se guarda como saldo a favor para el próximo pago.
    public decimal SaldoFavorResultante { get; set; }
}
