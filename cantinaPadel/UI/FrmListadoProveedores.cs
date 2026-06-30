using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmListadoProveedores : Form
    {
        // Se declaran las variables globales
        private readonly LogicaProveedor _logicaProveedor;
        private List<Proveedor>? _listaOriginal;

        public FrmListadoProveedores()
        {
            InitializeComponent();
            _logicaProveedor = new LogicaProveedor();
        }

        private void FrmListadoProveedores_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarDatosDesdeBase();
        }

        private void ConfigurarGrilla()
        {
            // Se suscriben los eventos de controles
            txtBuscarNombre.TextChanged    += txtBuscarNombre_TextChanged;
            cmbEstado.SelectedIndexChanged += cmbEstado_SelectedIndexChanged;
            btnNuevo.Click                 += btnNuevo_Click;
            btnModificar.Click             += btnModificar_Click;
            btnBajaLogica.Click            += btnBajaLogica_Click;

            // Combo: índice 0=Todos, 1=Activos, 2=Inactivos (sin items vacíos)
            cmbEstado.SelectedIndex = 1;

            // Ajustes de la grilla
            dgvProveedores.AutoGenerateColumns = false;
            dgvProveedores.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.ReadOnly            = true;
            dgvProveedores.MultiSelect         = false;

            // Columnas: Name es clave para Cells["..."], DataPropertyName bindea con el objeto anónimo
            dgvProveedores.Columns.Clear();
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "IdProveedor",  DataPropertyName = "IdProveedor",  Visible = false });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "Empresa",       DataPropertyName = "NombreEmpresa", HeaderText = "Empresa",   Width = 180 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "Apellido",      DataPropertyName = "ApellidoClon",  HeaderText = "Apellido",  Width = 120 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "Nombre",        DataPropertyName = "NombreClon",    HeaderText = "Nombre",    Width = 120 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "CUIT",          DataPropertyName = "CuitClon",      HeaderText = "CUIT",      Width = 130 });
            dgvProveedores.Columns.Add(new DataGridViewTextBoxColumn  { Name = "Telefono",      DataPropertyName = "TelefonoClon",  HeaderText = "Teléfono",  Width = 110 });
            dgvProveedores.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Activo",        DataPropertyName = "Activo",        HeaderText = "Activo",    Width = 60  });
        }

        private void CargarDatosDesdeBase()
        {
            try
            {
                _listaOriginal = _logicaProveedor.ObtenerTodos();
                FiltrarYMostrarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrarYMostrarDatos()
        {
            if (_listaOriginal == null) return;

            string buscar      = txtBuscarNombre.Text.Trim().ToLower();
            int    filtroEstado = cmbEstado.SelectedIndex; // 0=Todos, 1=Activos, 2=Inactivos

            var listaFiltrada = _listaOriginal.Where(p =>
            {
                bool coincideTexto = string.IsNullOrEmpty(buscar) ||
                                     p.Persona.Nombre.ToLower().Contains(buscar)   ||
                                     p.Persona.Apellido.ToLower().Contains(buscar) ||
                                     (p.NombreEmpresa != null && p.NombreEmpresa.ToLower().Contains(buscar)) ||
                                     (p.Persona.Cuit  != null && p.Persona.Cuit.Contains(buscar));

                bool coincideEstado = filtroEstado switch
                {
                    1 => p.Persona.Activo,       // Activos
                    2 => !p.Persona.Activo,      // Inactivos
                    _ => true                    // Todos
                };

                return coincideTexto && coincideEstado;
            }).Select(p => new
            {
                p.IdProveedor,
                p.NombreEmpresa,
                ApellidoClon = p.Persona.Apellido,
                NombreClon   = p.Persona.Nombre,
                CuitClon     = p.Persona.Cuit,
                TelefonoClon = p.Persona.Telefono,
                Activo       = p.Persona.Activo
            }).ToList();

            dgvProveedores.DataSource = listaFiltrada;
        }

        // ── Eventos filtros ────────────────────────────────────────────────────
        private void txtBuscarNombre_TextChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();
        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e) => FiltrarYMostrarDatos();

        // ── Botones de acción ──────────────────────────────────────────────────
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var frm = new FrmCRUDProveedor();
            frm.ShowDialog();
            CargarDatosDesdeBase();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un proveedor para modificar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idProveedor   = (int)dgvProveedores.CurrentRow.Cells["IdProveedor"].Value;
            Proveedor? proveedor = _logicaProveedor.ObtenerPorId(idProveedor);
            if (proveedor == null) return;

            var frm = new FrmCRUDProveedor(proveedor);
            frm.ShowDialog();
            CargarDatosDesdeBase();
        }

        private void btnBajaLogica_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un proveedor para dar de baja / alta.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool   estaActivo = (bool)dgvProveedores.CurrentRow.Cells["Activo"].Value;
            string accion     = estaActivo ? "dar de baja" : "dar de alta";

            var confirmacion = MessageBox.Show(
                $"¿Desea {accion} al proveedor seleccionado?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

            int idProveedor = (int)dgvProveedores.CurrentRow.Cells["IdProveedor"].Value;

            try
            {
                _logicaProveedor.BajaLogica(idProveedor);
                CargarDatosDesdeBase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar estado: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}