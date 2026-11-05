using ClosedXML.Excel;
using cantinaPadel.BLL;

namespace cantinaPadel.Tests;

[TestClass]
public class ExportadorReporteTabularTests
{
    private static readonly string[] Encabezados = { "N.º", "Fecha", "Empleado", "Importe" };

    private static List<object?[]> Filas() => new()
    {
        new object?[] { 12, new DateTime(2026, 10, 2, 18, 30, 0), "=HIPERVINCULO()", 1234.50m }
    };

    private static List<KeyValuePair<string, decimal>> Totales() => new()
    {
        new("Efectivo contado", 1234.50m),
        new("Total general", 1234.50m)
    };

    [TestMethod]
    public void GuardarCsv_IncluyeDatosTotalesBomYNeutralizaTextoConFormula()
    {
        var ruta = Ruta("csv");

        try
        {
            ExportadorReporteTabular.GuardarCsv(ruta, "Reporte de cajas", "Período: octubre 2026", Encabezados, Filas(), Totales());

            var bytes = File.ReadAllBytes(ruta);
            var contenido = File.ReadAllText(ruta);
            Assert.AreEqual((byte)0xEF, bytes[0]);
            Assert.AreEqual((byte)0xBB, bytes[1]);
            Assert.AreEqual((byte)0xBF, bytes[2]);
            StringAssert.Contains(contenido, "Reporte de cajas");
            StringAssert.Contains(contenido, "12;02/10/2026 18:30;'=HIPERVINCULO();1234,50");
            StringAssert.Contains(contenido, "Total general;1234,50");
        }
        finally { Borrar(ruta); }
    }

    [TestMethod]
    public void GuardarExcel_MantieneLosImportesComoNumerosYAgregaTotales()
    {
        var ruta = Ruta("xlsx");

        try
        {
            ExportadorReporteTabular.GuardarExcel(ruta, "Reporte de compras", "Proveedor: demo", Encabezados, Filas(), Totales());

            using var libro = new XLWorkbook(ruta);
            var hoja = libro.Worksheet("Reporte de compras");
            Assert.AreEqual("Proveedor: demo", hoja.Cell(2, 1).GetString());
            Assert.AreEqual("Empleado", hoja.Cell(4, 3).GetString());
            Assert.AreEqual("=HIPERVINCULO()", hoja.Cell(5, 3).GetString());
            Assert.AreEqual(1234.50m, hoja.Cell(5, 4).GetValue<decimal>());
            Assert.AreEqual(XLDataType.Number, hoja.Cell(5, 4).DataType);
            Assert.AreEqual("Total general", hoja.Cell(10, 1).GetString());
            Assert.AreEqual(1234.50m, hoja.Cell(10, 2).GetValue<decimal>());
            Assert.AreEqual(XLDataType.Number, hoja.Cell(10, 2).DataType);
        }
        finally { Borrar(ruta); }
    }

    [TestMethod]
    public void GuardarPdf_GeneraUnArchivoPdfNoVacio()
    {
        var ruta = Ruta("pdf");

        try
        {
            ExportadorReporteTabular.GuardarPdf(ruta, "Reporte de cajas", "Período: octubre 2026", Encabezados, Filas(), Totales());

            var bytes = File.ReadAllBytes(ruta);
            Assert.IsTrue(bytes.Length > 500);
            Assert.AreEqual("%PDF-", System.Text.Encoding.ASCII.GetString(bytes, 0, 5));
        }
        finally { Borrar(ruta); }
    }

    private static string Ruta(string extension) => Path.Combine(Path.GetTempPath(), $"reporte_{Guid.NewGuid():N}.{extension}");

    private static void Borrar(string ruta)
    {
        if (File.Exists(ruta)) File.Delete(ruta);
    }
}
