using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

// auditoría de la cuenta corriente. Se inserta una fila por cada
// venta registrada "a cuenta corriente" (tipo Cargo, informativo: no mueve
// saldo_cuenta_corriente) y por cada pago recibido (tipo Pago, sí mueve
// saldo_cuenta_corriente). No reemplaza a detalles_venta.pagado, que sigue
// siendo la fuente de verdad de qué productos están pendientes.
[Table("movimientos_cuenta_corriente")]
public class MovimientoCuentaCorriente
{
    public const string TipoCargo = "Cargo";
    public const string TipoPago = "Pago";
    public const string TipoPagoEfectivo = "Efectivo";
    public const string TipoPagoTransferencia = "Transferencia";
    public const string TipoPagoTarjeta = "Tarjeta";
    public const string TipoPagoMercadoPago = "MercadoPago";

    [Key]
    [Column("id_movimiento")]
    public int IdMovimiento { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("id_caja")]
    public int IdTurnoCaja { get; set; }

    [Column("id_venta")]
    public int? IdVenta { get; set; }

    [Column("id_instancia_turno")]
    public int? IdInstanciaTurno { get; set; }

    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column("tipo")]
    public string Tipo { get; set; } = TipoCargo;

    [Column("tipodepago")]
    public string? TipoDePago { get; set; }

    [Column("monto")]
    public decimal Monto { get; set; }

    [Column("saldo_posterior")]
    public decimal SaldoPosterior { get; set; }
}
