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
            // Se modifica el título de la ventana actual
            this.Text = "Gestión de Empleados - Cantina Pádel";

            // Se modifica el título en el formulario principal si está incrustado
            if (this.ParentForm is FrmMain mainForm)
            {
                mainForm.Text = "Cantina Pádel - Listado de Empleados";
            }

            ConfigurarGrilla();
            CargarDatosDesdeBase();

            // Se fuerza a que el combo arranque seleccionado en "Todos" (Índice 0) para ver el listado completo al inicio
            if (cmbEstado.Items.Count > 0)
            {
                cmbEstado.SelectedIndex = 0;
            }
        }

        private void ConfigurarGrilla()
        {
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
                // Se traen todos los registros de la base de datos (incluyendo activos e inactivos)
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
            // Se verifica que la lista original de la base de datos no esté vacía
            if (_listaOriginal == null) return;

            // Se pasa a minúsculas lo escrito por el usuario en el cuadro de búsqueda
            string buscar = txtBuscarNombre.Text.Trim().ToLower();

            // Se recupera el texto seleccionado en el combo en minúsculas. Si no hay nada, se asume "todos"
            string filtroEstado = cmbEstado.SelectedItem?.ToString()?.Trim().ToLower() ?? "todos";

            // Se filtra usando LINQ en memoria para buscar por texto y estado lógico
            var listaFiltrada = _listaOriginal.Where(e =>
            {
                // Se evalúa la coincidencia del texto controlando que Persona y sus propiedades no sean nulos
                bool coincideTexto = string.IsNullOrEmpty(buscar) ||
                                     (e.Persona?.Nombre != null && e.Persona.Nombre.ToLower().Contains(buscar)) ||
                                     (e.Persona?.Apellido != null && e.Persona.Apellido.ToLower().Contains(buscar));

                // Se evalúa el estado lógico leyendo el texto del combobox de forma explícita
                bool coincideEstado = false;

                if (filtroEstado.Contains("todos"))
                    coincideEstado = true; // Muestra tanto activos como inactivos

                else if (filtroEstado.Contains("activo") && !filtroEstado.Contains("inactivo"))
                    coincideEstado = e.Activo; // Debe ser true

                else if (filtroEstado.Contains("inactivo"))
                    coincideEstado = !e.Activo; // Debe ser false

                return coincideTexto && coincideEstado;
            }).Select(e => new
            {
                e.IdEmpleado,
                // Se usan operadores de coalescencia por si la navegación a Persona llega a fallar
                DniClon = e.Persona?.Dni ?? "S/D",
                ApellidoClon = e.Persona?.Apellido ?? "S/A",
                NombreClon = e.Persona?.Nombre ?? "S/N",
                e.NombreUsuario,
                e.Rol,
                e.Activo
            }).ToList();

            // Se actualiza el origen de datos de la grilla de forma instantánea
            dgvEmpleados.DataSource = listaFiltrada;
        }

        private void txtBuscarNombre_TextChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Se busca la instancia del formulario principal MDI/Contenedor
            if (this.ParentForm is FrmMain mainForm)
            {
                // Redirige incrustando el CRUD vacío en el panel principal
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
                    // Redirige incrustando el CRUD con los datos en el panel principal
                    FrmCRUDEmpleado frmCrud = new FrmCRUDEmpleado(empleadoAEditar);
                    mainForm.AbrirEnPanel(frmCrud);
                }
            }
        }

        // Método de dar de baja / alta lógica unificado con el nombre del botón de diseño
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
                        // Se invierte el estado lógico actual
                        empleado.Activo = !empleado.Activo;

                        // Se guarda el cambio en la base de datos usando la lógica unificada
                        _logicaEmpleado.RegistrarOGuardar(empleado);

                        MessageBox.Show($"Estado del empleado actualizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Se recargan los datos frescos desde la base de datos para impactar el cambio
                        CargarDatosDesdeBase();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al cambiar el estado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvEmpleados_SelectionChanged(object sender, EventArgs e)
        {
            // Se verifica que haya una fila seleccionada y que la lista original no esté vacía
            if (dgvEmpleados.CurrentRow != null && _listaOriginal != null)
            {
                // Se recupera la fila actual de la grilla como un objeto dinámico de tipo anónimo
                dynamic fila = dgvEmpleados.CurrentRow.DataBoundItem;

                // Se controla que la fila mapeada no sea nula para evitar excepciones de tipo dinámico
                if (fila != null)
                {
                    int idEmpleado = fila.IdEmpleado;

                    // Se busca al empleado en la lista original mediante su ID
                    var empleado = _listaOriginal.FirstOrDefault(x => x.IdEmpleado == idEmpleado);
                    if (empleado != null)
                    {
                        // Se modifica el texto del botón dinámicamente según el estado del empleado
                        btnBajaLogica.Text = empleado.Activo ? "Dar de Baja" : "Dar de Alta";

                        // Se modifica el color del texto para dar una alerta visual (Rojo para baja, Verde para alta)
                        btnBajaLogica.ForeColor = empleado.Activo ? Color.DarkRed : Color.DarkGreen;
                    }
                }
            }
        }
    }
}