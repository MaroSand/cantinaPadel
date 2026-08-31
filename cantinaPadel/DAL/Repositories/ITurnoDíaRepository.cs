using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface ITurnoDiaRepository
    {
        // En este módulo los horarios disponibles se calculan en memoria. La bd guarda solo franjas que alguna vez fueron reservadas
        HorarioCancha? ObtenerHorarioPorFranja(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin);

        HorarioCancha? ObtenerHorarioPorId(int idHorario);

        bool EstaDisponible(int idCancha, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin);

        // Inserta el turno y su instancia en una sola operación
        void RegistrarAlquiler(TurnoReservado turno, HorarioCancha horario, InstanciaTurno instancia);

        void RegistrarAlquiler(TurnoReservado turno, List<HorarioCancha> horarios, List<InstanciaTurno> instancias);

        List<InstanciaTurno> ObtenerInstanciasPorFecha(DateTime fecha, int? idCancha = null);

        // Todos los turnos activos (cualquier fecha/cancha) reservados por un cliente
        List<InstanciaTurno> ObtenerInstanciasPorCliente(int idCliente);

        InstanciaTurno? ObtenerInstanciaPorId(int idInstancia);

        void CancelarInstancia(int idInstancia);

        void CancelarTurnoDesdeInstancia(int idInstancia, DateTime fechaDesde);
    }
}