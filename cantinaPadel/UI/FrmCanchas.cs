using System;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCanchas : Form
    {
        private readonly LogicaCancha _logicaCancha;

        private Cancha? _canchaSeleccionada = null;

        public FrmCanchas()
        {
            InitializeComponent();
            _logicaCancha = new LogicaCancha();
        }

        private void FrmCanchas_Load(object sender, EventArgs e)
        {
            // Se preselecciona "Activos" en el combo para mostrar solo las canchas activas al entrar
            cmbEstado.SelectedIndex = 1;
            ActualizarGrilla();
            LimpiarFormulario();
        }

        private void ActualizarGrilla()
        {
            try
            {
                var lista = _logicaCancha.ObtenerTodas(activa: null);

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

                dgvCanchas.DataSource = null;
                dgvCanchas.DataSource = lista.Select(c => new
                {
                    c.IdCancha,
                    c.Nombre,
                    c.Activa
                }).ToList();

                // Solo lectura: evita que se pueda tildar/destildar "Activa" directo desde la grilla
                dgvCanchas.ReadOnly = true;

                if (dgvCanchas.Columns["IdCancha"] is DataGridViewColumn colId)
                    colId.Visible = false;

                var colNombre = dgvCanchas.Columns["Nombre"];
                if (colNombre != null) colNombre.HeaderText = "Cancha";

                var colActiva = dgvCanchas.Columns["Activa"];
                if (colActiva != null) colActiva.HeaderText = "Activa";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la grilla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e) => ActualizarGrilla();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => ActualizarGrilla();

        private void dgvCanchas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCanchas.CurrentRow == null) return;

            // La grilla se bindea a un tipo anónimo (para no exponer el Producto completo), así que se vuelve a pedir la cancha real por Id.
            int id = (int)dgvCanchas.CurrentRow.Cells["IdCancha"].Value;
            _canchaSeleccionada = _logicaCancha.ObtenerPorId(id);

            if (_canchaSeleccionada != null)
            {
                txtNombre.Text = _canchaSeleccionada.Nombre;
                ActualizarBotonEstado();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_canchaSeleccionada == null)
                {
                    var nueva = new Cancha { Nombre = txtNombre.Text };
                    _logicaCancha.Agregar(nueva);
                }
                else
                {
                    _canchaSeleccionada.Nombre = txtNombre.Text;
                    _logicaCancha.Modificar(_canchaSeleccionada);
                }

                MessageBox.Show("Cancha guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnHorarios_Click(object sender, EventArgs e)
        {
            using var frmHorarios = new FrmHorarios();
            frmHorarios.ShowDialog(this);
            ActualizarGrilla();
        }

        private void btnBajaAlta_Click(object sender, EventArgs e)
        {
            if (_canchaSeleccionada == null)
            {
                MessageBox.Show("Seleccione una cancha primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstado = !_canchaSeleccionada.Activa;
                string accion = nuevoEstado ? "activar" : "desactivar";

                var confirmacion = MessageBox.Show(
                    $"¿Desea {accion} la cancha '{_canchaSeleccionada.Nombre}'?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    _logicaCancha.CambiarEstado(_canchaSeleccionada.IdCancha, nuevoEstado);
                    ActualizarGrilla();
                    LimpiarFormulario();
                }
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

        private void LimpiarFormulario()
        {
            dgvCanchas.ClearSelection();
            dgvCanchas.CurrentCell = null;

            _canchaSeleccionada = null;
            txtNombre.Clear();
            btnBajaAlta.Text = "Activar/ Desactivar";
            btnBajaAlta.ForeColor = SystemColors.ControlText;

            txtNombre.Focus();
        }

        private void ActualizarBotonEstado()
        {
            if (_canchaSeleccionada == null) return;

            btnBajaAlta.Text = _canchaSeleccionada.Activa ? "Desactivar" : "Activar";
            btnBajaAlta.ForeColor = _canchaSeleccionada.Activa ? Color.DarkRed : Color.DarkGreen;
        }
    }
}