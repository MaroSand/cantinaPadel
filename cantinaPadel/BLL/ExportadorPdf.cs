using cantinaPadel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace cantinaPadel.BLL;

// Exporta el reporte de ventas a PDF usando QuestPDF. Hoja A4 apaisada, con la tabla que se repite en cada página
public static class ExportadorPdf
{
    private static readonly System.Globalization.CultureInfo Es = new("es-AR");

    public static void Guardar(string ruta, IReadOnlyList<FilaReporteVentas> filas, ResumenReporteVentas resumen, string descripcionFiltros)
    {
        ArgumentNullException.ThrowIfNull(filas);
        ArgumentNullException.ThrowIfNull(resumen);

        if (string.IsNullOrWhiteSpace(ruta))
            throw new ArgumentException("Indique dónde guardar el archivo.");

        // QuestPDF es gratis para uso no comercial / organizaciones chicas bajo la licencia Community; hay que declararla
        QuestPDF.Settings.License = LicenseType.Community;

        string generado = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";

        Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4.Landscape());
                pagina.Margin(25);
                pagina.DefaultTextStyle(x => x.FontSize(9));

                pagina.Header().Column(columna =>
                {
                    columna.Item().Text("Reporte de ventas").FontSize(16).Bold();
                    columna.Item().Text(descripcionFiltros).FontSize(9);
                    columna.Item().Text(generado).FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                pagina.Content().PaddingVertical(10).Column(columna =>
                {
                    columna.Item().Element(c => TablaVentas(c, filas));
                    columna.Item().PaddingTop(15).Element(c => TablaSubtotales(c, resumen));
                });

                pagina.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Página ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        }).GeneratePdf(ruta);
    }

    private static void TablaVentas(IContainer contenedor, IReadOnlyList<FilaReporteVentas> filas)
    {
        contenedor.Table(tabla =>
        {
            tabla.ColumnsDefinition(c =>
            {
                c.ConstantColumn(40);   // N.º venta
                c.ConstantColumn(80);   // Fecha
                c.ConstantColumn(45);   // Tipo
                c.RelativeColumn(2);    // Cliente
                c.RelativeColumn(2);    // Empleado
                c.ConstantColumn(75);   // Método de pago
                c.ConstantColumn(35);   // Unidades
                c.ConstantColumn(65);   // Total
                c.ConstantColumn(65);   // Neto
                c.ConstantColumn(65);   // Costo
                c.ConstantColumn(65);   // Ganancia
                c.ConstantColumn(50);   // Cobro
            });

            tabla.Header(h =>
            {
                Encabezado(h, "N.º");
                Encabezado(h, "Fecha");
                Encabezado(h, "Tipo");
                Encabezado(h, "Cliente");
                Encabezado(h, "Empleado");
                Encabezado(h, "Método de pago");
                Encabezado(h, "Unid.", derecha: true);
                Encabezado(h, "Total (c/IVA)", derecha: true);
                Encabezado(h, "Neto", derecha: true);
                Encabezado(h, "Costo", derecha: true);
                Encabezado(h, "Ganancia", derecha: true);
                Encabezado(h, "Cobro");
            });

            foreach (var f in filas)
            {
                Celda(tabla, f.IdVenta.ToString(Es));
                Celda(tabla, f.Fecha.ToString("dd/MM/yyyy HH:mm", Es));
                Celda(tabla, f.Tipo);
                Celda(tabla, f.Cliente);
                Celda(tabla, f.Empleado);
                Celda(tabla, f.FormaPago);
                Celda(tabla, f.Unidades.ToString(Es), derecha: true);
                Celda(tabla, Dinero(f.Total), derecha: true);
                Celda(tabla, Dinero(f.Neto), derecha: true);
                Celda(tabla, Dinero(f.Costo), derecha: true);
                Celda(tabla, Dinero(f.Ganancia), derecha: true);
                Celda(tabla, f.EstadoCobro);
            }
        });
    }

    private static void TablaSubtotales(IContainer contenedor, ResumenReporteVentas resumen)
    {
        // Ancho fijo: es un cuadro chico, alineado a la derecha, no una tabla de página completa
        contenedor.AlignRight().Width(460).Table(tabla =>
        {
            tabla.ColumnsDefinition(c =>
            {
                c.RelativeColumn(2);
                c.ConstantColumn(40);
                c.ConstantColumn(70);
                c.ConstantColumn(70);
                c.ConstantColumn(70);
                c.ConstantColumn(70);
            });

            tabla.Header(h =>
            {
                Encabezado(h, "Subtotales");
                Encabezado(h, "Unid.", derecha: true);
                Encabezado(h, "Total (c/IVA)", derecha: true);
                Encabezado(h, "Neto", derecha: true);
                Encabezado(h, "Costo", derecha: true);
                Encabezado(h, "Ganancia", derecha: true);
            });

            var todas = resumen.Filas;
            for (int i = 0; i < todas.Count; i++)
            {
                var s = todas[i];
                bool esTotal = i == todas.Count - 1;
                Celda(tabla, s.Nombre, negrita: esTotal);
                Celda(tabla, s.Unidades.ToString(Es), derecha: true, negrita: esTotal);
                Celda(tabla, Dinero(s.Total), derecha: true, negrita: esTotal);
                Celda(tabla, Dinero(s.Neto), derecha: true, negrita: esTotal);
                Celda(tabla, Dinero(s.Costo), derecha: true, negrita: esTotal);
                Celda(tabla, Dinero(s.Ganancia), derecha: true, negrita: esTotal);
            }
        });
    }

    private static void Encabezado(TableCellDescriptor h, string texto, bool derecha = false)
    {
        IContainer celda = h.Cell().Background(Colors.Amber.Lighten2).Padding(4);
        if (derecha) celda = celda.AlignRight();
        celda.Text(texto).Bold();
    }

    private static void Celda(TableDescriptor tabla, string texto, bool derecha = false, bool negrita = false)
    {
        IContainer celda = tabla.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(4);
        if (derecha) celda = celda.AlignRight();
        var t = celda.Text(texto);
        if (negrita) t.Bold();
    }

    private static string Dinero(decimal valor) => valor.ToString("C2", Es);
}