using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public static class CalculadorCaja
{
    public static CajaResumenDatos CalcularResumen(IEnumerable<MovimientoCajaDato> movimientos)
    {
        ArgumentNullException.ThrowIfNull(movimientos);
        var r = new CajaResumenDatos();
        foreach (var m in movimientos)
        {
            if (m.Monto < 0m) throw new ArgumentOutOfRangeException(nameof(movimientos), "Los montos no pueden ser negativos.");
            var medio = m.MedioPago == "MercadoPago" ? "Transferencia" : m.MedioPago;
            switch (medio)
            {
                case "Efectivo":
                    r.Efectivo += m.Monto;
                    if (m.EsPagoCuentaCorriente) r.CobrosCuentaCorrienteEfectivo += m.Monto;
                    break;
                case "Tarjeta":
                    r.Tarjeta += m.Monto;
                    if (m.EsPagoCuentaCorriente) r.CobrosCuentaCorrienteTarjeta += m.Monto;
                    break;
                case "Transferencia":
                    r.Transferencia += m.Monto;
                    if (m.EsPagoCuentaCorriente) r.CobrosCuentaCorrienteTransferencia += m.Monto;
                    break;
            }
        }
        r.Efectivo = Math.Round(r.Efectivo, 2); r.Tarjeta = Math.Round(r.Tarjeta, 2); r.Transferencia = Math.Round(r.Transferencia, 2);
        r.CobrosCuentaCorrienteEfectivo = Math.Round(r.CobrosCuentaCorrienteEfectivo, 2);
        r.CobrosCuentaCorrienteTarjeta = Math.Round(r.CobrosCuentaCorrienteTarjeta, 2);
        r.CobrosCuentaCorrienteTransferencia = Math.Round(r.CobrosCuentaCorrienteTransferencia, 2);
        return r;
    }

    public static decimal CalcularEfectivoDisponible(decimal apertura, decimal ventasEfectivo, decimal pagosCuentaCorriente, decimal retiros)
    {
        if (apertura < 0 || ventasEfectivo < 0 || pagosCuentaCorriente < 0 || retiros < 0)
            throw new ArgumentOutOfRangeException(nameof(apertura), "Los montos no pueden ser negativos.");
        return Math.Round(apertura + ventasEfectivo + pagosCuentaCorriente - retiros, 2);
    }
}
