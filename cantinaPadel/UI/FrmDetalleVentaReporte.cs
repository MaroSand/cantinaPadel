using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public sealed class FrmDetalleVentaReporte : Form
{
    private static readonly CultureInfo Es = new("es-AR");

    public FrmDetalleVentaReporte(FilaReporteVentas venta, IReadOnlyList<DetalleVentaReporte> detalles)
    {
        Text = $"Detalle de venta #{venta.IdVenta}";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(920, 590);
        MinimumSize = new Size(760, 440);
        Font = new Font("Segoe UI", 9F);
        BackColor = SystemColors.Info;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = SystemColors.Info
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 8, 16, 6),
            BackColor = Color.FromArgb(255, 248, 220),
            ColumnCount = 1,
            RowCount = 2
        };
        encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        encabezado.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        encabezado.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        encabezado.Controls.Add(new Label
        {
            Text = $"Detalle de venta #{venta.IdVenta}",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        encabezado.Controls.Add(new Label
        {
            Text = $"{venta.Cliente}  ·  {venta.Fecha:dd/MM/yyyy HH:mm}  ·  {venta.Empleado}  ·  {venta.FormaPago}",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = SystemColors.GrayText,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        var grilla = new DataGridView
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(12, 8, 12, 0),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowTemplate = { Height = 34 },
            ColumnHeadersHeight = 34,
            BackgroundColor = Color.White,
            GridColor = SystemColors.ControlLight
        };
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DetalleVentaReporte.Articulo),
            HeaderText = "Artículo",
            FillWeight = 45,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DetalleVentaReporte.PrecioUnitario),
            HeaderText = "Precio unitario",
            FillWeight = 18,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DetalleVentaReporte.Subtotal),
            HeaderText = "Subtotal",
            FillWeight = 18,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(DetalleVentaReporte.EstadoCobro),
            HeaderText = "Cobro",
            FillWeight = 14,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });

        var pie = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12, 7, 12, 7),
            BackColor = SystemColors.Info,
            ColumnCount = 2,
            RowCount = 1
        };
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        pie.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        string cantidadArticulos = detalles.Count == 1 ? "1 artículo" : $"{detalles.Count} artículos";
        pie.Controls.Add(new Label
        {
            Text = $"{cantidadArticulos}  ·  Total: {detalles.Sum(d => d.Subtotal).ToString("C2", Es)}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold)
        }, 0, 0);
        var cerrar = new Button
        {
            Text = "Cerrar",
            Size = new Size(110, 36),
            Dock = DockStyle.Right,
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        pie.Controls.Add(cerrar, 1, 0);
        cerrar.Click += (_, _) => Close();

        layout.Controls.Add(encabezado, 0, 0);
        layout.Controls.Add(grilla, 0, 1);
        layout.Controls.Add(pie, 0, 2);
        Controls.Add(layout);
        grilla.DataSource = detalles.ToList();
    }
}
