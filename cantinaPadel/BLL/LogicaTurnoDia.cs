using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using System.Configuration;

namespace cantinaPadel.BLL
{
    public class HorarioTurnoDiaDisponible
    {
        public int IdCancha { get; set; }
        public string Cancha { get; set; } = string.Empty;
        public string DiaSemana { get; set; } = string.Empty;
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public bool Disponible { get; set; }

        public string Horario => $"{HoraInicio:hh\\:mm} - {HoraFin:hh\\:mm}";
        public string Estado => Disponible ? "Libre" : "Ocupado";
    }

    public class ClienteBusquedaTurno
    {
        public int IdCliente { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    public class LogicaTurnoDia
    {
        private static readonly TimeSpan HoraApertura = new(8, 0, 0);
        private static readonly TimeSpan HoraCierre = new(23, 0, 0);
        private static readonly TimeSpan DuracionTurno = TimeSpan.FromHours(1);

        public const string ModalidadDia = "Por dia";
        public const string ModalidadMensual = "Mensual";
        public const string ModalidadAnual = "Anual";

        private readonly ITurnoDiaRepository _turnoRepo;
        private readonly ICanchaRepository _canchaRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly ICajaRepository _cajaRepo;

        public LogicaTurnoDia()
            : this(new TurnoDiaRepository(), new CanchaRepository(), new ClienteRepository(), new CajaRepository())
        {
        }

        public LogicaTurnoDia(
            ITurnoDiaRepository turnoRepo,
            ICanchaRepository canchaRepo,
            IClienteRepository clienteRepo,
            ICajaRepository cajaRepo)
        {
            _turnoRepo = turnoRepo;
            _canchaRepo = canchaRepo;
            _clienteRepo = clienteRepo;
            _cajaRepo = cajaRepo;
        }

        public List<Cancha> ObtenerCanchasActivas() => _canchaRepo.ObtenerActivas();

        public List<Cliente> ObtenerClientesActivos()
        {
            return _clienteRepo.ObtenerTodos()
                .Where(c => c.Persona.Activo)
                .OrderBy(c => c.Persona.Apellido)
                .ThenBy(c => c.Persona.Nombre)
                .ToList();
        }

        public List<ClienteBusquedaTurno> BuscarClientes(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length < 2)
                return new List<ClienteBusquedaTurno>();

            return _clienteRepo.Buscar(texto)
                .Select(c => new ClienteBusquedaTurno
                {
                    IdCliente = c.IdCliente,
                    NombreCompleto = $"{c.Persona.Apellido}, {c.Persona.Nombre}",
                    Dni = c.Persona.Dni ?? string.Empty,
                    Email = c.Email,
                    Telefono = c.Persona.Telefono ?? string.Empty
                })
                .ToList();
        }

        public List<HorarioTurnoDiaDisponible> ObtenerHorarios(int idCancha, DateTime fecha)
        {
            ValidarCanchaYFecha(idCancha, fecha);

            var cancha = _canchaRepo.ObtenerPorId(idCancha);
            if (cancha == null || !cancha.Activa)
                throw new ArgumentException("La cancha seleccionada no existe o está inactiva.");

            string diaSemana = ObtenerDiaSemana(fecha);
            var reservas = _turnoRepo.ObtenerInstanciasPorFecha(fecha.Date, idCancha);

            return GenerarFranjasDelDia()
                .Select(franja => new HorarioTurnoDiaDisponible
                {
                    IdCancha = cancha.IdCancha,
                    Cancha = cancha.Nombre,
                    DiaSemana = diaSemana,
                    HoraInicio = franja.HoraInicio,
                    HoraFin = franja.HoraFin,
                    Disponible = !reservas.Any(r =>
                        r.HorarioCancha.HoraInicio < franja.HoraFin &&
                        franja.HoraInicio < r.HorarioCancha.HoraFin)
                })
                .ToList();
        }

        public List<InstanciaTurno> ObtenerReservas(DateTime fecha, int? idCancha = null)
        {
            return _turnoRepo.ObtenerInstanciasPorFecha(fecha.Date, idCancha);
        }

        // Turnos del cliente, sin importar fecha ni cancha. Por default solo
        // los activos; con incluirCancelados=true trae también los cancelados.
        public List<InstanciaTurno> ObtenerReservasPorCliente(int idCliente, bool incluirCancelados = false)
        {
            if (idCliente <= 0)
                return new List<InstanciaTurno>();

            return _turnoRepo.ObtenerInstanciasPorCliente(idCliente, incluirCancelados);
        }

        public int CalcularCantidadTurnos(string modalidad, DateTime fechaInicio)
        {
            return GenerarFechasReserva(modalidad, fechaInicio).Count;
        }

        public DateTime CalcularFechaFin(string modalidad, DateTime fechaInicio)
        {
            return modalidad switch
            {
                ModalidadDia => fechaInicio.Date,
                ModalidadMensual => fechaInicio.Date.AddMonths(1),
                ModalidadAnual => fechaInicio.Date.AddYears(1),
                _ => throw new ArgumentException("La modalidad seleccionada no es válida.")
            };
        }

        public void RegistrarAlquiler(
            int idCliente,
            int idEmpleado,
            int idCancha,
            DateTime fechaInicio,
            TimeSpan horaInicio,
            TimeSpan horaFin,
            string modalidad)
        {
            if (idCliente <= 0)
                throw new ArgumentException("Debe seleccionar un cliente.");

            if (idEmpleado <= 0)
                throw new ArgumentException("No hay un empleado logueado para registrar el turno.");

            if (idCancha <= 0)
                throw new ArgumentException("Debe seleccionar una cancha.");

            ValidarFecha(fechaInicio);
            ValidarFranja(horaInicio, horaFin);
            ValidarModalidad(modalidad);

            var cancha = _canchaRepo.ObtenerPorId(idCancha);
            if (cancha == null || !cancha.Activa)
                throw new ArgumentException("La cancha seleccionada no existe o está inactiva.");

            var fechas = GenerarFechasReserva(modalidad, fechaInicio);
            var fechasOcupadas = fechas
                .Where(fecha => !_turnoRepo.EstaDisponible(idCancha, fecha, horaInicio, horaFin))
                .ToList();

            if (fechasOcupadas.Any())
            {
                string primerasFechas = string.Join(", ", fechasOcupadas.Take(5).Select(f => f.ToString("dd/MM/yyyy")));
                throw new InvalidOperationException($"No se puede registrar: hay solapamientos en {primerasFechas}.");
            }

            var caja = ObtenerCajaParaReserva(idEmpleado);

            var turno = new TurnoReservado
            {
                IdCliente = idCliente,
                IdEmpleado = idEmpleado,
                IdCaja = caja.IdCaja,
                Modalidad = modalidad == ModalidadDia ? TurnoReservado.ModalidadPorDia : TurnoReservado.ModalidadFijo,
                FechaInicio = fechaInicio.Date,
                FechaFin = modalidad == ModalidadDia ? null : CalcularFechaFin(modalidad, fechaInicio),
                Estado = TurnoReservado.EstadoActivo
            };

            var horarios = new List<HorarioCancha>();
            var instancias = new List<InstanciaTurno>();

            foreach (var fecha in fechas)
            {
                string diaSemana = ObtenerDiaSemana(fecha);
                var horario = _turnoRepo.ObtenerHorarioPorFranja(idCancha, diaSemana, horaInicio, horaFin)
                    ?? new HorarioCancha
                    {
                        IdCancha = idCancha,
                        DiaSemana = diaSemana,
                        HoraInicio = horaInicio,
                        HoraFin = horaFin,
                        Activo = true
                    };

                horarios.Add(horario);
                instancias.Add(new InstanciaTurno
                {
                    Fecha = fecha.Date,
                    Estado = InstanciaTurno.EstadoActiva,
                    EstadoPago = InstanciaTurno.EstadoPagoPendiente,
                    FormaPago = null
                });
            }

            _turnoRepo.RegistrarAlquiler(turno, horarios, instancias);
        }

        private Caja ObtenerCajaParaReserva(int idEmpleado)
        {
            var caja = _cajaRepo.ObtenerCajaAbierta(idEmpleado);
            if (caja != null)
                return caja;

            if (UsarBypassCajaTurnos())
                return _cajaRepo.ObtenerOCrearCajaTecnicaParaPruebas(idEmpleado);

            throw new InvalidOperationException("No hay una caja abierta para el empleado actual. Abra una caja antes de registrar alquileres.");
        }

        private static bool UsarBypassCajaTurnos()
        {
            return bool.TryParse(ConfigurationManager.AppSettings["BypassCajaTurnos"], out bool bypass)
                && bypass;
        }

        public void CancelarTurno(int idInstancia)
        {
            if (idInstancia <= 0)
                throw new ArgumentException("Debe seleccionar un turno para cancelar.");

            var instancia = _turnoRepo.ObtenerInstanciaPorId(idInstancia);
            if (instancia == null)
                throw new ArgumentException("El turno seleccionado no existe.");

            if (instancia.Estado == InstanciaTurno.EstadoCancelada)
                throw new ArgumentException("El turno seleccionado ya está cancelado.");

            // No se puede cancelar retroactivamente un turno cuya fecha ya pasó.
            if (instancia.Fecha.Date < DateTime.Today)
                throw new ArgumentException("No se puede cancelar un turno de una fecha que ya pasó.");

            // El corte es la fecha de la instancia elegida, no "hoy": en un
            // turno Fijo (Mensual/Anual), cancelar un día en el medio cancela
            // ese día y todos los siguientes, dejando intactos los anteriores.
            _turnoRepo.CancelarTurnoDesdeInstancia(idInstancia, instancia.Fecha);
        }

        private static void ValidarModalidad(string modalidad)
        {
            if (modalidad != ModalidadDia && modalidad != ModalidadMensual && modalidad != ModalidadAnual)
                throw new ArgumentException("La modalidad seleccionada no es válida.");
        }

        private static void ValidarCanchaYFecha(int idCancha, DateTime fecha)
        {
            if (idCancha <= 0)
                throw new ArgumentException("Debe seleccionar una cancha.");

            ValidarFecha(fecha);
        }

        private static void ValidarFecha(DateTime fecha)
        {
            if (fecha.Date < DateTime.Today)
                throw new ArgumentException("No se pueden registrar turnos en fechas pasadas.");
        }

        private static void ValidarFranja(TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (horaFin <= horaInicio)
                throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio.");

            if (horaInicio < HoraApertura || horaFin > HoraCierre)
                throw new ArgumentException("La banda horaria está fuera del horario de atención.");

            if (horaFin - horaInicio != DuracionTurno)
                throw new ArgumentException("Los turnos por día deben ser de una hora.");
        }

        private static List<(TimeSpan HoraInicio, TimeSpan HoraFin)> GenerarFranjasDelDia()
        {
            var franjas = new List<(TimeSpan HoraInicio, TimeSpan HoraFin)>();

            for (var inicio = HoraApertura; inicio + DuracionTurno <= HoraCierre; inicio += DuracionTurno)
            {
                franjas.Add((inicio, inicio + DuracionTurno));
            }

            return franjas;
        }

        private static List<DateTime> GenerarFechasReserva(string modalidad, DateTime fechaInicio)
        {
            ValidarModalidad(modalidad);

            if (modalidad == ModalidadDia)
                return new List<DateTime> { fechaInicio.Date };

            DateTime fechaFin = modalidad == ModalidadAnual
                ? fechaInicio.Date.AddYears(1)
                : fechaInicio.Date.AddMonths(1);

            var fechas = new List<DateTime>();
            for (DateTime fecha = fechaInicio.Date; fecha <= fechaFin; fecha = fecha.AddDays(7))
            {
                fechas.Add(fecha);
            }

            return fechas;
        }

        private static string ObtenerDiaSemana(DateTime fecha)
        {
            return fecha.DayOfWeek switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miercoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sabado",
                DayOfWeek.Sunday => "Domingo",
                _ => string.Empty
            };
        }
    }
}