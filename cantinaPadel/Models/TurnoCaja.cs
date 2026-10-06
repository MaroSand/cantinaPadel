using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("turnos_caja")]
public class TurnoCaja
{
    public const string EstadoAbierta = "Abierta";
    public const string EstadoCerrada = "Cerrada";

    [Key, Column("id_turno_caja")]
    public int IdTurnoCaja { get; set; }

    [Column("id_caja")]
    public int IdCaja { get; set; }

    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("fecha_apertura")]
    public DateTime FechaApertura { get; set; }

    [Column("monto_apertura")]
    public decimal MontoApertura { get; set; }

    [Column("fecha_cierre")]
    public DateTime? FechaCierre { get; set; }

    [Column("cierre_efectivo")]
    public decimal? CierreEfectivo { get; set; }

    [Column("cierre_tarjeta")]
    public decimal? CierreTarjeta { get; set; }

    [Column("cierre_transferencia")]
    public decimal? CierreTransferencia { get; set; }

    [Column("efectivo_final")]
    public decimal? EfectivoFinal { get; set; }

    [Column("efectivo_esperado")]
    public decimal? EfectivoEsperado { get; set; }

    [Column("diferencia_efectivo")]
    public decimal? DiferenciaEfectivo { get; set; }

    [Column("motivo_diferencia")]
    public string? MotivoDiferencia { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = EstadoAbierta;

    // Nombre y apellido del empleado que abrió la caja. No es una columna: lo completa el repositorio para mostrarlo en pantalla
    [NotMapped]
    public string NombreEmpleado { get; set; } = string.Empty;
}