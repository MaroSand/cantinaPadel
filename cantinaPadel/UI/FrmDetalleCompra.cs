using System.Globalization;
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
        MinimumSize = new Size(760, 480);
        Size = new Size(960, 620);
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

        var encabezado = new Label
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18, 12, 18, 12),
            BackColor = Color.Gold,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = $"Proveedor: {nombreProveedor}     ·     Fecha: {compra.FechaCompra:dd/MM/yyyy HH:mm}\n" +
                   $"Comprobante: {compra.NumeroComprobante ?? "-"}     ·     Registrada por: {registradoPor}\n" +
                   $"Estado: {(compra.Anulada ? "Anulada" : "Activa")}",
            AutoEllipsis = true
        };

        var grilla = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White,
            GridColor = SystemColors.Menu,
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

        var total = new Label
        {
            AutoSize = true,
            Text = $"Total: {compra.Total.ToString("C2", Es)}",
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight,
            Margin = new Padding(12)
        };
        var cerrar = new Button
        {
            Text = "Cerrar",
            Size = new Size(130, 38),
            BackColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel,
            Margin = new Padding(8)
        };
        CancelButton = cerrar;

        var pie = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(8),
            BackColor = SystemColors.Info
        };
        pie.Controls.Add(cerrar);
        pie.Controls.Add(total);

        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10),
            BackColor = SystemColors.Info
        };
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
        contenido.Controls.Add(encabezado, 0, 0);
        contenido.Controls.Add(grilla, 0, 1);
        contenido.Controls.Add(pie, 0, 2);
        Controls.Add(contenido);
    }
}
