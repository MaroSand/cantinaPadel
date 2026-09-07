using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // BLL: LogicaCancha - Alta/Modificación/Baja de canchas, con la regla de nombre único entre canchas
    // (activas o no) al Agregar, Modificar y, al reactivar, también en CambiarEstado. Además, al dar de
    // baja una cancha, sus horarios fijos se desactivan en cascada (ver región CambiarEstado)
    [TestClass]
    public class LogicaCanchaTests
    {
        private static Cancha CrearCancha(int id = 0, string nombre = "Cancha 1", bool activa = true)
            => new Cancha { IdCancha = id, Nombre = nombre, Activa = activa };

        private static HorarioCancha CrearHorario(int id, int idCancha, bool activo = true)
            => new HorarioCancha
            {
                IdHorario = id,
                IdCancha = idCancha,
                DiaSemana = "Lunes",
                HoraInicio = TimeSpan.Parse("08:00"),
                HoraFin = TimeSpan.Parse("10:00"),
                Activo = activo
            };

        // Usamos siempre la fake de horarios (nunca el repo real) para que CambiarEstado, al desactivar en cascada,
        // no intente pegarle a una base de datos real durante los tests
        private static LogicaCancha CrearLogica(ICanchaRepository repo, HorarioCanchaRepositoryFake? horarioRepo = null)
            => new LogicaCancha(repo, horarioRepo ?? new HorarioCanchaRepositoryFake());

        // Agregar

        [TestMethod]
        public void Agregar_DatosValidos_CreaElProductoHoraPadelAsociado()
        {
            var repo = new CanchaRepositoryFake();
            var logica = CrearLogica(repo);
            var cancha = CrearCancha(nombre: "  Cancha 3  ");

            logica.Agregar(cancha);

            // El nombre se normaliza (Trim) y se crea el producto "Hora Padel" en $0, a completar después desde Actualización de Precios
            Assert.AreEqual("Cancha 3", cancha.Nombre);
            Assert.IsNotNull(cancha.Producto);
            Assert.AreEqual("Hora Padel - Cancha 3", cancha.Producto!.Nombre);
            Assert.AreEqual(0m, cancha.Producto.PrecioVenta);
            Assert.IsTrue(cancha.Producto.Activo);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Agregar_NombreVacio_LanzaArgumentException(string? nombre)
        {
            var logica = CrearLogica(new CanchaRepositoryFake());
            var cancha = CrearCancha(nombre: nombre!);

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(cancha));
        }

        [TestMethod]
        public void Agregar_NombreYaUsadoPorCanchaActiva_LanzaArgumentException()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = CrearLogica(repo);

            var nueva = CrearCancha(nombre: "Cancha 1");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nueva));
        }

        [TestMethod]
        public void Agregar_NombreUsadoPorCanchaInactiva_LanzaArgumentException()
        {
            // El nombre debe ser único entre TODAS las canchas, estén activas o no:
            // no puede haber dos filas con el mismo nombre aunque una esté dada de baja.
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: false));
            var logica = CrearLogica(repo);

            var nueva = CrearCancha(nombre: "Cancha 1");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nueva));
        }

        // Modificar

        [TestMethod]
        public void Modificar_NombreYaUsadoPorOtraCanchaActiva_LanzaArgumentException()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(
                CrearCancha(id: 1, nombre: "Cancha 1", activa: true),
                CrearCancha(id: 2, nombre: "Cancha 2", activa: true));
            var logica = CrearLogica(repo);

            var modificada = CrearCancha(id: 2, nombre: "Cancha 1");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Modificar(modificada));
        }

        [TestMethod]
        public void Modificar_ConservaSuPropioNombre_NoLanzaExcepcion()
        {
            // La cancha no debe "chocar" contra sí misma al modificarse sin cambiar el nombre
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = CrearLogica(repo);

            var modificada = CrearCancha(id: 1, nombre: "Cancha 1");

            logica.Modificar(modificada);
        }

        // CambiarEstado

        [TestMethod]
        public void CambiarEstado_Desactivar_NoValidaNombreYCambiaElEstado()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = CrearLogica(repo);

            logica.CambiarEstado(1, nuevoEstado: false);

            Assert.IsFalse(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_Desactivar_DesactivaEnCascadaLosHorariosDeEsaCancha()
        {
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));

            var horarioRepo = new HorarioCanchaRepositoryFake();
            horarioRepo.Cargar(
                CrearHorario(id: 10, idCancha: 1, activo: true),
                CrearHorario(id: 11, idCancha: 1, activo: true));

            var logica = CrearLogica(canchaRepo, horarioRepo);

            logica.CambiarEstado(1, nuevoEstado: false);

            Assert.IsFalse(horarioRepo.ObtenerPorId(10)!.Activo);
            Assert.IsFalse(horarioRepo.ObtenerPorId(11)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_Desactivar_NoTocaHorariosDeOtraCancha()
        {
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(
                CrearCancha(id: 1, nombre: "Cancha 1", activa: true),
                CrearCancha(id: 2, nombre: "Cancha 2", activa: true));

            var horarioRepo = new HorarioCanchaRepositoryFake();
            horarioRepo.Cargar(CrearHorario(id: 20, idCancha: 2, activo: true));

            var logica = CrearLogica(canchaRepo, horarioRepo);

            logica.CambiarEstado(1, nuevoEstado: false);

            // El horario es de la Cancha 2: no debe verse afectado por la baja de la Cancha 1
            Assert.IsTrue(horarioRepo.ObtenerPorId(20)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_Activar_NoReactivaLosHorarios()
        {
            // Al reactivar la cancha, sus horarios quedan como estaban (inactivos): se
            // reactivan a mano desde la pantalla de Horarios, no automáticamente.
            var canchaRepo = new CanchaRepositoryFake();
            canchaRepo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: false));

            var horarioRepo = new HorarioCanchaRepositoryFake();
            horarioRepo.Cargar(CrearHorario(id: 10, idCancha: 1, activo: false));

            var logica = CrearLogica(canchaRepo, horarioRepo);

            logica.CambiarEstado(1, nuevoEstado: true);

            Assert.IsTrue(canchaRepo.ObtenerPorId(1)!.Activa);
            Assert.IsFalse(horarioRepo.ObtenerPorId(10)!.Activo);
        }

        [TestMethod]
        public void CambiarEstado_ActivarSinConflictoDeNombre_CambiaElEstado()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: false));
            var logica = CrearLogica(repo);

            logica.CambiarEstado(1, nuevoEstado: true);

            Assert.IsTrue(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_ActivarConOtraCanchaActivaConMismoNombre_LanzaArgumentException()
        {
            // Reproduce el bug reportado: Cancha 1 inactiva, se crea Cancha 2 activa con el mismo nombre, y al reactivar la 1 quedarían
            // dos canchas activas con el mismo nombre si CambiarEstado no volviera a validar
            var repo = new CanchaRepositoryFake();
            repo.Cargar(
                CrearCancha(id: 1, nombre: "Cancha 1", activa: false),
                CrearCancha(id: 2, nombre: "Cancha 1", activa: true));
            var logica = CrearLogica(repo);

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(1, nuevoEstado: true));

            // La excepción no debe dejar el estado a medio cambiar
            Assert.IsFalse(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_CanchaInexistente_LanzaArgumentException()
        {
            var logica = CrearLogica(new CanchaRepositoryFake());

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(999, nuevoEstado: true));
        }
    }
}