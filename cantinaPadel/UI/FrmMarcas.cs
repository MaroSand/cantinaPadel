using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.DAL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmMarcas : Form
    {
        // Se conecta con la capa de negocio de marcas
        private readonly LogicaMarca _logicaMarca;

        // Se guarda de forma global la marca que se toque en la grilla
        private Marca? _marcaSeleccionada = null;

        public FrmMarcas()
        {
            InitializeComponent();
            // Se inicializa la lógica pasándole la bd
            _logicaMarca = new LogicaMarca(new AppDbContext());
        }

        private void FrmMarcas_Load(object sender, EventArgs e)
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
                // Se trae la lista completa de marcas desde la bd
                var lista = _logicaMarca.ListarTodas();

                // Se filtra por lo que se escribe en el buscador
                string buscar = txtBuscar.Text.ToLower().Trim();
                if (!string.IsNullOrEmpty(buscar))
                {
                    lista = lista.Where(m => m.Nombre.ToLower().Contains(buscar)).ToList();
                }

                // Se filtra según el estado elegido (Activos o Inactivos)
                if (cmbEstado.SelectedIndex == 1)
                {
                    lista = lista.Where(m => m.Activa).ToList();
                }
                else if (cmbEstado.SelectedIndex == 2)
                {
                    lista = lista.Where(m => !m.Activa).ToList();
                }

                // Se refrescan los datos de la grilla
                dgvMarcas.DataSource = null;
                dgvMarcas.DataSource = lista;

                // Ocultar ID de forma segura contra nulls
                if (dgvMarcas.Columns["IdMarca"] is DataGridViewColumn colId)
                {
                    colId.Visible = false;
                }

                var colNombre = dgvMarcas.Columns["Nombre"];
                if (colNombre != null) colNombre.HeaderText = "Marca";

                var colActiva = dgvMarcas.Columns["Activa"];
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

        private void dgvMarcas_SelectionChanged(object sender, EventArgs e)
        {
            // Se pasa la marca elegida a los campos de texto para poder editarla
            if (dgvMarcas.CurrentRow != null && dgvMarcas.CurrentRow.DataBoundItem != null)
            {
                _marcaSeleccionada = (Marca)dgvMarcas.CurrentRow.DataBoundItem;
                txtNombre.Text = _marcaSeleccionada.Nombre;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Se crea una marca nueva o se edita la que ya estaba seleccionada
                var mar = _marcaSeleccionada ?? new Marca();
                mar.Nombre = txtNombre.Text;

                // Se manda a la capa de negocio para validarla y guardarla
                _logicaMarca.Guardar(mar);

                MessageBox.Show("Marca guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (_marcaSeleccionada == null)
            {
                MessageBox.Show("Seleccione una marca primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Se calcula el estado contrario al actual y se pide confirmación
                bool nuevoEstado = !_marcaSeleccionada.Activa;
                string accion = nuevoEstado ? "activar" : "desactivar";

                var confirmacion = MessageBox.Show($"¿Desea {accion} la marca '{_marcaSeleccionada.Nombre}'?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    // Se impacta el cambio de estado en la bd
                    _logicaMarca.CambiarEstado(_marcaSeleccionada.IdMarca, nuevoEstado);
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
            _marcaSeleccionada = null;
            txtNombre.Clear();
            if (dgvMarcas.CurrentRow != null) dgvMarcas.CurrentRow.Selected = false;
            txtNombre.Focus();
        }
    }
}