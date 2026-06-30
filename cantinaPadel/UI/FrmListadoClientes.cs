using cantinaPadel.BLL;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cantinaPadel.UI
{
    public partial class FrmListadoClientes : Form
    {
        // Instancia de la lógica de negocio para clientes
        private readonly LogicaCliente _logica;
        private List<Cliente>? _listaOriginal;

        public FrmListadoClientes()
        {
            InitializeComponent();
            _logica = new LogicaCliente();
        }

        private void FrmListadoClientes_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarDatos();
        }
        private void ConfigurarGrilla()
        {
            cmbEstado.SelectedIndex = 1; // Activos por defecto
            dgvClientes.AutoGenerateColumns = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.ReadOnly = true;
            dgvClientes.MultiSelect = false;

            dgvClientes.Columns.Clear();
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "IdCliente", Visible = false });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DNI", DataPropertyName = "DniClon", Width = 90 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Apellido", DataPropertyName = "ApellidoClon", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "NombreClon", Width = 130 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email", Width = 200 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Teléfono", DataPropertyName = "TelefonoClon", Width = 110 });
            dgvClientes.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Activo", DataPropertyName = "Activo", Width = 60 });
        }


        private void CargarDatos()
        {
            try
            {
                _listaOriginal = _logica.ObtenerTodos();
                FiltrarYMostrarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}", "Error de Conexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarYMostrarDatos()
        {
            if (_listaOriginal == null) return;

            string buscar = txtBuscar.Text.Trim().ToLower();
            int filtroEstado = cmbEstado.SelectedIndex; // 0 = Todos, 1 = Activos, 2 = Inactivos
            var listaFiltrada = _listaOriginal.Where(c =>
            {
                bool coincideTexto = string.IsNullOrEmpty(buscar) ||
                                c.Persona.Nombre.ToLower().Contains(buscar) ||
                                c.Persona.Apellido.ToLower().Contains(buscar) ||
                                c.Email.ToLower().Contains(buscar) ||
                                (c.Persona.Dni != null && c.Persona.Dni.ToLower().Contains(buscar));

                bool coincideEstado = true;
                if (filtroEstado == 1) coincideEstado = c.Persona.Activo;
                if (filtroEstado == 2) coincideEstado = !c.Persona.Activo;

                return coincideTexto && coincideEstado;
            }).Select(c => new
            {
                c.IdCliente,
                DniClon = c.Persona.Dni,
                ApellidoClon = c.Persona.Apellido,
                NombreClon = c.Persona.Nombre,
                c.Email,
                TelefonoClon = c.Persona.Telefono,
                c.Persona.Activo
            }).ToList();
            dgvClientes.DataSource = listaFiltrada;
        }
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }

        private void btnBajaLogica_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un cliente.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)dgvClientes.SelectedRows[0].Cells[0].Value;
            var cliente = _logica.ObtenerPorId(id);
            if (cliente == null) return;

            string accion = cliente.Persona.Activo ? "dar de baja" : "dar de alta";
            var confirmacion = MessageBox.Show(
                $"¿Querés {accion} a {cliente.Persona.Nombre} {cliente.Persona.Apellido}?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                _logica.Baja(id);
                CargarDatos();
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccioná un cliente para modificar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = (int)dgvClientes.SelectedRows[0].Cells[0].Value;
            var cliente = _logica.ObtenerPorId(id);
            if (cliente == null) return;

            var frm = new FrmCRUDCliente(cliente);
            frm.ShowDialog();
            CargarDatos();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var frm = new FrmCRUDCliente();
            frm.ShowDialog();
            CargarDatos();
        }

        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }
    }
}
