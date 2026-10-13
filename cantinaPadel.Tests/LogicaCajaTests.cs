using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

// LogicaCaja: apertura, cierre, ingreso de efectivo, retiro con clave de administrador e historial
[TestClass]
public class LogicaCajaTests
{
    private const int IdEmpleadoDuenio = 2;
    private const int IdAdmin = 1;
    private const int IdEmpleadoSinPermiso = 3;
    private const string ClaveAdmin = "12345678";
    private const string ClaveEmpleado = "87654321";

    private sealed record CierreRegistrado(decimal EfectivoEsperado, decimal EfectivoContado, decimal Diferencia, string? Motivo);

    private sealed class CajaRepositoryFake : ICajaRepository
    {
        public TurnoCaja Turno { get; } = new() { IdTurnoCaja = 1, IdCaja = 1, IdEmpleado = IdEmpleadoDuenio, MontoApertura = 1000m };
        public bool Cerrada { get; set; }
        public List<IngresoCaja> Ingresos { get; } = new();
        public List<RetiroCaja> Retiros { get; } = new();
        public decimal? MontoAperturaRegistrado { get; private set; }
        public EfectivoCajaDatos Datos { get; set; } = new(1000m, 0m, 0m, 0m);
        public CierreRegistrado? UltimoCierre { get; private set; }

        public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => Cerrada || Turno.IdEmpleado != idEmpleado ? null : Turno;
        public TurnoCaja? ObtenerCajaAbiertaGeneral() => Cerrada ? null : Turno;
        public TurnoCaja Abrir(int idEmpleado, decimal montoInicial)
        {
            MontoAperturaRegistrado = montoInicial;
            return Turno;
        }
        public bool ExisteIngreso(int idTurnoCaja) => Ingresos.Any(i => i.IdTurnoCaja == idTurnoCaja);
        public void RegistrarIngreso(IngresoCaja ingreso) => Ingresos.Add(ingreso);
        public void RegistrarRetiro(RetiroCaja retiro) => Retiros.Add(retiro);
        public TurnoCaja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado) => Turno;
        public TurnoCaja Obtener(int idTurnoCaja) => Turno;
        public EfectivoCajaDatos ObtenerDatosEfectivo(int idTurnoCaja) => Datos;
        public List<MovimientoCajaDato> ObtenerMovimientosResumen(int idTurnoCaja) => new();
        public void Cerrar(int idTurnoCaja, CajaResumenDatos resumen, decimal efectivoEsperado, decimal efectivoContado, decimal diferencia, string? motivoDiferencia)
        {
            Cerrada = true;
            UltimoCierre = new CierreRegistrado(efectivoEsperado, efectivoContado, diferencia, motivoDiferencia);
        }
    }

    private sealed class EmpleadoRepositoryFake : IEmpleadoRepository
    {
        public List<Empleado> Empleados { get; } = new()
        {
            new Empleado { IdEmpleado = IdAdmin, NombreUsuario = "admin", Contrasena = ClaveAdmin, Rol = "Admin", Activo = true },
            new Empleado { IdEmpleado = IdEmpleadoDuenio, NombreUsuario = "empleado1", Contrasena = ClaveEmpleado, Rol = "Empleado", Activo = true },
            new Empleado { IdEmpleado = IdEmpleadoSinPermiso, NombreUsuario = "empleado2", Contrasena = "79461358", Rol = "Empleado", Activo = true }
        };

        public List<Empleado> ObtenerTodos() => Empleados;
        public Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena) => throw new NotSupportedException();
        public Empleado? ObtenerPorUsuario(string nombreUsuario) => throw new NotSupportedException();
        public void Guardar(Empleado empleado) => throw new NotSupportedException();
        public bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir) => throw new NotSupportedException();
        public bool ExisteDni(string dni, int idPersonaExcluir) => throw new NotSupportedException();
    }

    private static (LogicaCaja logica, CajaRepositoryFake repo) Crear()
    {
        var repo = new CajaRepositoryFake();
        return (new LogicaCaja(repo, null!), repo);
    }

    // Igual que Crear pero con empleados cargados, necesario para validar la clave del administrador en los retiros
    private static (LogicaCaja logica, CajaRepositoryFake repo) CrearConEmpleados()
    {
        var repo = new CajaRepositoryFake();
        return (new LogicaCaja(repo, new EmpleadoRepositoryFake()), repo);
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

    [TestMethod]
    public void CerrarCaja_SinDiferencia_NoPideMotivoYRegistraEsperadoYDiferencia()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 2000m, 0m, 0m);

        logica.CerrarCaja(1, IdEmpleadoDuenio, 7000m, null, "Empleado");

        Assert.IsTrue(repo.Cerrada);
        Assert.IsNotNull(repo.UltimoCierre);
        Assert.AreEqual(7000m, repo.UltimoCierre.EfectivoEsperado);
        Assert.AreEqual(7000m, repo.UltimoCierre.EfectivoContado);
        Assert.AreEqual(0m, repo.UltimoCierre.Diferencia);
    }

    [TestMethod]
    public void CerrarCaja_FaltanteConMotivo_RegistraDiferenciaNegativaYMotivoSinEspacios()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 2000m, 0m, 0m);

        logica.CerrarCaja(1, IdEmpleadoDuenio, 6800m, "  Faltó un billete de 200  ", "Empleado");

        Assert.IsTrue(repo.Cerrada);
        Assert.IsNotNull(repo.UltimoCierre);
        Assert.AreEqual(-200m, repo.UltimoCierre.Diferencia);
        Assert.AreEqual("Faltó un billete de 200", repo.UltimoCierre.Motivo);
    }

    [TestMethod]
    public void CerrarCaja_SobranteSinMotivo_Lanza()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 2000m, 0m, 0m);

        Assert.ThrowsExactly<ArgumentException>(() => logica.CerrarCaja(1, IdEmpleadoDuenio, 7300m, null, "Empleado"));
        Assert.IsFalse(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_EfectivoContadoNegativo_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => logica.CerrarCaja(1, IdEmpleadoDuenio, -1m, "Error de carga", "Empleado"));
        Assert.IsFalse(repo.Cerrada);
    }

    [TestMethod]
    public void CerrarCaja_UsaVentasCobrosDeCuentaCorrienteYRetirosParaElEsperado()
    {
        var (logica, repo) = Crear();
        // Apertura + ingresos 6000, ventas en efectivo 2000, cobros de cuenta corriente 1000, retiros 1500
        repo.Datos = new EfectivoCajaDatos(6000m, 2000m, 1000m, 1500m);

        logica.CerrarCaja(1, IdEmpleadoDuenio, 7500m, null, "Empleado");

        Assert.IsNotNull(repo.UltimoCierre);
        Assert.AreEqual(7500m, repo.UltimoCierre.EfectivoEsperado);
        Assert.AreEqual(0m, repo.UltimoCierre.Diferencia);
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

    [TestMethod]
    public void AbrirCaja_SinUsuarioAutenticado_Lanza()
    {
        var (logica, repo) = Crear();
        repo.Cerrada = true;

        Assert.ThrowsExactly<ArgumentException>(() => logica.AbrirCaja(0, 1000m));
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

    [TestMethod]
    public void AgregarEfectivo_MontoNegativo_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.AgregarEfectivo(1, -100m, IdEmpleadoDuenio));
        Assert.AreEqual(0, repo.Ingresos.Count);
    }

    [TestMethod]
    public void AgregarEfectivo_CajaCerrada_Lanza()
    {
        var (logica, repo) = Crear();
        repo.Cerrada = true;

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.AgregarEfectivo(1, 500m, IdEmpleadoDuenio));
        Assert.AreEqual(0, repo.Ingresos.Count);
    }

    [TestMethod]
    public void AgregarEfectivo_SinUsuarioAutenticado_Lanza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.AgregarEfectivo(1, 500m, 0));
        Assert.AreEqual(0, repo.Ingresos.Count);
    }

    [TestMethod]
    public void AgregarEfectivo_NoRequiereContrasena_LoPuedeHacerUnEmpleadoComun()
    {
        // El ingreso no consulta ninguna contraseña: lo puede registrar un empleado sin rol de administrador
        var (logica, repo) = CrearConEmpleados();

        logica.AgregarEfectivo(1, 300m, IdEmpleadoSinPermiso);

        Assert.AreEqual(1, repo.Ingresos.Count);
        Assert.AreEqual(IdEmpleadoSinPermiso, repo.Ingresos[0].IdAdmin);
    }

    // Retiro de efectivo: solo con la clave del administrador, dentro del efectivo disponible
    [TestMethod]
    public void RetirarEfectivo_ConClaveDeAdmin_RegistraMontoYAdminQueAutorizo()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        logica.RetirarEfectivo(1, 1000m, ClaveAdmin);

        Assert.AreEqual(1, repo.Retiros.Count);
        Assert.AreEqual(1000m, repo.Retiros[0].Monto);
        Assert.AreEqual(IdAdmin, repo.Retiros[0].IdEmpleado);
        Assert.AreEqual(1, repo.Retiros[0].IdTurnoCaja);
    }

    [TestMethod]
    public void RetirarEfectivo_ContrasenaIncorrecta_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.RetirarEfectivo(1, 1000m, "00000000"));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_ContrasenaVacia_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.RetirarEfectivo(1, 1000m, ""));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_ClaveDeEmpleadoQueNoEsAdmin_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.RetirarEfectivo(1, 1000m, ClaveEmpleado));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_MontoMayorAlDisponible_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RetirarEfectivo(1, 6000m, ClaveAdmin));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_MontoIgualAlDisponible_Permite()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        logica.RetirarEfectivo(1, 5000m, ClaveAdmin);

        Assert.AreEqual(1, repo.Retiros.Count);
        Assert.AreEqual(5000m, repo.Retiros[0].Monto);
    }

    [TestMethod]
    public void RetirarEfectivo_DescuentaLosRetirosYaRealizados()
    {
        var (logica, repo) = CrearConEmpleados();
        // Apertura 5000 con retiros previos por 4500: quedan solo 500 disponibles
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 4500m);

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RetirarEfectivo(1, 1000m, ClaveAdmin));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_MontoCero_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();

        Assert.ThrowsExactly<ArgumentException>(() => logica.RetirarEfectivo(1, 0m, ClaveAdmin));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_MontoNegativo_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();

        Assert.ThrowsExactly<ArgumentException>(() => logica.RetirarEfectivo(1, -50m, ClaveAdmin));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    [TestMethod]
    public void RetirarEfectivo_CajaCerrada_Lanza()
    {
        var (logica, repo) = CrearConEmpleados();
        repo.Cerrada = true;

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RetirarEfectivo(1, 100m, ClaveAdmin));
        Assert.AreEqual(0, repo.Retiros.Count);
    }

    // Efectivo disponible: apertura + ingresos + ventas en efectivo + cobros de cuenta corriente - retiros
    [TestMethod]
    public void ObtenerEfectivoDisponible_SumaIngresosVentasYDescuentaRetiros()
    {
        var (logica, repo) = Crear();
        // Apertura 5000 + ingreso 1000 = 6000, venta en efectivo 2000, retiro 1500
        repo.Datos = new EfectivoCajaDatos(6000m, 2000m, 0m, 1500m);

        Assert.AreEqual(6500m, logica.ObtenerEfectivoDisponible(1));
    }

    [TestMethod]
    public void ObtenerEfectivoDisponible_IncluyeCobrosDeCuentaCorrienteEnEfectivo()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 2000m, 1000m, 0m);

        Assert.AreEqual(8000m, logica.ObtenerEfectivoDisponible(1));
    }

    [TestMethod]
    public void ObtenerEfectivoDisponible_SinMovimientos_EsElFondoInicial()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 0m, 0m, 0m);

        Assert.AreEqual(5000m, logica.ObtenerEfectivoDisponible(1));
    }

    // Historial: el completo es solo para administradores
    [TestMethod]
    public void ObtenerTodoHistorial_ComoEmpleado_Lanza()
    {
        var (logica, _) = Crear();

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.ObtenerTodoHistorial("Empleado"));
    }

    [TestMethod]
    public void ObtenerTodoHistorial_SinRol_Lanza()
    {
        var (logica, _) = Crear();

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.ObtenerTodoHistorial(null));
    }

    [TestMethod]
    public void ObtenerTodoHistorial_ComoAdmin_NoLanza()
    {
        var (logica, _) = Crear();

        Assert.IsNotNull(logica.ObtenerTodoHistorial("Admin"));
    }

    [TestMethod]
    public void ObtenerHistorialEfectivo_SinUsuarioAutenticado_Lanza()
    {
        var (logica, _) = Crear();

        Assert.ThrowsExactly<ArgumentException>(() => logica.ObtenerHistorialEfectivo(0, "Empleado"));
    }

    // Historial de ingresos y retiros: solo para el administrador
    [TestMethod]
    public void ObtenerHistorialEfectivo_ComoEmpleado_Lanza()
    {
        var (logica, _) = Crear();

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.ObtenerHistorialEfectivo(IdEmpleadoDuenio, "Empleado"));
    }

    [TestMethod]
    public void ObtenerHistorialEfectivo_SinRol_Lanza()
    {
        var (logica, _) = Crear();

        Assert.ThrowsExactly<UnauthorizedAccessException>(() => logica.ObtenerHistorialEfectivo(IdEmpleadoDuenio, null));
    }

    [TestMethod]
    public void ObtenerHistorialEfectivo_ComoAdmin_NoLanza()
    {
        var (logica, _) = Crear();

        Assert.IsNotNull(logica.ObtenerHistorialEfectivo(IdAdmin, "Admin"));
    }

    // Retiros e ingresos que se muestran en el cierre de caja
    [TestMethod]
    public void ObtenerRetiros_DevuelveElTotalRetiradoEnElTurno()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(6000m, 2000m, 0m, 1500m);

        Assert.AreEqual(1500m, logica.ObtenerRetiros(1));
    }

    [TestMethod]
    public void ObtenerRetiros_SinRetiros_EsCero()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(5000m, 2000m, 0m, 0m);

        Assert.AreEqual(0m, logica.ObtenerRetiros(1));
    }

    [TestMethod]
    public void ObtenerIngresos_ExcluyeElFondoInicial()
    {
        var (logica, repo) = Crear();
        // El turno del fake abre con 1000; el repositorio informa apertura + ingresos = 2500
        repo.Datos = new EfectivoCajaDatos(2500m, 0m, 0m, 0m);

        Assert.AreEqual(1500m, logica.ObtenerIngresos(1));
    }

    [TestMethod]
    public void ObtenerIngresos_SinIngresos_EsCero()
    {
        var (logica, repo) = Crear();
        repo.Datos = new EfectivoCajaDatos(1000m, 0m, 0m, 0m);

        Assert.AreEqual(0m, logica.ObtenerIngresos(1));
    }
}