using cantinaPadel.Models;

namespace cantinaPadel.Tests
{
    // ------------------------------------------------------------------
    // Modelo: Empleado - ValidarFormato valida los campos propios del
    // empleado (usuario, contraseña, rol) y delega en Persona lo que es
    // de Persona (nombre, apellido, dni, etc).
    // ------------------------------------------------------------------
    [TestClass]
    public class EmpleadoModelTests
    {
        // Arma un Empleado con datos válidos para no repetir el mismo
        // bloque en cada test; cada test pisa solo el campo que le interesa.
        private static Empleado CrearEmpleadoValido()
        {
            return new Empleado
            {
                NombreUsuario = "jgomez",
                Contrasena = "abc1234",
                Rol = "Empleado",
                Persona = new Persona
                {
                    Nombre = "Juan",
                    Apellido = "Gomez",
                    Dni = "30111222"
                }
            };
        }

        [TestMethod]
        public void ValidarFormato_PersonaNula_LanzaArgumentException()
        {
            var empleado = CrearEmpleadoValido();
            empleado.Persona = null!; // null! = "sé que Persona no admite null, lo hago a propósito para este test"

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void ValidarFormato_NombreUsuarioVacio_LanzaArgumentException(string? valor)
        {
            var empleado = CrearEmpleadoValido();
            empleado.NombreUsuario = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        public void ValidarFormato_NombreUsuarioExcedeLongitud_LanzaArgumentException()
        {
            var empleado = CrearEmpleadoValido();
            empleado.NombreUsuario = new string('a', 51); // límite es 50

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void ValidarFormato_ContrasenaVacia_LanzaArgumentException(string? valor)
        {
            var empleado = CrearEmpleadoValido();
            empleado.Contrasena = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        public void ValidarFormato_ContrasenaExcedeLongitud_LanzaArgumentException()
        {
            var empleado = CrearEmpleadoValido();
            empleado.Contrasena = new string('a', 10); // límite es 9

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void ValidarFormato_RolVacio_LanzaArgumentException(string? valor)
        {
            var empleado = CrearEmpleadoValido();
            empleado.Rol = valor!;

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato());
        }

        [TestMethod]
        public void ValidarFormato_DniObligatorioYFaltante_LanzaArgumentException()
        {
            // A diferencia de Cliente/Proveedor, el alta de Empleado exige DNI:
            // se llama con dniObligatorio: true, así que sin DNI debe fallar.
            var empleado = CrearEmpleadoValido();
            empleado.Persona.Dni = null;

            Assert.ThrowsExactly<ArgumentException>(() => empleado.ValidarFormato(dniObligatorio: true));
        }

        [TestMethod]
        public void ValidarFormato_ConEspaciosAlrededor_RecortaYAcepta()
        {
            var empleado = CrearEmpleadoValido();
            empleado.NombreUsuario = "  jgomez  ";
            empleado.Contrasena = "  abc1234  ";
            empleado.Rol = "  Empleado  ";

            empleado.ValidarFormato();

            Assert.AreEqual("jgomez", empleado.NombreUsuario);
            Assert.AreEqual("abc1234", empleado.Contrasena);
            Assert.AreEqual("Empleado", empleado.Rol);
        }

        [TestMethod]
        public void ValidarFormato_TodoValido_NoLanzaExcepcion()
        {
            // Caso "feliz": usuario, contraseña, rol y datos de persona
            // (incluido el dni obligatorio) pasan juntos la validación.
            var empleado = CrearEmpleadoValido();

            empleado.ValidarFormato(dniObligatorio: true);
        }
    }
}