namespace cantinaPadel
{
    public partial class FrmLogin : Form
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            //Trim() para eliminar cualquier espacio
            txtUsuario.Text = txtUsuario.Text.Trim();
            txtContrasenia.Text = txtContrasenia.Text.Trim();

            // Limpia los errores anteriores para que no se acumulen
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

            // Mensaje de prueba:
            MessageBox.Show("Validación exitosa. Próximamente conectará a la BD.",
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (e.KeyChar == (char)Keys.Space)
            {
                e.Handled = true;
            }
        }
    }

}
