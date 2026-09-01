using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    [TestClass]
    public class LogicaTurnoDiaTests
    {
        private const int IdClienteValido = 1;
        private const int IdEmpleadoValido = 1;
        private const int IdCanchaValida = 1;

        private static LogicaTurnoDia CrearLogica(
            TurnoDiaRepositoryFake? turnoRepo = null,
            CanchaRepositoryFake? canchaRepo = null,
            ClienteRepositoryFake? clienteRepo = null,
            CajaRepositoryFake? cajaRepo = null,
            HorarioCanchaRepositoryFake? horarioCanchaRepo = null)
        {
            return new LogicaTurnoDia(
                turnoRepo ?? new TurnoDiaRepositoryFake(),
                canchaRepo ?? new CanchaRepositoryFake(),
                clienteRepo ?? new ClienteRepositoryFake(),
                cajaRepo ?? new CajaRepositoryFake(),
                horarioCanchaRepo ?? CrearHorarioCanchaRepoAbierto());
        }

        // Por defecto, en los tests la cancha está "abierta" los 7 días de 08:00 a 23:00 — replica el
        // comportamiento fijo que tenía el sistema antes de que los horarios pasaran a configurarse
        // por cancha (ver FrmHorarios). Los tests que necesiten un horario particular pueden pasar
        // su propio HorarioCanchaRepositoryFake.
        private static HorarioCanchaRepositoryFake CrearHorarioCanchaRepoAbierto()
        {
            var repo = new HorarioCanchaRepositoryFake();
            foreach (var dia in HorarioCancha.DiasValidos)
            {
                repo.Cargar(new HorarioCancha
                {
                    IdCancha = IdCanchaValida,
                    DiaSemana = dia,
                    HoraInicio = new TimeSpan(8, 0, 0),
                    HoraFin = new TimeSpan(23, 0, 0),
                    Activo = true
                });
            }
            return repo;
        }

        private static Cancha CrearCanchaActiva(int idCancha = IdCanchaValida)
        {
            return new Cancha { IdCancha = idCancha, Nombre = "Cancha 1", Activa = true };
        }

        // Próxima fecha (a partir de mañana) que caiga en el día de la semana pedido.
        // Usarlo cuando el test necesita un día de la semana determinado (ej: para
        // que coincida con un HorarioCancha cargado específicamente para "Lunes").
        private static DateTime ProximoDia(DayOfWeek dia)
        {
            var fecha = DateTime.Today.AddDays(1);
            while (fecha.DayOfWeek != dia)
                fecha = fecha.AddDays(1);
            return fecha;
        }

        // RegistrarAlquiler

        [TestMethod]
        public void RegistrarAlquiler_ValidacionesDeEntrada_LanzaArgumentException()
        {
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var logica = CrearLogica(canchaRepo: canchaRepo);
            var manana = DateTime.Today.AddDays(1);
            var inicio = new TimeSpan(10, 0, 0);
            var fin = new TimeSpan(11, 0, 0);

            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(0, IdEmpleadoValido, IdCanchaValida, manana, inicio, fin, LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, 0, IdCanchaValida, manana, inicio, fin, LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, 0, manana, inicio, fin, LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, DateTime.Today.AddDays(-1), inicio, fin, LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana, inicio, new TimeSpan(9, 0, 0), LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana, new TimeSpan(6, 0, 0), new TimeSpan(7, 0, 0), LogicaTurnoDia.ModalidadDia));
        }

        [TestMethod]
        public void RegistrarAlquiler_FechaYaOcupada_LanzaInvalidOperationException()
        {
            var fecha = DateTime.Today.AddDays(1);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var turnoRepo = new TurnoDiaRepositoryFake();
            turnoRepo.MarcarOcupada(fecha);
            var logica = CrearLogica(turnoRepo, canchaRepo);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, fecha,
                    new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), LogicaTurnoDia.ModalidadDia));
        }

        [TestMethod]
        public void RegistrarAlquiler_DuracionNoMultiploDeLaGranularidad_LanzaArgumentException()
        {
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var logica = CrearLogica(canchaRepo: canchaRepo);
            var manana = DateTime.Today.AddDays(1);

            // 20 minutos no es múltiplo de la granularidad de 30 min
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana,
                    new TimeSpan(10, 0, 0), new TimeSpan(10, 20, 0), LogicaTurnoDia.ModalidadDia));
        }

        [TestMethod]
        public void RegistrarAlquiler_MediaHora_SeRegistraCorrectamente()
        {
            var fecha = DateTime.Today.AddDays(1);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var turnoRepo = new TurnoDiaRepositoryFake();
            var cajaRepo = new CajaRepositoryFake { CajaAbierta = new Caja { IdCaja = 55, IdEmpleado = IdEmpleadoValido } };
            var logica = CrearLogica(turnoRepo, canchaRepo, cajaRepo: cajaRepo);

            logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, fecha,
                new TimeSpan(10, 0, 0), new TimeSpan(10, 30, 0), LogicaTurnoDia.ModalidadDia);

            Assert.HasCount(1, turnoRepo.HorariosRegistrados!);
            Assert.AreEqual(new TimeSpan(10, 0, 0), turnoRepo.HorariosRegistrados![0].HoraInicio);
            Assert.AreEqual(new TimeSpan(10, 30, 0), turnoRepo.HorariosRegistrados[0].HoraFin);
        }

        [TestMethod]
        public void RegistrarAlquiler_ModalidadMensual_RegistraUnaInstanciaPorSemanaDentroDelRango()
        {
            var fecha = DateTime.Today.AddDays(1);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var turnoRepo = new TurnoDiaRepositoryFake();
            var cajaRepo = new CajaRepositoryFake { CajaAbierta = new Caja { IdCaja = 55, IdEmpleado = IdEmpleadoValido } };
            var logica = CrearLogica(turnoRepo, canchaRepo, cajaRepo: cajaRepo);

            logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, fecha,
                new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), LogicaTurnoDia.ModalidadMensual);

            var fechaFinEsperada = logica.CalcularFechaFin(LogicaTurnoDia.ModalidadMensual, fecha);

            Assert.AreEqual(TurnoReservado.ModalidadFijo, turnoRepo.TurnoRegistrado!.Modalidad);
            Assert.AreEqual(fechaFinEsperada, turnoRepo.TurnoRegistrado.FechaFin);
            Assert.IsGreaterThan(1, turnoRepo.InstanciasRegistradas!.Count);
            Assert.IsTrue(turnoRepo.InstanciasRegistradas.All(i => i.Fecha >= fecha.Date && i.Fecha <= fechaFinEsperada));
        }

        // CancelarTurno

        [TestMethod]
        public void CancelarTurno_CasosInvalidos_LanzaArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                CrearLogica().CancelarTurno(0));

            Assert.ThrowsExactly<ArgumentException>(() =>
                CrearLogica().CancelarTurno(999));

            var turnoRepoCancelada = new TurnoDiaRepositoryFake();
            turnoRepoCancelada.CargarInstancia(new InstanciaTurno { IdInstancia = 1, Fecha = DateTime.Today.AddDays(1), Estado = InstanciaTurno.EstadoCancelada });
            Assert.ThrowsExactly<ArgumentException>(() =>
                CrearLogica(turnoRepoCancelada).CancelarTurno(1));

            // No se puede cancelar retroactivamente un turno cuya fecha ya pasó
            var turnoRepoPasado = new TurnoDiaRepositoryFake();
            turnoRepoPasado.CargarInstancia(new InstanciaTurno { IdInstancia = 1, Fecha = DateTime.Today.AddDays(-1), Estado = InstanciaTurno.EstadoActiva });
            Assert.ThrowsExactly<ArgumentException>(() =>
                CrearLogica(turnoRepoPasado).CancelarTurno(1));
        }

        // ObtenerHorarios

        [TestMethod]
        public void ObtenerHorarios_ValidaCanchaYGeneraFranjasConOcupadas()
        {
            Assert.ThrowsExactly<ArgumentException>(() =>
                CrearLogica().ObtenerHorarios(IdCanchaValida, DateTime.Today.AddDays(1)));

            var fecha = DateTime.Today.AddDays(1);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var turnoRepo = new TurnoDiaRepositoryFake();
            turnoRepo.CargarInstancia(new InstanciaTurno
            {
                IdInstancia = 1,
                Fecha = fecha,
                Estado = InstanciaTurno.EstadoActiva,
                HorarioCancha = new HorarioCancha { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(11, 0, 0) }
            });
            var logica = CrearLogica(turnoRepo, canchaRepo);

            // Apertura 08:00, cierre 23:00, granularidad de 30 min -> 30 horas de inicio posibles,
            // pero 22:30 se descarta porque con la duración por defecto (1h) no entra antes del cierre -> 29 franjas.
            // La reserva existente de 10:00-11:00 se solapa con los inicios 09:30, 10:00 y 10:30 (cualquier turno
            // de 1h que arranque ahí pisa esa reserva), así que quedan 3 franjas ocupadas y 26 disponibles.
            var horarios = logica.ObtenerHorarios(IdCanchaValida, fecha);

            Assert.HasCount(29, horarios);
            var franjaOcupada = horarios.Single(h => h.Horario == "10:00 - 11:00");
            Assert.IsFalse(franjaOcupada.Disponible);
            Assert.AreEqual(26, horarios.Count(h => h.Disponible));
        }

        [TestMethod]
        public void ObtenerHorarios_RespetaLasBandasConfiguradasParaLaCanchaEseDia()
        {
            var fecha = ProximoDia(DayOfWeek.Monday);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var horarioRepo = new HorarioCanchaRepositoryFake();
            horarioRepo.Cargar(new HorarioCancha
            {
                IdCancha = IdCanchaValida,
                DiaSemana = "Lunes",
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                Activo = true
            });
            var logica = CrearLogica(canchaRepo: canchaRepo, horarioCanchaRepo: horarioRepo);

            // Banda cargada: 09:00-12:00. Con duración 1h, los inicios posibles son 09:00, 09:30, 10:00, 10:30 y 11:00 (5 en total)
            var horarios = logica.ObtenerHorarios(IdCanchaValida, fecha, TimeSpan.FromHours(1));

            Assert.HasCount(5, horarios);
            Assert.IsTrue(horarios.All(h => h.HoraInicio >= new TimeSpan(9, 0, 0) && h.HoraFin <= new TimeSpan(12, 0, 0)));
        }

        [TestMethod]
        public void RegistrarAlquiler_FueraDelHorarioConfiguradoParaLaCancha_LanzaArgumentException()
        {
            var fecha = ProximoDia(DayOfWeek.Monday);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var horarioRepo = new HorarioCanchaRepositoryFake();
            horarioRepo.Cargar(new HorarioCancha
            {
                IdCancha = IdCanchaValida,
                DiaSemana = "Lunes",
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                Activo = true
            });
            var logica = CrearLogica(canchaRepo: canchaRepo, horarioCanchaRepo: horarioRepo);

            // 20:00 caería dentro del viejo rango fijo (8-23), pero está fuera de la banda real cargada para esta cancha (9-12)
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, fecha,
                    new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0), LogicaTurnoDia.ModalidadDia));
        }

        // CalcularFechaFin / CalcularCantidadTurnos

        [TestMethod]
        public void CalcularFechaFinYCantidadTurnos_PorModalidad()
        {
            var logica = CrearLogica();

            var casos = new (string Modalidad, DateTime Inicio, DateTime FinEsperado, int CantidadEsperada)[]
            {
                (LogicaTurnoDia.ModalidadDia, new DateTime(2026, 3, 10), new DateTime(2026, 3, 10), 1),
                (LogicaTurnoDia.ModalidadMensual, new DateTime(2026, 1, 1), new DateTime(2026, 2, 1), 5),
                (LogicaTurnoDia.ModalidadAnual, new DateTime(2026, 3, 10), new DateTime(2027, 3, 10), 53),
            };

            foreach (var caso in casos)
            {
                Assert.AreEqual(caso.FinEsperado, logica.CalcularFechaFin(caso.Modalidad, caso.Inicio));
                Assert.AreEqual(caso.CantidadEsperada, logica.CalcularCantidadTurnos(caso.Modalidad, caso.Inicio));
            }
        }
    }

    // Fakes en memoria de los repos que consume LogicaTurnoDia

    internal sealed class TurnoDiaRepositoryFake : ITurnoDiaRepository
    {
        private readonly List<HorarioCancha> _horarios = new();
        private readonly List<InstanciaTurno> _instancias = new();
        private readonly HashSet<DateTime> _fechasOcupadas = new();

        public TurnoReservado? TurnoRegistrado { get; private set; }
        public List<HorarioCancha>? HorariosRegistrados { get; private set; }
        public List<InstanciaTurno>? InstanciasRegistradas { get; private set; }
        public (int IdInstancia, DateTime FechaDesde)? CancelacionDesdeInstancia { get; private set; }

        public void CargarInstancia(InstanciaTurno instancia) => _instancias.Add(instancia);

        public void MarcarOcupada(DateTime fecha) => _fechasOcupadas.Add(fecha.Date);

        public HorarioCancha? ObtenerHorarioPorFranja(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin)
            => _horarios.FirstOrDefault(h =>
                h.IdCancha == idCancha && h.DiaSemana == diaSemana && h.HoraInicio == horaInicio && h.HoraFin == horaFin);

        public HorarioCancha? ObtenerHorarioPorId(int idHorario)
            => _horarios.FirstOrDefault(h => h.IdHorario == idHorario);

        public bool EstaDisponible(int idCancha, DateTime fecha, TimeSpan horaInicio, TimeSpan horaFin)
            => !_fechasOcupadas.Contains(fecha.Date);

        public void RegistrarAlquiler(TurnoReservado turno, HorarioCancha horario, InstanciaTurno instancia)
        {
            TurnoRegistrado = turno;
            HorariosRegistrados = new List<HorarioCancha> { horario };
            InstanciasRegistradas = new List<InstanciaTurno> { instancia };
        }

        public void RegistrarAlquiler(TurnoReservado turno, List<HorarioCancha> horarios, List<InstanciaTurno> instancias)
        {
            TurnoRegistrado = turno;
            HorariosRegistrados = horarios;
            InstanciasRegistradas = instancias;
        }

        public List<InstanciaTurno> ObtenerInstanciasPorFecha(DateTime fecha, int? idCancha = null)
            => _instancias.Where(i => i.Fecha.Date == fecha.Date).ToList();

        public List<InstanciaTurno> ObtenerInstanciasPorCliente(int idCliente, bool incluirCancelados = false)
            => _instancias
                .Where(i => i.TurnoReservado != null && i.TurnoReservado.IdCliente == idCliente)
                .Where(i => incluirCancelados || i.Estado == InstanciaTurno.EstadoActiva)
                .ToList();

        public InstanciaTurno? ObtenerInstanciaPorId(int idInstancia)
            => _instancias.FirstOrDefault(i => i.IdInstancia == idInstancia);

        public void CancelarInstancia(int idInstancia)
        {
            var instancia = _instancias.FirstOrDefault(i => i.IdInstancia == idInstancia);
            if (instancia != null)
                instancia.Estado = InstanciaTurno.EstadoCancelada;
        }

        public void CancelarTurnoDesdeInstancia(int idInstancia, DateTime fechaDesde)
            => CancelacionDesdeInstancia = (idInstancia, fechaDesde);
    }

    internal sealed class CanchaRepositoryFake : ICanchaRepository
    {
        private readonly List<Cancha> _canchas = new();

        public void Cargar(params Cancha[] canchas) => _canchas.AddRange(canchas);

        public List<Cancha> ObtenerTodas(bool? activa = true)
            => _canchas.Where(c => !activa.HasValue || c.Activa == activa.Value).ToList();

        public Cancha? ObtenerPorId(int idCancha) => _canchas.FirstOrDefault(c => c.IdCancha == idCancha);

        public List<Cancha> ObtenerActivas() => _canchas.Where(c => c.Activa).ToList();

        public bool ExisteNombre(string nombre, int? idCanchaExcluir = null)
            => _canchas.Any(c => c.Nombre == nombre && (!idCanchaExcluir.HasValue || c.IdCancha != idCanchaExcluir.Value));

        public int ObtenerIdCategoriaHoraPadel() => 1;

        public void Agregar(Cancha cancha) => _canchas.Add(cancha);

        public void Modificar(Cancha cancha) { }

        public void CambiarEstado(int idCancha, bool nuevoEstado)
        {
            var cancha = ObtenerPorId(idCancha);
            if (cancha != null)
                cancha.Activa = nuevoEstado;
        }
    }

    internal sealed class ClienteRepositoryFake : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();

        public bool BuscarFueLlamado { get; private set; }

        public void Cargar(params Cliente[] clientes) => _clientes.AddRange(clientes);

        public List<Cliente> ObtenerTodos() => _clientes.ToList();

        public Cliente? ObtenerPorId(int id) => _clientes.FirstOrDefault(c => c.IdCliente == id);

        public List<Cliente> Buscar(string texto)
        {
            BuscarFueLlamado = true;
            texto = texto?.Trim().ToLower() ?? string.Empty;

            return _clientes
                .Where(c =>
                    c.Persona.Nombre.ToLower().Contains(texto) ||
                    c.Persona.Apellido.ToLower().Contains(texto) ||
                    (c.Persona.Dni != null && c.Persona.Dni.ToLower().Contains(texto)) ||
                    c.Email.ToLower().Contains(texto))
                .ToList();
        }

        public void Agregar(Cliente cliente) => _clientes.Add(cliente);

        public void Modificar(Cliente cliente) { }

        public void Bajalogica(int id) { }

        public Persona? BuscarPersonaPorDni(string dni) => null;
    }

    internal sealed class CajaRepositoryFake : ICajaRepository
    {
        public Caja? CajaAbierta { get; set; }

        public Caja? ObtenerCajaAbierta(int idEmpleado) => CajaAbierta;

        public Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado)
            => CajaAbierta ??= new Caja { IdCaja = 999, IdEmpleado = idEmpleado };
    }

    internal sealed class HorarioCanchaRepositoryFake : IHorarioCanchaRepository
    {
        private readonly List<HorarioCancha> _horarios = new();

        public void Cargar(params HorarioCancha[] horarios) => _horarios.AddRange(horarios);

        public List<HorarioCancha> ObtenerTodos(bool? activo = true)
            => _horarios.Where(h => !activo.HasValue || h.Activo == activo.Value).ToList();

        public List<HorarioCancha> ObtenerPorCancha(int idCancha, bool? activo = true)
            => _horarios.Where(h => h.IdCancha == idCancha && (!activo.HasValue || h.Activo == activo.Value)).ToList();

        public HorarioCancha? ObtenerPorId(int idHorario) => _horarios.FirstOrDefault(h => h.IdHorario == idHorario);

        public bool ExisteSolapamiento(int idCancha, string diaSemana, TimeSpan horaInicio, TimeSpan horaFin, int? idHorarioExcluir = null)
        {
            var candidato = new HorarioCancha { IdCancha = idCancha, DiaSemana = diaSemana, HoraInicio = horaInicio, HoraFin = horaFin };
            return _horarios.Any(h =>
                (!idHorarioExcluir.HasValue || h.IdHorario != idHorarioExcluir.Value) &&
                h.Activo &&
                h.Solapa(candidato));
        }

        public void Agregar(HorarioCancha horario) => _horarios.Add(horario);

        public void Modificar(HorarioCancha horario) { }

        public void CambiarEstado(int idHorario, bool nuevoEstado)
        {
            var horario = ObtenerPorId(idHorario);
            if (horario != null)
                horario.Activo = nuevoEstado;
        }
    }
}