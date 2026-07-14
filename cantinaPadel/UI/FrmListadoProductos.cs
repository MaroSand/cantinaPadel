using System;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmListadoProductos : Form
    {
        // Se declaran las variables globales
        private readonly LogicaProducto _logicaProducto;

        public FrmListadoProductos()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
        }

        private void FrmListadoProductos_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarCombosFiltro();

            // Se suscriben los eventos de controles
            txtBuscar.TextChanged += txtBuscar_TextChanged;
            txtBuscar.KeyDown += txtBuscar_KeyDown;
            cmbCategoriaFiltro.SelectedIndexChanged += Filtro_SelectedIndexChanged;
            cmbMarcaFiltro.SelectedIndexChanged += Filtro_SelectedIndexChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnModificar.Click += btnModificar_Click;
            btnBajaLogica.Click += btnBajaLogica_Click;
            // Manejo de pestañas: cargar formularios de Marcas / Categorías cuando se seleccione la pestaña
            tabControlMain.SelectedIndexChanged += TabControlMain_SelectedIndexChanged;

            ActualizarListado();
        }

        private void TabControlMain_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // Si la pestaña seleccionada es Marcas o Categorías, embebemos el formulario correspondiente
            if (tabControlMain.SelectedTab == tabPageMarcas)
            {
                CargarFormularioEnTab(typeof(FrmMarcas), tabPageMarcas);
            }
            else if (tabControlMain.SelectedTab == tabPageCategorias)
            {
                CargarFormularioEnTab(typeof(FrmCategorias), tabPageCategorias);
            }
            else if (tabControlMain.SelectedTab == tabPageActualizacionPrecios)
            {
                CargarFormularioEnTab(typeof(FrmActualizacionPrecios), tabPageActualizacionPrecios);
            }
        }

        private void CargarFormularioEnTab(Type formType, TabPage tab)
        {
            // Ya está cargado
            if (tab.Controls.Count > 0) return;

            var formulario = (Form?)Activator.CreateInstance(formType);
            if (formulario == null) return;

            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            tab.Controls.Add(formulario);
            formulario.Show();
        }

        private void ConfigurarGrilla()
        {
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.ReadOnly = true;
            dgvProductos.MultiSelect = false;

            // Columnas: Name es clave para Cells["..."], DataPropertyName bindea con el objeto anónimo
            dgvProductos.Columns.Clear();
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
                { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
                { Name = "Nombre", DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 180 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CodigoBarras", DataPropertyName = "CodigoBarras", HeaderText = "Código de Barras", Width = 130
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
                { Name = "Categoria", DataPropertyName = "Categoria", HeaderText = "Categoría", Width = 110 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
                { Name = "Marca", DataPropertyName = "Marca", HeaderText = "Marca", Width = 100 });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioVenta",
                DataPropertyName = "PrecioVenta",
                HeaderText = "Precio",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                    { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioConIva",
                DataPropertyName = "PrecioConIva",
                HeaderText = "Precio c/IVA",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                    { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StockActual",
                DataPropertyName = "StockActual",
                HeaderText = "Stock",
                Width = 70,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvProductos.Columns.Add(new DataGridViewCheckBoxColumn
                { Name = "StockBajo", DataPropertyName = "StockBajo", HeaderText = "Stock Bajo", Width = 80 });
        }

        // Carga los combos de filtro con las categorías y marcas activas, agregando
        // una opción "(Todas)" con valor null para no aplicar ese filtro
        private void CargarCombosFiltro()
        {
            try
            {
                var categorias = _logicaProducto.ObtenerCategoriasActivas()
                    .Select(c => new { Id = (int?)c.IdCategoria, Nombre = c.Nombre })
                    .ToList();
                categorias.Insert(0, new { Id = (int?)null, Nombre = "(Todas las categorías)" });

                cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbCategoriaFiltro.DisplayMember = "Nombre";
                cmbCategoriaFiltro.ValueMember = "Id";
                cmbCategoriaFiltro.DataSource = categorias;
                cmbCategoriaFiltro.SelectedIndex = 0;

                var marcas = _logicaProducto.ObtenerMarcasActivas()
                    .Select(m => new { Id = (int?)m.IdMarca, Nombre = m.Nombre })
                    .ToList();
                marcas.Insert(0, new { Id = (int?)null, Nombre = "(Todas las marcas)" });

                cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMarcaFiltro.DisplayMember = "Nombre";
                cmbMarcaFiltro.ValueMember = "Id";
                cmbMarcaFiltro.DataSource = marcas;
                cmbMarcaFiltro.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los filtros: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Vuelve a consultar la base con los filtros actuales. A diferencia de otros listados,
        // Producto expone un único método "Buscar" que ya combina texto + categoría + marca,
        // por lo que no hace falta mantener una lista completa en memoria para filtrar.
        private void ActualizarListado()
        {
            try
            {
                string? texto = string.IsNullOrWhiteSpace(txtBuscar.Text) ? null : txtBuscar.Text.Trim();
                int? idCategoria = cmbCategoriaFiltro.SelectedValue as int?;
                int? idMarca = cmbMarcaFiltro.SelectedValue as int?;

                var productos = _logicaProducto.Buscar(texto, idCategoria, idMarca);

                dgvProductos.DataSource = productos.Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    p.CodigoBarras,
                    Categoria = p.Categoria.Nombre,
                    Marca = p.Marca?.Nombre ?? "S/M",
                    p.PrecioVenta,
                    p.PrecioConIva,
                    p.StockActual,
                    p.StockBajo
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Eventos filtros ────────────────────────────────────────────────────
        private void txtBuscar_TextChanged(object sender, EventArgs e) => ActualizarListado();
        private void Filtro_SelectedIndexChanged(object sender, EventArgs e) => ActualizarListado();

        // Soporte para lector de código de barras (HID): el lector "tipea" el código
        // y termina enviando Enter. Se anula el "beep"/efecto por defecto y, si el
        // filtro dejó un único resultado, se lo selecciona en la grilla al toque.
        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;

            if (dgvProductos.Rows.Count == 1)
            {
                dgvProductos.CurrentCell = dgvProductos.Rows[0].Cells["Nombre"];
                dgvProductos.Focus();
            }
        }

        // ── Botones de acción ──────────────────────────────────────────────────
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var frm = new FrmCRUDProducto();
            frm.ShowDialog();
            ActualizarListado();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvProductos.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione un producto para modificar.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var cell = dgvProductos.CurrentRow.Cells["IdProducto"];
                if (cell == null || cell.Value == null)
                {
                    MessageBox.Show("No se pudo determinar el producto seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(cell.Value.ToString(), out int idProducto))
                {
                    MessageBox.Show("Id de producto inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Producto? producto = _logicaProducto.ObtenerPorId(idProducto);
                if (producto == null) return;

                var frm = new FrmCRUDProducto(producto);
                frm.ShowDialog();
                ActualizarListado();
            }
            catch (Exception ex)
            {
                // Mostrar detalle completo para depuración
                MessageBox.Show($"Excepción al modificar producto:\n{ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBajaLogica_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto para dar de baja.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Desea dar de baja al producto seleccionado? Dejará de aparecer en el listado.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            var cell2 = dgvProductos.CurrentRow.Cells["IdProducto"];
            if (cell2 == null || cell2.Value == null)
            {
                MessageBox.Show("No se pudo determinar el producto seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(cell2.Value.ToString(), out int idProducto))
            {
                MessageBox.Show("Id de producto inválido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _logicaProducto.BajaLogica(idProducto);
                ActualizarListado();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al dar de baja: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}