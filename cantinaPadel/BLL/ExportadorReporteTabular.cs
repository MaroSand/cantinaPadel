using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace cantinaPadel.BLL;

// Exportación común para los reportes de compras y caja.
public static class ExportadorReporteTabular
{
    private static readonly CultureInfo Es = new("es-AR");

    public static void GuardarCsv(string ruta, string titulo, string filtros, IReadOnlyList<string> encabezados,
        IReadOnlyList<object?[]> filas, IReadOnlyList<KeyValuePair<string, decimal>>? totales = null)
    {
        Validar(ruta, encabezados, filas);
        var sb = new StringBuilder();
        Linea(sb, titulo);
        Linea(sb, filtros);
        sb.AppendLine();
        Linea(sb, encabezados.ToArray());
        foreach (var fila in filas) Linea(sb, fila.Select(FormatearCsv).ToArray());
        if (totales is { Count: > 0 })
        {
            sb.AppendLine();
            Linea(sb, "Subtotales de caja");
            Linea(sb, "Concepto", "Importe");
            foreach (var total in totales) Linea(sb, total.Key, FormatearCsv(total.Value));
        }
        File.WriteAllText(ruta, sb.ToString(), new UTF8Encoding(true));
    }

    public static void GuardarExcel(string ruta, string titulo, string filtros, IReadOnlyList<string> encabezados,
        IReadOnlyList<object?[]> filas, IReadOnlyList<KeyValuePair<string, decimal>>? totales = null)
    {
        Validar(ruta, encabezados, filas);
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(titulo.Length > 31 ? titulo[..31] : titulo);
        hoja.Cell(1, 1).Value = titulo;
        hoja.Cell(1, 1).Style.Font.Bold = true;
        hoja.Cell(1, 1).Style.Font.FontSize = 14;
        hoja.Cell(2, 1).Value = filtros;
        for (var c = 0; c < encabezados.Count; c++) hoja.Cell(4, c + 1).Value = encabezados[c];
        var encabezado = hoja.Range(4, 1, 4, encabezados.Count);
        encabezado.Style.Font.Bold = true;
        encabezado.Style.Fill.BackgroundColor = XLColor.Gold;
        for (var r = 0; r < filas.Count; r++)
        for (var c = 0; c < encabezados.Count; c++)
        {
            var celda = hoja.Cell(r + 5, c + 1);
            var valor = filas[r][c];
            if (valor is DateTime fecha) celda.Value = fecha;
            else if (valor is decimal dinero) { celda.Value = dinero; celda.Style.NumberFormat.Format = "$ #,##0.00"; }
            else if (valor is int entero) celda.Value = entero;
            else celda.Value = Formatear(valor);
        }
        if (filas.Count > 0) hoja.Range(4, 1, filas.Count + 4, encabezados.Count).SetAutoFilter();
        if (totales is { Count: > 0 })
        {
            var filaTituloTotales = filas.Count + 6;
            var filaEncabezadoTotales = filaTituloTotales + 1;
            hoja.Cell(filaTituloTotales, 1).Value = "Subtotales de caja";
            hoja.Cell(filaTituloTotales, 1).Style.Font.Bold = true;
            hoja.Cell(filaTituloTotales, 1).Style.Font.FontSize = 12;
            hoja.Cell(filaEncabezadoTotales, 1).Value = "Concepto";
            hoja.Cell(filaEncabezadoTotales, 2).Value = "Importe";
            var encabezadoTotales = hoja.Range(filaEncabezadoTotales, 1, filaEncabezadoTotales, 2);
            encabezadoTotales.Style.Font.Bold = true;
            encabezadoTotales.Style.Fill.BackgroundColor = XLColor.Gold;

            for (var i = 0; i < totales.Count; i++)
            {
                var filaTotal = filaEncabezadoTotales + i + 1;
                hoja.Cell(filaTotal, 1).Value = totales[i].Key;
                hoja.Cell(filaTotal, 2).Value = totales[i].Value;
                hoja.Cell(filaTotal, 2).Style.NumberFormat.Format = "$ #,##0.00";
                if (i == totales.Count - 1)
                {
                    var totalGeneral = hoja.Range(filaTotal, 1, filaTotal, 2);
                    totalGeneral.Style.Font.Bold = true;
                    totalGeneral.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF2CC");
                }
            }
        }
        hoja.SheetView.FreezeRows(4);
        hoja.Columns().AdjustToContents();
        libro.SaveAs(ruta);
    }

    public static void GuardarPdf(string ruta, string titulo, string filtros, IReadOnlyList<string> encabezados,
        IReadOnlyList<object?[]> filas, IReadOnlyList<KeyValuePair<string, decimal>>? totales = null)
    {
        Validar(ruta, encabezados, filas);
        QuestPDF.Settings.License = LicenseType.Community;
        Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4.Landscape());
            pagina.Margin(24);
            pagina.DefaultTextStyle(t => t.FontSize(8));
            pagina.Header().Column(c =>
            {
                c.Item().Text(titulo).FontSize(15).Bold();
                c.Item().Text(filtros);
                c.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(7).FontColor(Colors.Grey.Darken1);
            });
            pagina.Content().PaddingVertical(10).Column(contenido =>
            {
                contenido.Item().Table(tabla =>
                {
                    tabla.ColumnsDefinition(cols => { for (var i = 0; i < encabezados.Count; i++) cols.RelativeColumn(); });
                    tabla.Header(h => { foreach (var nombre in encabezados) h.Cell().Background(Colors.Amber.Lighten2).Padding(4).Text(nombre).Bold(); });
                    foreach (var fila in filas)
                        foreach (var valor in fila)
                            tabla.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(Formatear(valor));
                });

                if (totales is { Count: > 0 })
                {
                    contenido.Item().PaddingTop(12).AlignRight().Width(300).Column(resumen =>
                    {
                        resumen.Item().Text("Subtotales de caja").FontSize(10).Bold();
                        resumen.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols => { cols.RelativeColumn(2); cols.RelativeColumn(); });
                            tabla.Header(h =>
                            {
                                h.Cell().Background(Colors.Amber.Lighten2).Padding(4).Text("Concepto").Bold();
                                h.Cell().Background(Colors.Amber.Lighten2).Padding(4).AlignRight().Text("Importe").Bold();
                            });
                            for (var i = 0; i < totales.Count; i++)
                            {
                                var esTotalGeneral = i == totales.Count - 1;
                                var concepto = tabla.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(totales[i].Key);
                                var importe = tabla.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignRight().Text(Formatear(totales[i].Value));
                                if (esTotalGeneral) { concepto.Bold(); importe.Bold(); }
                            }
                        });
                    });
                }
            });
            pagina.Footer().AlignCenter().Text(t => { t.Span("Página "); t.CurrentPageNumber(); t.Span(" de "); t.TotalPages(); });
        })).GeneratePdf(ruta);
    }

    private static string Formatear(object? valor) => valor switch
    {
        null => "—",
        DateTime fecha => fecha.ToString("dd/MM/yyyy HH:mm", Es),
        decimal dinero => dinero.ToString("C2", Es),
        _ => Convert.ToString(valor, Es) ?? string.Empty
    };

    private static string FormatearCsv(object? valor) => valor switch
    {
        decimal dinero => dinero.ToString("0.00", Es),
        DateTime fecha => fecha.ToString("dd/MM/yyyy HH:mm", Es),
        _ => Formatear(valor)
    };

    private static void Linea(StringBuilder sb, params string[] valores)
        => sb.AppendLine(string.Join(';', valores.Select(EscaparCsv)));

    private static string EscaparCsv(string valor)
    {
        if (valor.Length > 0 && "=+-@\t\r".Contains(valor[0])) valor = "'" + valor;
        return valor.IndexOfAny(new[] { ';', '"', '\r', '\n' }) >= 0 ? "\"" + valor.Replace("\"", "\"\"") + "\"" : valor;
    }

    private static void Validar(string ruta, IReadOnlyList<string> encabezados, IReadOnlyList<object?[]> filas)
    {
        if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Indique dónde guardar el archivo.");
        ArgumentNullException.ThrowIfNull(encabezados);
        ArgumentNullException.ThrowIfNull(filas);
        if (encabezados.Count == 0 || filas.Any(f => f.Length != encabezados.Count))
            throw new ArgumentException("Las filas del reporte no coinciden con sus encabezados.");
    }
}
