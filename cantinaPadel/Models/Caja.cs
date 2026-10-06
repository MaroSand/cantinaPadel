using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("cajas")]
    public class Caja
    {
        public const string EstadoAbierta = "Abierta";
        public const string EstadoCerrada = "Cerrada";

        [Key]
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

        [Column("estado")]
        public string Estado { get; set; } = EstadoAbierta;
    }
}
