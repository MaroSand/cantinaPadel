using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    // PLACEHOLDER: mapea sólo las columnas que necesita el módulo de
    // Alquiler por Día para saber si hay una caja abierta. El módulo de
    // Caja (apertura/cierre/retiros) todavía no existe en el código C#.
    // Cuando se implemente, fusionar esta clase con la definitiva en vez
    // de tener dos mapeos de la misma tabla.
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

        [Column("estado")]
        public string Estado { get; set; } = EstadoAbierta;
    }
}