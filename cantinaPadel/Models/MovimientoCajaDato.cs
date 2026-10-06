namespace cantinaPadel.Models;

public sealed record MovimientoCajaDato(string MedioPago, decimal Monto, bool EsPadel = false, bool EsCantina = false, bool EsFiadoPendiente = false, bool EsPagoCuentaCorriente = false);
