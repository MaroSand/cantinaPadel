using ClosedXML.Excel;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Exporta el reporte de ventas a Excel (.xlsx) usando ClosedXML
// Los importes quedan como números (no como texto), así se pueden sumar, filtrar y ordenar en Excel
public static class ExportadorExcel
{
    private const string FormatoDinero = "$ #,##0.00";
    private const string FormatoFecha = "dd/mm/yyyy hh:mm";

    private static readonly string[] Encabezados =
    {
        "N.º venta", "Fecha", "Tipo", "Cliente", "Empleado", "Método de pago",
        "Unidades", "Total (con IVA)", "Neto (sin IVA)", "Costo", "Ganancia", "Cobro"
    };

    public static void Guardar(string ruta, IReadOnlyList<FilaReporteVentas> filas, ResumenReporteVentas resumen, string descripcionFiltros)
    {
        ArgumentNullException.ThrowIfNull(filas);
        ArgumentNullException.ThrowIfNull(resumen);

        if (string.IsNullOrWhiteSpace(ruta))
            throw new ArgumentException("Indique dónde guardar el archivo.");

        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Ventas");

        // Título y filtros aplicados
        hoja.Cell(1, 1).Value = "Reporte de ventas";
        hoja.Cell(1, 1).Style.Font.Bold = true;
        hoja.Cell(1, 1).Style.Font.FontSize = 14;
        hoja.Cell(2, 1).Value = descripcionFiltros;

        // Encabezados
        const int filaEncabezado = 4;
        for (int c = 0; c < Encabezados.Length; c++)
            hoja.Cell(filaEncabezado, c + 1).Value = Encabezados[c];

        var rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, Encabezados.Length);
        rangoEncabezado.Style.Font.Bold = true;
        rangoEncabezado.Style.Fill.BackgroundColor = XLColor.Gold;

        // Filas de datos
        int fila = filaEncabezado + 1;
        foreach (var f in filas)
        {
            hoja.Cell(fila, 1).Value = f.IdVenta;
            hoja.Cell(fila, 2).Value = f.Fecha;
            hoja.Cell(fila, 3).Value = f.Tipo;
            hoja.Cell(fila, 4).Value = f.Cliente;
            hoja.Cell(fila, 5).Value = f.Empleado;
            hoja.Cell(fila, 6).Value = f.FormaPago;
            hoja.Cell(fila, 7).Value = f.Unidades;
            hoja.Cell(fila, 8).Value = f.Total;
            hoja.Cell(fila, 9).Value = f.Neto;
            hoja.Cell(fila, 10).Value = f.Costo;
            hoja.Cell(fila, 11).Value = f.Ganancia;
            hoja.Cell(fila, 12).Value = f.EstadoCobro;
            fila++;
        }

        int ultimaFilaDatos = fila - 1;
        if (filas.Count > 0)
        {
            hoja.Range(filaEncabezado + 1, 2, ultimaFilaDatos, 2).Style.NumberFormat.Format = FormatoFecha;
            hoja.Range(filaEncabezado + 1, 8, ultimaFilaDatos, 11).Style.NumberFormat.Format = FormatoDinero;
            hoja.Range(filaEncabezado, 1, ultimaFilaDatos, Encabezados.Length).SetAutoFilter();
        }

        // Subtotales de pádel vs. cantina y total general, dos filas debajo de los datos
        int filaSubtotales = ultimaFilaDatos + 2;
        string[] encabezadosSubtotales = { "Subtotales", "Unidades", "Total (con IVA)", "Neto (sin IVA)", "Costo", "Ganancia" };
        // Las columnas de importes del resumen se alinean con las de la tabla de arriba: Unidades en la 7 y Total..Ganancia de la 8 a la 11
        int[] columnasSubtotales = { 1, 7, 8, 9, 10, 11 };
        for (int i = 0; i < encabezadosSubtotales.Length; i++)
            hoja.Cell(filaSubtotales, columnasSubtotales[i]).Value = encabezadosSubtotales[i];
        var encabezadoSubtotales = hoja.Range(filaSubtotales, 1, filaSubtotales, Encabezados.Length);
        encabezadoSubtotales.Style.Font.Bold = true;
        encabezadoSubtotales.Style.Fill.BackgroundColor = XLColor.Gold;

        int filaActual = filaSubtotales + 1;
        foreach (var s in resumen.Filas)
        {
            hoja.Cell(filaActual, 1).Value = s.Nombre;
            hoja.Cell(filaActual, 7).Value = s.Unidades;
            hoja.Cell(filaActual, 8).Value = s.Total;
            hoja.Cell(filaActual, 9).Value = s.Neto;
            hoja.Cell(filaActual, 10).Value = s.Costo;
            hoja.Cell(filaActual, 11).Value = s.Ganancia;
            filaActual++;
        }
        hoja.Range(filaSubtotales + 1, 8, filaActual - 1, 11).Style.NumberFormat.Format = FormatoDinero;
        // La última fila del resumen es el Total general
        hoja.Range(filaActual - 1, 1, filaActual - 1, Encabezados.Length).Style.Font.Bold = true;

        hoja.SheetView.FreezeRows(filaEncabezado);
        hoja.Columns().AdjustToContents();
        // El título y los filtros son largos y no deben ensanchar la primera columna
        hoja.Column(1).Width = 14;

        libro.SaveAs(ruta);
    }
}