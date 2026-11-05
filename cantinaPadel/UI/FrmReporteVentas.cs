using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    // Reporte de ventas
    // Rango de fechas, tipo de venta (pádel / cantina), método de pago y empleado
    public class FrmReporteVentas : Form
    {
        // Importes siempre con formato argentino: 1.500,50
        private static readonly CultureInfo Es = new("es-AR");

        private const string TodosLosMetodos = "Todos los métodos";
        private static readonly OpcionFiltro TodosLosEmpleados = new(null, "Todos los empleados");

        private sealed record OpcionTipo(TipoVentaReporte Tipo, string Texto)
        {
            public override string ToString() => Texto;
        }

        private readonly LogicaReporteVentas _logica = new();

        // Cada cambio en un filtro reinicia este timer y la consulta real se hace cuando se deja de tocar 300ms
        private readonly System.Windows.Forms.Timer _debounce = new() { Interval = 300 };

        // Evita consultar la bd mientras se arman o se reinician los filtros por código (cada asignación dispara eventos)
        private bool _armandoFiltros;

        private readonly DateTimePicker dtpDesde = new();
        private readonly DateTimePicker dtpHasta = new();
        private readonly ComboBox cmbTipo = new();
        private readonly ComboBox cmbMetodoPago = new();
        private readonly ComboBox cmbEmpleado = new();
        private readonly Button btnLimpiar = new();
        private readonly Button btnMesAnterior = new() { Text = "‹ Mes anterior", Width = 125, Height = 27 };
        private readonly Button btnMesSiguiente = new() { Text = "Mes siguiente ›", Width = 125, Height = 27 };
        private readonly Label lblMesSeleccionado = new() { AutoSize = false, Width = 150, Height = 27, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        private readonly DataGridView dgvVentas = new();
        private readonly DataGridView dgvResumen = new();
        private readonly Label lblVentasActual = new();
        private readonly Label lblVentasAnterior = new();
        private readonly Label lblVentasVariacion = new();
        private readonly Label lblGananciaActual = new();
        private readonly Label lblGananciaAnterior = new();
        private readonly Label lblGananciaVariacion = new();
        private readonly Button btnCsv = new();
        private readonly Button btnExcel = new();
        private readonly Button btnPdf = new();

        private enum FormatoExportacion { Csv, Excel, Pdf }

        // Lo que se está mostrando en pantalla: es lo que se exporta (así el archivo coincide siempre con la grilla)
        private List<FilaReporteVentas> _filas = new();
        private ResumenReporteVentas? _resumen;
        private string _descripcionFiltros = string.Empty;

        public FrmReporteVentas()
        {
            Text = "Reporte de Ventas";
            Font = new Font("Segoe UI", 9F);
            BackColor = SystemColors.Info;

            ConstruirPantalla();
            Load += FrmReporteVentas_Load;
            Disposed += (_, _) => _debounce.Dispose();
        }

        // Armado de la pantalla

        private void ConstruirPantalla()
        {
            var panelFiltros = ConstruirPanelFiltros();

            // Área inferior: subtotales completos arriba y comparación en dos tarjetas legibles.
            var panelResumen = new Panel { Dock = DockStyle.Fill, BackColor = SystemColors.Info, Padding = new Padding(12, 5, 12, 8) };

            ConfigurarBotonExportar(btnCsv, "Exportar CSV", Color.SteelBlue);
            ConfigurarBotonExportar(btnExcel, "Exportar Excel", Color.ForestGreen);
            ConfigurarBotonExportar(btnPdf, "Exportar PDF", Color.IndianRed);
            var botones = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 3, 0, 0) };
            botones.Controls.AddRange(new Control[] { btnCsv, btnExcel, btnPdf });

            var comparacion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 2, 0, 0),
                ColumnCount = 1,
                RowCount = 1,
                BackColor = SystemColors.Info
            };
            comparacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            comparacion.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var tarjetas = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                BackColor = SystemColors.Info
            };
            tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tarjetas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tarjetas.Controls.Add(CrearTarjetaComparacion("VENTAS", lblVentasActual, lblVentasAnterior, lblVentasVariacion), 0, 0);
            tarjetas.Controls.Add(CrearTarjetaComparacion("GANANCIA", lblGananciaActual, lblGananciaAnterior, lblGananciaVariacion), 1, 0);
            comparacion.Controls.Add(tarjetas, 0, 0);

            var pie = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                BackColor = SystemColors.Info
            };
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pie.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            pie.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            pie.Controls.Add(comparacion, 0, 0);
            pie.Controls.Add(botones, 0, 1);

            dgvResumen.Dock = DockStyle.Top;
            dgvResumen.Height = 112;

            // Primero el pie flexible y después la grilla superior para que el Dock reserve el alto correctamente.
            panelResumen.Controls.Add(pie);
            panelResumen.Controls.Add(dgvResumen);

            dgvVentas.Dock = DockStyle.Fill;
            dgvVentas.Margin = new Padding(6, 5, 6, 5);

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = SystemColors.Info
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 122));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 255));
            panelFiltros.Dock = DockStyle.Fill;
            contenido.Controls.Add(panelFiltros, 0, 0);
            contenido.Controls.Add(dgvVentas, 0, 1);
            contenido.Controls.Add(panelResumen, 0, 2);
            Controls.Add(contenido);
        }

        private static void ConfigurarBotonExportar(Button boton, string texto, Color color)
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

        private static Panel CrearTarjetaComparacion(string titulo, Label actual, Label anterior, Label variacion)
        {
            var tarjeta = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(10, 6, 10, 5),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var acento = new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.DarkGoldenrod };
            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(8, 0, 0, 0),
                Margin = Padding.Empty,
                BackColor = Color.White
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 17));
            contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold)
            };
            actual.Dock = DockStyle.Fill;
            actual.TextAlign = ContentAlignment.MiddleLeft;
            actual.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            anterior.Dock = DockStyle.Fill;
            anterior.TextAlign = ContentAlignment.MiddleLeft;
            anterior.Font = new Font("Segoe UI", 8.5F);
            anterior.ForeColor = Color.DimGray;
            variacion.Dock = DockStyle.Fill;
            variacion.TextAlign = ContentAlignment.MiddleRight;
            variacion.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            variacion.ForeColor = Color.ForestGreen;

            var detalle = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
            detalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
            detalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            detalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            detalle.Controls.Add(anterior, 0, 0);
            detalle.Controls.Add(variacion, 1, 0);
            contenido.Controls.Add(etiqueta, 0, 0);
            contenido.Controls.Add(actual, 0, 1);
            contenido.Controls.Add(detalle, 0, 2);
            tarjeta.Controls.Add(contenido);
            tarjeta.Controls.Add(acento);
            return tarjeta;
        }

        private Panel ConstruirPanelFiltros()
        {
            var tabla = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 112,
                BackColor = Color.Gold,
                Padding = new Padding(12, 8, 12, 8),
                ColumnCount = 6,
                RowCount = 3
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var navegacion = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Margin = Padding.Empty };
            navegacion.Controls.AddRange(new Control[] { btnMesAnterior, lblMesSeleccionado, btnMesSiguiente });
            tabla.Controls.Add(navegacion, 0, 0);
            tabla.SetColumnSpan(navegacion, 6);

            AgregarFiltro(tabla, 0, "Desde", dtpDesde, 1, 2);
            AgregarFiltro(tabla, 1, "Hasta", dtpHasta, 1, 2);
            AgregarFiltro(tabla, 2, "Tipo de venta", cmbTipo, 1, 2);
            AgregarFiltro(tabla, 3, "Método de pago", cmbMetodoPago, 1, 2);
            AgregarFiltro(tabla, 4, "Empleado", cmbEmpleado, 1, 2);

            btnLimpiar.Text = "Limpiar";
            btnLimpiar.Dock = DockStyle.Fill;
            btnLimpiar.Margin = new Padding(6, 0, 0, 0);
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold);
            tabla.Controls.Add(btnLimpiar, 5, 2);

            return tabla;
        }

        private static void AgregarFiltro(TableLayoutPanel tabla, int columna, string titulo, Control control, int filaTitulo = 0, int filaControl = 1)
        {
            var etiqueta = new Label
            {
                Text = titulo,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            control.Dock = DockStyle.Top;
            control.Margin = new Padding(0, 2, 6, 0);
            tabla.Controls.Add(etiqueta, columna, filaTitulo);
            tabla.Controls.Add(control, columna, filaControl);
        }

        // ================= Carga inicial =================

        private void FrmReporteVentas_Load(object? sender, EventArgs e)
        {
            if (!EsAdministrador())
            {
                MessageBox.Show(this, "Los reportes están disponibles solo para el Administrador.",
                    "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            ConfigurarGrilla();
            ConfigurarGrillaResumen();
            ConfigurarFiltros();
            SuscribirEventos();
            BuscarVentas();
        }

        private void ConfigurarFiltros()
        {
            _armandoFiltros = true;
            try
            {
                dtpDesde.Format = DateTimePickerFormat.Short;
                dtpHasta.Format = DateTimePickerFormat.Short;
                dtpDesde.MaxDate = DateTime.Today;
                dtpHasta.MaxDate = DateTime.Today;

                cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbTipo.Items.Add(new OpcionTipo(TipoVentaReporte.Todas, "Todas"));
                cmbTipo.Items.Add(new OpcionTipo(TipoVentaReporte.Padel, "Pádel"));
                cmbTipo.Items.Add(new OpcionTipo(TipoVentaReporte.Cantina, "Cantina"));

                cmbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMetodoPago.Items.Add(TodosLosMetodos);
                foreach (var forma in LogicaReporteVentas.FormasPago)
                    cmbMetodoPago.Items.Add(forma);

                cmbEmpleado.DropDownStyle = ComboBoxStyle.DropDownList;
                CargarEmpleados();

                var ayuda = new ToolTip();
                Disposed += (_, _) => ayuda.Dispose();
                ayuda.SetToolTip(cmbTipo, "Una venta que mezcla turnos y productos aparece en dos filas, una por tipo.");
                ayuda.SetToolTip(dgvResumen, "Ganancia = neto (sin IVA) menos costo. El costo es el actual de cada producto; las horas de pádel no tienen costo.");
                ayuda.SetToolTip(cmbMetodoPago, "Se filtra por el método con el que se registró la venta.");

                RestablecerFiltros();
            }
            finally
            {
                _armandoFiltros = false;
            }
        }

        // Deja los filtros como al abrir la pantalla: del 1º del mes de hoy hasta hoy, y todo lo demás sin filtrar
        private void RestablecerFiltros()
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;
            cmbTipo.SelectedIndex = 0;
            cmbMetodoPago.SelectedIndex = 0;
            cmbEmpleado.SelectedIndex = 0;
            ActualizarNavegacionMes();
        }

        private void CargarEmpleados()
        {
            cmbEmpleado.Items.Clear();
            cmbEmpleado.Items.Add(TodosLosEmpleados);

            try
            {
                cmbEmpleado.Items.AddRange(_logica.ObtenerEmpleados().Cast<object>().ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudieron cargar los empleados: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGrilla()
        {
            dgvVentas.AutoGenerateColumns = false;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToDeleteRows = false;
            dgvVentas.ReadOnly = true;
            dgvVentas.MultiSelect = false;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVentas.RowTemplate.Height = 30;
            dgvVentas.ColumnHeadersHeight = 34;
            dgvVentas.BackgroundColor = Color.White;
            dgvVentas.GridColor = SystemColors.ControlLight;

            var derecha = DataGridViewContentAlignment.MiddleRight;

            dgvVentas.Columns.Clear();
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdVenta", DataPropertyName = nameof(FilaReporteVentas.IdVenta), HeaderText = "N.º venta", FillWeight = 9 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                DataPropertyName = nameof(FilaReporteVentas.Fecha),
                HeaderText = "Fecha",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cliente", DataPropertyName = nameof(FilaReporteVentas.Cliente), HeaderText = "Cliente", FillWeight = 17 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Empleado", DataPropertyName = nameof(FilaReporteVentas.Empleado), HeaderText = "Empleado", FillWeight = 14 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "FormaPago", DataPropertyName = nameof(FilaReporteVentas.FormaPago), HeaderText = "Método de pago", FillWeight = 12 });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Unidades",
                DataPropertyName = nameof(FilaReporteVentas.Unidades),
                HeaderText = "Unid./Hs.",
                FillWeight = 7,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = derecha, Format = "0.##" }
            });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                DataPropertyName = nameof(FilaReporteVentas.Total),
                HeaderText = "Total (con IVA)",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = derecha }
            });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ganancia",
                DataPropertyName = nameof(FilaReporteVentas.Ganancia),
                HeaderText = "Ganancia",
                FillWeight = 13,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = derecha }
            });
            dgvVentas.Columns.Add(new DataGridViewTextBoxColumn { Name = "EstadoCobro", DataPropertyName = nameof(FilaReporteVentas.EstadoCobro), HeaderText = "Cobro", FillWeight = 10 });
        }

        private void ConfigurarGrillaResumen()
        {
            dgvResumen.AutoGenerateColumns = false;
            dgvResumen.AllowUserToAddRows = false;
            dgvResumen.AllowUserToDeleteRows = false;
            dgvResumen.AllowUserToResizeRows = false;
            dgvResumen.ReadOnly = true;
            dgvResumen.MultiSelect = false;
            dgvResumen.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResumen.RowHeadersVisible = false;
            dgvResumen.ScrollBars = ScrollBars.None;
            dgvResumen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResumen.RowTemplate.Height = 28;
            dgvResumen.ColumnHeadersHeight = 30;
            dgvResumen.BackgroundColor = Color.White;
            dgvResumen.GridColor = SystemColors.ControlLight;

            var dinero = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight };

            dgvResumen.Columns.Clear();
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", DataPropertyName = nameof(SubtotalReporteVentas.Nombre), HeaderText = "Subtotales", FillWeight = 18 });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Unidades",
                DataPropertyName = nameof(SubtotalReporteVentas.Unidades),
                HeaderText = "Unid./Hs.",
                FillWeight = 9,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.##" }
            });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total", DataPropertyName = nameof(SubtotalReporteVentas.Total), HeaderText = "Total (con IVA)", FillWeight = 18, DefaultCellStyle = dinero });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "Neto", DataPropertyName = nameof(SubtotalReporteVentas.Neto), HeaderText = "Neto (sin IVA)", FillWeight = 18, DefaultCellStyle = dinero });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "Costo", DataPropertyName = nameof(SubtotalReporteVentas.Costo), HeaderText = "Costo", FillWeight = 18, DefaultCellStyle = dinero });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ganancia", DataPropertyName = nameof(SubtotalReporteVentas.Ganancia), HeaderText = "Ganancia", FillWeight = 19, DefaultCellStyle = dinero });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrecioHoraVigente", DataPropertyName = nameof(SubtotalReporteVentas.PrecioHoraVigente), HeaderText = "Precio/h vigente", FillWeight = 16 });
            dgvResumen.Columns.Add(new DataGridViewTextBoxColumn { Name = "VentasAlmacen", DataPropertyName = nameof(SubtotalReporteVentas.VentasAlmacen), HeaderText = "Ventas Almacén", FillWeight = 16, DefaultCellStyle = dinero });

            // Es un cuadro informativo: no tiene fila "seleccionada", y la última fila (Total) va en negrita
            dgvResumen.DataBindingComplete += (_, _) =>
            {
                dgvResumen.ClearSelection();
                if (dgvResumen.Rows.Count > 0)
                    dgvResumen.Rows[^1].DefaultCellStyle.Font = new Font(dgvResumen.Font, FontStyle.Bold);
            };
            dgvResumen.SelectionChanged += (_, _) => dgvResumen.ClearSelection();
        }

        private void SuscribirEventos()
        {
            dtpDesde.ValueChanged += (_, _) => { ActualizarNavegacionMes(); ProgramarBusqueda(); };
            dtpHasta.ValueChanged += (_, _) => { ActualizarNavegacionMes(); ProgramarBusqueda(); };
            cmbTipo.SelectedIndexChanged += (_, _) => ProgramarBusqueda();
            cmbMetodoPago.SelectedIndexChanged += (_, _) => ProgramarBusqueda();
            cmbEmpleado.SelectedIndexChanged += (_, _) => ProgramarBusqueda();

            btnCsv.Click += (_, _) => Exportar(FormatoExportacion.Csv);
            btnExcel.Click += (_, _) => Exportar(FormatoExportacion.Excel);
            btnPdf.Click += (_, _) => Exportar(FormatoExportacion.Pdf);

            btnLimpiar.Click += (_, _) =>
            {
                _armandoFiltros = true;
                try { RestablecerFiltros(); }
                finally { _armandoFiltros = false; }
                _debounce.Stop();
                BuscarVentas();
            };

            btnMesAnterior.Click += (_, _) => NavegarMes(-1);
            btnMesSiguiente.Click += (_, _) => NavegarMes(1);

            _debounce.Tick += (_, _) =>
            {
                _debounce.Stop();
                BuscarVentas();
            };
        }

        // ================= Búsqueda =================

        private void ProgramarBusqueda()
        {
            if (_armandoFiltros) return;

            _debounce.Stop();
            _debounce.Start();
        }

        private void NavegarMes(int desplazamiento)
        {
            var mes = new DateTime(dtpDesde.Value.Year, dtpDesde.Value.Month, 1).AddMonths(desplazamiento);
            var hoy = DateTime.Today;
            var hasta = mes.Year == hoy.Year && mes.Month == hoy.Month ? hoy : mes.AddMonths(1).AddDays(-1);

            _armandoFiltros = true;
            try
            {
                dtpDesde.Value = mes;
                dtpHasta.Value = hasta;
            }
            finally { _armandoFiltros = false; }

            _debounce.Stop();
            ActualizarNavegacionMes();
            BuscarVentas();
        }

        private void ActualizarNavegacionMes()
        {
            var mes = new DateTime(dtpDesde.Value.Year, dtpDesde.Value.Month, 1);
            lblMesSeleccionado.Text = mes.ToString("MMMM yyyy", Es);
            var hoy = DateTime.Today;
            btnMesSiguiente.Enabled = mes < new DateTime(hoy.Year, hoy.Month, 1);
        }

        private FiltroReporteVentas LeerFiltro() => new()
        {
            Desde = dtpDesde.Value.Date,
            Hasta = dtpHasta.Value.Date,
            Tipo = (cmbTipo.SelectedItem as OpcionTipo)?.Tipo ?? TipoVentaReporte.Todas,
            FormaPago = cmbMetodoPago.SelectedIndex > 0 ? cmbMetodoPago.SelectedItem as string : null,
            IdEmpleado = (cmbEmpleado.SelectedItem as OpcionFiltro)?.Id
        };

        private void BuscarVentas()
        {
            try
            {
                var filtroActual = LeerFiltro();
                var filas = _logica.Generar(filtroActual);
                var resumen = _logica.CalcularResumen(filas);
                resumen.Padel.PrecioHoraVigente = ObtenerPrecioHoraVigente();

                _filas = filas;
                _resumen = resumen;
                _descripcionFiltros = DescribirFiltros();
                ActualizarBotonesExportar();

                PaginadorGrilla.Mostrar(dgvVentas, filas);
                dgvResumen.DataSource = resumen.Filas;
                MostrarResumen(filas, resumen, filtroActual);
            }
            catch (ArgumentException ex)
            {
                // Filtros mal combinados (por ej. fecha inicial posterior a la final): se avisa en el pie sin interrumpir con un cartel
                LimpiarResultado();
                MessageBox.Show(this, ex.Message, "Rango de fechas inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                LimpiarResultado();
                MessageBox.Show(this, $"No se pudo generar el reporte: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string ObtenerPrecioHoraVigente()
        {
            var tarifas = new CanchaRepository().ObtenerTodas(true)
                .Where(c => c.Producto != null)
                .Select(c => (c.Nombre, Precio: c.Producto!.PrecioConIva))
                .ToList();
            if (tarifas.Count == 0) return "Sin canchas activas";
            if (tarifas.Select(t => t.Precio).Distinct().Count() == 1)
                return tarifas[0].Precio.ToString("C2", Es);
            return string.Join(" · ", tarifas.Select(t => $"{t.Nombre}: {t.Precio.ToString("C2", Es)}"));
        }

        // Sin resultado (filtros inválidos o error al consultar): se vacía todo y no se puede exportar nada viejo
        private void LimpiarResultado()
        {
            _filas = new List<FilaReporteVentas>();
            _resumen = null;
            ActualizarBotonesExportar();
            PaginadorGrilla.Limpiar(dgvVentas);
            dgvResumen.DataSource = null;
            lblVentasActual.Text = lblVentasAnterior.Text = lblVentasVariacion.Text = "—";
            lblGananciaActual.Text = lblGananciaAnterior.Text = lblGananciaVariacion.Text = "—";
        }

        // Solo se puede exportar si hay filas para exportar
        private void ActualizarBotonesExportar()
        {
            bool hayDatos = _filas.Count > 0 && _resumen != null;
            btnCsv.Enabled = hayDatos;
            btnExcel.Enabled = hayDatos;
            btnPdf.Enabled = hayDatos;
        }

        // Texto con los filtros aplicados: va en el encabezado de los archivos exportados
        private string DescribirFiltros()
            => $"Período: {dtpDesde.Value:dd/MM/yyyy} al {dtpHasta.Value:dd/MM/yyyy}  |  Tipo: {cmbTipo.Text}  |  " +
               $"Método de pago: {cmbMetodoPago.Text}  |  Empleado: {cmbEmpleado.Text}";

        // ================= Exportación =================

        private void Exportar(FormatoExportacion formato)
        {
            if (_filas.Count == 0 || _resumen == null) return;

            var (extension, descripcion) = formato switch
            {
                FormatoExportacion.Csv => ("csv", "Archivo CSV (*.csv)"),
                FormatoExportacion.Excel => ("xlsx", "Libro de Excel (*.xlsx)"),
                _ => ("pdf", "Documento PDF (*.pdf)")
            };

            using var dialogo = new SaveFileDialog
            {
                Title = "Exportar reporte de ventas",
                Filter = $"{descripcion}|*.{extension}",
                DefaultExt = extension,
                AddExtension = true,
                OverwritePrompt = true,
                FileName = $"ReporteVentas_{DateTime.Today:yyyyMMdd}"
            };

            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                switch (formato)
                {
                    case FormatoExportacion.Csv:
                        ExportadorCsv.Guardar(dialogo.FileName, _filas, _resumen, _descripcionFiltros);
                        break;
                    case FormatoExportacion.Excel:
                        ExportadorExcel.Guardar(dialogo.FileName, _filas, _resumen, _descripcionFiltros);
                        break;
                    default:
                        ExportadorPdf.Guardar(dialogo.FileName, _filas, _resumen, _descripcionFiltros);
                        break;
                }
            }
            catch (IOException)
            {
                MessageBox.Show(this, "No se pudo guardar el archivo. Si lo tenés abierto en otro programa (por ejemplo Excel), cerralo e intentá de nuevo.",
                    "No se pudo exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(this, "No tenés permiso para guardar en esa ubicación. Elegí otra carpeta (por ejemplo Documentos).",
                    "No se pudo exportar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudo exportar el reporte: {ex.Message}",
                    "Error al exportar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            var abrir = MessageBox.Show(this, "El reporte se exportó correctamente.\n\n¿Querés abrirlo ahora?",
                "Exportación completa", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (abrir != DialogResult.Yes) return;

            try
            {
                Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudo abrir el archivo: {ex.Message}",
                    "Abrir archivo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MostrarResumen(List<FilaReporteVentas> filas, ResumenReporteVentas resumenActual, FiltroReporteVentas filtroActual)
        {
            var filtroAnterior = new FiltroReporteVentas
            {
                Desde = filtroActual.Desde.AddMonths(-1),
                Hasta = filtroActual.Hasta.AddMonths(-1),
                Tipo = filtroActual.Tipo,
                FormaPago = filtroActual.FormaPago,
                IdEmpleado = filtroActual.IdEmpleado
            };
            var actual = resumenActual.General;
            lblVentasActual.Text = actual.Total.ToString("C2", Es);
            lblGananciaActual.Text = actual.Ganancia.ToString("C2", Es);
            try
            {
                var resumenAnterior = _logica.CalcularResumen(_logica.Generar(filtroAnterior)).General;
                var cambioTotal = actual.Total - resumenAnterior.Total;
                var cambioGanancia = actual.Ganancia - resumenAnterior.Ganancia;
                lblVentasAnterior.Text = $"Anterior ({filtroAnterior.Desde:dd/MM}–{filtroAnterior.Hasta:dd/MM}): {resumenAnterior.Total.ToString("C2", Es)}";
                lblVentasVariacion.Text = $"{FlechaCambio(cambioTotal)} {FormatearCambio(cambioTotal)}";
                lblGananciaAnterior.Text = $"Anterior ({filtroAnterior.Desde:dd/MM}–{filtroAnterior.Hasta:dd/MM}): {resumenAnterior.Ganancia.ToString("C2", Es)}";
                lblGananciaVariacion.Text = $"{FlechaCambio(cambioGanancia)} {FormatearCambio(cambioGanancia)}";
            }
            catch
            {
                lblVentasAnterior.Text = lblGananciaAnterior.Text = "Comparación no disponible";
                lblVentasVariacion.Text = lblGananciaVariacion.Text = "—";
            }
        }

        private static string FlechaCambio(decimal cambio)
            => cambio > 0m ? "↑" : cambio < 0m ? "↓" : "·";

        private static string FormatearCambio(decimal importe)
            => (importe > 0m ? "+" : string.Empty) + importe.ToString("C2", Es);

        private static bool EsAdministrador()
            => string.Equals(global::cantinaPadel.Sesion.Rol, "Admin", StringComparison.Ordinal);
    }
}
