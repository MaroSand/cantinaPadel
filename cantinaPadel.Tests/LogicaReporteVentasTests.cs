using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class LogicaReporteVentasTests
{
    // Repositorio falso: guarda con qué filtros se lo consultó y devuelve datos fijos
    private sealed class ReporteVentasFalso : IReporteVentasRepository
    {
        public DatosReporteVentas Datos { get; set; } = new();
        public int Consultas { get; private set; }
        public DateTime? Desde { get; private set; }
        public DateTime? Hasta { get; private set; }
        public string? FormaPago { get; private set; }
        public int? IdEmpleado { get; private set; }

        public DatosReporteVentas ObtenerDatos(DateTime desde, DateTime hasta, string? formaPago, int? idEmpleado)
        {
            Consultas++;
            Desde = desde;
            Hasta = hasta;
            FormaPago = formaPago;
            IdEmpleado = idEmpleado;
            return Datos;
        }
    }

    private sealed class EmpleadosFalso : IEmpleadoRepository
    {
        public List<Empleado> Lista { get; set; } = new();
        public Empleado? ObtenerPorCredenciales(string nombreUsuario, string contrasena) => null;
        public Empleado? ObtenerPorUsuario(string nombreUsuario) => null;
        public List<Empleado> ObtenerTodos() => Lista;
        public void Guardar(Empleado empleado) { }
        public bool ExisteUsuario(string nombreUsuario, int idEmpleadoExcluir) => false;
        public bool ExisteDni(string dni, int idPersonaExcluir) => false;
    }

    private static FiltroReporteVentas FiltroValido() => new()
    {
        Desde = new DateTime(2026, 10, 1),
        Hasta = new DateTime(2026, 10, 31)
    };

    private static (LogicaReporteVentas logica, ReporteVentasFalso repo, EmpleadosFalso empleados) Crear()
    {
        var repo = new ReporteVentasFalso();
        var empleados = new EmpleadosFalso();
        return (new LogicaReporteVentas(repo, empleados), repo, empleados);
    }

    // ---------- Validación ----------

    [TestMethod]
    public void Validar_FiltroCorrecto_NoLanzaExcepcion()
    {
        LogicaReporteVentas.Validar(FiltroValido());
    }

    [TestMethod]
    public void Validar_FiltroNulo_Lanza()
    {
        Assert.ThrowsExactly<ArgumentException>(() => LogicaReporteVentas.Validar(null!));
    }

    [TestMethod]
    public void Validar_DesdePosteriorAHasta_Lanza()
    {
        var filtro = FiltroValido();
        filtro.Desde = new DateTime(2026, 11, 1);

        var ex = Assert.ThrowsExactly<ArgumentException>(() => LogicaReporteVentas.Validar(filtro));
        StringAssert.Contains(ex.Message, "fecha inicial");
    }

    [TestMethod]
    public void Validar_MismoDiaConDistintaHora_EsValido()
    {
        var filtro = FiltroValido();
        filtro.Desde = new DateTime(2026, 10, 5, 22, 0, 0);
        filtro.Hasta = new DateTime(2026, 10, 5, 8, 0, 0);

        LogicaReporteVentas.Validar(filtro);
    }

    [TestMethod]
    public void Validar_FormaDePagoDesconocida_Lanza()
    {
        var filtro = FiltroValido();
        filtro.FormaPago = "Cheque";

        Assert.ThrowsExactly<ArgumentException>(() => LogicaReporteVentas.Validar(filtro));
    }

    [TestMethod]
    public void Validar_TodasLasFormasDePagoDelSistemaSonValidas()
    {
        foreach (var forma in new[] { "Efectivo", "Tarjeta", "Transferencia", "MercadoPago", "Cuenta Corriente" })
        {
            var filtro = FiltroValido();
            filtro.FormaPago = forma;
            LogicaReporteVentas.Validar(filtro);
        }
    }

    [TestMethod]
    public void Validar_EmpleadoInvalido_Lanza()
    {
        var filtro = FiltroValido();
        filtro.IdEmpleado = 0;

        Assert.ThrowsExactly<ArgumentException>(() => LogicaReporteVentas.Validar(filtro));
    }

    [TestMethod]
    public void Validar_TipoInexistente_Lanza()
    {
        var filtro = FiltroValido();
        filtro.Tipo = (TipoVentaReporte)99;

        Assert.ThrowsExactly<ArgumentException>(() => LogicaReporteVentas.Validar(filtro));
    }

    // ---------- Generar ----------

    [TestMethod]
    public void Generar_FiltroInvalido_NoConsultaLaBase()
    {
        var (logica, repo, _) = Crear();
        var filtro = FiltroValido();
        filtro.Desde = new DateTime(2026, 12, 1);

        Assert.ThrowsExactly<ArgumentException>(() => logica.Generar(filtro));
        Assert.AreEqual(0, repo.Consultas);
    }

    [TestMethod]
    public void Generar_PasaAlRepositorioLosFiltrosDeFechaPagoYEmpleado()
    {
        var (logica, repo, _) = Crear();
        var filtro = FiltroValido();
        filtro.FormaPago = "Tarjeta";
        filtro.IdEmpleado = 3;

        logica.Generar(filtro);

        Assert.AreEqual(new DateTime(2026, 10, 1), repo.Desde);
        Assert.AreEqual(new DateTime(2026, 10, 31), repo.Hasta);
        Assert.AreEqual("Tarjeta", repo.FormaPago);
        Assert.AreEqual(3, repo.IdEmpleado);
    }

    [TestMethod]
    public void Generar_AplicaElFiltroDeTipoSobreLosDatosLeidos()
    {
        var (logica, repo, _) = Crear();
        repo.Datos = new DatosReporteVentas
        {
            Ventas =
            {
                new Venta
                {
                    IdVenta = 1, FechaVenta = new DateTime(2026, 10, 2), IdCliente = 1, IdEmpleado = 1, FormaPago = "Efectivo",
                    Detalles =
                    {
                        new DetalleVenta { IdTurno = 1, PrecioUnitario = 5000m, Subtotal = 5000m },
                        new DetalleVenta { IdProducto = 1, PrecioUnitario = 1040m, Subtotal = 1040m }
                    }
                }
            },
            Clientes = { [1] = "Lucas Fernández" },
            Empleados = { [1] = "Empleado Uno" }
        };
        var filtro = FiltroValido();
        filtro.Tipo = TipoVentaReporte.Cantina;

        var filas = logica.Generar(filtro);

        Assert.HasCount(1, filas);
        Assert.AreEqual(1040m, filas[0].Total);
        Assert.AreEqual("Lucas Fernández", filas[0].Cliente);
    }

    // ---------- Empleados para el combo ----------

    [TestMethod]
    public void ObtenerEmpleados_IncluyeInactivosMarcadosYOrdenaPorNombre()
    {
        var (logica, _, empleados) = Crear();
        empleados.Lista = new List<Empleado>
        {
            new() { IdEmpleado = 2, NombreUsuario = "e2", Activo = false, Persona = new Persona { Nombre = "Zoe", Apellido = "Paz" } },
            new() { IdEmpleado = 1, NombreUsuario = "e1", Activo = true, Persona = new Persona { Nombre = "Ana", Apellido = "Ruiz" } }
        };

        var opciones = logica.ObtenerEmpleados();

        Assert.HasCount(2, opciones);
        Assert.AreEqual("Ana Ruiz", opciones[0].Texto);
        Assert.AreEqual(1, opciones[0].Id);
        Assert.AreEqual("Zoe Paz (inactivo)", opciones[1].Texto);
    }
}
