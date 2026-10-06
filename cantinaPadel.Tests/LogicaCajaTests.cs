using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

// LogicaCaja.CerrarCaja: el empleado cierra solo su caja; el Admin puede cerrar la de cualquiera
[TestClass]
public class LogicaCajaTests
{
    private const int IdEmpleadoDuenio = 2;

    private sealed class CajaRepositoryFake : ICajaRepository
    {
        public TurnoCaja Turno { get; } = new() { IdTurnoCaja = 1, IdCaja = 1, IdEmpleado = IdEmpleadoDuenio, MontoApertura = 1000m };
        public bool Cerrada { get; private set; }

        public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => Cerrada || Turno.IdEmpleado != idEmpleado ? null : Turno;
        public TurnoCaja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado) => Turno;
        public TurnoCaja Obtener(int idTurnoCaja) => Turno;
        public EfectivoCajaDatos ObtenerDatosEfectivo(int idTurnoCaja) => new(1000m, 0m, 0m, 0m);
        public List<MovimientoCajaDato> ObtenerMovimientosResumen(int idTurnoCaja) => new();
        public void Cerrar(int idTurnoCaja, CajaResumenDatos resumen, decimal efectivoEsperado, decimal efectivoContado, decimal diferencia, string? motivoDiferencia) => Cerrada = true;
    }

    private static (LogicaCaja logica, CajaRepositoryFake repo) Crear()
    {
        var repo = new CajaRepositoryFake();
        return (new LogicaCaja(repo, null!), repo);
    }

    [TestMethod]
    public void CerrarCaja_EmpleadoCierraSuPropiaCaja_Cierra()
    {
        var (logica, repo) = Crear();

        logica.CerrarCaja(1, IdEmpleadoDuenio, 1000m, null, "Empleado");

        Assert.IsTrue(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_EmpleadoIntentaCerrarCajaAjena_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.CerrarCaja(1, idEmpleado: 3, 1000m, null, "Empleado"));
        Assert.IsFalse(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_SinRol_NoPuedeCerrarCajaAjena()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.CerrarCaja(1, idEmpleado: 3, 1000m, null));
        Assert.IsFalse(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_AdminCierraCajaDeOtroEmpleado_Cierra()
    {
        var (logica, repo) = Crear();

        logica.CerrarCaja(1, idEmpleado: 1, 1000m, null, "Admin");

        Assert.IsTrue(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_AdminConDiferenciaSinMotivo_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.CerrarCaja(1, idEmpleado: 1, 900m, " ", "Admin"));
        Assert.IsFalse(repo.Cerrada);
    }
}