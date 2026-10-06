namespace cantinaPadel.Models;

public sealed record MovimientoCajaDato(string MedioPago, decimal Monto, bool EsPagoCuentaCorriente = false);
