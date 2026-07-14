using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;

namespace cantinaPadel.UI
{
    public partial class FrmActualizacionPrecios : Form
    {
        private readonly LogicaProducto _logicaProducto;

        // Resultado del último listado (según los filtros vigentes). Confirmar
        // aplica sobre los ítems de esta lista que tengan Aplicar = true, con
        // el PrecioNuevo que haya quedado en cada fila (por % o a mano).
        private List<ProductoPrecioPreview> _listado = new();

        // Envuelve _listado para que la grilla se refresque sola (ResetBindings)
        // cuando recalculamos precios por código, sin tener que reasignar DataSource.
        private readonly BindingSource _bindingSource = new();

        // Espera una pausa corta después de la última tecla antes de buscar,
        // para no pegarle a la base con una consulta por carácter tipeado.
        private readonly System.Windows.Forms.Timer _debounceTexto = new() { Interval = 350 };

        // Evita que el recálculo/refresco de la grilla dispare por error el
        // handler de "edité una celda a mano".
        private bool _refrescandoGrilla;

        public FrmActualizacionPrecios()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
        }

        private void FrmActualizacionPrecios_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarCombos();

            _debounceTexto.Tick += (s, e2) => { _debounceTexto.Stop(); ActualizarListado(); };
            txtProductoFiltro.TextChanged += (s, e2) => { _debounceTexto.Stop(); _debounceTexto.Start(); };

            cmbCategoriaFiltro.SelectedIndexChanged += (s, e2) => ActualizarListado();
            cmbMarcaFiltro.SelectedIndexChanged += (s, e2) => ActualizarListado();
            nudPorcentaje.ValueChanged += (s, e2) => RecalcularPreciosPorPorcentaje();

            dgvPreview.CellContentClick += dgvPreview_CellContentClick;
            dgvPreview.CellEndEdit += dgvPreview_CellEndEdit;
            dgvPreview.DataError += dgvPreview_DataError;

            btnConfirmar.Click += btnConfirmar_Click;

            // Primer listado: sin filtros = todos los productos activos,
            // igual que el patrón ya usado en FrmListadoProductos.
            ActualizarListado();
        }

        private void ConfigurarGrilla()
        {
            dgvPreview.AutoGenerateColumns = false;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.ReadOnly = false;
            dgvPreview.MultiSelect = false;
            dgvPreview.DataSource = _bindingSource;

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
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioNuevo",
                DataPropertyName = "PrecioNuevo",
                HeaderText = "Precio Nuevo",
                Width = 130,
                // Editable: click y tipeás. Sirve para aumentos que no son
                // por porcentaje (ej. sumar $5 fijo a un producto puntual).
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    BackColor = Color.LightYellow
                }
            });
        }

        // Carga los combos de categoría y marca con una opción "Todas" (Id
        // null) al principio, para que puedan quedar sin filtrar.
        private void CargarCombos()
        {
            try
            {
                var categorias = _logicaProducto.ObtenerCategoriasActivas()
                    .Select(c => new { Id = (int?)c.IdCategoria, Nombre = c.Nombre })
                    .ToList();
                categorias.Insert(0, new { Id = (int?)null, Nombre = "Todas las categorías" });
                cmbCategoriaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbCategoriaFiltro.DisplayMember = "Nombre";
                cmbCategoriaFiltro.ValueMember = "Id";
                cmbCategoriaFiltro.DataSource = categorias;

                var marcas = _logicaProducto.ObtenerMarcasActivas()
                    .Select(m => new { Id = (int?)m.IdMarca, Nombre = m.Nombre })
                    .ToList();
                marcas.Insert(0, new { Id = (int?)null, Nombre = "Todas las marcas" });
                cmbMarcaFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMarcaFiltro.DisplayMember = "Nombre";
                cmbMarcaFiltro.ValueMember = "Id";
                cmbMarcaFiltro.DataSource = marcas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los combos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Dispara la búsqueda con los 3 filtros combinados (texto + categoría
        // + marca), sin botón. Se llama al tipear (con debounce) y al cambiar
        // cualquiera de los combos.
        private void ActualizarListado()
        {
            try
            {
                string? texto = string.IsNullOrWhiteSpace(txtProductoFiltro.Text) ? null : txtProductoFiltro.Text.Trim();
                int? idCategoria = cmbCategoriaFiltro.SelectedValue as int?;
                int? idMarca = cmbMarcaFiltro.SelectedValue as int?;
                decimal porcentaje = nudPorcentaje.Value;

                // Antes de pisar el listado, guardamos qué precios habían
                // sido editados a mano, para no perderlos si el producto
                // sigue apareciendo en el nuevo resultado.
                var manuales = _listado
                    .Where(p => p.EditadoManualmente)
                    .ToDictionary(p => p.IdProducto, p => p.PrecioNuevo);

                _listado = _logicaProducto.BuscarParaActualizacion(texto, idCategoria, idMarca, porcentaje);

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

        // Recalcula "Precio Nuevo" para todas las filas que NO fueron editadas
        // a mano, cada vez que cambia el porcentaje. Las editadas a mano quedan
        // como el usuario las dejó.
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

        // Commitea el checkbox al toque, sin esperar a que la celda pierda el foco.
        private void dgvPreview_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPreview.Columns[e.ColumnIndex].Name != "Aplicar") return;

            dgvPreview.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // El usuario tipeó un precio distinto en "Precio Nuevo": lo marcamos
        // como editado a mano para que el porcentaje deje de pisarlo.
        private void dgvPreview_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (_refrescandoGrilla) return;
            if (e.RowIndex < 0) return;
            if (dgvPreview.Columns[e.ColumnIndex].Name != "PrecioNuevo") return;
            if (e.RowIndex >= _listado.Count) return;

            var item = _listado[e.RowIndex];

            if (item.PrecioNuevo < 0)
            {
                item.PrecioNuevo = item.PrecioActual;
                MessageBox.Show("El precio no puede ser negativo.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _refrescandoGrilla = true;
                _bindingSource.ResetBindings(false);
                _refrescandoGrilla = false;
                return;
            }

            item.EditadoManualmente = true;
        }

        // Si tipean algo que no es un número válido, avisamos y cancelamos
        // en vez de dejar que la excepción tire abajo la grilla.
        private void dgvPreview_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Ingresá un valor numérico válido para el precio.",
                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.ThrowException = false;
            e.Cancel = true;
        }

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