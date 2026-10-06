namespace cantinaPadel.Models;

// Totales derivados de ventas cobradas y pagos de cuenta corriente de una caja.
public sealed class CajaResumenDatos
{
    public decimal Efectivo { get; set; }
    public decimal Tarjeta { get; set; }
    public decimal Transferencia { get; set; }
    public decimal CobrosCuentaCorrienteEfectivo { get; set; }
    public decimal CobrosCuentaCorrienteTarjeta { get; set; }
    public decimal CobrosCuentaCorrienteTransferencia { get; set; }
}
