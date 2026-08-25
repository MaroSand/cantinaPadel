using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("horarios_cancha")]
    public class HorarioCancha
    {
        [Key]
        [Column("id_horario")]
        public int IdHorario { get; set; }

        [Column("id_cancha")]
        public int IdCancha { get; set; }

        [Column("dia_semana")]
        [Required]
        public string DiaSemana { get; set; } = string.Empty;

        [Column("hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Column("hora_fin")]
        public TimeSpan HoraFin { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [ForeignKey("IdCancha")]
        public Cancha? Cancha { get; set; }

        public static readonly string[] DiasValidos =
        {
            "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"
        };

        public void Normalizar()
        {
            DiaSemana = DiaSemana?.Trim() ?? string.Empty;
        }

        public void Validar()
        {
            Normalizar();

            if (IdCancha <= 0)
                throw new ArgumentException("Debe seleccionar una cancha.");

            if (string.IsNullOrWhiteSpace(DiaSemana))
                throw new ArgumentException("Debe seleccionar un día de la semana.");

            if (!DiasValidos.Contains(DiaSemana, StringComparer.OrdinalIgnoreCase))
                throw new ArgumentException("El día de la semana ingresado no es válido.");

            if (HoraFin <= HoraInicio)
                throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio.");
        }

        // Dos horarios solapan si, siendo de la misma cancha y mismo día, sus intervalos [HoraInicio, HoraFin) se cruzan en algún punto
        // Ej: 08:00-10:00 y 09:00-11:00 solapan; 08:00-10:00 y 10:00-12:00 NO (uno termina justo cuando arranca el otro)
        public bool Solapa(HorarioCancha otro)
        {
            if (otro == null) return false;
            if (IdCancha != otro.IdCancha) return false;
            if (!string.Equals(DiaSemana, otro.DiaSemana, StringComparison.OrdinalIgnoreCase)) return false;

            return HoraInicio < otro.HoraFin && otro.HoraInicio < HoraFin;
        }
    }
}