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

            // si HoraFin es menor o igual, se interpreta como que el horario cruza la medianoche. Si son sean exactamente iguales (duración cero)
            // no es válido
            if (HoraFin == HoraInicio)
                throw new ArgumentException("La hora de fin no puede ser igual a la hora de inicio.");
        }

        // Dos horarios solapan si, siendo de la misma cancha, sus intervalos se cruzan en algún punto
        // Mismo día: se comparan directamente con la versión normalizada de HoraFin (+24hs si cruza medianoche), para que "Lunes 22-10" contra
        // "Lunes 09-11" se detecte bien
        // Días distintos: solo pueden pisarse si uno de los dos cruza la medianoche y su franja de madrugada cae en el día del otro
        // (ej: "Lunes 22-02" invade la madrugada del "Martes")
        // Ej: 8-10 y 9-11 solapan; 8-10 y 10-12 no; Lunes 22-02 y Martes 01-03 sí
        public bool Solapa(HorarioCancha otro)
        {
            if (otro == null) return false;
            if (IdCancha != otro.IdCancha) return false;

            if (string.Equals(DiaSemana, otro.DiaSemana, StringComparison.OrdinalIgnoreCase))
                return HoraInicio < otro.HoraFinNormalizada && otro.HoraInicio < HoraFinNormalizada;

            if (CruzaMedianoche && EsDiaSiguiente(DiaSemana, otro.DiaSemana))
                return otro.HoraInicio < HoraFin;

            if (otro.CruzaMedianoche && EsDiaSiguiente(otro.DiaSemana, DiaSemana))
                return HoraInicio < otro.HoraFin;

            return false;
        }

        // Día de la semana siguiente a "dia", con el ciclo (Domingo -> Lunes). Si "dia" no es válido, se devuelve tal cual (Validar ya se encarga
        // de rechazar días inválidos antes)
        public static string ObtenerDiaSiguiente(string dia)
        {
            int idx = Array.FindIndex(DiasValidos, d => string.Equals(d, dia, StringComparison.OrdinalIgnoreCase));
            return idx < 0 ? dia : DiasValidos[(idx + 1) % DiasValidos.Length];
        }

        // Día de la semana anterior a "dia", con el ciclo (Lunes -> Domingo)
        public static string ObtenerDiaAnterior(string dia)
        {
            int idx = Array.FindIndex(DiasValidos, d => string.Equals(d, dia, StringComparison.OrdinalIgnoreCase));
            return idx < 0 ? dia : DiasValidos[(idx - 1 + DiasValidos.Length) % DiasValidos.Length];
        }

        private static bool EsDiaSiguiente(string dia, string siguiente)
            => string.Equals(ObtenerDiaSiguiente(dia), siguiente, StringComparison.OrdinalIgnoreCase);
    }
}