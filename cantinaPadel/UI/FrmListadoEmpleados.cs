using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmListadoEmpleados : Form
    {
        private readonly LogicaEmpleado _logicaEmpleado;
        private List<Empleado>? _listaOriginal;

        public FrmListadoEmpleados()
        {
            InitializeComponent();
            _logicaEmpleado = new LogicaEmpleado();
        }

        private void FrmListadoEmpleados_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarDatosDesdeBase();
        }

        private void ConfigurarGrilla()
        {
            cmbEstado.SelectedIndex = 1; // Activos por defecto
            dgvEmpleados.AutoGenerateColumns = false;
            dgvEmpleados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmpleados.ReadOnly = true;
            dgvEmpleados.MultiSelect = false;

            dgvEmpleados.Columns.Clear();
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdEmpleado", Visible = false });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI", DataPropertyName = "DniClon", Width = 90 });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Apellido", DataPropertyName = "ApellidoClon", Width = 120 });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "NombreClon", Width = 120 });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Usuario", DataPropertyName = "NombreUsuario", Width = 100 });
            dgvEmpleados.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Rol", DataPropertyName = "Rol", Width = 90 });
            dgvEmpleados.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = "Activo", Width = 60 });
        }

        private void CargarDatosDesdeBase()
        {
            try
            {
                _listaOriginal = _logicaEmpleado.ObtenerTodos();
                FiltrarYMostrarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarYMostrarDatos()
        {
            if (_listaOriginal == null) return;

            string buscar = txtBuscarNombre.Text.Trim().ToLower();
            int filtroEstado = cmbEstado.SelectedIndex;

            var listaFiltrada = _listaOriginal.Where(e =>
            {
                bool coincideTexto = string.IsNullOrEmpty(buscar) ||
                                     (e.Persona?.Nombre?.ToLower().Contains(buscar) ?? false) ||
                                     (e.Persona?.Apellido?.ToLower().Contains(buscar) ?? false);

                bool coincideEstado = true;
                if (filtroEstado == 1) coincideEstado = e.Activo;
                if (filtroEstado == 2) coincideEstado = !e.Activo;

                return coincideTexto && coincideEstado;
            }).Select(e => new
            {
                e.IdEmpleado,
                DniClon = e.Persona?.Dni ?? "",
                ApellidoClon = e.Persona?.Apellido ?? "",
                NombreClon = e.Persona?.Nombre ?? "",
                e.NombreUsuario,
                e.Rol,
                e.Activo
            }).ToList();

            dgvEmpleados.DataSource = listaFiltrada;
        }

        private void txtBuscarNombre_TextChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Buscamos la instancia del formulario principal MDI/Contenedor
            if (this.ParentForm is FrmMain mainForm)
            {
                // Redirigimos incrustando el CRUD vacío en el panel principal
                FrmCRUDEmpleado frmCrud = new FrmCRUDEmpleado();
                mainForm.AbrirEnPanel(frmCrud);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un empleado de la lista para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (this.ParentForm is FrmMain mainForm)
            {
                dynamic filaSeleccionada = dgvEmpleados.CurrentRow.DataBoundItem;
                int idEmpleado = filaSeleccionada.IdEmpleado;

                Empleado? empleadoAEditar = _listaOriginal?.FirstOrDefault(x => x.IdEmpleado == idEmpleado);

                if (empleadoAEditar != null)
                {
                    // Redirigimos incrustando el CRUD con los datos en el panel principal
                    FrmCRUDEmpleado frmCrud = new FrmCRUDEmpleado(empleadoAEditar);
                    mainForm.AbrirEnPanel(frmCrud);
                }
            }
        }

        // --- MÉTODO PARA DAR DE BAJA / ALTA LÓGICA ---
        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Por favor, seleccione un empleado de la lista.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dynamic filaSeleccionada = dgvEmpleados.CurrentRow.DataBoundItem;
            int idEmpleado = filaSeleccionada.IdEmpleado;
            Empleado? empleado = _listaOriginal?.FirstOrDefault(x => x.IdEmpleado == idEmpleado);

            if (empleado != null)
            {
                string accion = empleado.Activo ? "dar de BAJA" : "dar de ALTA";
                var seguro = MessageBox.Show($"¿Está seguro que desea {accion} al empleado {empleado.Persona.Nombre} {empleado.Persona.Apellido}?",
                    "Confirmar Estado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (seguro == DialogResult.Yes)
                {
                    try
                    {
                        // Invertimos el estado lógico actual
                        empleado.Activo = !empleado.Activo;

                        // Guardamos el cambio en la base de datos usando el método del repositorio unificado
                        _logicaEmpleado.RegistrarOGuardar(empleado);

                        MessageBox.Show($"Empleado modificado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarDatosDesdeBase(); // Refrescamos la grilla
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cambiar el estado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}