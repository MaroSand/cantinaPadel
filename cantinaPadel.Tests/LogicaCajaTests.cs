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
        public bool Cerrada { get; set; }
        public List<IngresoCaja> Ingresos { get; } = new();
        public decimal? MontoAperturaRegistrado { get; private set; }

        public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => Cerrada || Turno.IdEmpleado != idEmpleado ? null : Turno;
        public TurnoCaja? ObtenerCajaAbiertaGeneral() => Cerrada ? null : Turno;
        public TurnoCaja Abrir(int idEmpleado, decimal montoInicial)
        {
            MontoAperturaRegistrado = montoInicial;
            return Turno;
        }
        public bool ExisteIngreso(int idTurnoCaja) => Ingresos.Any(i => i.IdTurnoCaja == idTurnoCaja);
        public void RegistrarIngreso(IngresoCaja ingreso) => Ingresos.Add(ingreso);
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

    // Apertura: el monto inicial lo define quien abre la caja (0 por defecto)
    [TestMethod]
    public void AbrirCaja_ConCajaYaAbierta_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.AbrirCaja(IdEmpleadoDuenio, 1000m));
        Assert.IsNull(repo.MontoAperturaRegistrado);
    }

    [TestMethod]
    public void AbrirCaja_SinMonto_AbreConCero()
    {
        var (logica, repo) = Crear();
        repo.Cerrada = true;

        logica.AbrirCaja(IdEmpleadoDuenio);

        Assert.AreEqual(0m, repo.MontoAperturaRegistrado);
    }

    [TestMethod]
    public void AbrirCaja_ConMonto_UsaElMontoIngresado()
    {
        var (logica, repo) = Crear();
        repo.Cerrada = true;

        logica.AbrirCaja(IdEmpleadoDuenio, 2500m);

        Assert.AreEqual(2500m, repo.MontoAperturaRegistrado);
    }

    [TestMethod]
    public void AbrirCaja_MontoNegativo_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.AbrirCaja(IdEmpleadoDuenio, -1m));
        Assert.IsNull(repo.MontoAperturaRegistrado);
    }

    // Ingreso de efectivo: sin contraseña, una sola vez por turno de caja, con monto y empleado registrados
    [TestMethod]
    public void AgregarEfectivo_PrimerIngreso_RegistraMontoYEmpleado()
    {
        var (logica, repo) = Crear();

        logica.AgregarEfectivo(1, 500m, IdEmpleadoDuenio);

        Assert.AreEqual(1, repo.Ingresos.Count);
        Assert.AreEqual(500m, repo.Ingresos[0].Monto);
        Assert.AreEqual(IdEmpleadoDuenio, repo.Ingresos[0].IdAdmin);
        Assert.AreEqual(1, repo.Ingresos[0].IdTurnoCaja);
    }

    [TestMethod]
    public void AgregarEfectivo_SegundoIngresoEnElMismoTurno_Lanza()
    {
        var (logica, repo) = Crear();
        logica.AgregarEfectivo(1, 500m, IdEmpleadoDuenio);

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.AgregarEfectivo(1, 200m, IdEmpleadoDuenio));
        Assert.AreEqual(1, repo.Ingresos.Count);
    }

    [TestMethod]
    public void AgregarEfectivo_MontoCero_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.AgregarEfectivo(1, 0m, IdEmpleadoDuenio));
        Assert.AreEqual(0, repo.Ingresos.Count);
    }
}