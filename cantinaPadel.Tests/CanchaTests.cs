using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // BLL: LogicaCancha - Alta/Modificación/Baja de canchas, con la regla de nombre único entre canchas activas (Agregar, Modificar y,
    // desde el fix de "no permitir nombres repetidos al activar", también CambiarEstado)
    [TestClass]
    public class LogicaCanchaTests
    {
        private static Cancha CrearCancha(int id = 0, string nombre = "Cancha 1", bool activa = true)
            => new Cancha { IdCancha = id, Nombre = nombre, Activa = activa };

        // Agregar

        [TestMethod]
        public void Agregar_DatosValidos_CreaElProductoHoraPadelAsociado()
        {
            var repo = new CanchaRepositoryFake();
            var logica = new LogicaCancha(repo);
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
            var logica = new LogicaCancha(new CanchaRepositoryFake());
            var cancha = CrearCancha(nombre: nombre!);

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(cancha));
        }

        [TestMethod]
        public void Agregar_NombreYaUsadoPorCanchaActiva_LanzaArgumentException()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = new LogicaCancha(repo);

            var nueva = CrearCancha(nombre: "Cancha 1");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Agregar(nueva));
        }

        [TestMethod]
        public void Agregar_NombreUsadoPorCanchaInactiva_LanzaArgumentException()
        {
            // El nombre debe ser único entre todas las canchas, estén activas o no
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: false));
            var logica = new LogicaCancha(repo);

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
            var logica = new LogicaCancha(repo);

            var modificada = CrearCancha(id: 2, nombre: "Cancha 1");

            Assert.ThrowsExactly<ArgumentException>(() => logica.Modificar(modificada));
        }

        [TestMethod]
        public void Modificar_ConservaSuPropioNombre_NoLanzaExcepcion()
        {
            // La cancha no debe "chocar" contra sí misma al modificarse sin cambiar el nombre
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = new LogicaCancha(repo);

            var modificada = CrearCancha(id: 1, nombre: "Cancha 1");

            logica.Modificar(modificada);
        }

        // CambiarEstado

        [TestMethod]
        public void CambiarEstado_Desactivar_NoValidaNombreYCambiaElEstado()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: true));
            var logica = new LogicaCancha(repo);

            logica.CambiarEstado(1, nuevoEstado: false);

            Assert.IsFalse(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_ActivarSinConflictoDeNombre_CambiaElEstado()
        {
            var repo = new CanchaRepositoryFake();
            repo.Cargar(CrearCancha(id: 1, nombre: "Cancha 1", activa: false));
            var logica = new LogicaCancha(repo);

            logica.CambiarEstado(1, nuevoEstado: true);

            Assert.IsTrue(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_ActivarConOtraCanchaActivaConMismoNombre_LanzaArgumentException()
        {
            // Cancha 1 inactiva, se crea Cancha 2 activa con el mismo nombre, y al reactivar la 1 quedarían dos canchas activas con el mismo
            // nombre si CambiarEstado no volviera a validar
            var repo = new CanchaRepositoryFake();
            repo.Cargar(
                CrearCancha(id: 1, nombre: "Cancha 1", activa: false),
                CrearCancha(id: 2, nombre: "Cancha 1", activa: true));
            var logica = new LogicaCancha(repo);

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(1, nuevoEstado: true));

            // La excepción no debe dejar el estado a medio cambiar
            Assert.IsFalse(repo.ObtenerPorId(1)!.Activa);
        }

        [TestMethod]
        public void CambiarEstado_CanchaInexistente_LanzaArgumentException()
        {
            var logica = new LogicaCancha(new CanchaRepositoryFake());

            Assert.ThrowsExactly<ArgumentException>(() => logica.CambiarEstado(999, nuevoEstado: true));
        }
    }
}