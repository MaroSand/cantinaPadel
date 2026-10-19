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

        // Evita consultar la base mientras se arman o se reinician los filtros por código (cada asignación dispara eventos)
        private bool _armandoFiltros;

        public FrmListadoCompras()
        {
            InitializeComponent();
            _logica = new LogicaCompra();

            Load += FrmListadoCompras_Load;
        }

        // ================= Carga inicial =================

        private void FrmListadoCompras_Load(object? sender, EventArgs e)
        {
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

                txtProducto.MaxLength = 100;
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

            // Name es la clave para Cells["..."], DataPropertyName bindea con el objeto de la fila
            dgvCompras.Columns.Clear();
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "IdCompra", DataPropertyName = "IdCompra", HeaderText = "N°", FillWeight = 8 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                DataPropertyName = "Fecha",
                HeaderText = "Fecha",
                FillWeight = 16,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Proveedor", DataPropertyName = "Proveedor", HeaderText = "Proveedor", FillWeight = 28 });
            dgvCompras.Columns.Add(new DataGridViewTextBoxColumn { Name = "Comprobante", DataPropertyName = "Comprobante", HeaderText = "N° de comprobante", FillWeight = 18 });
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
            dgvCompras.DataBindingComplete += dgvCompras_DataBindingComplete;

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
                // Se escribió algo que no corresponde a ningún proveedor: se avisa en pantalla (no en una ventana) y la grilla queda como estaba
                MostrarResumen("No hay ningún proveedor con ese nombre. Elíjalo de la lista desplegable.", esError: true);
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
                // Filtros inconsistentes (por ejemplo, "desde" posterior a "hasta"): se explica en pantalla y se conserva la grilla anterior
                MostrarResumen(ex.Message, esError: true);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}