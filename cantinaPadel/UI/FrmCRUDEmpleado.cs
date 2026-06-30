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
            // Se validan los campos obligatorios antes de continuar
            if (string.IsNullOrWhiteSpace(txtDni.Text) || string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text) || cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Se verifica si es un registro nuevo
                if (_empleadoEdicion == null)
                {
                    // Se crea la nueva entidad completa
                    Empleado nuevoEmpleado = new Empleado
                    {
                        NombreUsuario = txtUsuario.Text.Trim(),
                        Contrasena = txtContrasena.Text.Trim(),
                        Rol = cmbRol.SelectedItem.ToString()!,
                        Activo = true,

                        Persona = new Persona
                        {
                            Dni = txtDni.Text.Trim(),
                            Apellido = txtApellido.Text.Trim(),
                            Nombre = txtNombre.Text.Trim(),
                            Telefono = txtTelefono.Text.Trim()
                        }
                    };

                    // Se envía el nuevo objeto a la capa de negocio
                    _logicaEmpleado.RegistrarOGuardar(nuevoEmpleado);
                    MessageBox.Show("Empleado registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Se actualizan los datos de la persona existente
                    _empleadoEdicion.Persona.Apellido = txtApellido.Text.Trim();
                    _empleadoEdicion.Persona.Nombre = txtNombre.Text.Trim();
                    _empleadoEdicion.Persona.Telefono = txtTelefono.Text.Trim();

                    // Se actualizan los datos del empleado existente
                    _empleadoEdicion.NombreUsuario = txtUsuario.Text.Trim();
                    _empleadoEdicion.Contrasena = txtContrasena.Text.Trim();
                    _empleadoEdicion.Rol = cmbRol.SelectedItem.ToString()!;

                    // Se envían las modificaciones a la capa de negocio
                    _logicaEmpleado.RegistrarOGuardar(_empleadoEdicion);
                    MessageBox.Show("Empleado personalizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Se ejecuta la redirección limpia hacia la pantalla del listado
                RegresarAlListado();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el empleado: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}