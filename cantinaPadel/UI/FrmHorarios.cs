using System;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmHorarios : Form
    {
        private readonly LogicaHorarioCancha _logicaHorario;
        private readonly LogicaCancha _logicaCancha;

        private HorarioCancha? _horarioSeleccionado = null;

        public FrmHorarios()
        {
            InitializeComponent();
            _logicaHorario = new LogicaHorarioCancha();
            _logicaCancha = new LogicaCancha();
        }

        private void FrmHorarios_Load(object sender, EventArgs e)
        {
            CargarCombos();
            cmbEstado.SelectedIndex = 0;
            ActualizarGrilla();
            LimpiarFormulario();
        }

        private void CargarCombos()
        {
            // Combo de canchas (solo activas, para no cargar horarios de una cancha dada de baja)
            cmbCancha.DataSource = _logicaCancha.ObtenerTodas(activa: true);
            cmbCancha.DisplayMember = "Nombre";
            cmbCancha.ValueMember = "IdCancha";

            // Combo de filtro (incluye "Todas")
            var canchasFiltro = _logicaCancha.ObtenerTodas(activa: null).ToList();
            canchasFiltro.Insert(0, new Cancha { IdCancha = 0, Nombre = "Todas" });
            cmbFiltroCancha.DataSource = canchasFiltro;
            cmbFiltroCancha.DisplayMember = "Nombre";
            cmbFiltroCancha.ValueMember = "IdCancha";

            cmbDiaSemana.DataSource = HorarioCancha.DiasValidos.ToList();

            dtpHoraInicio.Format = DateTimePickerFormat.Custom;
            dtpHoraInicio.CustomFormat = "HH:mm";
            dtpHoraInicio.ShowUpDown = true;

            dtpHoraFin.Format = DateTimePickerFormat.Custom;
            dtpHoraFin.CustomFormat = "HH:mm";
            dtpHoraFin.ShowUpDown = true;
        }

        private void ActualizarGrilla()
        {
            try
            {
                var lista = _logicaHorario.ObtenerTodos(activo: null);

                if (cmbFiltroCancha.SelectedValue is int idCanchaFiltro && idCanchaFiltro > 0)
                {
                    lista = lista.Where(h => h.IdCancha == idCanchaFiltro).ToList();
                }

                if (cmbEstado.SelectedIndex == 1)
                {
                    lista = lista.Where(h => h.Activo).ToList();
                }
                else if (cmbEstado.SelectedIndex == 2)
                {
                    lista = lista.Where(h => !h.Activo).ToList();
                }

                dgvHorarios.DataSource = null;
                dgvHorarios.DataSource = lista.Select(h => new
                {
                    h.IdHorario,
                    Cancha = h.Cancha?.Nombre ?? "-",
                    h.DiaSemana,
                    HoraInicio = h.HoraInicio.ToString(@"hh\:mm"),
                    HoraFin = h.HoraFin.ToString(@"hh\:mm"),
                    h.Activo
                }).ToList();

                if (dgvHorarios.Columns["IdHorario"] is DataGridViewColumn colId)
                    colId.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la grilla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbFiltroCancha_SelectedIndexChanged(object sender, EventArgs e) => ActualizarGrilla();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => ActualizarGrilla();

        private void dgvHorarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHorarios.CurrentRow == null) return;

            int id = (int)dgvHorarios.CurrentRow.Cells["IdHorario"].Value;
            _horarioSeleccionado = _logicaHorario.ObtenerPorId(id);

            if (_horarioSeleccionado != null)
            {
                cmbCancha.SelectedValue = _horarioSeleccionado.IdCancha;
                cmbDiaSemana.SelectedItem = _horarioSeleccionado.DiaSemana;
                dtpHoraInicio.Value = DateTime.Today.Add(_horarioSeleccionado.HoraInicio);
                dtpHoraFin.Value = DateTime.Today.Add(_horarioSeleccionado.HoraFin);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCancha.SelectedValue is not int idCancha)
                {
                    MessageBox.Show("Seleccione una cancha.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string diaSemana = cmbDiaSemana.SelectedItem?.ToString() ?? string.Empty;

                if (_horarioSeleccionado == null)
                {
                    var nuevo = new HorarioCancha
                    {
                        IdCancha = idCancha,
                        DiaSemana = diaSemana,
                        HoraInicio = dtpHoraInicio.Value.TimeOfDay,
                        HoraFin = dtpHoraFin.Value.TimeOfDay
                    };
                    _logicaHorario.Agregar(nuevo);
                }
                else
                {
                    _horarioSeleccionado.IdCancha = idCancha;
                    _horarioSeleccionado.DiaSemana = diaSemana;
                    _horarioSeleccionado.HoraInicio = dtpHoraInicio.Value.TimeOfDay;
                    _horarioSeleccionado.HoraFin = dtpHoraFin.Value.TimeOfDay;
                    _logicaHorario.Modificar(_horarioSeleccionado);
                }

                MessageBox.Show("Horario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (_horarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un horario primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstado = !_horarioSeleccionado.Activo;
                string accion = nuevoEstado ? "activar" : "desactivar";

                var confirmacion = MessageBox.Show(
                    $"¿Desea {accion} este horario?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    _logicaHorario.CambiarEstado(_horarioSeleccionado.IdHorario, nuevoEstado);
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
            _horarioSeleccionado = null;

            if (cmbCancha.Items.Count > 0) cmbCancha.SelectedIndex = 0;
            if (cmbDiaSemana.Items.Count > 0) cmbDiaSemana.SelectedIndex = 0;
            dtpHoraInicio.Value = DateTime.Today.AddHours(8);
            dtpHoraFin.Value = DateTime.Today.AddHours(9);

            dgvHorarios.ClearSelection();
            dgvHorarios.CurrentCell = null;
        }
    }
}