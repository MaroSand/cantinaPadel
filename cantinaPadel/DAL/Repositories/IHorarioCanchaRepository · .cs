using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IHorarioCanchaRepository
    {
        List<HorarioCancha> ObtenerTodos(bool? activo = true);
        List<HorarioCancha> ObtenerPorCancha(int idCancha, bool? activo = true);
        HorarioCancha? ObtenerPorId(int idHorario);

        // Chequea si ya existe, para esa cancha y ese día, un horario ACTIVO
        // cuyo rango se cruce con [horaInicio, horaFin).
        bool ExisteSolapamiento(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idHorarioExcluir = null);

        void Agregar(HorarioCancha horario);
        void Modificar(HorarioCancha horario);
        void CambiarEstado(int idHorario, bool nuevoEstado);
    }
}