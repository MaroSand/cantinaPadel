using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using cantinaPadel.BLL;

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
            // No se puede cerrar sesión con la caja propia abierta
            if (TieneCajaAbierta())
            {
                AvisarCajaAbierta("cerrar sesión");
                return;
            }

            var confirmacion = MessageBox.Show(
                "¿Desea cerrar sesión?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion == DialogResult.Yes)
            {
                Sesion.CerrarSesion();
                this.Close();
            }
        }

        // Evita salir del programa con la X (o Alt+F4) mientras el usuario tenga su caja abierta.
        // Solo se bloquea el cierre iniciado por el usuario: un apagado de Windows no se frena.
        // Al cerrar sesión, Sesion.CerrarSesion() ya se ejecutó antes de Close(), por eso Sesion.Activa evita volver a validar
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.Cancel || e.CloseReason != CloseReason.UserClosing || !Sesion.Activa)
                return;

            if (TieneCajaAbierta())
            {
                e.Cancel = true;
                AvisarCajaAbierta("salir del programa");
            }
        }

        // Indica si el usuario logueado tiene una caja abierta. Si no se puede consultar la base, no se bloquea:
        // sin conexión tampoco se podría cerrar la caja y el usuario quedaría atrapado en el programa
        private static bool TieneCajaAbierta()
        {
            try
            {
                return new LogicaCaja().ObtenerCajaAbierta(Sesion.IdUsuario) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void AvisarCajaAbierta(string accion)
        {
            MessageBox.Show(this,
                $"La caja está abierta. Cerrala desde el módulo Caja antes de {accion}.\n\nSi no podés cerrarla, pedile a un administrador que la cierre.",
                "Caja abierta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Método para navegar a un módulo específico, actualizando el título del módulo en la interfaz
        private void Navegar(string modulo)
        {
            lblTituloModulo.Text = modulo;
        }

        // Eventos de clic para los botones del menú, cada uno llama al método Navegar con el nombre del módulo correspondiente
        // => es una expresión lambda que simplifica la sintaxis del método, permitiendo escribirlo en una sola línea
        private void btnInicio_Click(object sender, EventArgs e) => Navegar("Inicio");
        private void btnClientes_Click(object sender, EventArgs e)
        {
            Navegar("Clientes");
            cantinaPadel.UI.FrmListadoClientes frm = new cantinaPadel.UI.FrmListadoClientes();
            AbrirEnPanel(frm);
        }
        private void btnPuntoVenta_Click(object sender, EventArgs e)
        {
            Navegar("Punto de Venta");
            cantinaPadel.UI.FrmPuntoVenta frm = new cantinaPadel.UI.FrmPuntoVenta();
            AbrirEnPanel(frm);
        }
        private void btnStock_Click(object sender, EventArgs e)
        {
            Navegar("Stock");
            cantinaPadel.UI.FrmListadoProductos frm = new cantinaPadel.UI.FrmListadoProductos();
            AbrirEnPanel(frm);
        }
        private void btnCompras_Click(object sender, EventArgs e) => Navegar("Compras");
        private void btnTurnos_Click(object sender, EventArgs e)
        {
            Navegar("Alquiler por Dia");
            cantinaPadel.UI.FrmAlquilerDia frm = new cantinaPadel.UI.FrmAlquilerDia();
            AbrirEnPanel(frm);
        }
        private void btnCanchas_Click(object sender, EventArgs e)
        {
            Navegar("Canchas");
            cantinaPadel.UI.FrmCanchas frm = new cantinaPadel.UI.FrmCanchas();
            AbrirEnPanel(frm);
        }
        private void btnCaja_Click(object sender, EventArgs e)
        {
            Navegar("Caja");
            AbrirEnPanel(new cantinaPadel.UI.FrmGestionCaja());
        }
        private void btnProveedores_Click(object sender, EventArgs e)
        {
            Navegar("Proveedores");
            cantinaPadel.UI.FrmListadoProveedores frm = new cantinaPadel.UI.FrmListadoProveedores();
            AbrirEnPanel(frm);
        }
        private void btnEmpleados_Click(object sender, EventArgs e)
        {
            Navegar("Empleados");
            cantinaPadel.UI.FrmListadoEmpleados frm = new cantinaPadel.UI.FrmListadoEmpleados();
            AbrirEnPanel(frm);
        }
        private void btnReportes_Click(object sender, EventArgs e) => Navegar("Reportes");

    }
}