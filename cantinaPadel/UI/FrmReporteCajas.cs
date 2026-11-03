using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public sealed class FrmReporteCajas : Form
{
    private static readonly CultureInfo Es = new("es-AR");
    private enum Formato { Csv, Excel, Pdf }
    private readonly LogicaCaja _logicaCaja = new();
    private readonly LogicaReporteVentas _logicaVentas = new();
    private readonly LogicaProducto _logicaProducto = new();
    private readonly System.Windows.Forms.Timer _debounce = new() { Interval = 300 };
    private readonly DateTimePicker _desde = new();
    private readonly DateTimePicker _hasta = new();
    private readonly ComboBox _empleado = new();
    private readonly ComboBox _estado = new();
    private readonly DataGridView _grilla = new();
    private readonly Label _estadisticas = new();
    private readonly Label _resumen = new();
    private readonly Label _nota = new();
    private readonly Button _limpiar = new();
    private readonly Button _csv = Boton("Exportar CSV", Color.SteelBlue);
    private readonly Button _excel = Boton("Exportar Excel", Color.ForestGreen);
    private readonly Button _pdf = Boton("Exportar PDF", Color.IndianRed);
    private List<TurnoCaja> _turnos = new();
    private string _filtros = string.Empty;
    private static readonly string[] Encabezados = { "N.º turno", "Empleado", "Estado", "Apertura", "Cierre", "Inicial", "Efectivo esperado", "Efectivo final", "Dif. efectivo", "Ventas tarjeta", "Dif. tarjeta", "Ventas transferencia", "Dif. transferencia" };
    private sealed record OpcionEmpleado(int? Id, string Texto) { public override string ToString() => Texto; }

    public FrmReporteCajas()
    {
        Text = "Reporte de Cajas"; Font = new Font("Segoe UI", 9F); BackColor = SystemColors.Info;
        Construir();
        Load += (_, _) => CargarFiltros();
        _limpiar.Click += (_, _) =>
        {
            _desde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); _hasta.Value = DateTime.Today;
            _empleado.SelectedIndex = _empleado.Items.Count > 0 ? 0 : -1; _estado.SelectedIndex = 0; Buscar();
        };
        _csv.Click += (_, _) => Exportar(Formato.Csv); _excel.Click += (_, _) => Exportar(Formato.Excel); _pdf.Click += (_, _) => Exportar(Formato.Pdf);
        _debounce.Tick += (_, _) => { _debounce.Stop(); Buscar(); };
        Disposed += (_, _) => _debounce.Dispose();
    }

    private void Construir()
    {
        var filtros = new TableLayoutPanel { Dock = DockStyle.Top, Height = 82, BackColor = Color.Gold, Padding = new Padding(12, 7, 12, 7), ColumnCount = 5, RowCount = 2 };
        foreach (var ancho in new[] { 17f, 17f, 25f, 23f, 18f }) filtros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, ancho));
        filtros.RowStyles.Add(new RowStyle(SizeType.Absolute, 22)); filtros.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        AgregarFiltro(filtros, 0, "Desde", _desde); AgregarFiltro(filtros, 1, "Hasta", _hasta);
        AgregarFiltro(filtros, 2, "Empleado", _empleado); AgregarFiltro(filtros, 3, "Estado", _estado);
        _limpiar.Text = "Limpiar"; _limpiar.Dock = DockStyle.Fill; _limpiar.BackColor = Color.White; _limpiar.FlatStyle = FlatStyle.Flat; filtros.Controls.Add(_limpiar, 4, 1);

        var panelStats = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(14, 8, 14, 7), BackColor = Color.FromArgb(255, 248, 220) };
        _estadisticas.Dock = DockStyle.Fill; _estadisticas.Font = new Font("Segoe UI", 10F, FontStyle.Bold); _estadisticas.TextAlign = ContentAlignment.MiddleLeft;
        panelStats.Controls.Add(_estadisticas);

        _grilla.Dock = DockStyle.Fill; _grilla.ReadOnly = true; _grilla.AllowUserToAddRows = false; _grilla.AllowUserToDeleteRows = false; _grilla.MultiSelect = false;
        _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect; _grilla.RowHeadersVisible = false; _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grilla.RowTemplate.Height = 30; _grilla.ColumnHeadersHeight = 34; _grilla.BackgroundColor = Color.White; _grilla.GridColor = SystemColors.ControlLight;
        _grilla.AutoGenerateColumns = false;
        AddCol("IdTurnoCaja", "N.º turno", 7); AddCol("NombreEmpleado", "Empleado", 13); AddCol("Estado", "Estado", 8);
        AddCol("FechaApertura", "Apertura", 12, "dd/MM/yyyy HH:mm"); AddCol("FechaCierre", "Cierre", 12, "dd/MM/yyyy HH:mm");
        AddCol("MontoApertura", "Inicial", 9, "C2"); AddCol("EfectivoEsperado", "Efectivo esperado", 10, "C2");
        AddCol("EfectivoFinal", "Efectivo final", 9, "C2"); AddCol("DiferenciaEfectivo", "Dif. efectivo", 9, "C2");
        AddCol("CierreTarjeta", "Ventas tarjeta", 9, "C2"); AddCol("DiferenciaTarjeta", "Dif. tarjeta", 8);
        AddCol("CierreTransferencia", "Ventas transferencia", 10, "C2"); AddCol("DiferenciaTransferencia", "Dif. transferencia", 8);

        var pie = new Panel { Dock = DockStyle.Bottom, Height = 76, Padding = new Padding(12, 4, 12, 4), BackColor = SystemColors.Info };
        var botones = new FlowLayoutPanel { Dock = DockStyle.Left, Width = 470, WrapContents = false }; botones.Controls.AddRange(new Control[] { _csv, _excel, _pdf });
        var texto = new Panel { Dock = DockStyle.Fill }; _resumen.Dock = DockStyle.Top; _resumen.Height = 30; _resumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold); _resumen.TextAlign = ContentAlignment.MiddleRight;
        _nota.Dock = DockStyle.Fill; _nota.TextAlign = ContentAlignment.MiddleRight; _nota.ForeColor = SystemColors.GrayText;
        texto.Controls.Add(_nota); texto.Controls.Add(_resumen); pie.Controls.Add(texto); pie.Controls.Add(botones);
        Controls.Add(_grilla); Controls.Add(pie); Controls.Add(panelStats); Controls.Add(filtros);
    }

    private void CargarFiltros()
    {
        try
        {
            _desde.CustomFormat = "d/M/yyyy";
            _hasta.CustomFormat = "d/M/yyyy";
            _desde.Format = DateTimePickerFormat.Custom;
            _hasta.Format = DateTimePickerFormat.Custom;
            _desde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); _hasta.Value = DateTime.Today;
            _empleado.DropDownStyle = ComboBoxStyle.DropDownList; _empleado.Items.Add(new OpcionEmpleado(null, "Todos los empleados"));
            _empleado.Items.AddRange(_logicaVentas.ObtenerEmpleados().Select(e => new OpcionEmpleado(e.Id, e.Texto)).Cast<object>().ToArray()); _empleado.SelectedIndex = 0;
            _estado.DropDownStyle = ComboBoxStyle.DropDownList; _estado.Items.AddRange(new object[] { "Todos", TurnoCaja.EstadoCerrada, TurnoCaja.EstadoAbierta }); _estado.SelectedIndex = 0;
            Buscar();
            _desde.ValueChanged += (_, _) => ProgramarBusqueda(); _hasta.ValueChanged += (_, _) => ProgramarBusqueda();
            _empleado.SelectedIndexChanged += (_, _) => ProgramarBusqueda(); _estado.SelectedIndexChanged += (_, _) => ProgramarBusqueda();
        }
        catch (Exception ex) { MessageBox.Show(this, $"No se pudieron cargar los datos: {ex.Message}", "Reporte de cajas", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Buscar()
    {
        try
        {
            var desde = _desde.Value.Date; var hasta = _hasta.Value.Date;
            if (desde > hasta) throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");
            var empleado = _empleado.SelectedItem as OpcionEmpleado;
            var filtroEstado = _estado.SelectedItem?.ToString() ?? "Todos";
            _turnos = _logicaCaja.ObtenerTodoHistorial(Sesion.Rol)
                .Where(t => t.FechaApertura.Date >= desde && t.FechaApertura.Date <= hasta)
                .Where(t => empleado?.Id == null || t.IdEmpleado == empleado.Id)
                .Where(t => filtroEstado == "Todos" || t.Estado == filtroEstado)
                .ToList();
            _grilla.DataSource = _turnos.Select(t => new FilaCaja
            {
                IdTurnoCaja = t.IdTurnoCaja, NombreEmpleado = t.NombreEmpleado, Estado = t.Estado,
                FechaApertura = t.FechaApertura, FechaCierre = t.FechaCierre, MontoApertura = t.MontoApertura,
                EfectivoEsperado = t.EfectivoEsperado, EfectivoFinal = t.EfectivoFinal,
                DiferenciaEfectivo = t.DiferenciaEfectivo, CierreTarjeta = t.CierreTarjeta,
                CierreTransferencia = t.CierreTransferencia
            }).ToList();
            var ventas = _logicaVentas.Generar(new FiltroReporteVentas { Desde = desde, Hasta = hasta, Tipo = TipoVentaReporte.Todas });
            var padel = ventas.Where(v => v.Tipo == CalculadorReporteVentas.TipoPadel).Sum(v => v.Unidades);
            var almacen = ventas.Where(v => v.Tipo == CalculadorReporteVentas.TipoCantina).Sum(v => v.Total);
            var productosHora = _logicaProducto.ObtenerTodos(true).Where(p => p.Categoria?.Nombre == CalculadorReporteVentas.CategoriaHoraPadel).ToList();
            var precios = productosHora.Count == 0 ? "Sin productos de hora configurados" : string.Join("  |  ", productosHora.Select(p => $"{p.Nombre}: {p.PrecioConIva.ToString("C2", Es)}"));
            _estadisticas.Text = $"Resumen del mes: {padel} horas de pádel vendidas  •  Precio vigente por cancha: {precios}\nVentas de almacén: {almacen.ToString("C2", Es)}";
            _filtros = $"Período: {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}  |  Empleado: {empleado?.Texto ?? "Todos"}  |  Estado: {filtroEstado}  |  Horas de pádel: {padel}  |  Precio vigente por cancha: {precios}  |  Ventas de almacén: {almacen.ToString("C2", Es)}";
            var totalInicial = _turnos.Sum(t => t.MontoApertura);
            var totalEfectivoFinal = _turnos.Sum(t => t.EfectivoFinal ?? 0m);
            _resumen.Text = $"{_turnos.Count} turnos  |  Inicial: {totalInicial.ToString("C2", Es)}  |  Efectivo final registrado: {totalEfectivoFinal.ToString("C2", Es)}";
            HabilitarExportacion(_turnos.Count > 0);
        }
        catch (Exception ex) { _turnos.Clear(); _grilla.DataSource = null; _resumen.Text = ex.Message; _estadisticas.Text = string.Empty; HabilitarExportacion(false); }
    }

    private void Exportar(Formato formato)
    {
        if (_turnos.Count == 0) return;
        var (extension, tipo) = formato switch { Formato.Csv => ("csv", "Archivo CSV (*.csv)"), Formato.Excel => ("xlsx", "Libro de Excel (*.xlsx)"), _ => ("pdf", "Documento PDF (*.pdf)") };
        using var dialogo = new SaveFileDialog { Title = "Exportar reporte de cajas", Filter = $"{tipo}|*.{extension}", DefaultExt = extension, AddExtension = true, OverwritePrompt = true, FileName = $"ReporteCajas_{DateTime.Today:yyyyMMdd}" };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        var filas = _turnos.Select(t => (object?[])new object?[] { t.IdTurnoCaja, t.NombreEmpleado, t.Estado, t.FechaApertura, t.FechaCierre, t.MontoApertura, t.EfectivoEsperado, t.EfectivoFinal, t.DiferenciaEfectivo, t.CierreTarjeta, "Sin arqueo", t.CierreTransferencia, "Sin arqueo" }).ToList();
        var titulo = "Reporte de cajas";
        try
        {
            Cursor = Cursors.WaitCursor;
            if (formato == Formato.Csv) ExportadorReporteTabular.GuardarCsv(dialogo.FileName, titulo, _filtros, Encabezados, filas);
            else if (formato == Formato.Excel) ExportadorReporteTabular.GuardarExcel(dialogo.FileName, titulo, _filtros, Encabezados, filas);
            else ExportadorReporteTabular.GuardarPdf(dialogo.FileName, titulo, _filtros, Encabezados, filas);
        }
        catch (Exception ex) { MessageBox.Show(this, $"No se pudo exportar el reporte: {ex.Message}", "Reporte de cajas", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        finally { Cursor = Cursors.Default; }
        if (MessageBox.Show(this, "El reporte se exportó correctamente.\n\n¿Querés abrirlo ahora?", "Exportación completa", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            try { Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true }); } catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se pudo abrir el archivo", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void AddCol(string name, string title, float weight, string? format = null)
    {
        var col = new DataGridViewTextBoxColumn { Name = name, DataPropertyName = name, HeaderText = title, FillWeight = weight };
        if (format != null) col.DefaultCellStyle = new DataGridViewCellStyle { Format = format, FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight };
        _grilla.Columns.Add(col);
    }
    private void ProgramarBusqueda() { _debounce.Stop(); _debounce.Start(); }
    private static void AgregarFiltro(TableLayoutPanel panel, int col, string titulo, Control control)
    {
        panel.Controls.Add(new Label { Text = titulo, Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft, Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, col, 0);
        control.Dock = DockStyle.Fill; control.Margin = new Padding(0, 2, 6, 0); panel.Controls.Add(control, col, 1);
    }
    private static Button Boton(string texto, Color color) => new() { Text = texto, Size = new Size(145, 36), Margin = new Padding(0, 0, 8, 0), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Enabled = false };
    private void HabilitarExportacion(bool valor) { _csv.Enabled = valor; _excel.Enabled = valor; _pdf.Enabled = valor; }
    private sealed class FilaCaja
    {
        public int IdTurnoCaja { get; set; } public string NombreEmpleado { get; set; } = ""; public string Estado { get; set; } = "";
        public DateTime FechaApertura { get; set; } public DateTime? FechaCierre { get; set; } public decimal MontoApertura { get; set; }
        public decimal? EfectivoEsperado { get; set; } public decimal? EfectivoFinal { get; set; } public decimal? DiferenciaEfectivo { get; set; }
        public decimal? CierreTarjeta { get; set; } public decimal? CierreTransferencia { get; set; }
        public string DiferenciaTarjeta => "Sin arqueo"; public string DiferenciaTransferencia => "Sin arqueo";
    }
}
