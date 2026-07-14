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
        // Se conecta con la capa de negocio
        private readonly LogicaCategoria _logicaCategoria;

        // Se guarda de forma global la categoría que se toque en la grilla
        private Categoria? _categoriaSeleccionada = null;

        public FrmCategorias()
        {
            InitializeComponent();
            // Se inicializa la lógica pasándole la bd
            _logicaCategoria = new LogicaCategoria(new AppDbContext());
        }

        private void FrmCategorias_Load(object sender, EventArgs e)
        {
            // Se preselecciona "Todos" en el combo, se llena la grilla y se limpia la pantalla
            cmbEstado.SelectedIndex = 0;
            ActualizarGrilla();
            LimpiarFormulario();
        }

        private void ActualizarGrilla()
        {
            try
            {
                // Se trae la lista completa desde la base de datos
                var lista = _logicaCategoria.ListarTodas();

                // Se filtra por lo que se escribe en el buscador
                string buscar = txtBuscar.Text.ToLower().Trim();
                if (!string.IsNullOrEmpty(buscar))
                {
                    lista = lista.Where(c => c.Nombre.ToLower().Contains(buscar)).ToList();
                }

                // Se filtra según el estado elegido (Activos o Inactivos)
                if (cmbEstado.SelectedIndex == 1)
                {
                    lista = lista.Where(c => c.Activa).ToList();
                }
                else if (cmbEstado.SelectedIndex == 2)
                {
                    lista = lista.Where(c => !c.Activa).ToList();
                }

                // Se refrescan los datos de la grilla
                dgvCategorias.DataSource = null;
                dgvCategorias.DataSource = lista;

                // Ocultar is de forma segura contra nulls
                if (dgvCategorias.Columns["IdCategoria"] is DataGridViewColumn colId)
                {
                    colId.Visible = false;
                }

                // Se acomodan los títulos de las columnas para que queden prolijos
                var colNombre = dgvCategorias.Columns["Nombre"];
                if (colNombre != null) colNombre.HeaderText = "Categoría";

                var colActiva = dgvCategorias.Columns["Activa"];
                if (colActiva != null) colActiva.HeaderText = "Activa";
            }
            catch (Exception ex)
            {
                // Se avisa si falla la carga por algún problema de conexión
                MessageBox.Show($"Error al cargar la grilla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Se actualiza la grilla en tiempo real al escribir o cambiar el combo
        private void txtBuscar_TextChanged(object sender, EventArgs e) => ActualizarGrilla();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => ActualizarGrilla();

        private void dgvCategorias_SelectionChanged(object sender, EventArgs e)
        {
            // Se pasa la categoría elegida a los campos de texto para poder editarla
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
                // Se crea una categoría nueva o se edita la que ya estaba seleccionada
                var cat = _categoriaSeleccionada ?? new Categoria();
                cat.Nombre = txtNombre.Text;

                // Se manda a la capa de negocio para validarla y guardarla
                _logicaCategoria.Guardar(cat);

                MessageBox.Show("Categoría guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ActualizarGrilla();
                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                // Se capturan las alertas de negocio (por ejemplo, si el nombre está repetido)
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Se resetean los campos para arrancar una carga desde cero
        private void btnNuevo_Click(object sender, EventArgs e) => LimpiarFormulario();

        private void btnBajaAlta_Click(object sender, EventArgs e)
        {
            // Se frena el flujo si se quiere dar de baja algo sin haberlo tocado antes
            if (_categoriaSeleccionada == null)
            {
                MessageBox.Show("Seleccione una categoría primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Se calcula el estado contrario al actual y se pide confirmación
                bool nuevoEstado = !_categoriaSeleccionada.Activa;
                string accion = nuevoEstado ? "activar" : "desactivar";

                var confirmacion = MessageBox.Show($"¿Desea {accion} la categoría '{_categoriaSeleccionada.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    // Se impacta el cambio de estado en la bd
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
            // Se resetea la variable global, se vacía el texto, se desmarca la grilla y se hace foco
            _categoriaSeleccionada = null;
            txtNombre.Clear();
            if (dgvCategorias.CurrentRow != null) dgvCategorias.CurrentRow.Selected = false;
            txtNombre.Focus();
        }
    }
}