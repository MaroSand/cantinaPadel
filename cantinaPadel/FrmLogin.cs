namespace cantinaPadel
{
    public partial class FrmLogin : Form
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        public FrmLogin()
        {
            InitializeComponent();
        }

        //Botón INGRESAR
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            //Trim() para eliminar cualquier espacio
            txtUsuario.Text = txtUsuario.Text.Trim();
            txtContrasenia.Text = txtContrasenia.Text.Trim();
            errorProvider.Clear();

            // Validar si el campo de Usuario está vacío
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "No ha ingresado Usuario.");
                txtUsuario.Focus();
                return;
            }

            // Validar si el campo de Contraseña está vacío
            if (string.IsNullOrWhiteSpace(txtContrasenia.Text))
            {
                errorProvider.SetError(txtContrasenia, "Ingrese una contraseña.");
                txtContrasenia.Focus();
                return;
            }

            //Verificar que tenga entre 7 y 8 dígitos
            if (txtContrasenia.Text.Length < 7)
            {
                errorProvider.SetError(txtContrasenia, "El DNI debe tener entre 7 y 8 dígitos.");
                txtContrasenia.Focus();
                return;
            }

            //INTEGRACIÓN: VALIDACIÓN CON LA CAPA DE DATOS
            string usuarioIngresado = txtUsuario.Text;
            string dniIngresado = txtContrasenia.Text;

            // Llamamos al método que simula la base de datos
            bool credencialesValidas = ConsultarUsuarioEnBaseDeDatos(usuarioIngresado, dniIngresado);

            if (credencialesValidas)
            {
                // Guardamos los datos en la clase estática global Sesion
                // Cuando esté EF Core, estos datos vendrán de la fila encontrada en la BD
                Sesion.IdUsuario = 1;
                Sesion.Rol = "Administrador";

                // Le avisamos a Program.cs que el Login fue exitoso y cerramos
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Si las credenciales no coinciden en la simulación
                MessageBox.Show("El usuario o el DNI son incorrectos.", "Error de Autenticación",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // MÉTODO SIMULADO (Preparado para que después se use con Entity Framework Core)
        private bool ConsultarUsuarioEnBaseDeDatos(string usuario, string dni)
        {
            // Por ahora, usamos datos de prueba fijos para que se pueda testear el flujo
            // Probar con: admin y contraseña: 12345678
            if (usuario == "admin" && dni == "12345678")
            {
                return true;
            }

            return false;
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            //Si el usuario ya escribió algo, se pasa un texto vacío("") al SetError para que el icono rojo desaparezca
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
            // Si la tecla presionada es el espacio, se anula
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }

        private void txtContrasenia_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Verifica si la tecla presionada no es un número ni la tecla de borrado
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                // Bloqueamos cualquier otra tecla (letras, símbolos, espacios)
                e.Handled = true;
            }
        }
    }

}
