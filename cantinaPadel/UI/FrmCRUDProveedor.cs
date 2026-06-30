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
                cmbCondicionIva.Text = _proveedorEdicion.Persona.CondicionIva;
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
                CondicionIva = cmbCondicionIva.Text.Trim()
            };

            var proveedor = new Proveedor
            {
                NombreEmpresa = txtNombreEmpresa.Text.Trim()
            };

            (bool ok, string error) resultado;

            if (_proveedorEdicion == null)
            {
                // Alta
                resultado = _logicaProveedor.Agregar(persona, proveedor);
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

                resultado = _logicaProveedor.Modificar(persona, proveedor);
            }

            if (!resultado.ok)
            {
                MessageBox.Show(resultado.error, "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Proveedor guardado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}