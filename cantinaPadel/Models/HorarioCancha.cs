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

        // Si HoraFin <= HoraInicio, el horario "cruza la medianoche": arranca un día y termina de madrugada del día siguiente, pero sigue
        // perteneciendo al DiaSemana elegido (ej: lunes de 8 a 2 sigue siendo el turno del lunes)
        public bool CruzaMedianoche => HoraFin <= HoraInicio;

        // HoraFin "estirada" +24hs cuando cruza medianoche, solo para comparar rangos. No se persiste así en la bd pq queda el valor de reloj real (02:00)
        public TimeSpan HoraFinNormalizada => CruzaMedianoche ? HoraFin.Add(TimeSpan.FromHours(24)) : HoraFin;

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

            // si HoraFin es menor o igual, se interpreta como que el horario cruza la medianoche
            // <Si son sean exactamente iguales (duración cero) no es válido
            if (HoraFin == HoraInicio)
                throw new ArgumentException("La hora de fin no puede ser igual a la hora de inicio.");
        }

        // Dos horarios solapan si, siendo de la misma cancha y mismo día, sus intervalos [HoraInicio, HoraFinNormalizada) se cruzan en algún punto
        // Se usa la versión normalizada de HoraFin para que los horarios que cruzan medianoche se comparen bien.
        // Ej: 8-10 y 9-11 solapan; 8-10 y 10-12 no
        // Ej: 22-2 y 1-3 sí
        public bool Solapa(HorarioCancha otro)
        {
            if (otro == null) return false;
            if (IdCancha != otro.IdCancha) return false;
            if (!string.Equals(DiaSemana, otro.DiaSemana, StringComparison.OrdinalIgnoreCase)) return false;

            return HoraInicio < otro.HoraFinNormalizada && otro.HoraInicio < HoraFinNormalizada;
        }
    }
}