using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class ExportadorCsvTests
{
    private static FilaReporteVentas Fila(string cliente = "Lucas Fernández", decimal ganancia = 240m) => new()
    {
        IdVenta = 7,
        Fecha = new DateTime(2026, 10, 2, 18, 30, 0),
        Tipo = "Cantina",
        Cliente = cliente,
        Empleado = "Empleado Uno",
        FormaPago = "Efectivo",
        Pagado = true,
        Unidades = 2,
        Total = 2516.80m,
        Neto = 2080m,
        Costo = 1600m,
        Ganancia = ganancia
    };

    private static ResumenReporteVentas Resumen(params FilaReporteVentas[] filas) => CalculadorReporteVentas.CalcularResumen(filas);

    private static string[] Lineas(string csv) =>
        csv.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

    [TestMethod]
    public void Generar_IncluyeTituloFiltrosYEncabezados()
    {
        var fila = Fila();

        var lineas = Lineas(ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "Período: 01/10/2026 al 31/10/2026"));

        Assert.AreEqual("Reporte de ventas", lineas[0]);
        Assert.AreEqual("Período: 01/10/2026 al 31/10/2026", lineas[1]);
        StringAssert.Contains(lineas[3], "N.º venta;Fecha;Tipo;Cliente");
        StringAssert.Contains(lineas[3], "Ganancia;Cobro");
    }

    [TestMethod]
    public void Generar_FilaConFechaYDecimalesConComa()
    {
        var fila = Fila();

        var lineas = Lineas(ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f"));

        Assert.AreEqual("7;02/10/2026 18:30;Cantina;Lucas Fernández;Empleado Uno;Efectivo;2;2516,80;2080,00;1600,00;240,00;Cobrada", lineas[4]);
    }

    [TestMethod]
    public void Generar_GananciaNegativaNoSeAlteraNiSeConfundeConFormula()
    {
        var fila = Fila(ganancia: -50.5m);

        string csv = ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f");

        StringAssert.Contains(csv, ";-50,50;Cobrada");
        Assert.IsFalse(csv.Contains("'-50"));
    }

    [TestMethod]
    public void Generar_TextoConSeparadorOComillas_SeEntrecomilla()
    {
        var fila = Fila(cliente: "Pérez; \"el Gringo\"");

        string csv = ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f");

        StringAssert.Contains(csv, "\"Pérez; \"\"el Gringo\"\"\"");
    }

    [TestMethod]
    public void Generar_TextoQueEmpiezaComoFormula_SeNeutraliza()
    {
        var fila = Fila(cliente: "=CMD()");

        string csv = ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f");

        StringAssert.Contains(csv, ";'=CMD();");
        Assert.IsFalse(csv.Contains(";=CMD()"));
    }

    [TestMethod]
    public void Generar_IncluyeLosSubtotalesDePadelCantinaYTotal()
    {
        var fila = Fila();

        var lineas = Lineas(ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f"));
        int i = Array.FindIndex(lineas, l => l.StartsWith("Subtotales;"));

        Assert.IsTrue(i > 0);
        Assert.AreEqual("Pádel;0;0,00;0,00;0,00;0,00", lineas[i + 1]);
        Assert.AreEqual("Cantina;2;2516,80;2080,00;1600,00;240,00", lineas[i + 2]);
        Assert.AreEqual("Total;2;2516,80;2080,00;1600,00;240,00", lineas[i + 3]);
    }

    [TestMethod]
    public void Guardar_EscribeUtf8ConBomYElMismoContenido()
    {
        var fila = Fila();
        string ruta = Path.Combine(Path.GetTempPath(), $"ventas_{Guid.NewGuid():N}.csv");
        try
        {
            ExportadorCsv.Guardar(ruta, new[] { fila }, Resumen(fila), "f");

            byte[] bytes = File.ReadAllBytes(ruta);
            Assert.AreEqual((byte)0xEF, bytes[0]);
            Assert.AreEqual((byte)0xBB, bytes[1]);
            Assert.AreEqual((byte)0xBF, bytes[2]);
            Assert.AreEqual(ExportadorCsv.Generar(new[] { fila }, Resumen(fila), "f"), File.ReadAllText(ruta));
        }
        finally
        {
            File.Delete(ruta);
        }
    }

    [TestMethod]
    public void Guardar_RutaVacia_Lanza()
    {
        var fila = Fila();

        Assert.ThrowsExactly<ArgumentException>(() => ExportadorCsv.Guardar(" ", new[] { fila }, Resumen(fila), "f"));
    }
}