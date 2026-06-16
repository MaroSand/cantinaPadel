
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cantinaPadel
{
    public partial class FrmMain : Form
    {


        // inicializa el formulario principal, configura la interfaz según el rol del usuario y carga el módulo por defecto
        public FrmMain()
        {
            InitializeComponent();
        }

        // Evento que se ejecuta al cargar el formulario, configura la información del usuario, el menú según el rol y el módulo por defecto
        private void FrmMain_Load(object sender, EventArgs e)
        {
            ConfigurarUsuario();
            ConfigurarMenuSegunRol();
            CargarModuloPorDefecto();
        }

        // Configura la información del usuario en la interfaz, mostrando el rol y el nombre del usuario
        private void ConfigurarUsuario()
        {

            lblRolUsuario.Text = Sesion.Rol;
            lblUsuario.Text = "Usuario · " + Sesion.Rol;
        }

        // Configura la visibilidad de los botones del menú según el rol del usuario, mostrando solo las opciones permitidas para cada rol
        private void ConfigurarMenuSegunRol()
        {
            bool esAdmin = Sesion.Rol == "Admin";
            btnStock.Visible = esAdmin;
            btnCompras.Visible = esAdmin;
            btnCanchas.Visible = esAdmin;
            btnProveedores.Visible = esAdmin;
            btnEmpleados.Visible = esAdmin;
            btnReportes.Visible = esAdmin;
        }

        // Carga el módulo por defecto al iniciar la aplicación, mostrando el título "Inicio" en la interfaz
        private void CargarModuloPorDefecto()
        {

            lblTituloModulo.Text = "Inicio";
        }


        // Método para abrir un formulario dentro del panel de contenido, configurando su apariencia y mostrando el formulario
        public void AbrirEnPanel(Form formulario)
        {
            // Limpia el panel de contenido antes de agregar el nuevo formulario, asegurando que solo se muestre un módulo a la vez
            pnlContenido.Controls.Clear();
            formulario.TopLevel = false;
            formulario.FormBorderStyle = FormBorderStyle.None;
            formulario.Dock = DockStyle.Fill;
            pnlContenido.Controls.Add(formulario);
            formulario.Show();
        }

        // Evento que se ejecuta al hacer clic en el botón de cerrar sesión, muestra una confirmación y cierra la sesión si el usuario confirma
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            var confirmacion = MessageBox.Show(
                "¿Desea cerrar Sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                Sesion.CerrarSesion();
                this.Close();
            }
        }

        // Método para navegar a un módulo específico, actualizando el título del módulo en la interfaz
        private void Navegar(string modulo)
        {
            lblTituloModulo.Text = modulo;
        }

        // Eventos de clic para los botones del menú, cada uno llama al método Navegar con el nombre del módulo correspondiente
        // => es una expresión lambda que simplifica la sintaxis del método, permitiendo escribirlo en una sola línea
        private void btnInicio_Click(object sender, EventArgs e) => Navegar("Inicio");
        private void btnClientes_Click(object sender, EventArgs e) => Navegar("Clientes");
        private void btnPuntoVenta_Click(object sender, EventArgs e) => Navegar("Punto de Venta");
        private void btnStock_Click(object sender, EventArgs e) => Navegar("Stock");
        private void btnCompras_Click(object sender, EventArgs e) => Navegar("Compras");
        private void btnTurnos_Click(object sender, EventArgs e) => Navegar("Turnos");
        private void btnCanchas_Click(object sender, EventArgs e) => Navegar("Canchas");
        private void btnCaja_Click(object sender, EventArgs e) => Navegar("Caja");
        private void btnProveedores_Click(object sender, EventArgs e) => Navegar("Proveedores");
        private void btnEmpleados_Click(object sender, EventArgs e) => Navegar("Empleados");
        private void btnReportes_Click(object sender, EventArgs e) => Navegar("Reportes");

    }
}

     