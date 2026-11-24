using cantinaPadel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace cantinaPadel.BLL;

internal static class GeneradorPdfComprobante
{
    private static readonly CultureInfo CulturaArgentina = CultureInfo.GetCultureInfo("es-AR");

    public static byte[] Generar(Comprobante comprobante, DatosVentaParaComprobante datos)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        bool esRemito = comprobante.Tipo == TipoComprobante.Remito;
        IReadOnlyList<DetalleComprobante> items = esRemito && datos.ItemsRemito.Count > 0
            ? datos.ItemsRemito
            : datos.Items;

        return Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4);
            pagina.Margin(40);
            pagina.DefaultTextStyle(estilo => estilo.FontSize(10));
            pagina.Header().Column(columna =>
            {
                columna.Item().Text("CANTINA PADEL").FontSize(10).FontColor(Colors.Grey.Darken1).Bold();
                columna.Item().PaddingTop(6).Text(LogicaComprobante.EtiquetaTipo(comprobante.Tipo).ToUpperInvariant()).FontSize(20).Bold();
                columna.Item().Text($"Comprobante Nº {comprobante.NumeroFormateado}").FontSize(12).Bold();
                columna.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
            pagina.Content().PaddingVertical(18).Column(columna =>
            {
                columna.Spacing(8);
                columna.Item().Text($"Fecha: {comprobante.FechaEmision:dd/MM/yyyy HH:mm}");
                columna.Item().Text($"Cliente: {datos.NombreCliente}");
                if (!string.IsNullOrWhiteSpace(datos.CuitCliente))
                    columna.Item().Text($"CUIT: {datos.CuitCliente}");

                columna.Item().PaddingTop(12).Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.RelativeColumn(5);
                        columnas.ConstantColumn(65);
                        if (!esRemito)
                        {
                            columnas.ConstantColumn(105);
                            columnas.ConstantColumn(105);
                        }
                    });
                    tabla.Header(encabezado =>
                    {
                        CeldaEncabezado(encabezado, "Descripción");
                        CeldaEncabezado(encabezado, "Cantidad", derecha: true);
                        if (!esRemito)
                        {
                            CeldaEncabezado(encabezado, "Precio unitario", derecha: true);
                            CeldaEncabezado(encabezado, "Subtotal", derecha: true);
                        }
                    });

                    foreach (var item in items)
                    {
                        Celda(tabla, item.Nombre);
                        Celda(tabla, item.Cantidad.ToString("N0", CulturaArgentina), derecha: true);
                        if (!esRemito)
                        {
                            Celda(tabla, item.PrecioUnitario.ToString("C2", CulturaArgentina), derecha: true);
                            Celda(tabla, item.Subtotal.ToString("C2", CulturaArgentina), derecha: true);
                        }
                    }
                });

                if (esRemito)
                {
                    decimal deuda = datos.DeudaEstimada > 0 ? datos.DeudaEstimada : datos.Total;
                    if (deuda > 0)
                        columna.Item().PaddingTop(12).AlignRight().Text($"Saldo estimado: {deuda.ToString("C2", CulturaArgentina)}").Bold();
                    if (datos.CreditoRestante > 0)
                        columna.Item().AlignRight().Text($"Pago a cuenta: {datos.CreditoRestante.ToString("C2", CulturaArgentina)}");
                    columna.Item().PaddingTop(8).Text("Los montos son estimativos y pueden cambiar por variaciones de precios.").FontSize(9).FontColor(Colors.Grey.Darken1);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(datos.MetodoPago))
                        columna.Item().PaddingTop(12).Text($"Método de pago: {datos.MetodoPago}");
                    columna.Item().AlignRight().Text($"TOTAL: {comprobante.Total.ToString("C2", CulturaArgentina)}").FontSize(15).Bold();
                    if (datos.PagoCon > 0)
                    {
                        columna.Item().AlignRight().Text($"Pagó con: {datos.PagoCon.ToString("C2", CulturaArgentina)}");
                        columna.Item().AlignRight().Text($"Vuelto: {datos.Vuelto.ToString("C2", CulturaArgentina)}");
                    }
                }
            });
            pagina.Footer().AlignCenter().Text("Comprobante generado por Cantina Padel").FontSize(8).FontColor(Colors.Grey.Darken1);
        })).GeneratePdf();
    }

    private static void CeldaEncabezado(TableCellDescriptor fila, string texto, bool derecha = false)
    {
        var celda = fila.Cell().Background(Colors.Grey.Lighten3).Padding(6);
        if (derecha) celda = celda.AlignRight();
        celda.Text(texto).Bold();
    }

    private static void Celda(TableDescriptor tabla, string texto, bool derecha = false)
    {
        var celda = tabla.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(6);
        if (derecha) celda = celda.AlignRight();
        celda.Text(texto);
    }
}
