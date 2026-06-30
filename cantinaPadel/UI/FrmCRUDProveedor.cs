using System;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDProveedor : Form
    {
        private readonly LogicaProveedor _logicaProveedor;
        private Proveedor? _proveedorEdicion;

        // Constructor para alta de nuevo proveedor
        public FrmCRUDProveedor()
        {
            InitializeComponent();
            CargarCondicionesIva();
            _logicaProveedor  = new LogicaProveedor();
            _proveedorEdicion = null;
            this.Text         = "Nuevo Proveedor";
        }

        // Constructor para modificar proveedor existente
        public FrmCRUDProveedor(Proveedor proveedor) : this()
        {
            _proveedorEdicion = proveedor;
            this.Text         = "Modificar Proveedor";
        }

        private void FrmCRUDProveedor_Load(object sender, EventArgs e)
        {
            // Se suscriben los eventos de botones
            btnGuardar.Click  += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;

            if (_proveedorEdicion != null)
                CargarDatosEnFormulario();
        }

        private void CargarCondicionesIva()
        {
            cmbCondicionIva.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCondicionIva.Items.Clear();
            cmbCondicionIva.Items.AddRange(Persona.CondicionesIvaValidas);
            cmbCondicionIva.SelectedIndex = -1;
        }

        // Rellena los campos cuando es una modificación
        private void CargarDatosEnFormulario()
        {
            txtNombre.Text        = _proveedorEdicion!.Persona.Nombre;
            txtApellido.Text      = _proveedorEdicion.Persona.Apellido;
            txtDni.Text           = _proveedorEdicion.Persona.Dni       ?? string.Empty;
            txtCuit.Text          = _proveedorEdicion.Persona.Cuit      ?? string.Empty;
            txtTelefono.Text      = _proveedorEdicion.Persona.Telefono  ?? string.Empty;
            txtDireccion.Text     = _proveedorEdicion.Persona.Direccion ?? string.Empty;
            txtNombreEmpresa.Text = _proveedorEdicion.NombreEmpresa     ?? string.Empty;

            if (!string.IsNullOrEmpty(_proveedorEdicion.Persona.CondicionIva))
                cmbCondicionIva.SelectedItem = _proveedorEdicion.Persona.CondicionIva;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Arma los objetos con los datos del formulario
            var persona = new Persona
            {
                Nombre       = txtNombre.Text.Trim(),
                Apellido     = txtApellido.Text.Trim(),
                Dni          = txtDni.Text.Trim(),
                Cuit         = txtCuit.Text.Trim(),
                Telefono     = txtTelefono.Text.Trim(),
                Direccion    = txtDireccion.Text.Trim(),
                CondicionIva = cmbCondicionIva.SelectedItem?.ToString() ?? string.Empty
            };

            var proveedor = new Proveedor
            {
                NombreEmpresa = txtNombreEmpresa.Text.Trim()
            };

            try
            {
                if (_proveedorEdicion == null)
                {
                    // Alta
                    _logicaProveedor.Agregar(persona, proveedor);
                }
                else
                {
                    // Modificación: conserva los IDs originales
                    persona.IdPersona     = _proveedorEdicion.IdPersona;
                    persona.EsProveedor   = true;
                    persona.FechaAlta     = _proveedorEdicion.Persona.FechaAlta;
                    persona.Activo        = _proveedorEdicion.Persona.Activo;
                    proveedor.IdProveedor = _proveedorEdicion.IdProveedor;
                    proveedor.IdPersona   = _proveedorEdicion.IdPersona;

                    _logicaProveedor.Modificar(persona, proveedor);
                }

                MessageBox.Show("Proveedor guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
