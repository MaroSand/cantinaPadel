using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaHorarioCancha
    {
        private readonly IHorarioCanchaRepository _repo;

        public LogicaHorarioCancha()
            : this(new HorarioCanchaRepository())
        {
        }

        public LogicaHorarioCancha(IHorarioCanchaRepository repo)
        {
            _repo = repo;
        }

        public List<HorarioCancha> ObtenerTodos(bool? activo = true) => _repo.ObtenerTodos(activo);

        public List<HorarioCancha> ObtenerPorCancha(int idCancha, bool? activo = true) => _repo.ObtenerPorCancha(idCancha, activo);

        public HorarioCancha? ObtenerPorId(int idHorario) => _repo.ObtenerPorId(idHorario);

        public void Agregar(HorarioCancha horario)
        {
            Validar(horario, esAlta: true);

            _repo.Agregar(horario);
        }

        public void Modificar(HorarioCancha horario)
        {
            Validar(horario, esAlta: false);

            _repo.Modificar(horario);
        }

        public void CambiarEstado(int idHorario, bool nuevoEstado) => _repo.CambiarEstado(idHorario, nuevoEstado);

        private void Validar(HorarioCancha horario, bool esAlta)
        {
            if (horario == null)
                throw new ArgumentException("Los datos del horario son obligatorios.");

            // Valida cancha/día obligatorios y que fin > inicio (ver HorarioCancha.Validar)
            horario.Validar();

            // No permitir dos horarios activos de la misma cancha, mismo día, que se pisen en el tiempo
            int? idExcluir = esAlta ? null : horario.IdHorario;
            if (_repo.ExisteSolapamiento(horario.IdCancha, horario.DiaSemana, horario.HoraInicio, horario.HoraFin, idExcluir))
                throw new ArgumentException(
                    $"El horario se superpone con otro horario activo de esta cancha el día {horario.DiaSemana}.");
        }
    }
}