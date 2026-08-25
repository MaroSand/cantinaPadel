using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDEmpleado : Form
    {
        private readonly LogicaPersonaRoles _logicaPersonaRoles;
        private Empleado? _empleadoEdicion;
        private CheckBox chkEsCliente = null!;
        private CheckBox chkEsProveedor = null!;
        private Label lblEmailCliente = null!;
        private TextBox txtEmailCliente = null!;
        private Label lblNombreEmpresa = null!;
        private TextBox txtNombreEmpresa = null!;

        public FrmCRUDEmpleado()
        {
            InitializeComponent();
            CargarCondicionesIva();
            ConfigurarControlesRoles();
            ConfigurarFormatoCuit();
            _logicaPersonaRoles = new LogicaPersonaRoles();
            _empleadoEdicion = null;
            Text = "Nuevo Empleado";
        }

        public FrmCRUDEmpleado(Empleado empleado) : this()
        {
            _empleadoEdicion = empleado;
            Text = "Modificar Empleado";
        }

        private void ConfigurarFormatoCuit()
        {
            // Se limita la longitud máxima para soportar los 11 dígitos y los 2 guiones de separación
            txtCuit.MaxLength = 13;
            txtCuit.KeyPress += txtCuit_KeyPress;
            txtCuit.TextChanged += txtCuit_TextChanged;
        }

        private void CargarCondicionesIva()
        {
            cmbCondicionIva.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCondicionIva.Items.Clear();
            cmbCondicionIva.Items.AddRange(Persona.CondicionesIvaValidas);
            cmbCondicionIva.SelectedIndex = -1;
        }

        private void ConfigurarControlesRoles()
        {
            ClientSize = new Size(ClientSize.Width, 860);
            btnCancelar.Location = new Point(btnCancelar.Location.X, 720);
            btnGuardar.Location = new Point(btnGuardar.Location.X, 720);

            chkEsCliente = new CheckBox { Text = "También es cliente", Location = new Point(55, 570), AutoSize = true };
            lblEmailCliente = new Label { Text = "Email cliente: *", Location = new Point(55, 620), AutoSize = true };
            txtEmailCliente = new TextBox { Location = new Point(228, 614), Size = new Size(251, 39), MaxLength = 100 };

            chkEsProveedor = new CheckBox { Text = "También es proveedor", Location = new Point(700, 570), AutoSize = true };
            lblNombreEmpresa = new Label { Text = "Empresa proveedor: *", Location = new Point(700, 620), AutoSize = true };
            txtNombreEmpresa = new TextBox { Location = new Point(916, 614), Size = new Size(275, 39), MaxLength = 50 };

            chkEsCliente.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();
            chkEsProveedor.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();

            Controls.AddRange(new Control[]
            {
                chkEsCliente, lblEmailCliente, txtEmailCliente,
                chkEsProveedor, lblNombreEmpresa, txtNombreEmpresa
            });

            ActualizarVisibilidadRoles();
        }

        private void ActualizarVisibilidadRoles()
        {
            lblEmailCliente.Visible = txtEmailCliente.Visible = chkEsCliente.Checked;
            lblNombreEmpresa.Visible = txtNombreEmpresa.Visible = chkEsProveedor.Checked;
        }

        private void FrmCRUDEmpleado_Load(object sender, EventArgs e)
        {
            if (_empleadoEdicion == null)
            {
                if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
                return;
            }

            txtDni.Text = _empleadoEdicion.Persona.Dni ?? string.Empty;
            txtApellido.Text = _empleadoEdicion.Persona.Apellido;
            txtNombre.Text = _empleadoEdicion.Persona.Nombre;
            txtTelefono.Text = _empleadoEdicion.Persona.Telefono ?? string.Empty;
            txtCuit.Text = _empleadoEdicion.Persona.Cuit ?? string.Empty;

            if (!string.IsNullOrEmpty(_empleadoEdicion.Persona.CondicionIva))
                cmbCondicionIva.SelectedItem = _empleadoEdicion.Persona.CondicionIva;

            txtUsuario.Text = _empleadoEdicion.NombreUsuario;
            txtContrasena.Text = _empleadoEdicion.Contrasena;
            cmbRol.SelectedItem = _empleadoEdicion.Rol;
            txtDni.ReadOnly = true;

            CargarRolesExistentes();
        }

        private void CargarRolesExistentes()
        {
            if (_empleadoEdicion == null) return;

            var cliente = _logicaPersonaRoles.ObtenerClientePorPersonaId(_empleadoEdicion.IdPersona);
            if (cliente != null)
            {
                chkEsCliente.Checked = true;
                txtEmailCliente.Text = cliente.Email;
            }

            var proveedor = _logicaPersonaRoles.ObtenerProveedorPorPersonaId(_empleadoEdicion.IdPersona);
            if (proveedor != null)
            {
                chkEsProveedor.Checked = true;
                txtNombreEmpresa.Text = proveedor.NombreEmpresa;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var persona = new Persona
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Dni = txtDni.Text.Trim(),
                    // Se asigna el CUIT obligatoriamente ingresado por el usuario
                    Cuit = txtCuit.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    CondicionIva = cmbCondicionIva.SelectedItem?.ToString() ?? string.Empty,
                    Direccion = _empleadoEdicion?.Persona.Direccion,
                    Activo = _empleadoEdicion?.Persona.Activo ?? true,
                    FechaAlta = _empleadoEdicion?.Persona.FechaAlta ?? DateTime.Now
                };

                if (_empleadoEdicion != null)
                {
                    persona.IdPersona = _empleadoEdicion.IdPersona;
                    persona.EsEmpleado = true;
                }

                var empleado = new Empleado
                {
                    IdEmpleado = _empleadoEdicion?.IdEmpleado ?? 0,
                    IdPersona = _empleadoEdicion?.IdPersona ?? 0,
                    Persona = persona,
                    NombreUsuario = txtUsuario.Text.Trim(),
                    Contrasena = txtContrasena.Text.Trim(),
                    Rol = cmbRol.SelectedItem?.ToString() ?? string.Empty,
                    Activo = _empleadoEdicion?.Activo ?? true
                };

                Cliente? cliente = chkEsCliente.Checked
                    ? new Cliente { Persona = persona, Email = txtEmailCliente.Text.Trim() }
                    : null;

                Proveedor? proveedor = chkEsProveedor.Checked
                    ? new Proveedor { Persona = persona, NombreEmpresa = txtNombreEmpresa.Text.Trim() }
                    : null;

                _logicaPersonaRoles.GuardarRoles(persona, cliente, proveedor, empleado);

                MessageBox.Show("Empleado guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RegresarAlListado();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación de Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error interno al procesar la solicitud: {ex.Message}", "Error General", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            RegresarAlListado();
        }

        private void RegresarAlListado()
        {
            if (ParentForm is FrmMain mainForm)
            {
                FrmListadoEmpleados listado = new FrmListadoEmpleados();
                mainForm.AbrirEnPanel(listado);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void txtCuit_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se descarta cualquier entrada que no corresponda a un número o a teclas de control
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtCuit_TextChanged(object sender, EventArgs e)
        {
            // Se desvincula temporalmente el evento para evitar recursividad al modificar el contenido del control
            txtCuit.TextChanged -= txtCuit_TextChanged;

            // Se filtran únicamente los caracteres numéricos
            string numeros = new string(txtCuit.Text.Where(char.IsDigit).ToArray());

            if (numeros.Length > 11)
            {
                numeros = numeros.Substring(0, 11);
            }

            string cuitFormateado = "";

            if (numeros.Length > 0)
            {
                // Se incorporan los dos primeros dígitos
                cuitFormateado += numeros.Substring(0, Math.Min(2, numeros.Length));

                if (numeros.Length > 2)
                {
                    // Se inserta el primer guion seguido del bloque central de ocho dígitos
                    cuitFormateado += "-" + numeros.Substring(2, Math.Min(8, numeros.Length - 2));

                    if (numeros.Length > 10)
                    {
                        // Se agrega el segundo guion junto al dígito verificador final
                        cuitFormateado += "-" + numeros.Substring(10, 1);
                    }
                }
            }

            txtCuit.Text = cuitFormateado;
            // Se ubica la posición del cursor al final de la cadena generada
            txtCuit.SelectionStart = txtCuit.Text.Length;

            // Se reactiva la escucha del evento de cambio de texto
            txtCuit.TextChanged += txtCuit_TextChanged;
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }
    }
}