using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    // Listado de compras a proveedores (US-25). Es la pantalla de entrada del módulo Compras (se abre desde FrmMain dentro del panel principal)
    // Los controles se crean en el Designer; acá solo se les asigna comportamiento y los eventos se suscriben en el constructor/Load,
    // por eso en el Designer no hace falta configurar ningún evento (los nombres de los controles tienen que ser exactamente los indicados)
    // La búsqueda es automática, como en el resto del sistema: cualquier cambio en un filtro actualiza la grilla
    public partial class FrmListadoCompras : Form
    {
        // Importes siempre con formato argentino: 1.500,50
        private static readonly CultureInfo Es = new("es-AR");

        // Primer item del combo de proveedores: no filtra por proveedor (el id 0 se traduce a null al buscar)
        private static readonly ProveedorOpcion TodosLosProveedores = new(0, "Todos los proveedores");

        // Por defecto se muestran las compras de los últimos 30 días, para no traer todo el historial cada vez que se abre la pantalla
        private const int DiasPorDefecto = 30;

        private readonly LogicaCompra _logica;

        // Cada cambio en un filtro reinicia este timer y la consulta real se hace cuando se deja de tocar 300ms
        // (así, por ejemplo, tipear en el filtro de producto no consulta la base en cada letra)
        private readonly System.Windows.Forms.Timer _debounce = new() { Interval = 300 };
        private readonly ToolTip _ayudaFiltros = new();

        // Evita consultar la base mientras se arman o se reinician los filtros por código (cada asignación dispara eventos)
        private bool _armandoFiltros;

        public FrmListadoCompras()
        {
            InitializeComponent();
            _logica = new LogicaCompra();

            Load += FrmListadoCompras_Load;
            Disposed += (_, _) => _ayudaFiltros.Dispose();
        }

        // ================= Carga inicial =================

        private void FrmListadoCompras_Load(object? sender, EventArgs e)
        {
            if (!EsAdministrador())
            {
                MessageBox.Show(this, "El módulo de compras está disponible solo para el Administrador.",
                    "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
                return;
            }

            Text = "Listado de Compras";

            ConfigurarGrilla();
            ConfigurarFiltros();
            SuscribirEventos();
            BuscarCompras();
        }

        private void ConfigurarFiltros()
        {
            _armandoFiltros = true;
            try
            {
                // Proveedor: combo editable con autocompletado. Se puede elegir de la lista o empezar a escribir el nombre
                cmbProveedor.DropDownStyle = ComboBoxStyle.DropDown;
                cmbProveedor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbProveedor.AutoCompleteSource = AutoCompleteSource.ListItems;
                CargarProveedores();

                // Rango de fechas: cada fecha tiene su casilla para poder quitarla (sin tildar = sin límite de ese lado)
                dtpDesde.Format = DateTimePickerFormat.Short;
                dtpHasta.Format = DateTimePickerFormat.Short;
                dtpDesde.ShowCheckBox = true;
                dtpHasta.ShowCheckBox = true;
                dtpDesde.MaxDate = DateTime.Today;
                dtpHasta.MaxDate = DateTime.Today;

                _ayudaFiltros.SetToolTip(dtpDesde, "Desmarcá la casilla para quitar el límite de fecha inicial.");
                _ayudaFiltros.SetToolTip(dtpHasta, "Desmarcá la casilla para quitar el límite de fecha final.");
                _ayudaFiltros.SetToolTip(cmbProveedor, "Escribí para buscar un proveedor o elegí “Todos los proveedores”.");
                _ayudaFiltros.SetToolTip(txtProducto, "Filtra las compras que incluyan este producto.");
                _ayudaFiltros.SetToolTip(chkIncluirAnuladas, "Incluye las compras anuladas en la grilla.");

                txtProducto.MaxLength = 100;
                txtProducto.PlaceholderText = "Nombre del producto...";
                RestablecerFiltros();
            }
            finally
            {
                _armandoFiltros = false;
            }
        }

        // Deja los filtros como al abrir la pantalla: todos los proveedores, últimos 30 días, sin texto y sin anuladas
        private void RestablecerFiltros()
        {
            cmbProveedor.SelectedIndex = 0;

            dtpDesde.Value = DateTime.Today.AddDays(-DiasPorDefecto);
            dtpDesde.Checked = true;
            dtpHasta.Value = DateTime.Today;
            dtpHasta.Checked = true;

            txtProducto.Clear();
            chkIncluirAnuladas.Checked = false;
        }

        // Se incluyen también los proveedores dados de baja: puede haber compras viejas de un proveedor que ya no está activo
        private void CargarProveedores()
        {
            cmbProveedor.Items.Clear();
            cmbProveedor.Items.Add(TodosLosProveedores);

            try
            {
                cmbProveedor.Items.AddRange(_logica.ObtenerTodosLosProveedores().Cast<object>().ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudieron cargar los proveedores: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGrilla()
        {
            dgvCompras.AutoGenerateColumns = false;
            dgvCompras.AllowUserToAddRows = false;
            dgvCompras.AllowUserToDeleteRows = false;
            dgvCompras.ReadOnly = true;
            dgvCompras.MultiSelect = false;
            dgvCompras.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCompras.RowHeadersVisible = false;
            dgvCompras.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCompras.RowTemplate.Height = 30;
            dgvCompras.ColumnHeadersHeight = 34;
            dgvCompras.BackgroundColor = Color.White;
            dgvCompras.GridColor = SystemColors.ControlLight;

            // Name es la clave para Cells["..."], DataPropertyName bindea con el objeto de la fila
            dgvCompras.Columns.Clear();
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCompra", DataPropertyName = "IdCompra", HeaderText = "N.º compra", FillWeight = 10 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                DataPropertyName = "Fecha",
                HeaderText = "Fecha",
                FillWeight = 16,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Proveedor", DataPropertyName = "Proveedor", HeaderText = "Proveedor", FillWeight = 28 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comprobante", DataPropertyName = "Comprobante", HeaderText = "Factura/remito", FillWeight = 18 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "RegistradaPor", DataPropertyName = "RegistradaPor", HeaderText = "Registrada por", FillWeight = 20 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                DataPropertyName = "Total",
                HeaderText = "Total",
                FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = Es, Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", DataPropertyName = "Estado", HeaderText = "Estado", FillWeight = 10 });
        }

        private void SuscribirEventos()
        {
            cmbProveedor.SelectedIndexChanged += (_, _) => ProgramarBusqueda();
            cmbProveedor.SelectionChangeCommitted += (_, _) => ProgramarBusqueda();
            cmbProveedor.Leave += (_, _) => ProgramarBusqueda();
            cmbProveedor.KeyDown += cmbProveedor_KeyDown;

            // Si se borra todo el texto del proveedor, equivale a "todos los proveedores"
            cmbProveedor.TextChanged += (_, _) =>
            {
                if (cmbProveedor.Text.Length == 0) ProgramarBusqueda();
            };

            dtpDesde.ValueChanged += (_, _) => ProgramarBusqueda();
            dtpHasta.ValueChanged += (_, _) => ProgramarBusqueda();
            txtProducto.TextChanged += (_, _) => ProgramarBusqueda();
            chkIncluirAnuladas.CheckedChanged += (_, _) => ProgramarBusqueda();

            btnLimpiar.Click += btnLimpiar_Click;
            btnNueva.Click += btnNueva_Click;
            btnDetalle.Click += btnDetalle_Click;
            btnAnular.Click += btnAnular_Click;
            dgvCompras.DataBindingComplete += dgvCompras_DataBindingComplete;
            dgvCompras.SelectionChanged += (_, _) => ActualizarEstadoAcciones();
            dgvCompras.CellDoubleClick += (_, e) =>
            {
                if (e.RowIndex >= 0) btnDetalle.PerformClick();
            };

            _debounce.Tick += (_, _) =>
            {
                _debounce.Stop();
                BuscarCompras();
            };
            Disposed += (_, _) => _debounce.Dispose();
        }

        // Búsqueda

        private void ProgramarBusqueda()
        {
            if (_armandoFiltros) return;

            _debounce.Stop();
            _debounce.Start();
        }

        // Enter en el proveedor confirma lo escrito y busca ya mismo, sin esperar el retardo
        private void cmbProveedor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            _debounce.Stop();
            BuscarCompras();
        }

        private void BuscarCompras()
        {
            if (_armandoFiltros) return;

            if (!TryObtenerProveedorFiltrado(out int? idProveedor))
            {
                // Si falla un filtro, se limpia la grilla para no dejar resultados anteriores que parezcan vigentes.
                dgvCompras.DataSource = null;
                MostrarResumen("No hay ningún proveedor con ese nombre. Elíjalo de la lista desplegable.", esError: true);
                ActualizarEstadoAcciones();
                return;
            }

            DateTime? desde = dtpDesde.Checked ? dtpDesde.Value.Date : null;
            DateTime? hasta = dtpHasta.Checked ? dtpHasta.Value.Date : null;

            List<Compra> compras;
            try
            {
                compras = _logica.Buscar(idProveedor, desde, hasta, txtProducto.Text, chkIncluirAnuladas.Checked);
            }
            catch (ArgumentException ex)
            {
                // Filtros inconsistentes (por ejemplo, "desde" posterior a "hasta") se explican en el resumen.
                dgvCompras.DataSource = null;
                MostrarResumen(ex.Message, esError: true);
                ActualizarEstadoAcciones();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvCompras.DataSource = null;
                ActualizarEstadoAcciones();
                return;
            }

            dgvCompras.DataSource = compras.Select(c => new
            {
                c.IdCompra,
                Fecha = c.FechaCompra,
                Proveedor = NombreProveedor(c),
                Comprobante = c.NumeroComprobante ?? "-",
                RegistradaPor = NombreEmpleado(c),
                c.Total,
                Estado = c.Anulada ? "Anulada" : "Activa"
            }).ToList();

            MostrarResumenDeResultados(compras);
        }

        // Devuelve false solo si en el combo hay texto escrito que no coincide con ningún proveedor
        private bool TryObtenerProveedorFiltrado(out int? idProveedor)
        {
            idProveedor = null;

            // Si lo escrito coincide exactamente con un proveedor de la lista, queda seleccionado
            if (cmbProveedor.SelectedItem == null && !string.IsNullOrWhiteSpace(cmbProveedor.Text))
            {
                string escrito = cmbProveedor.Text.Trim();
                var coincidencia = cmbProveedor.Items.Cast<ProveedorOpcion>()
                    .FirstOrDefault(p => string.Equals(p.Texto, escrito, StringComparison.CurrentCultureIgnoreCase));

                if (coincidencia == null)
                    return false;

                cmbProveedor.SelectedItem = coincidencia;
            }

            if (cmbProveedor.SelectedItem is ProveedorOpcion elegido && elegido.IdProveedor > 0)
                idProveedor = elegido.IdProveedor;

            return true;
        }

        private static string NombreProveedor(Compra compra)
        {
            if (compra.Proveedor == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(compra.Proveedor.NombreEmpresa)) return compra.Proveedor.NombreEmpresa;

            var persona = compra.Proveedor.Persona;
            return persona == null ? string.Empty : $"{persona.Apellido} {persona.Nombre}".Trim();
        }

        private static string NombreEmpleado(Compra compra)
        {
            var persona = compra.Empleado?.Persona;
            return persona == null ? string.Empty : $"{persona.Nombre} {persona.Apellido}".Trim();
        }

        // Las compras anuladas (si se pidió incluirlas) se ven en gris para distinguirlas de las activas
        private void dgvCompras_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow fila in dgvCompras.Rows)
            {
                bool anulada = (fila.Cells["Estado"].Value as string) == "Anulada";
                fila.DefaultCellStyle.ForeColor = anulada ? Color.Gray : Color.Black;
            }
            ActualizarEstadoAcciones();
        }

        private void ActualizarEstadoAcciones()
        {
            bool haySeleccion = dgvCompras.SelectedRows.Count > 0;
            btnDetalle.Enabled = haySeleccion;
            btnAnular.Enabled = haySeleccion && EsAdministrador() &&
                (dgvCompras.SelectedRows[0].Cells["Estado"].Value as string) != "Anulada";
        }

        //Resumen debajo de la grilla
        private void MostrarResumenDeResultados(List<Compra> compras)
        {
            if (compras.Count == 0)
            {
                MostrarResumen("No se encontraron compras con los filtros elegidos.", esError: false);
                return;
            }

            // El total suma solo las compras activas: una compra anulada no es plata que se haya gastado
            int activas = compras.Count(c => !c.Anulada);
            decimal totalActivas = compras.Where(c => !c.Anulada).Sum(c => c.Total);
            int anuladas = compras.Count - activas;

            string texto = $"{compras.Count} compra{(compras.Count == 1 ? string.Empty : "s")}";
            if (anuladas > 0)
                texto += $" ({anuladas} anulada{(anuladas == 1 ? string.Empty : "s")})";
            texto += $"   ·   Total de compras activas: {totalActivas.ToString("C2", Es)}";

            MostrarResumen(texto, esError: false);
        }

        private void MostrarResumen(string mensaje, bool esError)
        {
            lblResumen.ForeColor = esError ? Color.Firebrick : Color.DimGray;
            lblResumen.Text = mensaje;
        }

        // Botones

        private void btnLimpiar_Click(object? sender, EventArgs e)
        {
            _armandoFiltros = true;
            try { RestablecerFiltros(); }
            finally { _armandoFiltros = false; }

            _debounce.Stop();
            BuscarCompras();
        }

        // El alta es una ventana modal; si se registró una compra, se recarga el listado para que aparezca
        private void btnNueva_Click(object? sender, EventArgs e)
        {
            using var frm = new FrmAltaCompra();
            frm.ShowDialog(this);

            if (frm.CompraRegistrada)
            {
                // La compra nueva tiene fecha de hoy: si los filtros la dejaban afuera, se vuelve a la vista por defecto para que se vea
                _armandoFiltros = true;
                try { RestablecerFiltros(); }
                finally { _armandoFiltros = false; }

                _debounce.Stop();
                BuscarCompras();
            }
        }

        private void btnDetalle_Click(object? sender, EventArgs e)
        {
            if (!TryObtenerCompraSeleccionada(out int idCompra))
                return;

            try
            {
                var compra = _logica.ObtenerPorId(idCompra);
                if (compra == null)
                {
                    MessageBox.Show(this, "La compra seleccionada ya no existe.", "Compra no encontrada",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BuscarCompras();
                    return;
                }

                using var detalle = new FrmDetalleCompra(compra);
                detalle.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudo cargar el detalle de la compra: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAnular_Click(object? sender, EventArgs e)
        {
            if (!EsAdministrador())
            {
                MessageBox.Show(this, "Solo un administrador puede anular compras.",
                    "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!TryObtenerCompraSeleccionada(out int idCompra))
                return;

            Compra? compra;
            try
            {
                compra = _logica.ObtenerPorId(idCompra);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudo consultar la compra: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (compra == null)
            {
                MessageBox.Show(this, "La compra seleccionada ya no existe.", "Compra no encontrada",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                BuscarCompras();
                return;
            }

            if (compra.Anulada)
            {
                MessageBox.Show(this, "Esta compra ya está anulada.", "Compra anulada",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(this,
                $"¿Seguro que quiere anular la compra N° {compra.IdCompra}?\n\nSe descontará el stock ingresado. Esta acción no se puede deshacer.",
                "Confirmar anulación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                _logica.AnularCompra(idCompra);
                MessageBox.Show(this, "La compra fue anulada y se actualizó el stock, costo y proveedor de sus productos.",
                    "Compra anulada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BuscarCompras();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo anular la compra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(this, ex.Message, "Acceso restringido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"No se pudo anular la compra: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryObtenerCompraSeleccionada(out int idCompra)
        {
            idCompra = 0;
            if (dgvCompras.SelectedRows.Count == 0 ||
                !int.TryParse(dgvCompras.SelectedRows[0].Cells["IdCompra"].Value?.ToString(), out idCompra))
            {
                MessageBox.Show(this, "Seleccione una compra de la grilla.", "Sin selección",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        private static bool EsAdministrador()
            => string.Equals(global::cantinaPadel.Sesion.Rol, "Admin", StringComparison.Ordinal);
    }
}
