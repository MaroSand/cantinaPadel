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
            CajaRepositoryFake? cajaRepo = null)
        {
            return new LogicaTurnoDia(
                turnoRepo ?? new TurnoDiaRepositoryFake(),
                canchaRepo ?? new CanchaRepositoryFake(),
                clienteRepo ?? new ClienteRepositoryFake(),
                cajaRepo ?? new CajaRepositoryFake());
        }

        private static Cancha CrearCanchaActiva(int idCancha = IdCanchaValida)
        {
            return new Cancha { IdCancha = idCancha, Nombre = "Cancha 1", Activa = true };
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
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana, inicio, new TimeSpan(12, 0, 0), LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana, new TimeSpan(6, 0, 0), new TimeSpan(7, 0, 0), LogicaTurnoDia.ModalidadDia));
            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, manana, inicio, fin, "Quincenal"));
        }

        [TestMethod]
        public void RegistrarAlquiler_CanchaInactiva_LanzaArgumentException()
        {
            // Misma validación cubre cancha inexistente (ObtenerPorId null) y cancha dada de baja
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(new Cancha { IdCancha = IdCanchaValida, Nombre = "Cancha 1", Activa = false });
            var logica = CrearLogica(canchaRepo: canchaRepo);

            Assert.ThrowsExactly<ArgumentException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), LogicaTurnoDia.ModalidadDia));
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
        public void RegistrarAlquiler_SinCajaAbiertaYSinBypass_LanzaInvalidOperationException()
        {
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var logica = CrearLogica(canchaRepo: canchaRepo);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), LogicaTurnoDia.ModalidadDia));
        }

        [TestMethod]
        public void RegistrarAlquiler_ModalidadPorDia_RegistraUnaSolaInstanciaPendienteDeCobro()
        {
            var fecha = DateTime.Today.AddDays(1);
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCanchaActiva());
            var turnoRepo = new TurnoDiaRepositoryFake();
            var cajaRepo = new CajaRepositoryFake { CajaAbierta = new Caja { IdCaja = 55, IdEmpleado = IdEmpleadoValido } };
            var logica = CrearLogica(turnoRepo, canchaRepo, cajaRepo: cajaRepo);

            logica.RegistrarAlquiler(IdClienteValido, IdEmpleadoValido, IdCanchaValida, fecha,
                new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), LogicaTurnoDia.ModalidadDia);

            Assert.AreEqual(55, turnoRepo.TurnoRegistrado!.IdCaja);
            Assert.AreEqual(TurnoReservado.ModalidadPorDia, turnoRepo.TurnoRegistrado.Modalidad);
            Assert.IsNull(turnoRepo.TurnoRegistrado.FechaFin);
            Assert.HasCount(1, turnoRepo.InstanciasRegistradas!);
            Assert.AreEqual(fecha.Date, turnoRepo.InstanciasRegistradas![0].Fecha);
            Assert.AreEqual(InstanciaTurno.EstadoPagoPendiente, turnoRepo.InstanciasRegistradas[0].EstadoPago);
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

        [TestMethod]
        public void CancelarTurno_Valida_CancelaDesdeLaFechaDeLaInstancia()
        {
            var fecha = DateTime.Today.AddDays(3);
            var turnoRepo = new TurnoDiaRepositoryFake();
            turnoRepo.CargarInstancia(new InstanciaTurno { IdInstancia = 1, Fecha = fecha, Estado = InstanciaTurno.EstadoActiva });
            var logica = CrearLogica(turnoRepo);

            logica.CancelarTurno(1);

            // El corte es la fecha de la instancia elegida (no "hoy"): en un turno Fijo,
            // cancelar un día en el medio corta desde ahí en adelante sin tocar los anteriores
            Assert.AreEqual((1, fecha), turnoRepo.CancelacionDesdeInstancia);
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

            // Apertura 08:00, cierre 23:00, franjas de 1h -> 15 franjas en total (08-09 ... 22-23)
            var horarios = logica.ObtenerHorarios(IdCanchaValida, fecha);

            Assert.HasCount(15, horarios);
            var franjaOcupada = horarios.Single(h => h.Horario == "10:00 - 11:00");
            Assert.IsFalse(franjaOcupada.Disponible);
            Assert.AreEqual(14, horarios.Count(h => h.Disponible));
        }

        // CalcularFechaFin / CalcularCantidadTurnos

        [TestMethod]
        public void CalcularFechaFinYCantidadTurnos_PorModalidad()
        {
            var logica = CrearLogica();

            Assert.ThrowsExactly<ArgumentException>(() => logica.CalcularFechaFin("Quincenal", DateTime.Today));

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

        // BuscarClientes

        [TestMethod]
        public void BuscarClientes_TextoCortoNoConsultaYTextoValidoMapeaDatos()
        {
            var clienteRepo = new ClienteRepositoryFake();
            var logica = CrearLogica(clienteRepo: clienteRepo);

            Assert.HasCount(0, logica.BuscarClientes("a"));
            Assert.IsFalse(clienteRepo.BuscarFueLlamado);

            clienteRepo.Cargar(new Cliente
            {
                IdCliente = 7,
                Email = "lucas.f@gmail.com",
                Persona = new Persona { Nombre = "Lucas", Apellido = "Fernandez", Dni = "38123456", Telefono = "1155667788" }
            });

            var resultado = logica.BuscarClientes("fernandez");

            Assert.HasCount(1, resultado);
            Assert.AreEqual("Fernandez, Lucas", resultado[0].NombreCompleto);
            Assert.AreEqual("38123456", resultado[0].Dni);
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
}