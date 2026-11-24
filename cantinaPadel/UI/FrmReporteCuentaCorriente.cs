using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.BLL;

namespace cantinaPadel.UI;

public sealed class FrmReporteCuentaCorriente : Form
{
    private const int FilasPorPagina = 20;
    private static readonly CultureInfo Es = new("es-AR");
    private readonly LogicaReporteCuentaCorriente _logica = new();
    private readonly TextBox _buscar = new();
    private readonly ComboBox _estado = new();
    private readonly Button _ordenar = new();
    private readonly DataGridView _grilla = new();
    private readonly Label _totalCobrar = new();
    private readonly Label _cantidadDeudores = new();
    private readonly Label _paginaTexto = new();
    private readonly Button _anterior = new();
    private readonly Button _siguiente = new();
    private List<ClienteCuentaCorrienteReporte> _clientes = new();
    private int _paginaActual;
    private bool _ordenDescendente = true;

    public FrmReporteCuentaCorriente()
    {
        Text = "Reporte de cuenta corriente";
        Font = new Font("Segoe UI", 9F);
        BackColor = SystemColors.Info;
        Construir();

        Load += (_, _) => CargarClientes();
        _buscar.TextChanged += (_, _) =>
        {
            _paginaActual = 0;
            ActualizarListado();
        };
        _estado.SelectedIndexChanged += (_, _) =>
        {
            _paginaActual = 0;
            ActualizarListado();
        };
        _ordenar.Click += (_, _) =>
        {
            _ordenDescendente = !_ordenDescendente;
            ActualizarIndicadorOrden();
            _paginaActual = 0;
            ActualizarListado();
        };
        _anterior.Click += (_, _) =>
        {
            if (_paginaActual == 0) return;
            _paginaActual--;
            ActualizarListado();
        };
        _siguiente.Click += (_, _) =>
        {
            _paginaActual++;
            ActualizarListado();
        };
        _grilla.CellDoubleClick += (_, e) => AbrirHistorial(e.RowIndex);
    }

    private void Construir()
    {
        var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, Padding = Padding.Empty, ColumnCount = 1, RowCount = 3, BackColor = SystemColors.Info };
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, ConvencionReportes.AltoEncabezado));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));

        var filtros = ConvencionReportes.CrearPanelFiltros();
        ConvencionReportes.AgregarTitulo(filtros, "Saldos de clientes");

        _buscar.PlaceholderText = "Nombre, apellido o DNI";
        ConvencionReportes.AgregarFiltro(filtros, 0, "Buscar cliente", _buscar, 4);
        _estado.DropDownStyle = ComboBoxStyle.DropDownList;
        _estado.Items.AddRange(new object[] { "Deudores", "Al día", "Todos" });
        _estado.SelectedIndex = 0;
        ConvencionReportes.AgregarFiltro(filtros, 4, "Estado de cuenta", _estado);
        ConvencionReportes.AgregarEtiquetaFiltro(filtros, 5, "Ordenar por monto");
        ConvencionReportes.ConfigurarAccionFiltro(filtros, _ordenar, "Ordenar");
        ActualizarIndicadorOrden();

        _grilla.Dock = DockStyle.Fill;
        ConvencionReportes.ConfigurarMargenGrilla(_grilla);
        _grilla.ReadOnly = true;
        _grilla.AllowUserToAddRows = false;
        _grilla.AllowUserToDeleteRows = false;
        _grilla.AllowUserToResizeRows = false;
        _grilla.MultiSelect = false;
        _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grilla.RowHeadersVisible = false;
        _grilla.AutoGenerateColumns = false;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grilla.RowTemplate.Height = 36;
        _grilla.ColumnHeadersHeight = 34;
        _grilla.BackgroundColor = Color.White;
        _grilla.GridColor = SystemColors.ControlLight;
        _grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "NombreCompleto",
            DataPropertyName = nameof(ClienteCuentaCorrienteReporte.NombreCompleto),
            HeaderText = "Cliente",
            FillWeight = 58,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Dni",
            DataPropertyName = nameof(ClienteCuentaCorrienteReporte.Dni),
            HeaderText = "DNI",
            FillWeight = 18,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
        _grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "MontoAdeudado",
            DataPropertyName = nameof(ClienteCuentaCorrienteReporte.MontoAdeudado),
            HeaderText = "Monto adeudado",
            FillWeight = 24,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "C2",
                FormatProvider = Es,
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });
        _grilla.DataBindingComplete += (_, _) => ColorearDeudores();

        var pie = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(12, 5, 12, 8),
            BackColor = SystemColors.Info,
            ColumnCount = 2,
            RowCount = 1
        };
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 315));
        pie.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var resumen = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 1, RowCount = 2 };
        resumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        resumen.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
        resumen.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        _cantidadDeudores.Dock = DockStyle.Fill;
        _cantidadDeudores.Margin = Padding.Empty;
        _cantidadDeudores.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _cantidadDeudores.ForeColor = SystemColors.GrayText;
        _cantidadDeudores.TextAlign = ContentAlignment.MiddleLeft;
        _totalCobrar.Dock = DockStyle.Fill;
        _totalCobrar.Margin = Padding.Empty;
        _totalCobrar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _totalCobrar.ForeColor = Color.FromArgb(45, 55, 45);
        _totalCobrar.TextAlign = ContentAlignment.MiddleLeft;
        resumen.Controls.Add(_cantidadDeudores, 0, 0);
        resumen.Controls.Add(_totalCobrar, 0, 1);

        var paginacion = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 3, RowCount = 1 };
        paginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 102));
        paginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        paginacion.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 102));
        paginacion.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        ConfigurarBotonPagina(_anterior, "‹ Anterior");
        ConfigurarBotonPagina(_siguiente, "Siguiente ›");
        _anterior.Dock = DockStyle.Fill;
        _siguiente.Dock = DockStyle.Fill;
        _paginaTexto.Dock = DockStyle.Fill;
        _paginaTexto.Margin = new Padding(2, 0, 2, 0);
        _paginaTexto.TextAlign = ContentAlignment.MiddleCenter;
        _paginaTexto.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        paginacion.Controls.Add(_anterior, 0, 0);
        paginacion.Controls.Add(_paginaTexto, 1, 0);
        paginacion.Controls.Add(_siguiente, 2, 0);
        pie.Controls.Add(resumen, 0, 0);
        pie.Controls.Add(paginacion, 1, 0);

        contenido.Controls.Add(filtros, 0, 0);
        contenido.Controls.Add(_grilla, 0, 1);
        contenido.Controls.Add(pie, 0, 2);
        Controls.Add(contenido);
    }

    private void CargarClientes()
    {
        try
        {
            _clientes = _logica.ObtenerClientesConActividad();
            var totalDeudores = _clientes.Where(c => c.MontoAdeudado > 0m).Sum(c => c.MontoAdeudado);
            _cantidadDeudores.Text = $"{_clientes.Count(c => c.MontoAdeudado > 0m)} deudores";
            _totalCobrar.Text = $"Total a cobrar: {totalDeudores.ToString("C2", Es)}";
            _paginaActual = 0;
            ActualizarListado();
        }
        catch (Exception ex)
        {
            _clientes.Clear();
            _grilla.DataSource = null;
            _cantidadDeudores.Text = "0 deudores";
            _totalCobrar.Text = $"Total a cobrar: {0m.ToString("C2", Es)}";
            ActualizarListado();
            MessageBox.Show(this, $"No se pudo cargar el reporte de cuenta corriente: {ex.Message}",
                "Reporte de cuenta corriente", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActualizarListado()
    {
        IEnumerable<ClienteCuentaCorrienteReporte> consulta = _clientes;
        var estado = _estado.SelectedItem?.ToString() ?? "Deudores";
        if (estado == "Deudores") consulta = consulta.Where(c => c.MontoAdeudado > 0m);
        else if (estado == "Al día") consulta = consulta.Where(c => c.MontoAdeudado <= 0m);

        var busqueda = _buscar.Text.Trim();
        if (!string.IsNullOrEmpty(busqueda))
            consulta = consulta.Where(c => c.NombreCompleto.Contains(busqueda, StringComparison.CurrentCultureIgnoreCase)
                || c.Dni.Contains(busqueda, StringComparison.CurrentCultureIgnoreCase));

        var registros = _ordenDescendente
            ? consulta.OrderByDescending(c => c.MontoAdeudado).ThenBy(c => c.NombreCompleto, StringComparer.CurrentCultureIgnoreCase).ToList()
            : consulta.OrderBy(c => c.MontoAdeudado).ThenBy(c => c.NombreCompleto, StringComparer.CurrentCultureIgnoreCase).ToList();

        var totalPaginas = Math.Max(1, (int)Math.Ceiling(registros.Count / (double)FilasPorPagina));
        _paginaActual = Math.Clamp(_paginaActual, 0, totalPaginas - 1);
        var pagina = registros.Skip(_paginaActual * FilasPorPagina).Take(FilasPorPagina).ToList();
        _grilla.DataSource = pagina;
        _paginaTexto.Text = $"{_paginaActual + 1}/{totalPaginas}";
        _anterior.Enabled = _paginaActual > 0;
        _siguiente.Enabled = _paginaActual < totalPaginas - 1;
    }

    private void ColorearDeudores()
    {
        foreach (DataGridViewRow fila in _grilla.Rows)
        {
            if (fila.DataBoundItem is not ClienteCuentaCorrienteReporte cliente) continue;
            bool adeuda = cliente.MontoAdeudado > 0m;
            fila.DefaultCellStyle.BackColor = adeuda ? Color.FromArgb(255, 232, 232) : Color.FromArgb(230, 246, 232);
            fila.DefaultCellStyle.ForeColor = adeuda ? Color.FromArgb(125, 35, 35) : Color.FromArgb(35, 100, 50);
            fila.DefaultCellStyle.SelectionBackColor = adeuda ? Color.FromArgb(244, 195, 195) : Color.FromArgb(190, 225, 195);
            fila.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 45, 35);
        }
    }

    private void AbrirHistorial(int indiceFila)
    {
        if (indiceFila < 0 || _grilla.Rows[indiceFila].DataBoundItem is not ClienteCuentaCorrienteReporte cliente) return;

        try
        {
            var movimientos = _logica.ObtenerMovimientos(cliente.IdCliente);
            using var historial = new FrmHistorialCuentaCorriente(cliente.NombreCompleto, movimientos);
            historial.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"No se pudo cargar el historial: {ex.Message}",
                "Historial de cuenta corriente", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActualizarIndicadorOrden()
    {
        _ordenar.Text = _ordenDescendente ? "↓ Monto" : "↑ Monto";
        _ordenar.AccessibleName = _ordenDescendente
            ? "Ordenar por monto descendente"
            : "Ordenar por monto ascendente";
    }

    private static void ConfigurarBotonPagina(Button boton, string texto)
    {
        boton.Text = texto;
        boton.Size = new Size(98, 30);
        boton.Anchor = AnchorStyles.None;
        boton.Margin = new Padding(2);
        boton.BackColor = SystemColors.Info;
        boton.ForeColor = SystemColors.GrayText;
        boton.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
    }
}
