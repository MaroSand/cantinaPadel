using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IHorarioCanchaRepository
    {
        List<HorarioCancha> ObtenerTodos(bool? activo = true);
        List<HorarioCancha> ObtenerPorCancha(int idCancha, bool? activo = true);
        HorarioCancha? ObtenerPorId(int idHorario);

        // Chequea si ya existe, para esa cancha y ese día, un horario activo cuyo rango se cruce con [horaInicio, horaFin)
        bool ExisteSolapamiento(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idHorarioExcluir = null);

        void Agregar(HorarioCancha horario);
        void Modificar(HorarioCancha horario);
        void CambiarEstado(int idHorario, bool nuevoEstado);

        // Desactiva de una sola vez todos los horarios activos de una cancha (se usa en cascada cuando se da de baja la cancha, para no dejar
        // horarios "vigentes" de una cancha inactiva)
        void DesactivarTodosPorCancha(int idCancha);
    }
}