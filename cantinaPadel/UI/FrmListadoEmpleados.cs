using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmListadoEmpleados : Form
    {
        // Se declaran las variables globales
        private readonly IEmpleadoRepository _empleadoRepository;
        private List<Empleado>? _listaOriginal;

        public FrmListadoEmpleados()
        {
            InitializeComponent();
            // Se instancia el repositorio para usarlo en toda la pantalla
            _empleadoRepository = new EmpleadoRepository();
        }

        private void FrmListadoEmpleados_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarDatosDesdeBase();
        }

        private void ConfigurarGrilla()
        {
            // Se selecciona "Activo" por defecto en el combox de estados
            cmbEstado.SelectedIndex = 1;

            // Ajustes de comportamientos de la grilla
            dgvEmpleados.AutoGenerateColumns = false;
            dgvEmpleados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmpleados.ReadOnly = true;
            dgvEmpleados.MultiSelect = false;

            // Se crean las columnas para enganchar con el mapeo de las tablas
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
                // Trae los empleados de la base de datos con el .Include() de personas
                _listaOriginal = _empleadoRepository.ObtenerTodos();
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

            // Se pasa a minúsculas lo escrito por el usuario
            string buscar = txtBuscarNombre.Text.Trim().ToLower();
            int filtroEstado = cmbEstado.SelectedIndex; // 0 = Todos - 1 = Activos - 2 = Inactivos

            // Se filtra usando LINQ en memoria para que sea instantáneo
            var listaFiltrada = _listaOriginal.Where(e =>
            {
                // Busca coincidencia con Nombre o Apellido dentro de la tabla Persona
                bool coincideTexto = string.IsNullOrEmpty(buscar) ||
                                     e.Persona.Nombre.ToLower().Contains(buscar) ||
                                     e.Persona.Apellido.ToLower().Contains(buscar);

                // Se filtra por estado
                bool coincideEstado = true;
                if (filtroEstado == 1) coincideEstado = e.Activo;
                if (filtroEstado == 2) coincideEstado = !e.Activo;

                return coincideTexto && coincideEstado;
            }).Select(e => new {
                e.IdEmpleado,
                DniClon = e.Persona.Dni,
                ApellidoClon = e.Persona.Apellido,
                NombreClon = e.Persona.Nombre,
                e.NombreUsuario,
                e.Rol,
                e.Activo
            }).ToList();

            dgvEmpleados.DataSource = listaFiltrada;
        }

        private void txtBuscarNombre_TextChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }

        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }
    }
}