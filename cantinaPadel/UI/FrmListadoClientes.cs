using cantinaPadel.BLL;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cantinaPadel.UI
{
    public partial class FrmListadoClientes : Form
    {
        // Instancia de la lógica de negocio para clientes
        private readonly LogicaCliente _logica;
        private List<Cliente>? _listaOriginal;

        // false = pantalla de administración de clientes (menú Clientes), como siempre
        // true  = selector de cliente para otra pantalla (por ejemplo el punto de venta)
        private readonly bool _modoSeleccion;

        // Cliente elegido en modo selección (existente o recién creado). Queda en null si se cancela
        public Cliente? ClienteSeleccionado { get; private set; }

        public FrmListadoClientes() : this(false) { }

        public FrmListadoClientes(bool modoSeleccion)
        {
            InitializeComponent();
            _logica = new LogicaCliente();
            _modoSeleccion = modoSeleccion;
            ConfigurarModo();
        }

        // En modo normal, btnSeleccionar y btnCancelar no se ven y la pantalla queda idéntica a la de siempre
        // En modo selección se ocultan las acciones de administración (Modificar, Activar/Desactivar y el filtro de estado): en medio de una venta
        // no corresponde dar de baja ni editar clientes, y solo se pueden elegir clientes activos (una venta a un cliente inactivo se rechaza)
        private void ConfigurarModo()
        {
            btnSeleccionar.Visible = _modoSeleccion;
            btnCancelar.Visible = _modoSeleccion;
            if (!_modoSeleccion) return;

            Text = "Seleccionar cliente";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;

            btnModificar.Visible = false;
            btnBajaLogica.Visible = false;
            cmbEstado.Visible = false;   // el filtro queda en "Activos", que se fija en ConfigurarGrilla
            label2.Visible = false;

            // Los botones de selección ocupan el lugar de los que se ocultaron
            btnSeleccionar.Location = btnModificar.Location;
            btnCancelar.Location = btnBajaLogica.Location;
            btnCancelar.DialogResult = DialogResult.Cancel;
            CancelButton = btnCancelar;

            ActiveControl = txtBuscar; // se puede empezar a escribir para buscar apenas se abre
        }

        private void FrmListadoClientes_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            dgvClientes.SelectionChanged += dgvClientes_SelectionChanged;

            if (_modoSeleccion)
            {
                dgvClientes.CellDoubleClick += dgvClientes_CellDoubleClick;
                btnSeleccionar.Click += btnSeleccionar_Click;

                // Se asegura que la ventana entre en pantalla (el diseño está pensado para ir embebido en el panel principal)
                var area = Screen.FromControl(this).WorkingArea;
                Width = Math.Min(Width, area.Width - 40);
                Height = Math.Min(Height, area.Height - 40);
                CenterToScreen();
            }

            CargarDatos();
        }
        // Configura las propiedades de la grilla de clientes
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

        // Carga los datos de los clientes desde la base de datos y los muestra en la grilla
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

        // Filtra los datos según el texto de búsqueda y el estado seleccionado, y los muestra en la grilla
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
                                $"{c.Persona.Nombre} {c.Persona.Apellido}".ToLower().Contains(buscar) ||
                                $"{c.Persona.Apellido} {c.Persona.Nombre}".ToLower().Contains(buscar) ||
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
        // Evento que se dispara cuando cambia el texto de búsqueda, para filtrar los datos
        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }

        // Evento que se dispara al hacer clic en el botón de baja lógica, para dar de baja o alta a un cliente
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

            string accion = cliente.Persona.Activo ? "desactivar" : "activar";
            var confirmacion = MessageBox.Show(
                $"¿Querés {accion} a {cliente.Persona.Nombre} {cliente.Persona.Apellido}?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                _logica.Baja(id);
                CargarDatos();
            }
        }
        // Evento que se dispara al hacer clic en el botón de modificar, para abrir el formulario de modificación de cliente
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
            using var frm = new FrmCRUDCliente();
            bool seGuardo = frm.ShowDialog() == DialogResult.OK;

            // En modo selección, el cliente recién creado queda elegido directamente, sin tener que buscarlo en la lista
            if (_modoSeleccion && seGuardo && frm.IdClienteGuardado > 0)
            {
                try
                {
                    var creado = _logica.ObtenerPorId(frm.IdClienteGuardado);
                    if (creado != null)
                    {
                        ClienteSeleccionado = creado;
                        DialogResult = DialogResult.OK;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"El cliente se guardó, pero no se pudo cargar: {ex.Message}. Buscalo en la lista.",
                        "Error de Conexion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            CargarDatos();
        }

        // Modo selección: doble clic sobre un cliente o botón "Seleccionar cliente"
        private void dgvClientes_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) ConfirmarSeleccion();
        }

        private void btnSeleccionar_Click(object? sender, EventArgs e) => ConfirmarSeleccion();

        private void ConfirmarSeleccion()
        {
            if (dgvClientes.CurrentRow?.Cells[0].Value is not int idCliente)
            {
                MessageBox.Show("Seleccioná un cliente de la lista, o creá uno nuevo.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var cliente = _listaOriginal?.FirstOrDefault(c => c.IdCliente == idCliente);
            if (cliente == null) return;

            // Red de seguridad: el filtro de estado está fijo en "Activos", pero igual no se deja elegir un cliente inactivo
            if (!cliente.Persona.Activo)
            {
                MessageBox.Show("El cliente está inactivo y no puede usarse en una venta.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ClienteSeleccionado = cliente;
            DialogResult = DialogResult.OK;
        }

        private void cmbEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrarYMostrarDatos();
        }

        // Se modifica el texto del botón dinámicamente según el estado del cliente seleccionado
        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null || _listaOriginal == null)
            {
                btnBajaLogica.Text = "Activar / Desactivar";
                btnBajaLogica.ForeColor = SystemColors.ControlText;
                return;
            }

            dynamic fila = dgvClientes.CurrentRow.DataBoundItem;
            if (fila == null) return;

            int idCliente = fila.IdCliente;
            var cliente = _listaOriginal.FirstOrDefault(c => c.IdCliente == idCliente);
            if (cliente == null) return;

            btnBajaLogica.Text = cliente.Persona.Activo ? "Desactivar" : "Activar";
            btnBajaLogica.ForeColor = cliente.Persona.Activo ? Color.DarkRed : Color.DarkGreen;
        }
    }
}