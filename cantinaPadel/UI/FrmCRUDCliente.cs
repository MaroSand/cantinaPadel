using System;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDCliente : Form
    {
        private readonly LogicaCliente _logica;
        private Cliente? _clienteEdicion;
        public FrmCRUDCliente()
        {
            InitializeComponent();
            _logica = new LogicaCliente();
            _clienteEdicion = null;
        }

        public FrmCRUDCliente(Cliente cliente) : this()
        {
            _clienteEdicion = cliente;
            this.Text = "Modificar Cliente";

            txtNombre.Text = cliente.Persona.Nombre;
            txtApellido.Text = cliente.Persona.Apellido;
            txtDni.Text = cliente.Persona.Dni;
            txtTelefono.Text = cliente.Persona.Telefono;
            txtEmail.Text = cliente.Email;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Nombre y Apellido son obligatorios.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("El Email es obligatorio.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (_clienteEdicion == null)
                {
                    // Alta de cliente nuevo
                    var persona = new Persona
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Dni = string.IsNullOrWhiteSpace(txtDni.Text) ? null : txtDni.Text.Trim(),
                        Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim()
                    };
                    var cliente = new Cliente
                    {
                        Persona = persona,
                        Email = txtEmail.Text.Trim()
                    };
                    _logica.Alta(cliente);
                }
                else
                {
                    // Modificación de cliente existente
                    _clienteEdicion.Persona.Nombre = txtNombre.Text.Trim();
                    _clienteEdicion.Persona.Apellido = txtApellido.Text.Trim();
                    _clienteEdicion.Persona.Dni = string.IsNullOrWhiteSpace(txtDni.Text) ? null : txtDni.Text.Trim();
                    _clienteEdicion.Persona.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
                    _clienteEdicion.Email = txtEmail.Text.Trim();
                    _logica.Modificar(_clienteEdicion);
                }
                MessageBox.Show("Cliente guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
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
            this.Close();
        }

        // Permite solo letras (incluye acentos/ñ), espacios y teclas de control (Backspace, etc.)
        // Coincide con la regla de Persona.ValidarNombre / ValidarApellido.
        private void txtNombreApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        // Permite solo dígitos y teclas de control. Coincide con Persona.ValidarDni (7-8 dígitos).
        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Permite dígitos, espacio, guion y '+'. Coincide con Persona.ValidarTelefono.
        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar) && e.KeyChar != ' ' && e.KeyChar != '-' && e.KeyChar != '+')
                e.Handled = true;
        }
    }
}