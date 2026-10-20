using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // BLL: LogicaUsuario - Login: distingue usuario inexistente, contraseña incorrecta, usuario inhabilitado y credenciales válidas
    [TestClass]
    public class LogicaUsuarioTests
    {
        private sealed class EmpleadoRepositoryFake : IEmpleadoRepository
        {
            private readonly List<Empleado> _empleados;

            public EmpleadoRepositoryFake(params Empleado[] empleados) => _empleados = empleados.ToList();

            // Devuelve el empleado por usuario sin filtrar por estado
            public Empleado? ObtenerPorUsuario(string nombreUsuario)
                => _empleados.FirstOrDefault(e => e.NombreUsuario == nombreUsuario);

            public Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena) => throw new NotSupportedException();
            public List<Empleado> ObtenerTodos() => _empleados;
            public void Guardar(Empleado empleado) => throw new NotSupportedException();
            public bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir) => throw new NotSupportedException();
            public bool ExisteDni(string dni, int idPersonaExcluir) => throw new NotSupportedException();
        }

        private static Empleado CrearEmpleado(bool activo = true)
            => new Empleado
            {
                IdEmpleado = 7,
                NombreUsuario = "empleado1",
                Contrasena = "87654321",
                Rol = "Empleado",
                Activo = activo
            };

        [TestMethod]
        public void ValidarCredenciales_CredencialesCorrectasYActivo_DevuelveOk()
        {
            var logica = new LogicaUsuario(new EmpleadoRepositoryFake(CrearEmpleado()));

            var resultado = logica.ValidarCredenciales("empleado1", "87654321", out int id, out string? rol);

            Assert.AreEqual(ResultadoLogin.Ok, resultado);
            Assert.AreEqual(7, id);
            Assert.AreEqual("Empleado", rol);
        }

        [TestMethod]
        public void ValidarCredenciales_UsuarioInexistente_DevuelveUsuarioNoEncontrado()
        {
            var logica = new LogicaUsuario(new EmpleadoRepositoryFake(CrearEmpleado()));

            var resultado = logica.ValidarCredenciales("otro", "87654321", out int id, out string? rol);

            Assert.AreEqual(ResultadoLogin.UsuarioNoEncontrado, resultado);
            Assert.AreEqual(0, id);
            Assert.IsNull(rol);
        }

        [TestMethod]
        public void ValidarCredenciales_ContrasenaIncorrecta_DevuelveContrasenaIncorrecta()
        {
            var logica = new LogicaUsuario(new EmpleadoRepositoryFake(CrearEmpleado()));

            var resultado = logica.ValidarCredenciales("empleado1", "000", out _, out _);

            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, resultado);
        }

        [TestMethod]
        public void ValidarCredenciales_EmpleadoInactivoConCredencialesCorrectas_DevuelveUsuarioInhabilitado()
        {
            var logica = new LogicaUsuario(new EmpleadoRepositoryFake(CrearEmpleado(activo: false)));

            var resultado = logica.ValidarCredenciales("empleado1", "87654321", out int id, out string? rol);

            Assert.AreEqual(ResultadoLogin.UsuarioInhabilitado, resultado);
            // No se le asigna sesión a un usuario inhabilitado
            Assert.AreEqual(0, id);
            Assert.IsNull(rol);
        }

        [TestMethod]
        public void ValidarCredenciales_EmpleadoInactivoConContrasenaIncorrecta_NoRevelaElEstado()
        {
            var logica = new LogicaUsuario(new EmpleadoRepositoryFake(CrearEmpleado(activo: false)));

            var resultado = logica.ValidarCredenciales("empleado1", "000", out _, out _);

            Assert.AreEqual(ResultadoLogin.ContrasenaIncorrecta, resultado);
        }
    }
}