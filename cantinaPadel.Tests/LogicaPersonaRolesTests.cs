using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class LogicaPersonaRolesTests
{
    [TestMethod]
    public void GuardarOtroRol_EmpleadoExistenteSinIdEnFormulario_NoEsDuplicado()
    {
        var empleadoGuardado = new Empleado { IdEmpleado = 17 };
        var empleadoDesdeOtroFormulario = new Empleado { IdEmpleado = 0 };

        LogicaPersonaRoles.ValidarEmpleadoExistente(empleadoGuardado, empleadoDesdeOtroFormulario);
    }

    [TestMethod]
    public void GuardarEmpleadoConIdDeOtraPersona_LanzaErrorDeDniDuplicado()
    {
        var empleadoGuardado = new Empleado { IdEmpleado = 17 };
        var empleadoDeOtraPersona = new Empleado { IdEmpleado = 23 };

        var ex = Assert.ThrowsExactly<ArgumentException>(() =>
            LogicaPersonaRoles.ValidarEmpleadoExistente(empleadoGuardado, empleadoDeOtraPersona));

        Assert.AreEqual("El DNI ingresado ya pertenece a otra persona registrada.", ex.Message);
    }
}
