using System.Globalization;
using System.Text;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Exporta el reporte de ventas a CSV (US-25). No usa librerías: es texto plano.
// Formato pensado para abrirse con Excel en configuración regional argentina:
// separador ";" y decimales con coma (1258,40), y se guarda en UTF-8 con BOM para que se vean bien las tildes y la "ñ"
public static class ExportadorCsv
{
    public const char Separador = ';';

    private static readonly CultureInfo Es = new("es-AR");

    private static readonly string[] Encabezados =
    {
        "N.º venta", "Fecha", "Tipo", "Cliente", "Empleado", "Método de pago",
        "Unidades", "Total (con IVA)", "Neto (sin IVA)", "Costo", "Ganancia", "Cobro"
    };

    private static readonly string[] EncabezadosSubtotales =
    {
        "Subtotales", "Unidades", "Total (con IVA)", "Neto (sin IVA)", "Costo", "Ganancia"
    };

    // Arma el contenido completo del archivo. Está separado de Guardar para poder probarlo sin tocar el disco
    public static string Generar(IReadOnlyList<FilaReporteVentas> filas, ResumenReporteVentas resumen, string descripcionFiltros)
    {
        ArgumentNullException.ThrowIfNull(filas);
        ArgumentNullException.ThrowIfNull(resumen);

        var sb = new StringBuilder();

        AgregarLinea(sb, "Reporte de ventas");
        AgregarLinea(sb, Texto(descripcionFiltros));
        sb.AppendLine();

        AgregarLinea(sb, Encabezados);
        foreach (var f in filas)
        {
            AgregarLinea(sb,
                f.IdVenta.ToString(Es),
                f.Fecha.ToString("dd/MM/yyyy HH:mm", Es),
                Texto(f.Tipo),
                Texto(f.Cliente),
                Texto(f.Empleado),
                Texto(f.FormaPago),
                f.Unidades.ToString(Es),
                Dinero(f.Total),
                Dinero(f.Neto),
                Dinero(f.Costo),
                Dinero(f.Ganancia),
                Texto(f.EstadoCobro));
        }

        sb.AppendLine();
        AgregarLinea(sb, EncabezadosSubtotales);
        foreach (var s in resumen.Filas)
        {
            AgregarLinea(sb,
                Texto(s.Nombre),
                s.Unidades.ToString(Es),
                Dinero(s.Total),
                Dinero(s.Neto),
                Dinero(s.Costo),
                Dinero(s.Ganancia));
        }

        return sb.ToString();
    }

    public static void Guardar(string ruta, IReadOnlyList<FilaReporteVentas> filas, ResumenReporteVentas resumen, string descripcionFiltros)
    {
        if (string.IsNullOrWhiteSpace(ruta))
            throw new ArgumentException("Indique dónde guardar el archivo.");

        // UTF-8 con BOM: sin él, Excel muestra mal las tildes
        File.WriteAllText(ruta, Generar(filas, resumen, descripcionFiltros), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    // Importe con 2 decimales y coma, sin separador de miles (así Excel lo reconoce como número)
    private static string Dinero(decimal valor) => valor.ToString("0.00", Es);

    // Texto escrito por personas (clientes, empleados...): se entrecomilla si hace falta y se neutraliza si empieza como una fórmula,
    // porque Excel ejecutaría una celda que arranque con = + - @ (inyección de fórmulas)
    private static string Texto(string? valor)
    {
        string t = valor ?? string.Empty;

        if (t.Length > 0 && "=+-@\t\r".Contains(t[0]))
            t = "'" + t;

        if (t.IndexOfAny(new[] { Separador, '"', '\r', '\n' }) >= 0)
            t = "\"" + t.Replace("\"", "\"\"") + "\"";

        return t;
    }

    private static void AgregarLinea(StringBuilder sb, params string[] celdas)
        => sb.AppendLine(string.Join(Separador, celdas));
}