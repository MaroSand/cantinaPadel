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

// Estado actual de la cuenta corriente de un cliente.
public class ResumenCuentaCorriente
{
    public List<ItemDeudaCliente> Pendientes { get; init; } = new();

    // Plata entregada que todavía no saldó por completo la próxima unidad.
    public decimal Credito { get; init; }

    public decimal DeudaBruta => Pendientes.Sum(p => p.Monto);
    public decimal DeudaNeta => CalculadorCuentaCorriente.CalcularDeudaNeta(DeudaBruta, Credito);
    public decimal SaldoAFavor => CalculadorCuentaCorriente.CalcularSaldoAFavor(DeudaBruta, Credito);

    // Parte del crédito que ya está descontada de lo que se debe.
    public decimal PagosParcialesAcreditados => Math.Min(Credito, DeudaBruta);
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

    // Lo entregado que no alcanzó a saldar por completo la próxima unidad.
    // Queda acreditado y se descuenta de esa unidad; NO es saldo a favor.
    public decimal CreditoResultante { get; set; }

    public decimal SaldoAFavor => CalculadorCuentaCorriente.CalcularSaldoAFavor(DeudaPendiente, CreditoResultante);
}
