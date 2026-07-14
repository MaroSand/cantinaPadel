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

        // Guarda la última previsualización, para que Confirmar aplique
        // exactamente sobre los productos que se mostraron en pantalla
        // (y no vuelva a resolver el criterio, por si el usuario tocó los combos entremedio).
        private List<ProductoPrecioPreview> _ultimaPreview = new();

        public FrmActualizacionPrecios()
        {
            InitializeComponent();
            _logicaProducto = new LogicaProducto();
        }

        private void FrmActualizacionPrecios_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarCombos();

            cmbCriterio.SelectedIndexChanged += cmbCriterio_SelectedIndexChanged;
            btnPrevisualizar.Click += btnPrevisualizar_Click;
            btnConfirmar.Click += btnConfirmar_Click;

            cmbCriterio.SelectedIndex = 0;
            ActualizarVisibilidadCombos();
        }

        private void ConfigurarGrilla()
        {
            dgvPreview.AutoGenerateColumns = false;
            dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPreview.ReadOnly = true;
            dgvPreview.MultiSelect = false;

            dgvPreview.Columns.Clear();
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "IdProducto", DataPropertyName = "IdProducto", Visible = false });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "Nombre", DataPropertyName = "Nombre", HeaderText = "Nombre", Width = 280 });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioActual",
                DataPropertyName = "PrecioActual",
                HeaderText = "Precio Actual",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvPreview.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioNuevo",
                DataPropertyName = "PrecioNuevo",
                HeaderText = "Precio Nuevo",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "C2", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
        }

        // Carga cmbCriterio (fijo) y los tres combos de valores (categoría, marca, producto)
        private void CargarCombos()
        {
            try
            {
                cmbCriterio.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbCriterio.Items.Clear();
                cmbCriterio.Items.Add("Categoría");
                cmbCriterio.Items.Add("Marca");
                cmbCriterio.Items.Add("Producto individual");

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

                var productos = _logicaProducto.ObtenerTodos()
                    .Select(p => new { Id = (int?)p.IdProducto, Nombre = p.Nombre })
                    .ToList();
                cmbProductoFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbProductoFiltro.DisplayMember = "Nombre";
                cmbProductoFiltro.ValueMember = "Id";
                cmbProductoFiltro.DataSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los combos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbCriterio_SelectedIndexChanged(object? sender, EventArgs e)
            => ActualizarVisibilidadCombos();

        // Muestra únicamente el combo correspondiente al criterio elegido.
        // Los tres están superpuestos en el mismo lugar del Designer.
        private void ActualizarVisibilidadCombos()
        {
            cmbCategoriaFiltro.Visible = cmbCriterio.SelectedIndex == 0;
            lblCategoria.Visible = cmbCriterio.SelectedIndex == 0;

            cmbMarcaFiltro.Visible = cmbCriterio.SelectedIndex == 1;
            lblMarca.Visible = cmbCriterio.SelectedIndex == 1;

            cmbProductoFiltro.Visible = cmbCriterio.SelectedIndex == 2;
            lblProducto.Visible = cmbCriterio.SelectedIndex == 2;

            // Cambió el criterio: la preview anterior ya no es válida
            _ultimaPreview.Clear();
            dgvPreview.DataSource = null;
        }

        private void btnPrevisualizar_Click(object? sender, EventArgs e)
        {
            try
            {
                int? idCategoria = cmbCriterio.SelectedIndex == 0 ? cmbCategoriaFiltro.SelectedValue as int? : null;
                int? idMarca = cmbCriterio.SelectedIndex == 1 ? cmbMarcaFiltro.SelectedValue as int? : null;
                int? idProducto = cmbCriterio.SelectedIndex == 2 ? cmbProductoFiltro.SelectedValue as int? : null;

                if (idCategoria == null && idMarca == null && idProducto == null)
                {
                    MessageBox.Show("Seleccione un valor para el criterio elegido.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal porcentaje = nudPorcentaje.Value;

                _ultimaPreview = _logicaProducto.PrevisualizarActualizacion(idCategoria, idMarca, idProducto, porcentaje);

                if (_ultimaPreview.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos para el criterio seleccionado.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dgvPreview.DataSource = _ultimaPreview;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al previsualizar: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConfirmar_Click(object? sender, EventArgs e)
        {
            if (_ultimaPreview.Count == 0)
            {
                MessageBox.Show("Primero generá una previsualización antes de confirmar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmacion = MessageBox.Show(
                $"¿Confirma la actualización de precios para {_ultimaPreview.Count} producto(s)?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            try
            {
                var ids = _ultimaPreview.Select(p => p.IdProducto).ToList();
                decimal porcentaje = nudPorcentaje.Value;

                _logicaProducto.ConfirmarActualizacion(ids, porcentaje);

                MessageBox.Show("Precios actualizados correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _ultimaPreview.Clear();
                dgvPreview.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar la actualización: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}