using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    // La ocurrencia concreta de un turno en una fecha puntual, contra una
    // banda horaria (HorarioCancha) específica. En "Por dia" hay una sola
    // por TurnoReservado; en "Fijo" hay varias (una por semana).
    [Table("instancias_turno")]
    public class InstanciaTurno
    {
        public const string EstadoActiva = "Activa";
        public const string EstadoCancelada = "Cancelada";

        public const string EstadoPagoPendiente = "Pendiente";
        public const string EstadoPagoPagado = "Pagado";
        public const string EstadoPagoCuentaCorriente = "Cuenta Corriente";

        [Key]
        [Column("id_instancia")]
        public int IdInstancia { get; set; }

        [Column("id_turno")]
        public int IdTurno { get; set; }

        [Column("id_horario")]
        public int IdHorario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        // 'Activa' = el slot está ocupado por este turno.
        // 'Cancelada' = se liberó (vuelve a figurar "Libre" en la grilla).
        [Column("estado")]
        public string Estado { get; set; } = EstadoActiva;

        // Al crear el alquiler queda Pendiente/null: el cobro real
        // (marcar Pagado + forma_pago) lo hace el módulo de Caja/Ventas.
        [Column("estado_pago")]
        public string EstadoPago { get; set; } = EstadoPagoPendiente;

        [Column("forma_pago")]
        public string? FormaPago { get; set; }

        [ForeignKey("IdTurno")]
        public TurnoReservado TurnoReservado { get; set; } = null!;

        [ForeignKey("IdHorario")]
        public HorarioCancha HorarioCancha { get; set; } = null!;
    }
}