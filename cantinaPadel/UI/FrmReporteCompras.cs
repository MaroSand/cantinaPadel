using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public sealed class FrmReporteCompras : Form
{
    private static readonly CultureInfo Es = new("es-AR");
    private enum Formato { Csv, Excel, Pdf }
    private readonly LogicaCompra _logica = new();
    private readonly System.Windows.Forms.Timer _debounce = new() { Interval = 300 };
    private readonly DateTimePicker _desde = new();
    private readonly DateTimePicker _hasta = new();
    private readonly Button _mesAnterior = new() { Text = "‹ Mes anterior", Width = 125, Height = 27 };
    private readonly Button _mesSiguiente = new() { Text = "Mes siguiente ›", Width = 125, Height = 27 };
    private readonly Label _mesSeleccionado = new() { AutoSize = false, Width = 150, Height = 27, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
    private readonly ComboBox _proveedor = new();
    private readonly TextBox _producto = new();
    private readonly DataGridView _grilla = new();
    private readonly Label _resumen = new();
    private readonly Button _csv = Boton("Exportar CSV", Color.SteelBlue);
    private readonly Button _excel = Boton("Exportar Excel", Color.ForestGreen);
    private readonly Button _pdf = Boton("Exportar PDF", Color.IndianRed);
    private readonly Button _limpiar = new();
    private List<Compra> _compras = new();
    private string _filtros = string.Empty;
    private static readonly string[] Encabezados = { "N.º compra", "Fecha", "Proveedor", "Productos", "Unidades", "Comprobante", "Empleado", "Total" };

    private sealed record FiltroProveedor(int? Id, string Nombre) { public override string ToString() => Nombre; }

    public FrmReporteCompras()
    {
        Text = "Reporte de Compras";
        Font = new Font("Segoe UI", 9F);
        BackColor = SystemColors.Info;
        Construir();
        Load += (_, _) =>
        {
            _desde.CustomFormat = "d/M/yyyy";
            _hasta.CustomFormat = "d/M/yyyy";
            _desde.Format = DateTimePickerFormat.Custom;
            _hasta.Format = DateTimePickerFormat.Custom;
            _desde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _hasta.Value = DateTime.Today;
            ActualizarNavegacionMes();
            try
            {
                _proveedor.Items.Add(new FiltroProveedor(null, "Todos los proveedores"));
                _proveedor.Items.AddRange(_logica.ObtenerTodosLosProveedores().Select(p => new FiltroProveedor(p.IdProveedor, p.Texto)).Cast<object>().ToArray());
                _proveedor.SelectedIndex = 0;
                Buscar();
                _desde.ValueChanged += (_, _) => { ActualizarNavegacionMes(); ProgramarBusqueda(); };
                _hasta.ValueChanged += (_, _) => { ActualizarNavegacionMes(); ProgramarBusqueda(); };
                _proveedor.SelectedIndexChanged += (_, _) => ProgramarBusqueda();
                _producto.TextChanged += (_, _) => ProgramarBusqueda();
            }
            catch (Exception ex) { MostrarError("No se pudieron cargar los proveedores", ex); }
        };
        _limpiar.Click += (_, _) =>
        {
            _desde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _hasta.Value = DateTime.Today;
            _producto.Clear();
            if (_proveedor.Items.Count > 0) _proveedor.SelectedIndex = 0;
            ActualizarNavegacionMes();
            _debounce.Stop();
            Buscar();
        };
        _mesAnterior.Click += (_, _) => NavegarMes(-1);
        _mesSiguiente.Click += (_, _) => NavegarMes(1);
        _csv.Click += (_, _) => Exportar(Formato.Csv);
        _excel.Click += (_, _) => Exportar(Formato.Excel);
        _pdf.Click += (_, _) => Exportar(Formato.Pdf);
        _debounce.Tick += (_, _) => { _debounce.Stop(); Buscar(); };
        Disposed += (_, _) => _debounce.Dispose();
    }

    private void Construir()
    {
        var filtros = new TableLayoutPanel { Dock = DockStyle.Top, Height = 110, BackColor = Color.Gold, Padding = new Padding(12, 7, 12, 7), ColumnCount = 5, RowCount = 3 };
        foreach (var ancho in new[] { 16f, 16f, 27f, 25f, 16f }) filtros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, ancho));
        filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); filtros.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var navegacion = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Margin = Padding.Empty };
        navegacion.Controls.AddRange(new Control[] { _mesAnterior, _mesSeleccionado, _mesSiguiente });
        filtros.Controls.Add(navegacion, 0, 0); filtros.SetColumnSpan(navegacion, 5);
        AgregarFiltro(filtros, 0, "Desde", _desde, 1, 2); AgregarFiltro(filtros, 1, "Hasta", _hasta, 1, 2);
        _proveedor.DropDownStyle = ComboBoxStyle.DropDownList;
        AgregarFiltro(filtros, 2, "Proveedor", _proveedor, 1, 2); AgregarFiltro(filtros, 3, "Producto o código", _producto, 1, 2);
        _limpiar.Text = "Limpiar"; _limpiar.Dock = DockStyle.Fill; _limpiar.BackColor = Color.White; _limpiar.FlatStyle = FlatStyle.Flat;
        filtros.Controls.Add(_limpiar, 4, 2);

        _grilla.Dock = DockStyle.Fill; _grilla.ReadOnly = true; _grilla.AllowUserToAddRows = false; _grilla.AllowUserToDeleteRows = false;
        _grilla.MultiSelect = false; _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect; _grilla.RowHeadersVisible = false;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grilla.RowTemplate.Height = 30; _grilla.ColumnHeadersHeight = 34;
        _grilla.BackgroundColor = Color.White; _grilla.GridColor = SystemColors.ControlLight; _grilla.AutoGenerateColumns = false;
        AddCol("IdCompra", "N.º compra", 8); AddCol("Fecha", "Fecha", 14, "dd/MM/yyyy HH:mm"); AddCol("Proveedor", "Proveedor", 17);
        AddCol("Productos", "Productos", 26); AddCol("Unidades", "Unid.", 7); AddCol("Comprobante", "Comprobante", 11);
        AddCol("Empleado", "Empleado", 11); AddCol("Total", "Total", 12, "C2");

        var pie = new Panel { Dock = DockStyle.Bottom, Height = 62, Padding = new Padding(12, 8, 12, 6), BackColor = SystemColors.Info };
        var botones = new FlowLayoutPanel { Dock = DockStyle.Left, Width = 470, WrapContents = false };
        botones.Controls.AddRange(new Control[] { _csv, _excel, _pdf });
        _resumen.Dock = DockStyle.Fill; _resumen.TextAlign = ContentAlignment.MiddleRight; _resumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        pie.Controls.Add(_resumen); pie.Controls.Add(botones);
        Controls.Add(_grilla); Controls.Add(pie); Controls.Add(filtros);
    }

    private void AddCol(string name, string title, float weight, string? format = null)
    {
        var col = new DataGridViewTextBoxColumn { Name = name, DataPropertyName = name, HeaderText = title, FillWeight = weight };
        if (format != null) col.DefaultCellStyle = new DataGridViewCellStyle { Format = format, FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight };
        _grilla.Columns.Add(col);
    }

    private static void AgregarFiltro(TableLayoutPanel panel, int col, string titulo, Control control, int filaTitulo = 0, int filaControl = 1)
    {
        panel.Controls.Add(new Label { Text = titulo, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft, Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, col, filaTitulo);
        control.Dock = DockStyle.Fill; control.Margin = new Padding(0, 2, 6, 0); panel.Controls.Add(control, col, filaControl);
    }

    private void NavegarMes(int desplazamiento)
    {
        var mes = new DateTime(_desde.Value.Year, _desde.Value.Month, 1).AddMonths(desplazamiento);
        var hoy = DateTime.Today;
        var hasta = mes.Year == hoy.Year && mes.Month == hoy.Month ? hoy : mes.AddMonths(1).AddDays(-1);
        _desde.Value = mes;
        _hasta.Value = hasta;
        _debounce.Stop();
        ActualizarNavegacionMes();
        Buscar();
    }

    private void ActualizarNavegacionMes()
    {
        var mes = new DateTime(_desde.Value.Year, _desde.Value.Month, 1);
        _mesSeleccionado.Text = mes.ToString("MMMM yyyy", Es);
        var hoy = DateTime.Today;
        _mesSiguiente.Enabled = mes < new DateTime(hoy.Year, hoy.Month, 1);
    }

    private void Buscar()
    {
        try
        {
            var desde = _desde.Value.Date; var hasta = _hasta.Value.Date;
            if (desde > hasta) throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");
            var proveedor = _proveedor.SelectedItem as FiltroProveedor;
            _compras = _logica.Buscar(proveedor?.Id, desde, hasta, _producto.Text.Trim());
            var filas = _compras.Select(c => new FilaCompra
            {
                IdCompra = c.IdCompra, Fecha = c.FechaCompra, Proveedor = c.Proveedor?.NombreEmpresa ?? "—",
                Productos = string.Join(", ", c.Detalles.Select(d => d.Producto?.Nombre ?? $"Producto {d.IdProducto}")),
                Unidades = c.Detalles.Sum(d => d.Cantidad), Comprobante = c.NumeroComprobante ?? "—",
                Empleado = $"{c.Empleado?.Persona?.Nombre} {c.Empleado?.Persona?.Apellido}".Trim(), Total = c.Total
            }).ToList();
            PaginadorGrilla.Mostrar(_grilla, filas);
            _filtros = $"Período: {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}  |  Proveedor: {proveedor?.Nombre ?? "Todos"}  |  Producto: {(string.IsNullOrWhiteSpace(_producto.Text) ? "Todos" : _producto.Text.Trim())}";
            var total = _compras.Sum(c => c.Total);
            var desdeAnterior = desde.AddMonths(-1);
            var hastaAnterior = hasta.AddMonths(-1);
            try
            {
                var comprasAnteriores = _logica.Buscar(proveedor?.Id, desdeAnterior, hastaAnterior, _producto.Text.Trim());
                var totalAnterior = comprasAnteriores.Sum(c => c.Total);
                var diferencia = total - totalAnterior;
                var signo = diferencia > 0m ? "+" : string.Empty;
                _resumen.Text = $"{_compras.Count} compras  |  Total: {total.ToString("C2", Es)}\n" +
                    $"Período anterior ({desdeAnterior:dd/MM} al {hastaAnterior:dd/MM}): {totalAnterior.ToString("C2", Es)}  |  Variación: {signo}{diferencia.ToString("C2", Es)}";
            }
            catch
            {
                _resumen.Text = $"{_compras.Count} compras  |  Total del período: {total.ToString("C2", Es)}\nNo se pudo cargar la comparación con el período anterior.";
            }
            HabilitarExportacion(_compras.Count > 0);
        }
        catch (Exception ex) { _compras.Clear(); PaginadorGrilla.Limpiar(_grilla); _resumen.Text = ex.Message; HabilitarExportacion(false); }
    }

    private void ProgramarBusqueda() { _debounce.Stop(); _debounce.Start(); }

    private void Exportar(Formato formato)
    {
        if (_compras.Count == 0) return;
        var (extension, tipo) = formato switch { Formato.Csv => ("csv", "Archivo CSV (*.csv)"), Formato.Excel => ("xlsx", "Libro de Excel (*.xlsx)"), _ => ("pdf", "Documento PDF (*.pdf)") };
        using var dialogo = new SaveFileDialog { Title = "Exportar reporte de compras", Filter = $"{tipo}|*.{extension}", DefaultExt = extension, AddExtension = true, OverwritePrompt = true, FileName = $"ReporteCompras_{DateTime.Today:yyyyMMdd}" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        var filas = _compras.Select(c => (object?[])new object?[] { c.IdCompra, c.FechaCompra, c.Proveedor?.NombreEmpresa ?? "—", string.Join(", ", c.Detalles.Select(d => d.Producto?.Nombre ?? $"Producto {d.IdProducto}")), c.Detalles.Sum(d => d.Cantidad), c.NumeroComprobante ?? "—", $"{c.Empleado?.Persona?.Nombre} {c.Empleado?.Persona?.Apellido}".Trim(), c.Total }).ToList();
        try
        {
            Cursor = Cursors.WaitCursor;
            if (formato == Formato.Csv) ExportadorReporteTabular.GuardarCsv(dialogo.FileName, "Reporte de compras", _filtros, Encabezados, filas);
            else if (formato == Formato.Excel) ExportadorReporteTabular.GuardarExcel(dialogo.FileName, "Reporte de compras", _filtros, Encabezados, filas);
            else ExportadorReporteTabular.GuardarPdf(dialogo.FileName, "Reporte de compras", _filtros, Encabezados, filas);
        }
        catch (Exception ex) { MostrarError("No se pudo exportar el reporte", ex); return; }
        finally { Cursor = Cursors.Default; }
        if (MessageBox.Show(this, "El reporte se exportó correctamente.\n\n¿Querés abrirlo ahora?", "Exportación completa", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            try { Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true }); } catch (Exception ex) { MostrarError("No se pudo abrir el archivo", ex); }
    }

    private void HabilitarExportacion(bool habilitado) { _csv.Enabled = habilitado; _excel.Enabled = habilitado; _pdf.Enabled = habilitado; }
    private static Button Boton(string texto, Color color) => new() { Text = texto, Size = new Size(145, 36), Margin = new Padding(0, 0, 8, 0), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Enabled = false };
    private void MostrarError(string titulo, Exception ex) => MessageBox.Show(this, $"{titulo}: {ex.Message}", "Reporte de compras", MessageBoxButtons.OK, MessageBoxIcon.Error);
    private sealed class FilaCompra { public int IdCompra { get; set; } public DateTime Fecha { get; set; } public string Proveedor { get; set; } = ""; public string Productos { get; set; } = ""; public int Unidades { get; set; } public string Comprobante { get; set; } = ""; public string Empleado { get; set; } = ""; public decimal Total { get; set; } }
}
