using System;
using System.Windows.Forms;
using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDEmpleado : Form
    {
        // Se declaran las variables globales
        private readonly LogicaEmpleado _logicaEmpleado;
        private Empleado? _empleadoEdicion;

        public FrmCRUDEmpleado()
        {
            InitializeComponent();
            // Se instancia la lógica para usarla en toda la pantalla
            _logicaEmpleado = new LogicaEmpleado();
            _empleadoEdicion = null;
            this.Text = "Nuevo Empleado";
        }

        public FrmCRUDEmpleado(Empleado empleado) : this()
        {
            // Se recibe el empleado desde el listado para el modo edición
            _empleadoEdicion = empleado;
            this.Text = "Modificar Empleado";
        }

        private void FrmCRUDEmpleado_Load(object sender, EventArgs e)
        {
            // Se valida si es un alta nueva
            if (_empleadoEdicion == null)
            {
                if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
                return;
            }

            // Se cargan los datos de la Persona en los controles
            txtDni.Text = _empleadoEdicion.Persona.Dni;
            txtApellido.Text = _empleadoEdicion.Persona.Apellido;
            txtNombre.Text = _empleadoEdicion.Persona.Nombre;
            txtTelefono.Text = _empleadoEdicion.Persona.Telefono;

            // Se cargan los datos del Empleado en los controles
            txtUsuario.Text = _empleadoEdicion.NombreUsuario;
            txtContrasena.Text = _empleadoEdicion.Contrasena;
            cmbRol.SelectedItem = _empleadoEdicion.Rol;

            // Se bloquea el DNI para que no pueda ser editado
            txtDni.ReadOnly = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_empleadoEdicion == null)
                {
                    // Se crea una nueva instancia para el alta
                    Empleado nuevoEmpleado = new Empleado
                    {
                        NombreUsuario = txtUsuario.Text,
                        Contrasena = txtContrasena.Text,
                        Rol = cmbRol.SelectedItem?.ToString() ?? "",
                        Activo = true,

                        Persona = new Persona
                        {
                            Dni = txtDni.Text,
                            Apellido = txtApellido.Text,
                            Nombre = txtNombre.Text,
                            Telefono = txtTelefono.Text
                        }
                    };

                    // Se envía a la lógica (donde se aplican los Trims y validaciones)
                    _logicaEmpleado.RegistrarOGuardar(nuevoEmpleado);
                    MessageBox.Show("Empleado registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Se cargan las modificaciones en el objeto existente
                    _empleadoEdicion.Persona.Apellido = txtApellido.Text;
                    _empleadoEdicion.Persona.Nombre = txtNombre.Text;
                    _empleadoEdicion.Persona.Telefono = txtTelefono.Text;

                    _empleadoEdicion.NombreUsuario = txtUsuario.Text;
                    _empleadoEdicion.Contrasena = txtContrasena.Text;
                    _empleadoEdicion.Rol = cmbRol.SelectedItem?.ToString() ?? "";

                    // Se envía a la lógica
                    _logicaEmpleado.RegistrarOGuardar(_empleadoEdicion);
                    MessageBox.Show("Empleado modificado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Se redirige al listado si la operación fue exitosa
                RegresarAlListado();
            }
            catch (ArgumentException ex)
            {
                // Se muestran las excepciones de negocio de forma clara y limpia al usuario
                MessageBox.Show(ex.Message, "Validación de Datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // Se capturan errores inesperados de la base de datos o del sistema
                MessageBox.Show($"Error interno al procesar la solicitud: {ex.Message}", "Error General", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Se vuelve al listado sin realizar cambios
            RegresarAlListado();
        }

        private void RegresarAlListado()
        {
            // Se busca la instancia del formulario principal MDI/Contenedor
            if (this.ParentForm is FrmMain mainForm)
            {
                // Se vuelve a instanciar el listado para recargar la grilla desde la base de datos
                FrmListadoEmpleados listado = new FrmListadoEmpleados();
                mainForm.AbrirEnPanel(listado);
            }
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se permite únicamente números y la tecla de borrar (BackSpace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Se cancela la pulsación de la tecla
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se permite únicamente números y la tecla de borrar (BackSpace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Se cancela la pulsación de la tecla
            }
        }

        private void txtApellido_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Se permite únicamente letras, espacios y teclas de control como borrar
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true; // Se cancela la pulsación si es un número o símbolo
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}