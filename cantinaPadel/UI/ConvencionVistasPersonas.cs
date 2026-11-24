using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace cantinaPadel.UI;

internal static class ConvencionVistasPersonas
{
    private const int AltoEncabezado = 64;
    private const int AltoAcciones = 60;
    private const int AnchoBotonAccion = 170;
    private const int SeparacionAcciones = 16;

    public static void ConfigurarListado(
        Form formulario,
        Panel encabezado,
        Panel acciones,
        Label etiquetaBusqueda,
        TextBox campoBusqueda,
        Label etiquetaEstado,
        ComboBox filtroEstado,
        Button nuevo,
        Button modificar,
        Button cambiarEstado,
        DataGridView grilla,
        Button? cancelar = null)
    {
        formulario.ClientSize = new Size(1000, 600);
        encabezado.Dock = DockStyle.Top;
        encabezado.Height = AltoEncabezado;
        encabezado.BackColor = Color.Gold;
        encabezado.Margin = Padding.Empty;
        acciones.Dock = DockStyle.Bottom;
        acciones.Height = AltoAcciones;
        acciones.BackColor = SystemColors.Info;
        acciones.BorderStyle = BorderStyle.None;
        acciones.Margin = Padding.Empty;
        grilla.Dock = DockStyle.Fill;
        grilla.Margin = Padding.Empty;
        grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grilla.BackgroundColor = Color.White;
        grilla.BorderStyle = BorderStyle.None;
        grilla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grilla.GridColor = SystemColors.Menu;
        grilla.MultiSelect = false;
        grilla.ReadOnly = true;
        grilla.RowHeadersVisible = false;
        grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        etiquetaBusqueda.AutoSize = false;
        etiquetaBusqueda.Text = "Buscar:";
        etiquetaBusqueda.Location = new Point(24, 18);
        etiquetaBusqueda.Size = new Size(64, 28);
        etiquetaBusqueda.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        etiquetaBusqueda.TextAlign = ContentAlignment.MiddleLeft;
        etiquetaBusqueda.Anchor = AnchorStyles.Left | AnchorStyles.Top;

        campoBusqueda.Location = new Point(100, 18);
        campoBusqueda.Size = new Size(Math.Max(160, encabezado.ClientSize.Width - 340), 28);
        campoBusqueda.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        campoBusqueda.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

        etiquetaEstado.AutoSize = false;
        etiquetaEstado.Text = "Estado:";
        etiquetaEstado.Size = new Size(60, 28);
        etiquetaEstado.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        etiquetaEstado.TextAlign = ContentAlignment.MiddleLeft;
        etiquetaEstado.Location = new Point(encabezado.ClientSize.Width - 224, 18);
        etiquetaEstado.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        filtroEstado.Location = new Point(encabezado.ClientSize.Width - 156, 18);
        filtroEstado.Size = new Size(132, 28);
        filtroEstado.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        filtroEstado.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        void AcomodarEncabezado(object? sender, EventArgs e)
        {
            filtroEstado.Left = encabezado.ClientSize.Width - 156;
            etiquetaEstado.Left = filtroEstado.Left - etiquetaEstado.Width - 8;
            int limiteBusqueda = etiquetaEstado.Visible && filtroEstado.Visible
                ? etiquetaEstado.Left - 16
                : encabezado.ClientSize.Width - 24;
            campoBusqueda.Width = Math.Max(160, limiteBusqueda - campoBusqueda.Left);
        }

        void AcomodarAcciones(object? sender, EventArgs e)
        {
            var visibles = new[] { nuevo, modificar, cambiarEstado, cancelar }
                .Where(boton => boton?.Visible == true)
                .Cast<Button>()
                .ToArray();
            if (visibles.Length == 0) return;

            foreach (var boton in visibles)
            {
                boton.Size = new Size(AnchoBotonAccion, 38);
                boton.FlatStyle = FlatStyle.Flat;
                boton.BackColor = Color.White;
                boton.ForeColor = Color.Black;
                boton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
                boton.UseVisualStyleBackColor = false;
                boton.Margin = Padding.Empty;
                boton.Top = (acciones.ClientSize.Height - boton.Height) / 2;
                boton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            int anchoTotal = visibles.Sum(boton => boton.Width) + SeparacionAcciones * (visibles.Length - 1);
            int izquierda = Math.Max(16, (acciones.ClientSize.Width - anchoTotal) / 2);
            foreach (var boton in visibles)
            {
                boton.Left = izquierda;
                izquierda += boton.Width + SeparacionAcciones;
            }
        }

        encabezado.Resize += AcomodarEncabezado;
        acciones.Resize += AcomodarAcciones;
        nuevo.VisibleChanged += AcomodarAcciones;
        modificar.VisibleChanged += AcomodarAcciones;
        cambiarEstado.VisibleChanged += AcomodarAcciones;
        if (cancelar != null) cancelar.VisibleChanged += AcomodarAcciones;
        formulario.Shown += (_, _) => AcomodarAcciones(null, EventArgs.Empty);
        formulario.Shown += (_, _) => AcomodarEncabezado(null, EventArgs.Empty);
        AcomodarEncabezado(null, EventArgs.Empty);
        AcomodarAcciones(null, EventArgs.Empty);
    }
}
