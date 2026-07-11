using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.DAL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCategorias : Form
    {
        private readonly LogicaCategoria _logicaCategoria;
        private Categoria? _categoriaSeleccionada = null;

        public FrmCategorias()
        {
            InitializeComponent();
            _logicaCategoria = new LogicaCategoria(new AppDbContext());
        }

        private void FrmCategorias_Load(object sender, EventArgs e)
        {
            cmbEstado.SelectedIndex = 0; // "Todos"
            ActualizarGrilla();
            LimpiarFormulario();
        }

        private void ActualizarGrilla()
        {
            try
            {
                var lista = _logicaCategoria.ListarTodas();

                string buscar = txtBuscar.Text.ToLower().Trim();
                if (!string.IsNullOrEmpty(buscar))
                {
                    lista = lista.Where(c => c.Nombre.ToLower().Contains(buscar)).ToList();
                }

                if (cmbEstado.SelectedIndex == 1)
                {
                    lista = lista.Where(c => c.Activa).ToList();
                }
                else if (cmbEstado.SelectedIndex == 2)
                {
                    lista = lista.Where(c => !c.Activa).ToList();
                }

                dgvCategorias.DataSource = null;
                dgvCategorias.DataSource = lista;

                var colId = dgvCategorias.Columns["IdCategoria"];
                if (colId != null) colId.HeaderText = "ID";

                var colNombre = dgvCategorias.Columns["Nombre"];
                if (colNombre != null) colNombre.HeaderText = "Categoría";

                var colActiva = dgvCategorias.Columns["Activa"];
                if (colActiva != null) colActiva.HeaderText = "Activa";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la grilla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => ActualizarGrilla();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => ActualizarGrilla();

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow != null && dgvCategorias.CurrentRow.DataBoundItem is Categoria cat)
            {
                _categoriaSeleccionada = cat;
                txtNombre.Text = _categoriaSeleccionada.Nombre;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var cat = _categoriaSeleccionada ?? new Categoria();
                cat.Nombre = txtNombre.Text;

                _logicaCategoria.Guardar(cat);

                MessageBox.Show("Categoría guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActualizarGrilla();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e) => LimpiarFormulario();

        private void btnBajaAlta_Click(object sender, EventArgs e)
        {
            if (_categoriaSeleccionada == null)
            {
                MessageBox.Show("Seleccione una categoría primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstado = !_categoriaSeleccionada.Activa;
                string accion = nuevoEstado ? "dar de alta" : "dar de baja";

                var confirmacion = MessageBox.Show($"¿Desea {accion} la categoría '{_categoriaSeleccionada.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    _logicaCategoria.CambiarEstado(_categoriaSeleccionada.IdCategoria, nuevoEstado);
                    ActualizarGrilla();
                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            _categoriaSeleccionada = null;
            txtNombre.Clear();
            if (dgvCategorias.CurrentRow != null) dgvCategorias.CurrentRow.Selected = false;
            txtNombre.Focus();
        }
    }
}