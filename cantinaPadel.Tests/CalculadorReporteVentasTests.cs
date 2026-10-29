using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class CalculadorReporteVentasTests
{
    private static readonly HashSet<int> ProductosPadel = new() { 100 };
    // Costo actual por producto: la Coca (1) cuesta 800, el Gatorade (2) 1000, el agua (3) 400
    private static readonly Dictionary<int, decimal> Costos = new() { [1] = 800m, [2] = 1000m, [3] = 400m };
    private static readonly Dictionary<int, string> Clientes = new() { [1] = "Lucas Fernández", [2] = "Consumidor Final" };
    private static readonly Dictionary<int, string> Empleados = new() { [10] = "Empleado Uno", [11] = "Empleado Dos" };

    private static DetalleVenta Cantina(decimal precio, int idProducto = 1) =>
        new() { IdProducto = idProducto, PrecioUnitario = precio, Subtotal = precio };

    private static DetalleVenta Turno(decimal precio, int idTurno = 1) =>
        new() { IdTurno = idTurno, PrecioUnitario = precio, Subtotal = precio };

    private static Venta NuevaVenta(int id, DateTime fecha, string formaPago = "Efectivo", params DetalleVenta[] detalles) =>
        new()
        {
            IdVenta = id,
            FechaVenta = fecha,
            IdCliente = 1,
            IdEmpleado = 10,
            FormaPago = formaPago,
            Total = detalles.Sum(d => d.Subtotal),
            Detalles = detalles.ToList()
        };

    private static List<FilaReporteVentas> Armar(IEnumerable<Venta> ventas, TipoVentaReporte tipo = TipoVentaReporte.Todas) =>
        CalculadorReporteVentas.ArmarFilas(ventas, ProductosPadel, Costos, Clientes, Empleados, tipo);

    // ---------- Clasificación pádel / cantina ----------

    [TestMethod]
    public void EsDetallePadel_DetalleConTurno_EsPadel()
    {
        Assert.IsTrue(CalculadorReporteVentas.EsDetallePadel(Turno(5000m), ProductosPadel));
    }

    [TestMethod]
    public void EsDetallePadel_ProductoHoraPadel_EsPadel()
    {
        Assert.IsTrue(CalculadorReporteVentas.EsDetallePadel(Cantina(5000m, idProducto: 100), ProductosPadel));
    }

    [TestMethod]
    public void EsDetallePadel_ProductoComun_EsCantina()
    {
        Assert.IsFalse(CalculadorReporteVentas.EsDetallePadel(Cantina(1000m, idProducto: 1), ProductosPadel));
    }

    // ---------- Armado de filas ----------

    [TestMethod]
    public void ArmarFilas_UnidadesDelMismoTipoSeSumanEnUnaSolaFila()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(1040m), Cantina(1040m), Cantina(1300m, 2));

        var filas = Armar(new[] { venta });

        Assert.HasCount(1, filas);
        Assert.AreEqual(CalculadorReporteVentas.TipoCantina, filas[0].Tipo);
        Assert.AreEqual(3, filas[0].Unidades);
        Assert.AreEqual(3380m, filas[0].Total);
    }

    [TestMethod]
    public void ArmarFilas_VentaMixta_GeneraUnaFilaPorTipo()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Tarjeta", Turno(5000m), Cantina(1040m), Cantina(520m, 3));

        var filas = Armar(new[] { venta });

        Assert.HasCount(2, filas);
        var padel = filas.Single(f => f.Tipo == CalculadorReporteVentas.TipoPadel);
        var cantina = filas.Single(f => f.Tipo == CalculadorReporteVentas.TipoCantina);
        Assert.AreEqual(5000m, padel.Total);
        Assert.AreEqual(1, padel.Unidades);
        Assert.AreEqual(1560m, cantina.Total);
        Assert.AreEqual(2, cantina.Unidades);
        Assert.AreEqual(venta.Total, CalculadorReporteVentas.CalcularTotal(filas));
    }

    [TestMethod]
    public void ArmarFilas_CompletaClienteEmpleadoYFormaDePago()
    {
        var venta = NuevaVenta(7, new DateTime(2026, 10, 2), "Transferencia", Cantina(1000m));
        venta.IdCliente = 2;
        venta.IdEmpleado = 11;

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual(7, fila.IdVenta);
        Assert.AreEqual("Consumidor Final", fila.Cliente);
        Assert.AreEqual("Empleado Dos", fila.Empleado);
        Assert.AreEqual("Transferencia", fila.FormaPago);
    }

    [TestMethod]
    public void ArmarFilas_ClienteOEmpleadoDesconocido_MuestraGuion()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(1000m));
        venta.IdCliente = 99;
        venta.IdEmpleado = 99;

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual("—", fila.Cliente);
        Assert.AreEqual("—", fila.Empleado);
    }

    // ---------- Qué ventas cuentan ----------

    [TestMethod]
    public void ArmarFilas_IgnoraVentasAnuladas()
    {
        var anulada = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(1000m));
        anulada.Estado = "Anulada";
        var activa = NuevaVenta(2, new DateTime(2026, 10, 1), "Efectivo", Cantina(500m));

        var filas = Armar(new[] { anulada, activa });

        Assert.HasCount(1, filas);
        Assert.AreEqual(2, filas[0].IdVenta);
    }

    [TestMethod]
    public void ArmarFilas_IgnoraVentasHijas_CobrosDeCuentaCorriente()
    {
        // La venta original a Cuenta Corriente ya cuenta. El cobro posterior (venta hija) no debe duplicarla
        var original = NuevaVenta(1, new DateTime(2026, 10, 1), "Cuenta Corriente", Cantina(2000m));
        var cobro = NuevaVenta(2, new DateTime(2026, 10, 5), "Efectivo", Cantina(2000m));
        cobro.IdVentaPadre = 1;

        var filas = Armar(new[] { original, cobro });

        Assert.HasCount(1, filas);
        Assert.AreEqual("Cuenta Corriente", filas[0].FormaPago);
        Assert.AreEqual(2000m, CalculadorReporteVentas.CalcularTotal(filas));
    }

    [TestMethod]
    public void ArmarFilas_VentaDeCuentaCorrienteImpaga_QuedaPendiente()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Cuenta Corriente", Cantina(2000m));
        venta.Pagado = false;

        var fila = Armar(new[] { venta }).Single();

        Assert.IsFalse(fila.Pagado);
        Assert.AreEqual("Pendiente", fila.EstadoCobro);
    }

    [TestMethod]
    public void ArmarFilas_VentaSinDetalles_NoGeneraFilas()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo");

        Assert.HasCount(0, Armar(new[] { venta }));
    }

    // ---------- Filtro por tipo ----------

    [TestMethod]
    public void ArmarFilas_FiltroPadel_SoloDevuelveLasFilasDePadel()
    {
        var mixta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(5000m), Cantina(1040m));
        var soloCantina = NuevaVenta(2, new DateTime(2026, 10, 1), "Efectivo", Cantina(520m, 3));

        var filas = Armar(new[] { mixta, soloCantina }, TipoVentaReporte.Padel);

        Assert.HasCount(1, filas);
        Assert.AreEqual(1, filas[0].IdVenta);
        Assert.AreEqual(5000m, filas[0].Total);
    }

    [TestMethod]
    public void ArmarFilas_FiltroCantina_NoIncluyeLoQueEsPadelDeUnaVentaMixta()
    {
        var mixta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(5000m), Cantina(1040m));

        var filas = Armar(new[] { mixta }, TipoVentaReporte.Cantina);

        Assert.HasCount(1, filas);
        Assert.AreEqual(1040m, filas[0].Total);
    }

    [TestMethod]
    public void ArmarFilas_ProductoHoraPadelSeClasificaComoPadel()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(5000m, idProducto: 100));

        var filas = Armar(new[] { venta }, TipoVentaReporte.Padel);

        Assert.HasCount(1, filas);
    }

    // ---------- Orden y totales ----------

    [TestMethod]
    public void ArmarFilas_OrdenaDeLaMasRecienteALaMasAntigua()
    {
        var vieja = NuevaVenta(1, new DateTime(2026, 9, 1, 10, 0, 0), "Efectivo", Cantina(100m));
        var nueva = NuevaVenta(2, new DateTime(2026, 10, 1, 10, 0, 0), "Efectivo", Cantina(100m));
        var media = NuevaVenta(3, new DateTime(2026, 9, 15, 10, 0, 0), "Efectivo", Cantina(100m));

        var filas = Armar(new[] { vieja, nueva, media });

        CollectionAssert.AreEqual(new[] { 2, 3, 1 }, filas.Select(f => f.IdVenta).ToArray());
    }

    [TestMethod]
    public void CalcularTotal_SumaTodasLasFilas()
    {
        var filas = new List<FilaReporteVentas>
        {
            new() { Total = 1040.50m },
            new() { Total = 5000m },
            new() { Total = 0.25m }
        };

        Assert.AreEqual(6040.75m, CalculadorReporteVentas.CalcularTotal(filas));
    }

    [TestMethod]
    public void CalcularTotal_SinFilas_EsCero()
    {
        Assert.AreEqual(0m, CalculadorReporteVentas.CalcularTotal(new List<FilaReporteVentas>()));
    }

    // ---------- Ganancias ----------
    // Los precios de detalle incluyen IVA: una Coca de precio_venta 1040 sin IVA se vende a 1258,40 (costo 800)

    [TestMethod]
    public void CalcularNeto_SacaElIvaDelImporte()
    {
        Assert.AreEqual(1040m, CalculadorReporteVentas.CalcularNeto(1258.40m));
        Assert.AreEqual(0m, CalculadorReporteVentas.CalcularNeto(0m));
    }

    [TestMethod]
    public void ArmarFilas_Cantina_GananciaEsNetoMenosCosto()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(1258.40m, 1));

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual(1258.40m, fila.Total);
        Assert.AreEqual(1040m, fila.Neto);
        Assert.AreEqual(800m, fila.Costo);
        Assert.AreEqual(240m, fila.Ganancia);
    }

    [TestMethod]
    public void ArmarFilas_Cantina_CadaUnidadSumaSuCosto()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo",
            Cantina(1258.40m, 1), Cantina(1258.40m, 1), Cantina(1258.40m, 1));

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual(3775.20m, fila.Total);
        Assert.AreEqual(3120m, fila.Neto);
        Assert.AreEqual(2400m, fila.Costo);
        Assert.AreEqual(720m, fila.Ganancia);
    }

    [TestMethod]
    public void ArmarFilas_Padel_NoTieneCostoYLaGananciaEsElNeto()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(6050m));

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual(5000m, fila.Neto);
        Assert.AreEqual(0m, fila.Costo);
        Assert.AreEqual(5000m, fila.Ganancia);
    }

    [TestMethod]
    public void ArmarFilas_ProductoSinCostoCargado_CuentaCostoCero()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Cantina(1210m, idProducto: 99));

        var fila = Armar(new[] { venta }).Single();

        Assert.AreEqual(0m, fila.Costo);
        Assert.AreEqual(1000m, fila.Ganancia);
    }

    [TestMethod]
    public void ArmarFilas_VentaMixta_CadaTipoTieneSuPropiaGanancia()
    {
        var venta = NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(6050m), Cantina(1258.40m, 1));

        var filas = Armar(new[] { venta });

        Assert.AreEqual(5000m, filas.Single(f => f.Tipo == CalculadorReporteVentas.TipoPadel).Ganancia);
        Assert.AreEqual(240m, filas.Single(f => f.Tipo == CalculadorReporteVentas.TipoCantina).Ganancia);
    }

    // ---------- Resumen: subtotales pádel vs cantina ----------

    [TestMethod]
    public void CalcularResumen_SeparaPadelDeCantinaYSumaElGeneral()
    {
        var ventas = new[]
        {
            NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(6050m), Cantina(1258.40m, 1)),
            NuevaVenta(2, new DateTime(2026, 10, 2), "Tarjeta", Cantina(1258.40m, 1), Cantina(1258.40m, 1)),
            NuevaVenta(3, new DateTime(2026, 10, 3), "Efectivo", Turno(6050m))
        };

        var r = CalculadorReporteVentas.CalcularResumen(Armar(ventas));

        Assert.AreEqual(2, r.Padel.Unidades);
        Assert.AreEqual(12100m, r.Padel.Total);
        Assert.AreEqual(10000m, r.Padel.Neto);
        Assert.AreEqual(0m, r.Padel.Costo);
        Assert.AreEqual(10000m, r.Padel.Ganancia);

        Assert.AreEqual(3, r.Cantina.Unidades);
        Assert.AreEqual(3775.20m, r.Cantina.Total);
        Assert.AreEqual(3120m, r.Cantina.Neto);
        Assert.AreEqual(2400m, r.Cantina.Costo);
        Assert.AreEqual(720m, r.Cantina.Ganancia);

        Assert.AreEqual(5, r.General.Unidades);
        Assert.AreEqual(15875.20m, r.General.Total);
        Assert.AreEqual(10720m, r.General.Ganancia);
    }

    [TestMethod]
    public void CalcularResumen_ElGeneralEsLaSumaDeLosDosTipos()
    {
        var ventas = new[] { NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(6050m), Cantina(1258.40m, 1)) };

        var r = CalculadorReporteVentas.CalcularResumen(Armar(ventas));

        Assert.AreEqual(r.Padel.Total + r.Cantina.Total, r.General.Total);
        Assert.AreEqual(r.Padel.Neto + r.Cantina.Neto, r.General.Neto);
        Assert.AreEqual(r.Padel.Costo + r.Cantina.Costo, r.General.Costo);
        Assert.AreEqual(r.Padel.Ganancia + r.Cantina.Ganancia, r.General.Ganancia);
    }

    [TestMethod]
    public void CalcularResumen_ConFiltroDeTipo_ElOtroTipoQuedaEnCero()
    {
        var ventas = new[] { NuevaVenta(1, new DateTime(2026, 10, 1), "Efectivo", Turno(6050m), Cantina(1258.40m, 1)) };

        var r = CalculadorReporteVentas.CalcularResumen(Armar(ventas, TipoVentaReporte.Cantina));

        Assert.AreEqual(0m, r.Padel.Total);
        Assert.AreEqual(0, r.Padel.Unidades);
        Assert.AreEqual(240m, r.General.Ganancia);
    }

    [TestMethod]
    public void CalcularResumen_SinFilas_TodoEnCero()
    {
        var r = CalculadorReporteVentas.CalcularResumen(new List<FilaReporteVentas>());

        Assert.AreEqual(0m, r.General.Total);
        Assert.AreEqual(0m, r.General.Ganancia);
        Assert.HasCount(3, r.Filas);
    }
}
