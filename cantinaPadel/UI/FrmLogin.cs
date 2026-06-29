using System;
using System.Windows.Forms;
using cantinaPadel.BLL;

namespace cantinaPadel
{
    public partial class FrmLogin : Form
    {
        private ErrorProvider errorProvider = new ErrorProvider();

        public FrmLogin()
        {
            InitializeComponent();
        }

        // Botón INGRESAR
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // Trim() para eliminar cualquier espacio accidental
            txtUsuario.Text = txtUsuario.Text.Trim();
            txtContrasenia.Text = txtContrasenia.Text.Trim();
            errorProvider.Clear();  

            // Valida si el campo de Usuario está vacío
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "No ha ingresado Usuario.");
                txtUsuario.Focus();
                return;
            }

            // Valida si el campo de Contraseña está vacío
            if (string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                errorProvider.SetError(txtContrasenia, "Ingrese una contraseña.");
                txtContrasenia.Focus();
                return;
            }

            // Verifica extensión de la contraseña (DNI entre 7 y 8 dígitos)
            if (txtContrasenia.Text.Length < 7 || txtContrasenia.Text.Length > 8)
            {
                errorProvider.SetError(txtContrasenia, "La contraseña debe tener entre 7 y 8 dígitos.");
                txtContrasenia.Focus();
                return;
            }

            // Valida que sean solo números (permite el pegado de texto con Ctrl+V)
            if (!long.TryParse(txtContrasenia.Text, out _))
            {
                errorProvider.SetError(txtContrasenia, "La contraseña debe contener solo números.");
                txtContrasenia.Focus();
                return;
            }

            // INTEGRACIÓN: COMUNICACIÓN CON LA CAPA DE NEGOCIO (BLL)
            string usuarioIngresado = txtUsuario.Text;
            string contraseniaIngresada = txtContrasenia.Text;

            // Se instancia la clase en la carpeta BLL
            LogicaUsuario bll = new LogicaUsuario();
            int idUsuarioEncontrado;
            string rolAsignado;

            // Se llama al método externo pasándole las variables de salida
            bool credencialesValidas = bll.ValidarCredenciales(usuarioIngresado, contraseniaIngresada, out idUsuarioEncontrado, out rolAsignado);

            if (credencialesValidas)
            {
                // Se guardan los datos reales que procesó la capa lógica
                Sesion.IdUsuario = idUsuarioEncontrado;
                Sesion.Rol = rolAsignado;

                // Avisa que el Login fue exitoso y cierra
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Mensaje de alerta ante credenciales incorrectas
                MessageBox.Show("El usuario o contraseña son incorrectos.", "Error de Autenticación",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Control de eventos visuales
        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "");
            }
        }

        private void txtContrasenia_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                errorProvider.SetError(txtContrasenia, "");
            }
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si la tecla presionada es el espacio, se anula en el usuario
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

    }
}