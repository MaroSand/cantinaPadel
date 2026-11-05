using ClosedXML.Excel;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class ExportadoresVentasArchivoTests
{
    private static FilaReporteVentas Fila(string tipo, decimal total, decimal neto, decimal costo, decimal ganancia) => new()
    {
        IdVenta = tipo == CalculadorReporteVentas.TipoPadel ? 1 : 2,
        Fecha = new DateTime(2026, 10, 2, 18, 30, 0),
        Tipo = tipo,
        Cliente = "Lucas Fernández",
        Empleado = "Empleado Uno",
        FormaPago = "Efectivo",
        Pagado = true,
        Unidades = 1,
        Total = total,
        Neto = neto,
        Costo = costo,
        Ganancia = ganancia
    };

    private static (List<FilaReporteVentas> filas, ResumenReporteVentas resumen) Datos()
    {
        var filas = new List<FilaReporteVentas>
        {
            Fila(CalculadorReporteVentas.TipoPadel, 6050m, 5000m, 0m, 5000m),
            Fila(CalculadorReporteVentas.TipoCantina, 1258.40m, 1040m, 800m, 240m)
        };
        return (filas, CalculadorReporteVentas.CalcularResumen(filas));
    }

    [TestMethod]
    public void GuardarExcel_IncluyeFiltrosDetalleYSubtotalesConImportesNumericos()
    {
        var (filas, resumen) = Datos();
        var ruta = Path.Combine(Path.GetTempPath(), $"ventas_{Guid.NewGuid():N}.xlsx");

        try
        {
            ExportadorExcel.Guardar(ruta, filas, resumen, "Período: octubre 2026");

            using var libro = new XLWorkbook(ruta);
            var hoja = libro.Worksheet("Ventas");
            Assert.AreEqual("Reporte de ventas", hoja.Cell(1, 1).GetString());
            Assert.AreEqual("Período: octubre 2026", hoja.Cell(2, 1).GetString());
            Assert.AreEqual("Tipo", hoja.Cell(4, 3).GetString());
            Assert.AreEqual("Pádel", hoja.Cell(5, 3).GetString());
            Assert.AreEqual(6050m, hoja.Cell(5, 8).GetValue<decimal>());
            Assert.AreEqual(XLDataType.Number, hoja.Cell(5, 8).DataType);
            Assert.AreEqual("Pádel", hoja.Cell(9, 1).GetString());
            Assert.AreEqual("Cantina", hoja.Cell(10, 1).GetString());
            Assert.AreEqual("Total", hoja.Cell(11, 1).GetString());
            Assert.AreEqual(7308.40m, hoja.Cell(11, 8).GetValue<decimal>());
            Assert.AreEqual(5240m, hoja.Cell(11, 11).GetValue<decimal>());
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }

    [TestMethod]
    public void GuardarPdf_GeneraUnArchivoPdfNoVacio()
    {
        var (filas, resumen) = Datos();
        var ruta = Path.Combine(Path.GetTempPath(), $"ventas_{Guid.NewGuid():N}.pdf");

        try
        {
            ExportadorPdf.Guardar(ruta, filas, resumen, "Período: octubre 2026");

            var bytes = File.ReadAllBytes(ruta);
            Assert.IsTrue(bytes.Length > 500);
            Assert.AreEqual("%PDF-", System.Text.Encoding.ASCII.GetString(bytes, 0, 5));
        }
        finally
        {
            if (File.Exists(ruta)) File.Delete(ruta);
        }
    }
}
