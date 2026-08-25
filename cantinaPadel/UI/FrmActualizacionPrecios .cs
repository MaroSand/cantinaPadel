using cantinaPadel.BLL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace cantinaPadel.UI
{

    // Formulario para actualizar precios de productos en lote, con filtros
    public partial class FrmActualizacionPrecios : Form
    {

        private static readonly CultureInfo _culturaPesos = CrearCulturaPesos();

        // Crea una cultura clonada de la actual, pero con símbolo de moneda "$"
        private static CultureInfo CrearCulturaPesos()
        {
            var cultura = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultura.NumberFormat.CurrencySymbol = "$";
            return cultura;
        }
        private readonly LogicaProducto _logicaProducto;
        // Listado de productos que se muestra en la grilla, con sus precios

        private List<ProductoPrecioPreview> _listado = new();

        private readonly BindingSource _bindingSource = new();

        // Timer para hacer debounce en la búsqueda de productos al tipear
        private readonly System.Windows.Forms.Timer _debounceTexto = new() { Interval = 350 };

        private bool _refrescandoGrilla;

        // Constructor del formulario, inicializa componentes y lógica de negocio
        public FrmActualizacionPrecios()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
        }

        // Evento Load del formulario, configura la grilla, carga los combos y asigna eventos
        private void FrmActualizacionPrecios_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarCombos();
            // Asignar eventos a los controles
            _debounceTexto.Tick += (s, e2) => { _debounceTexto.Stop(); ActualizarListado(); };
            txtProductoFiltro.TextChanged += (s, e2) => { _debounceTexto.Stop(); _debounceTexto.Start(); };
            // Cuando cambian los combos de categoría, marca o proveedor, se actualiza el listado
            cmbCategoriaFiltro.SelectedIndexChanged += (s, e2) => ActualizarListado();
            cmbMarcaFiltro.SelectedIndexChanged += (s, e2) => ActualizarListado();
            cmbProveedorFiltro.SelectedIndexChanged += (s, e2) => ActualizarListado();
            nudPorcentaje.ValueChanged += (s, e2) => RecalcularPreciosPorPorcentaje();
            dgvPreview.CellContentClick += dgvPreview_CellContentClick;

            rbPorcentaje.CheckedChanged += (s, e2) => AlternarModo();
            rbManual.CheckedChanged += (s, e2) => AlternarModo();

            btnAplicarPrecioManual.Click += btnAplicarPrecioManual_Click;
            btnConfirmar.Click += btnConfirmar_Click;

            AlternarModo();
            ActualizarListado();
        }
        // Configura la grilla de vista previa de productos y precios, con columnas específicas
        private void ConfigurarGrilla()
        {
            dgvPreview.AutoGenerateColumns = false;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.ReadOnly = false;
            dgvPreview.MultiSelect = false;
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.DataSource = _bindingSource;
            // Configura las columnas de la grilla
            dgvPreview.Columns.Clear();
            dgvPreview.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "Aplicar",
                DataPropertyName = "Aplicar",
                HeaderText = "Aplicar",
                Width = 60,
                ReadOnly = false
            });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                DataPropertyName = "Nombre",
                HeaderText = "Nombre",
                Width = 280,
                ReadOnly = true
            });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioActual",
                DataPropertyName = "PrecioActual",
                HeaderText = "Precio Actual",
                Width = 130,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    FormatProvider = _culturaPesos,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioNuevo",
                DataPropertyName = "PrecioNuevo",
                HeaderText = "Precio Nuevo",
                Width = 130,
                // Ya no se edita directo en la grilla: se calcula por
                // porcentaje o se asigna con el campo "Precio manual".
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    FormatProvider = _culturaPesos,
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.WhiteSmoke
                }
            });
        }

        // Carga los combos de categoría, marca y proveedor con opciones activas.
        // Categoría y marca NO tienen opción "Todas": el usuario siempre tiene
        // que elegir una puntual, así se evita traer/actualizar el catálogo
        // entero por error. Proveedor sí mantiene "Todos" porque es un filtro
        // opcional adicional.
        private void CargarCombos()
        {
            try
            {
                var categorias = _logicaProducto.ObtenerCategoriasActivas()
                    .Select(c => new { Id = (int?)c.IdCategoria, Nombre = c.Nombre })
                    .ToList();
                cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbCategoriaFiltro.DisplayMember = "Nombre";
                cmbCategoriaFiltro.ValueMember = "Id";
                cmbCategoriaFiltro.DataSource = categorias;

                var marcas = _logicaProducto.ObtenerMarcasActivas()
                    .Select(m => new { Id = (int?)m.IdMarca, Nombre = m.Nombre })
                    .ToList();
                cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMarcaFiltro.DisplayMember = "Nombre";
                cmbMarcaFiltro.ValueMember = "Id";
                cmbMarcaFiltro.DataSource = marcas;

                var proveedores = _logicaProducto.ObtenerProveedoresActivos()
                    .Select(p => new { Id = (int?)p.IdProveedor, Nombre = p.NombreEmpresa })
                    .ToList();
                proveedores.Insert(0, new { Id = (int?)null, Nombre = "Todos los proveedores" });
                cmbProveedorFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbProveedorFiltro.DisplayMember = "Nombre";
                cmbProveedorFiltro.ValueMember = "Id";
                cmbProveedorFiltro.DataSource = proveedores;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los combos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Actualiza el listado de productos según los filtros y el porcentaje, preservando precios editados manualmente
        private void ActualizarListado()
        {
            try
            {
                string? texto = string.IsNullOrWhiteSpace(txtProductoFiltro.Text) ? null : txtProductoFiltro.Text.Trim();
                int? idCategoria = cmbCategoriaFiltro.SelectedValue as int?;
                int? idMarca = cmbMarcaFiltro.SelectedValue as int?;
                int? idProveedor = cmbProveedorFiltro.SelectedValue as int?;
                decimal porcentaje = nudPorcentaje.Value;

                // Guardamos los precios editados manualmente antes de recargar el listado
                var manuales = _listado
                    .Where(p => p.EditadoManualmente)
                    .ToDictionary(p => p.IdProducto, p => p.PrecioNuevo);

                // Buscamos los productos según los filtros y el porcentaje
                _listado = _logicaProducto.BuscarParaActualizacion(texto, idCategoria, idMarca, idProveedor, porcentaje);

                // Restauramos los precios editados manualmente en el nuevo listado
                foreach (var item in _listado)
                {
                    if (manuales.TryGetValue(item.IdProducto, out var precioManual))
                    {
                        item.PrecioNuevo = precioManual;
                        item.EditadoManualmente = true;
                    }
                }

                _refrescandoGrilla = true;
                _bindingSource.DataSource = _listado;
                _refrescandoGrilla = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Recalcula los precios nuevos de los productos en la grilla según el porcentaje ingresado,
        private void RecalcularPreciosPorPorcentaje()
        {
            if (_listado.Count == 0) return;

            decimal factor = 1 + (nudPorcentaje.Value / 100m);
            foreach (var item in _listado)
            {
                if (!item.EditadoManualmente)
                    item.PrecioNuevo = Math.Round(item.PrecioActual * factor, 2);
            }

            _refrescandoGrilla = true;
            _bindingSource.ResetBindings(false);
            _refrescandoGrilla = false;
        }

        // El usuario hizo click en el checkbox "Aplicar" de la grilla: confirmamos el cambio
        private void dgvPreview_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPreview.Columns[e.ColumnIndex].Name != "Aplicar") return;

            dgvPreview.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }


        // Alterna entre modo "Por porcentaje" y modo "Precio manual": habilita
        // los controles del modo activo, deshabilita los del otro, y descarta
        // los precios calculados/cargados en el modo anterior para no mezclarlos.
        private void AlternarModo()
        {
            bool porPorcentaje = rbPorcentaje.Checked;

            nudPorcentaje.Enabled = porPorcentaje;
            nudPrecioManual.Enabled = !porPorcentaje;
            btnAplicarPrecioManual.Enabled = !porPorcentaje;

            foreach (var item in _listado)
            {
                item.EditadoManualmente = false;
                item.PrecioNuevo = item.PrecioActual;
            }

            if (porPorcentaje)
                RecalcularPreciosPorPorcentaje();

            _refrescandoGrilla = true;
            _bindingSource.ResetBindings(false);
            _refrescandoGrilla = false;
        }

        // El usuario cargó un precio en el campo "Precio manual" y hace click en "Aplicar a tildados":
        // pisa el PrecioNuevo de los productos tildados en la columna "Aplicar" y los marca como editados a mano
        // (para que el recálculo automático por porcentaje no los sobrescriba).
        private void btnAplicarPrecioManual_Click(object? sender, EventArgs e)
        {
            var seleccionados = _listado.Where(p => p.Aplicar).ToList();

            if (seleccionados.Count == 0)
            {
                MessageBox.Show("Tildá al menos un producto en la columna \"Aplicar\" para asignarle el precio manual.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal precioManual = nudPrecioManual.Value;

            // No se permite $0 ni negativos: el precio manual tiene que ser mayor a $0.
            if (precioManual <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a $0.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var item in seleccionados)
            {
                item.PrecioNuevo = precioManual;
                item.EditadoManualmente = true;
            }

            _refrescandoGrilla = true;
            _bindingSource.ResetBindings(false);
            _refrescandoGrilla = false;
        }

        // El usuario hizo click en el botón "Confirmar": valida y confirma la actualización de precios
        private void btnConfirmar_Click(object? sender, EventArgs e)
        {
            var seleccionados = _listado.Where(p => p.Aplicar).ToList();

            if (seleccionados.Count == 0)
            {
                MessageBox.Show("No hay productos tildados para actualizar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Confirma la actualización de precios para {seleccionados.Count} producto(s)?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                var preciosFinales = seleccionados
                    .GroupBy(p => p.IdProducto)
                    .ToDictionary(g => g.Key, g => g.First().PrecioNuevo);

                _logicaProducto.ConfirmarActualizacion(preciosFinales);

                MessageBox.Show("Precios actualizados correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recarga el listado para reflejar los nuevos "Precio Actual"
                ActualizarListado();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar la actualización: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}