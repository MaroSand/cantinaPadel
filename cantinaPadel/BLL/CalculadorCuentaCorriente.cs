namespace cantinaPadel.BLL;

// Resultado de aplicar un crédito a una lista de montos pendientes.
public readonly record struct AplicacionDeCredito(int CantidadSaldada, decimal CreditoRemanente);

// Reglas de negocio de la cuenta corriente, sin acceso a datos.
//
// Conceptos:
//  - Deuda bruta: suma del precio completo de cada unidad impaga.
//  - Crédito: plata que el cliente ya entregó pero que no alcanzó para
//    saldar por completo la próxima unidad (se guarda en
//    clientes.saldo_cuenta_corriente).
//  - Deuda neta: lo que efectivamente falta cobrar (deuda bruta - crédito).
//  - Saldo a favor: solo existe si el crédito supera la deuda bruta, es
//    decir, cuando el cliente pagó de más.
public static class CalculadorCuentaCorriente
{
    // Aplica el crédito a las unidades pendientes en orden FIFO (la más
    // vieja primero). Una unidad solo se salda si el crédito la cubre por
    // completo; en cuanto una no alcanza se corta, para que el sobrante
    // siempre se acumule sobre la unidad más antigua que sigue impaga.
    public static AplicacionDeCredito AplicarCredito(IReadOnlyList<decimal> montosPendientes, decimal credito)
    {
        ArgumentNullException.ThrowIfNull(montosPendientes);
        ArgumentOutOfRangeException.ThrowIfNegative(credito);

        int saldadas = 0;
        foreach (decimal monto in montosPendientes)
        {
            if (credito < monto)
                break;

            credito -= monto;
            saldadas++;
        }

        return new AplicacionDeCredito(saldadas, credito);
    }

    public static decimal CalcularDeudaNeta(decimal deudaBruta, decimal credito)
        => Math.Max(deudaBruta - credito, 0m);

    public static decimal CalcularSaldoAFavor(decimal deudaBruta, decimal credito)
        => Math.Max(credito - deudaBruta, 0m);
}
