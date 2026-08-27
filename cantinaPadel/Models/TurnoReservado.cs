using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    // Cabecera de una reserva. Para "Por dia" (esta US) FechaFin queda null
    // y sólo cuelga UNA InstanciaTurno. Para "Fijo" (otra US, a futuro)
    // FechaFin se completa y se generan ~52 instancias, una por semana.
    [Table("turnos_reservados")]
    public class TurnoReservado
    {
        public const string ModalidadPorDia = "Por dia";
        public const string ModalidadFijo = "Fijo";

        public const string EstadoActivo = "Activo";
        public const string EstadoCancelado = "Cancelado";

        [Key]
        [Column("id_turno")]
        public int IdTurno { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_empleado")]
        public int IdEmpleado { get; set; }

        // La DB exige una caja abierta para poder reservar.
        [Column("id_caja")]
        public int IdCaja { get; set; }

        [Column("modalidad")]
        public string Modalidad { get; set; } = ModalidadPorDia;

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = EstadoActivo;

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<InstanciaTurno> Instancias { get; set; } = new List<InstanciaTurno>();
    }
}