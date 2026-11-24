using System.Drawing;
using System.Windows.Forms;

namespace cantinaPadel.UI;

internal static class ConvencionReportes
{
    public const int AltoEncabezado = 122;

    public static TableLayoutPanel CrearPanelFiltros()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            BackColor = Color.Gold,
            Padding = new Padding(12, 8, 12, 8),
            ColumnCount = 6,
            RowCount = 3
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        return panel;
    }

    public static void AgregarNavegacionMes(TableLayoutPanel panel, params Control[] controles)
    {
        var navegacion = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = Padding.Empty
        };
        navegacion.Controls.AddRange(controles);
        panel.Controls.Add(navegacion, 0, 0);
        panel.SetColumnSpan(navegacion, 6);
    }

    public static void AgregarTitulo(TableLayoutPanel panel, string texto)
    {
        var titulo = new Label
        {
            Text = texto,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 10.2F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        panel.Controls.Add(titulo, 0, 0);
        panel.SetColumnSpan(titulo, 6);
    }

    public static void AgregarFiltro(TableLayoutPanel panel, int columna, string titulo, Control control, int span = 1)
    {
        AgregarEtiquetaFiltro(panel, columna, titulo, span);
        control.Dock = DockStyle.Top;
        control.Margin = new Padding(0, 2, 6, 0);
        panel.Controls.Add(control, columna, 2);
        if (span > 1)
        {
            panel.SetColumnSpan(control, span);
        }
    }

    public static void AgregarEtiquetaFiltro(TableLayoutPanel panel, int columna, string titulo, int span = 1)
    {
        var etiqueta = new Label
        {
            Text = titulo,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        };
        panel.Controls.Add(etiqueta, columna, 1);
        if (span > 1)
            panel.SetColumnSpan(etiqueta, span);
    }

    public static void ConfigurarAccionFiltro(TableLayoutPanel panel, Button boton, string texto)
    {
        boton.Text = texto;
        boton.Dock = DockStyle.Fill;
        boton.Margin = new Padding(6, 0, 0, 0);
        boton.BackColor = Color.White;
        boton.FlatStyle = FlatStyle.Flat;
        boton.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
        panel.Controls.Add(boton, 5, 2);
    }

    public static void ConfigurarMargenGrilla(DataGridView grilla)
    {
        grilla.Margin = new Padding(6, 5, 6, 5);
    }

    public static void ConfigurarBotonExportar(Button boton, string texto, Color color)
    {
        boton.Text = texto;
        boton.Size = new Size(145, 36);
        boton.Margin = new Padding(0, 0, 8, 0);
        boton.BackColor = color;
        boton.ForeColor = Color.White;
        boton.FlatStyle = FlatStyle.Flat;
        boton.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
        boton.Enabled = false;
    }

    public static Button CrearBotonExportar(string texto, Color color)
    {
        var boton = new Button();
        ConfigurarBotonExportar(boton, texto, color);
        return boton;
    }

    public static FlowLayoutPanel CrearContenedorBotonesExportar()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 3, 0, 0),
            Margin = Padding.Empty
        };
    }
}
