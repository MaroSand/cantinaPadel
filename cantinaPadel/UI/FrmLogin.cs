using System;
using System.Windows.Forms;
using cantinaPadel.BLL;

namespace cantinaPadel
{
    public partial class FrmLogin : Form
    {
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

            // Valida si el campo de Usuario está vacío
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Debe ingresar un usuario.", "Usuario requerido",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            // Valida si el campo de Contraseña está vacío
            if (string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                MessageBox.Show("Debe ingresar una contraseña.", "Contraseña requerida",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasenia.Focus();
                return;
            }

            // No se valida acá longitud ni formato numérico: si llega algo con letras (por ejemplo pegado con Ctrl+V, ya que el KeyPress solo filtra
            // lo tipeado), al no coincidir con una contraseña real en la bd, cae en el caso ContrasenaIncorrecta y muestra el mismo mensaje

            // INTEGRACIÓN: COMUNICACIÓN CON LA CAPA DE NEGOCIO (BLL)
            string usuarioIngresado = txtUsuario.Text;
            string contraseniaIngresada = txtContrasenia.Text;

            // Se instancia la clase en la carpeta BLL
            LogicaUsuario bll = new LogicaUsuario();
            int idUsuarioEncontrado;
            string rolAsignado;

            // Se llama al método externo pasándole las variables de salida
            ResultadoLogin resultado = bll.ValidarCredenciales(usuarioIngresado, contraseniaIngresada, out idUsuarioEncontrado, out rolAsignado);

            switch (resultado)
            {
                case ResultadoLogin.Ok:
                    // Se guardan los datos reales que procesó la capa lógica
                    Sesion.IdUsuario = idUsuarioEncontrado;
                    Sesion.Rol = rolAsignado;

                    // Avisa que el Login fue exitoso y cierra
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;

                case ResultadoLogin.ContrasenaIncorrecta:
                    MessageBox.Show("La contraseña ingresada es incorrecta.", "Contraseña incorrecta",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContrasenia.Focus();
                    txtContrasenia.SelectAll();
                    break;

                case ResultadoLogin.UsuarioNoEncontrado:
                    MessageBox.Show("No existe un usuario activo con ese nombre.", "Usuario no encontrado",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUsuario.Focus();
                    txtUsuario.SelectAll();
                    break;

                case ResultadoLogin.ErrorConexion:
                default:
                    MessageBox.Show("No se pudo conectar con la base de datos. Intente nuevamente.", "Error de conexión",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        // Botón "ojito": alterna entre mostrar y ocultar la contraseña en texto plano
        private void btnVerContrasenia_Click(object sender, EventArgs e)
        {
            txtContrasenia.UseSystemPasswordChar = !txtContrasenia.UseSystemPasswordChar;
            btnVerContrasenia.Text = txtContrasenia.UseSystemPasswordChar ? "🔒" : "🔓";

            // Devuelve el foco a la contraseña, con el cursor al final, para no interrumpir la escritura
            txtContrasenia.Focus();
            txtContrasenia.SelectionStart = txtContrasenia.Text.Length;
        }

        private void txtUsuario_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Si la tecla presionada es el espacio, se anula en el usuario
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        // Solo permite dígitos en la contraseña y Backspace, para poder borrar
        // char.IsControl cubre Backspace, Delete, etc. sin bloquear la edición normal
        private void txtContrasenia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}