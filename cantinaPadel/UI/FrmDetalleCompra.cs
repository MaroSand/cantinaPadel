using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

// Detalle histórico de una compra. Se muestra únicamente en modo lectura.
public sealed class FrmDetalleCompra : Form
{
    private static readonly CultureInfo Es = new("es-AR");

    public FrmDetalleCompra(Compra compra)
    {
        ArgumentNullException.ThrowIfNull(compra);

        Text = $"Detalle de compra N° {compra.IdCompra}";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 440);
        Size = new Size(920, 590);
        BackColor = SystemColors.Info;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        var personaProveedor = compra.Proveedor?.Persona;
        string nombreProveedor = !string.IsNullOrWhiteSpace(compra.Proveedor?.NombreEmpresa)
            ? compra.Proveedor.NombreEmpresa
            : personaProveedor == null
                ? "Sin proveedor"
                : $"{personaProveedor.Apellido} {personaProveedor.Nombre}".Trim();
        var personaEmpleado = compra.Empleado?.Persona;
        string registradoPor = personaEmpleado == null
            ? "No disponible"
            : $"{personaEmpleado.Nombre} {personaEmpleado.Apellido}".Trim();

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
            Text = $"Detalle de compra #{compra.IdCompra}",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        encabezado.Controls.Add(new Label
        {
            Text = $"Proveedor: {nombreProveedor}  ·  Fecha: {compra.FechaCompra:dd/MM/yyyy HH:mm}  ·  Estado: {(compra.Anulada ? "Anulada" : "Activa")}\n" +
                   $"Comprobante: {compra.NumeroComprobante ?? "-"}  ·  Registrada por: {registradoPor}",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = SystemColors.GrayText,
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        }, 0, 1);

        var grilla = new DataGridView
        {
            Dock = DockStyle.Fill,
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
            GridColor = SystemColors.ControlLight,
            EditMode = DataGridViewEditMode.EditProgrammatically
        };
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = "Producto", HeaderText = "Producto", FillWeight = 32 });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = "Codigo", HeaderText = "Código", FillWeight = 18 });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        { DataPropertyName = "Cantidad", HeaderText = "Cantidad", FillWeight = 12 });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Costo",
            HeaderText = "Costo unitario",
            FillWeight = 19,
            DefaultCellStyle = new DataGridViewCellStyle
            { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Subtotal",
            HeaderText = "Subtotal",
            FillWeight = 19,
            DefaultCellStyle = new DataGridViewCellStyle
            { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.DataSource = compra.Detalles.Select(d => new
        {
            Producto = d.Producto?.Nombre ?? $"Producto #{d.IdProducto}",
            Codigo = d.Producto?.CodigoBarras ?? "-",
            Cantidad = d.Cantidad.ToString("N0", Es),
            Costo = d.PrecioUnitario,
            d.Subtotal
        }).ToList();

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
        string cantidadProductos = compra.Detalles.Count == 1 ? "1 producto" : $"{compra.Detalles.Count} productos";
        pie.Controls.Add(new Label
        {
            Text = $"{cantidadProductos}  ·  Total: {compra.Total.ToString("C2", Es)}",
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
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel
        };
        CancelButton = cerrar;
        pie.Controls.Add(cerrar, 1, 0);
        cerrar.Click += (_, _) => Close();

        grilla.Margin = new Padding(12, 8, 12, 0);
        layout.Controls.Add(encabezado, 0, 0);
        layout.Controls.Add(grilla, 0, 1);
        layout.Controls.Add(pie, 0, 2);
        Controls.Add(layout);
    }
}
