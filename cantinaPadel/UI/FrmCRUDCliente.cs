using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDCliente : Form
    {
        private readonly LogicaPersonaRoles _logicaPersonaRoles;
        private Cliente? _clienteEdicion;
        private CheckBox chkEsProveedor = null!;
        private CheckBox chkEsEmpleado = null!;
        private Label lblNombreEmpresa = null!;
        private TextBox txtNombreEmpresa = null!;
        private Label lblUsuarioEmpleado = null!;
        private TextBox txtUsuarioEmpleado = null!;
        private Label lblContrasenaEmpleado = null!;
        private TextBox txtContrasenaEmpleado = null!;
        private Label lblRolEmpleado = null!;
        private ComboBox cmbRolEmpleado = null!;
        private Label lblCondicionIva = null!;
        private ComboBox cmbCondicionIva = null!;

        public FrmCRUDCliente()
        {
            InitializeComponent();
            ConfigurarControlesRoles();
            CargarCondicionesIva();
            ConfigurarFormatoCuit();
            _logicaPersonaRoles = new LogicaPersonaRoles();
            _clienteEdicion = null;
        }

        public FrmCRUDCliente(Cliente cliente) : this()
        {
            _clienteEdicion = cliente;
            Text = "Modificar Cliente";
            CargarDatosEnFormulario();
        }

        private void ConfigurarControlesRoles()
        {
            ClientSize = new Size(1000, 650);
            btnCancelar.Location = new Point(104, 535);
            btnGuardar.Location = new Point(531, 535);

            chkEsProveedor = new CheckBox { Text = "También es proveedor", Location = new Point(525, 82), AutoSize = true };
            lblNombreEmpresa = new Label { Text = "Empresa proveedor: *", Location = new Point(545, 122), AutoSize = true };
            txtNombreEmpresa = new TextBox { Location = new Point(705, 118), Size = new Size(230, 27), MaxLength = 50 };

            chkEsEmpleado = new CheckBox { Text = "También es empleado", Location = new Point(525, 178), AutoSize = true };
            lblUsuarioEmpleado = new Label { Text = "Usuario: *", Location = new Point(545, 218), AutoSize = true };
            txtUsuarioEmpleado = new TextBox { Location = new Point(705, 214), Size = new Size(230, 27), MaxLength = 50 };
            lblContrasenaEmpleado = new Label { Text = "Contraseña: *", Location = new Point(545, 258), AutoSize = true };
            txtContrasenaEmpleado = new TextBox { Location = new Point(705, 254), Size = new Size(230, 27), MaxLength = 9, UseSystemPasswordChar = true };
            lblRolEmpleado = new Label { Text = "Rol: *", Location = new Point(545, 298), AutoSize = true };
            cmbRolEmpleado = new ComboBox { Location = new Point(705, 294), Size = new Size(230, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRolEmpleado.Items.AddRange(new object[] { "Admin", "Empleado" });
            cmbRolEmpleado.SelectedIndex = 1;

            cmbCondicionIva = new ComboBox
            {
                Location = new Point(txtEmail.Left, txtEmail.Bottom + 20),
                Size = new Size(txtEmail.Width, 27),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            lblCondicionIva = new Label
            {
                Text = "Condición IVA:",
                Location = new Point(label5.Left, cmbCondicionIva.Top + 7),
                AutoSize = true
            };

            chkEsProveedor.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();
            chkEsEmpleado.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();

            panelDatos.Controls.AddRange(new Control[]
            {
                chkEsProveedor, lblNombreEmpresa, txtNombreEmpresa,
                chkEsEmpleado, lblUsuarioEmpleado, txtUsuarioEmpleado,
                lblContrasenaEmpleado, txtContrasenaEmpleado, lblRolEmpleado, cmbRolEmpleado,
                lblCondicionIva, cmbCondicionIva
            });

            ActualizarVisibilidadRoles();
        }

        // Carga las condiciones de IVA válidas en el combo, igual que en FrmCRUDProveedor y FrmCRUDEmpleado
        private void CargarCondicionesIva()
        {
            cmbCondicionIva.Items.Clear();
            cmbCondicionIva.Items.AddRange(Persona.CondicionesIvaValidas);
            cmbCondicionIva.SelectedIndex = -1;
        }

        private void ConfigurarFormatoCuit()
        {
            txtCuit.MaxLength = 13;
            txtCuit.KeyPress += txtCuit_KeyPress;
            txtCuit.TextChanged += txtCuit_TextChanged;
        }

        private void ActualizarVisibilidadRoles()
        {
            lblNombreEmpresa.Visible = txtNombreEmpresa.Visible = chkEsProveedor.Checked;
            lblUsuarioEmpleado.Visible = txtUsuarioEmpleado.Visible = chkEsEmpleado.Checked;
            lblContrasenaEmpleado.Visible = txtContrasenaEmpleado.Visible = chkEsEmpleado.Checked;
            lblRolEmpleado.Visible = cmbRolEmpleado.Visible = chkEsEmpleado.Checked;
        }

        private void CargarDatosEnFormulario()
        {
            if (_clienteEdicion == null) return;

            txtNombre.Text = _clienteEdicion.Persona.Nombre;
            txtApellido.Text = _clienteEdicion.Persona.Apellido;
            txtDni.Text = _clienteEdicion.Persona.Dni ?? string.Empty;
            txtCuit.Text = _clienteEdicion.Persona.Cuit ?? string.Empty;
            txtTelefono.Text = _clienteEdicion.Persona.Telefono ?? string.Empty;
            txtEmail.Text = _clienteEdicion.Email;

            if (!string.IsNullOrEmpty(_clienteEdicion.Persona.CondicionIva))
                cmbCondicionIva.SelectedItem = _clienteEdicion.Persona.CondicionIva;

            CargarRolesExistentes();
        }

        private void CargarRolesExistentes()
        {
            if (_clienteEdicion == null) return;

            var proveedor = _logicaPersonaRoles.ObtenerProveedorPorPersonaId(_clienteEdicion.IdPersona);
            if (proveedor != null)
            {
                chkEsProveedor.Checked = true;
                txtNombreEmpresa.Text = proveedor.NombreEmpresa;
            }

            var empleado = _logicaPersonaRoles.ObtenerEmpleadoPorPersonaId(_clienteEdicion.IdPersona);
            if (empleado != null)
            {
                chkEsEmpleado.Checked = true;
                txtUsuarioEmpleado.Text = empleado.NombreUsuario;
                txtContrasenaEmpleado.Text = empleado.Contrasena;
                cmbRolEmpleado.SelectedItem = empleado.Rol;
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
                    Dni = string.IsNullOrWhiteSpace(txtDni.Text) ? null : txtDni.Text.Trim(),
                    Cuit = txtCuit.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                    CondicionIva = cmbCondicionIva.SelectedItem?.ToString() ?? string.Empty,
                    Direccion = _clienteEdicion?.Persona.Direccion,
                    Activo = _clienteEdicion?.Persona.Activo ?? true,
                    FechaAlta = _clienteEdicion?.Persona.FechaAlta ?? DateTime.Now
                };

                if (_clienteEdicion != null)
                {
                    persona.IdPersona = _clienteEdicion.IdPersona;
                    persona.EsCliente = true;
                }

                var cliente = new Cliente
                {
                    IdCliente = _clienteEdicion?.IdCliente ?? 0,
                    IdPersona = _clienteEdicion?.IdPersona ?? 0,
                    Persona = persona,
                    Email = txtEmail.Text.Trim(),
                    SaldoCuentaCorriente = _clienteEdicion?.SaldoCuentaCorriente ?? 0
                };

                Proveedor? proveedor = chkEsProveedor.Checked
                    ? new Proveedor { Persona = persona, NombreEmpresa = txtNombreEmpresa.Text.Trim() }
                    : null;

                Empleado? empleado = chkEsEmpleado.Checked
                    ? new Empleado
                    {
                        Persona = persona,
                        NombreUsuario = txtUsuarioEmpleado.Text.Trim(),
                        Contrasena = txtContrasenaEmpleado.Text.Trim(),
                        Rol = cmbRolEmpleado.SelectedItem?.ToString() ?? string.Empty,
                        Activo = true
                    }
                    : null;

                _logicaPersonaRoles.GuardarRoles(persona, cliente, proveedor, empleado);

                MessageBox.Show("Cliente guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtNombreApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtCuit_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtCuit_TextChanged(object? sender, EventArgs e)
        {
            txtCuit.TextChanged -= txtCuit_TextChanged;

            string numeros = new string(txtCuit.Text.Where(char.IsDigit).ToArray());

            if (numeros.Length > 11)
                numeros = numeros.Substring(0, 11);

            string cuitFormateado = string.Empty;

            if (numeros.Length > 0)
            {
                cuitFormateado += numeros.Substring(0, Math.Min(2, numeros.Length));

                if (numeros.Length > 2)
                {
                    cuitFormateado += "-" + numeros.Substring(2, Math.Min(8, numeros.Length - 2));

                    if (numeros.Length > 10)
                        cuitFormateado += "-" + numeros.Substring(10, 1);
                }
            }

            txtCuit.Text = cuitFormateado;
            txtCuit.SelectionStart = txtCuit.Text.Length;

            txtCuit.TextChanged += txtCuit_TextChanged;
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '-' && e.KeyChar != '+')
                e.Handled = true;
        }
    }
}