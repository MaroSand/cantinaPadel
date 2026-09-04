using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // BLL: LogicaHorarioCancha - Alta/Modificación/Baja de bandas horarias fijas por cancha y día, con la regla de no solapamiento entre
    // horarios ACTIVOS de la misma cancha y mismo día (Agregar, Modificar y, al reactivar, también CambiarEstado)
    [TestClass]
    public class LogicaHorarioCanchaTests
    {
        private const int IdCanchaValida = 1;
        private const int OtraCancha = 2;

        private static HorarioCancha CrearHorario(
            int id = 0,
            int idCancha = IdCanchaValida,
            string dia = "Lunes",
            string horaInicio = "08:00",
            string horaFin = "10:00",
            bool activo = true)
            => new HorarioCancha
            {
                IdHorario = id,
                IdCancha = idCancha,
                DiaSemana = dia,
                HoraInicio = TimeSpan.Parse(horaInicio),
                HoraFin = TimeSpan.Parse(horaFin),
                Activo = activo
            };

        // Agregar

        [TestMethod]
        public void Agregar_DatosValidos_LoGuarda()
        {
            var repo = new HorarioCanchaRepositoryFake();
            var logica = new LogicaHorarioCancha(repo);
            var horario = CrearHorario(dia: "  Lunes  ");

            logica.Agregar(horario);

            Assert.AreEqual("Lunes", horario.DiaSemana);
            Assert.AreEqual(1, repo.ObtenerTodos().Count);
        }

        [TestMethod]
        public void Agregar_SinCanchaSeleccionada_LanzaArgumentException()
        {
            var logica = new LogicaHorarioCancha(new HorarioCanchaRepositoryFake());
            var horario = CrearHorario(idCancha: 0);

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(horario));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Agregar_DiaSemanaVacio_LanzaArgumentException(string? dia)
        {
            var logica = new LogicaHorarioCancha(new HorarioCanchaRepositoryFake());
            var horario = CrearHorario(dia: dia!);

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(horario));
        }

        [TestMethod]
        public void Agregar_DiaSemanaInvalido_LanzaArgumentException()
        {
            var logica = new LogicaHorarioCancha(new HorarioCanchaRepositoryFake());
            var horario = CrearHorario(dia: "Funday");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(horario));
        }

        [TestMethod]
        public void Agregar_HoraFinIgualAHoraInicio_LanzaArgumentException()
        {
            var logica = new LogicaHorarioCancha(new HorarioCanchaRepositoryFake());
            var horario = CrearHorario(horaInicio: "08:00", horaFin: "08:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(horario));
        }

        [TestMethod]
        public void Agregar_SeSolapaConHorarioActivoDeLaMismaCanchaYDia_LanzaArgumentException()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            // 09:00-11:00 se cruza con el 08:00-10:00 ya cargado
            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "09:00", horaFin: "11:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nuevo));
        }

        [TestMethod]
        public void Agregar_HorarioContiguoSinSuperposicion_NoLanzaExcepcion()
        {
            // 08-10 y 10-12: comparten el límite pero no se pisan (no hay solapamiento real)
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "10:00", horaFin: "12:00");

            logica.Agregar(nuevo);
        }

        [TestMethod]
        public void Agregar_MismoHorarioPeroOtroDia_NoLanzaExcepcion()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Martes", horaInicio: "08:00", horaFin: "10:00");

            logica.Agregar(nuevo);
        }

        [TestMethod]
        public void Agregar_MismoHorarioPeroOtraCancha_NoLanzaExcepcion()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, idCancha: IdCanchaValida, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(idCancha: OtraCancha, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00");

            logica.Agregar(nuevo);
        }

        [TestMethod]
        public void Agregar_SeSolapaSoloConHorarioInactivo_NoLanzaExcepcion()
        {
            // ExisteSolapamiento solo debe chequear horarios ACTIVOS
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00", activo: false));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "09:00", horaFin: "11:00");

            logica.Agregar(nuevo);
        }

        [TestMethod]
        public void Agregar_HorarioQueCruzaMedianocheSeSolapaConOtroQueTambienCruza_LanzaArgumentException()
        {
            // Caso "normal" de cruce de medianoche: ambos horarios cruzan, ahí la normalización (+24hs) sí compara bien
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Viernes", horaInicio: "22:00", horaFin: "02:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Viernes", horaInicio: "23:00", horaFin: "03:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nuevo));
        }

        [TestMethod]
        public void Agregar_CruzaMedianocheYPisaHorarioYaOcupadoDelDiaSiguiente_LanzaArgumentException()
        {
            // "Lunes 22-02" cruza a la madrugada del martes; si ya hay un horario "Martes 01-03" (que arranca dentro de esa madrugada),
            // no debe poder guardarse
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Martes", horaInicio: "01:00", horaFin: "03:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "22:00", horaFin: "02:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nuevo));
        }

        [TestMethod]
        public void Agregar_CruzaMedianocheYNoLlegaAPisarElHorarioDelDiaSiguiente_NoLanzaExcepcion()
        {
            // La madrugada del "Lunes 22-02" termina justo a las 02:00: un horario que arranca "Martes 02-04" es contiguo, no se pisan
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Martes", horaInicio: "02:00", horaFin: "04:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "22:00", horaFin: "02:00");

            logica.Agregar(nuevo);
        }

        [TestMethod]
        public void Agregar_CruzaMedianocheDeDomingoALunes_PisaHorarioDeLunes_LanzaArgumentException()
        {
            // El ciclo de días también tiene que dar la vuelta la semana: Domingo -> Lunes
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "01:00", horaFin: "03:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Domingo", horaInicio: "22:00", horaFin: "02:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nuevo));
        }

        [TestMethod]
        public void Agregar_MismaMadrugadaPeroDiaNoAdyacenteAlQueCruza_NoLanzaExcepcion()
        {
            // "Lunes 22-02" solo puede pisar la madrugada del Martes (día siguiente), nunca la
            // del Miercoles, aunque el horario tenga el mismo rango horario.
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Miercoles", horaInicio: "01:00", horaFin: "03:00"));
            var logica = new LogicaHorarioCancha(repo);

            var nuevo = CrearHorario(dia: "Lunes", horaInicio: "22:00", horaFin: "02:00");

            logica.Agregar(nuevo);
        }

        // Modificar

        [TestMethod]
        public void Modificar_SeSolapaConOtroHorarioActivo_LanzaArgumentException()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(
                CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"),
                CrearHorario(id: 2, dia: "Lunes", horaInicio: "10:00", horaFin: "12:00"));
            var logica = new LogicaHorarioCancha(repo);

            // Se intenta correr el horario 2 para que pise al 1
            var modificado = CrearHorario(id: 2, dia: "Lunes", horaInicio: "09:00", horaFin: "11:00");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Modificar(modificado));
        }

        [TestMethod]
        public void Modificar_ConservaSuPropioRangoHorario_NoLanzaExcepcion()
        {
            // El horario no debe "chocar" contra sí mismo al modificarse sin cambiar el rango
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            var modificado = CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00");

            logica.Modificar(modificado);
        }

        // CambiarEstado

        [TestMethod]
        public void CambiarEstado_Desactivar_NoValidaSolapamientoYCambiaElEstado()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00"));
            var logica = new LogicaHorarioCancha(repo);

            logica.CambiarEstado(1, nuevoEstado: false);

            Assert.IsFalse(repo.ObtenerPorId(1)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_ActivarSinConflictoDeSolapamiento_CambiaElEstado()
        {
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00", activo: false));
            var logica = new LogicaHorarioCancha(repo);

            logica.CambiarEstado(1, nuevoEstado: true);

            Assert.IsTrue(repo.ObtenerPorId(1)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_ActivarConOtroHorarioActivoSuperpuesto_LanzaArgumentException()
        {
            // Horario 1 inactivo (08-10 lunes); horario 2 activo (09-11 lunes) ya ocupa esa franja: al querer reactivar el 1 quedarían dos horarios activos pisándose
            var repo = new HorarioCanchaRepositoryFake();
            repo.Cargar(
                CrearHorario(id: 1, dia: "Lunes", horaInicio: "08:00", horaFin: "10:00", activo: false),
                CrearHorario(id: 2, dia: "Lunes", horaInicio: "09:00", horaFin: "11:00", activo: true));
            var logica = new LogicaHorarioCancha(repo);

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(1, nuevoEstado: true));

            // La excepción no debe dejar el estado a medio cambiar
            Assert.IsFalse(repo.ObtenerPorId(1)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_HorarioInexistente_LanzaArgumentException()
        {
            var logica = new LogicaHorarioCancha(new HorarioCanchaRepositoryFake());

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(999, nuevoEstado: true));
        }
    }
}