using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.BLL;

namespace cantinaPadel.UI;

public sealed class FrmHistorialCuentaCorriente : Form
{
    private static readonly CultureInfo Es = new("es-AR");

    public FrmHistorialCuentaCorriente(string nombreCliente, IReadOnlyList<MovimientoCuentaCorrienteReporte> movimientos)
    {
        Text = $"Historial de cuenta corriente - {nombreCliente}";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(1050, 620);
        MinimumSize = new Size(820, 460);
        Font = new Font("Segoe UI", 9F);
        BackColor = SystemColors.Info;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = SystemColors.Info };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

        var encabezado = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 8, 16, 6),
            BackColor = Color.FromArgb(255, 248, 220),
            ColumnCount = 1,
            RowCount = 2
        };
        encabezado.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        encabezado.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        encabezado.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        encabezado.Controls.Add(new Label
        {
            Text = "Historial de cuenta corriente",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        encabezado.Controls.Add(new Label
        {
            Text = $"{nombreCliente}  ·  {movimientos.Count} movimientos",
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            ForeColor = SystemColors.GrayText,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        var grilla = new DataGridView
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
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
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.Fecha),
            HeaderText = "Fecha y hora",
            FillWeight = 17,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.Tipo),
            HeaderText = "Movimiento",
            FillWeight = 10,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.Referencia),
            HeaderText = "Referencia",
            FillWeight = 13,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.MedioPago),
            HeaderText = "Medio de pago",
            FillWeight = 14,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.Monto),
            HeaderText = "Monto",
            FillWeight = 13,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.CreditoPosterior),
            HeaderText = "Crédito posterior",
            FillWeight = 16,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
        });
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(MovimientoCuentaCorrienteReporte.Responsable),
            HeaderText = "Responsable",
            FillWeight = 17,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        grilla.DataBindingComplete += (_, _) =>
        {
            foreach (DataGridViewRow fila in grilla.Rows)
            {
                if (fila.DataBoundItem is not MovimientoCuentaCorrienteReporte movimiento) continue;
                bool esCargo = movimiento.Tipo == "Cargo";
                fila.DefaultCellStyle.BackColor = esCargo ? Color.FromArgb(255, 238, 238) : Color.FromArgb(235, 247, 236);
                fila.DefaultCellStyle.SelectionBackColor = esCargo ? Color.FromArgb(244, 198, 198) : Color.FromArgb(195, 228, 199);
                fila.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 45, 35);
            }
        };
        PaginadorGrilla.Mostrar(grilla, movimientos);

        var pie = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 7, 12, 7), BackColor = SystemColors.Info };
        var cerrar = new Button { Text = "Cerrar", Size = new Size(110, 36), Dock = DockStyle.Right, BackColor = Color.White, FlatStyle = FlatStyle.Flat };
        pie.Controls.Add(cerrar);
        cerrar.Click += (_, _) => Close();

        layout.Controls.Add(encabezado, 0, 0);
        layout.Controls.Add(grilla, 0, 1);
        layout.Controls.Add(pie, 0, 2);
        Controls.Add(layout);
    }
}
