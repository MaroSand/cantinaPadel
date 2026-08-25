using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDProveedor : Form
    {
        private readonly LogicaPersonaRoles _logicaPersonaRoles;
        private Proveedor? _proveedorEdicion;
        private CheckBox chkEsCliente = null!;
        private CheckBox chkEsEmpleado = null!;
        private Label lblEmailCliente = null!;
        private TextBox txtEmailCliente = null!;
        private Label lblUsuarioEmpleado = null!;
        private TextBox txtUsuarioEmpleado = null!;
        private Label lblContrasenaEmpleado = null!;
        private TextBox txtContrasenaEmpleado = null!;
        private Label lblRolEmpleado = null!;
        private ComboBox cmbRolEmpleado = null!;

        // Constructor para alta de nuevo proveedor
        public FrmCRUDProveedor()
        {
            InitializeComponent();
            CargarCondicionesIva();
            ConfigurarControlesRoles();
            ConfigurarFormatoCuit();
            _logicaPersonaRoles = new LogicaPersonaRoles();
            _proveedorEdicion = null;
            this.Text = "Nuevo Proveedor";
        }

        // Constructor para modificar proveedor existente
        public FrmCRUDProveedor(Proveedor proveedor) : this()
        {
            _proveedorEdicion = proveedor;
            this.Text = "Modificar Proveedor";
        }

        private void ConfigurarFormatoCuit()
        {
            // Se limita la longitud máxima para soportar los 11 dígitos y los 2 guiones de separación
            txtCuit.MaxLength = 13;
            txtCuit.KeyPress += txtCuit_KeyPress;
            txtCuit.TextChanged += txtCuit_TextChanged;
        }

        private void FrmCRUDProveedor_Load(object sender, EventArgs e)
        {
            // Se suscriben los eventos de botones
            btnGuardar.Click += btnGuardar_Click;
            btnCancelar.Click += btnCancelar_Click;

            if (_proveedorEdicion != null)
                CargarDatosEnFormulario();
        }

        private void ConfigurarControlesRoles()
        {
            ClientSize = new Size(1040, 690);
            panelHeader.Size = new Size(1040, panelHeader.Height);
            panelCuerpo.AutoScroll = true;

            int labelIzq = 35;
            int inputIzq = 255;
            int labelDer = 515;
            int inputDer = 745;
            var inputSize = new Size(230, 39);

            lblNombreEmpresa.Location = new Point(labelIzq, 35);
            txtNombreEmpresa.Location = new Point(inputIzq, 28);
            txtNombreEmpresa.Size = inputSize;

            lblCuit.Location = new Point(labelDer, 35);
            txtCuit.Location = new Point(inputDer, 28);
            txtCuit.Size = inputSize;

            lblApellido.Location = new Point(labelIzq, 100);
            txtApellido.Location = new Point(inputIzq, 93);
            txtApellido.Size = inputSize;

            lblCondicionIva.Location = new Point(labelDer, 100);
            cmbCondicionIva.Location = new Point(inputDer, 93);
            cmbCondicionIva.Size = new Size(inputSize.Width, 40);

            lblNombre.Location = new Point(labelIzq, 165);
            txtNombre.Location = new Point(inputIzq, 158);
            txtNombre.Size = inputSize;

            lblTelefono.Location = new Point(labelDer, 165);
            txtTelefono.Location = new Point(inputDer, 158);
            txtTelefono.Size = inputSize;

            lblDni.Location = new Point(labelIzq, 230);
            txtDni.Location = new Point(inputIzq, 223);
            txtDni.Size = inputSize;

            lblDireccion.Location = new Point(labelDer, 230);
            txtDireccion.Location = new Point(inputDer, 223);
            txtDireccion.Size = inputSize;

            chkEsCliente = new CheckBox { Text = "También es cliente", Location = new Point(labelIzq, 305), AutoSize = true };
            lblEmailCliente = new Label { Text = "Email cliente:", Location = new Point(labelIzq, 355), AutoSize = true };
            txtEmailCliente = new TextBox { Location = new Point(inputIzq, 348), Size = inputSize, MaxLength = 100 };

            chkEsEmpleado = new CheckBox { Text = "También es empleado", Location = new Point(labelDer, 305), AutoSize = true };
            lblUsuarioEmpleado = new Label { Text = "Usuario:", Location = new Point(labelDer, 355), AutoSize = true };
            txtUsuarioEmpleado = new TextBox { Location = new Point(inputDer, 348), Size = inputSize, MaxLength = 50 };
            lblContrasenaEmpleado = new Label { Text = "Contraseña:", Location = new Point(labelDer, 410), AutoSize = true };
            txtContrasenaEmpleado = new TextBox
            {Location = new Point(inputDer, 403),Size = inputSize, MaxLength = 8, UseSystemPasswordChar = true};
            lblRolEmpleado = new Label { Text = "Rol:", Location = new Point(labelDer, 465), AutoSize = true };
            cmbRolEmpleado = new ComboBox { Location = new Point(inputDer, 458), Size = new Size(inputSize.Width, 40), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRolEmpleado.Items.AddRange(new object[] { "Admin", "Empleado" });
            cmbRolEmpleado.SelectedIndex = 1;

            btnCancelar.Location = new Point(100, 535);
            btnCancelar.Size = new Size(380, 58);
            btnGuardar.Location = new Point(555, 535);
            btnGuardar.Size = new Size(380, 58);

            chkEsCliente.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();
            chkEsEmpleado.CheckedChanged += (_, _) => ActualizarVisibilidadRoles();

            panelCuerpo.Controls.AddRange(new Control[]
            {
                chkEsCliente, lblEmailCliente, txtEmailCliente,
                chkEsEmpleado, lblUsuarioEmpleado, txtUsuarioEmpleado,
                lblContrasenaEmpleado, txtContrasenaEmpleado, lblRolEmpleado, cmbRolEmpleado
            });

            ActualizarVisibilidadRoles();
        }

        private void ActualizarVisibilidadRoles()
        {
            lblEmailCliente.Visible = txtEmailCliente.Visible = chkEsCliente.Checked;
            lblUsuarioEmpleado.Visible = txtUsuarioEmpleado.Visible = chkEsEmpleado.Checked;
            lblContrasenaEmpleado.Visible = txtContrasenaEmpleado.Visible = chkEsEmpleado.Checked;
            lblRolEmpleado.Visible = cmbRolEmpleado.Visible = chkEsEmpleado.Checked;
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
            txtNombre.Text = _proveedorEdicion!.Persona.Nombre;
            txtApellido.Text = _proveedorEdicion.Persona.Apellido;
            txtDni.Text = _proveedorEdicion.Persona.Dni ?? string.Empty;
            txtCuit.Text = _proveedorEdicion.Persona.Cuit ?? string.Empty;
            txtTelefono.Text = _proveedorEdicion.Persona.Telefono ?? string.Empty;
            txtDireccion.Text = _proveedorEdicion.Persona.Direccion ?? string.Empty;
            txtNombreEmpresa.Text = _proveedorEdicion.NombreEmpresa ?? string.Empty;

            if (!string.IsNullOrEmpty(_proveedorEdicion.Persona.CondicionIva))
                cmbCondicionIva.SelectedItem = _proveedorEdicion.Persona.CondicionIva;

            CargarRolesExistentes();
        }

        private void CargarRolesExistentes()
        {
            if (_proveedorEdicion == null) return;

            var cliente = _logicaPersonaRoles.ObtenerClientePorPersonaId(_proveedorEdicion.IdPersona);
            if (cliente != null)
            {
                chkEsCliente.Checked = true;
                txtEmailCliente.Text = cliente.Email;
            }

            var empleado = _logicaPersonaRoles.ObtenerEmpleadoPorPersonaId(_proveedorEdicion.IdPersona);
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
            // Arma los objetos con los datos del formulario
            var persona = new Persona
            {
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Dni = txtDni.Text.Trim(),
                // Se asigna la cadena limpia sin espacios, delegando la obligatoriedad y el formato a la lógica del modelo
                Cuit = txtCuit.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                CondicionIva = cmbCondicionIva.SelectedItem?.ToString() ?? string.Empty
            };

            var proveedor = new Proveedor
            {
                NombreEmpresa = txtNombreEmpresa.Text.Trim()
            };

            Cliente? cliente = chkEsCliente.Checked
                ? new Cliente { Persona = persona, Email = txtEmailCliente.Text.Trim() }
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

            try
            {
                if (_proveedorEdicion != null)
                {
                    // Modificación: conserva los IDs originales
                    persona.IdPersona = _proveedorEdicion.IdPersona;
                    persona.EsProveedor = true;
                    persona.FechaAlta = _proveedorEdicion.Persona.FechaAlta;
                    persona.Activo = _proveedorEdicion.Persona.Activo;
                    proveedor.IdProveedor = _proveedorEdicion.IdProveedor;
                    proveedor.IdPersona = _proveedorEdicion.IdPersona;
                }

                _logicaPersonaRoles.GuardarRoles(persona, cliente, proveedor, empleado);

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
    }
}