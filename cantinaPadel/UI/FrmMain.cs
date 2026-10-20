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
            // El botón debe quedar conectado independientemente de si hay una caja pendiente de cierre.
            btnCompras.Click += btnCompras_Click;
            Shown += FrmMain_Shown;
        }

        // Si el usuario dejó una caja abierta en una sesión anterior (cierre abrupto del programa, corte de luz),
        // se lo obliga a cerrarla antes de poder hacer cualquier otra cosa. Después puede abrir una caja nueva
        private void FrmMain_Shown(object? sender, EventArgs e)
        {
            try
            {
                var logicaCaja = new LogicaCaja();
                var pendiente = logicaCaja.ObtenerCajaPendienteDeCierre(Sesion.IdUsuario);
                if (pendiente == null) return;

                using var cierre = new cantinaPadel.UI.FrmCierreCaja(logicaCaja, pendiente, obligatorio: true);
                cierre.ShowDialog(this);
                MessageBox.Show(this, "La caja quedó cerrada. Abrí una caja nueva para poder vender.", "Caja cerrada",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                AbrirCaja();
            }
            catch (Exception)
            {
                // Sin conexión a la base no se puede consultar ni cerrar la caja: no se bloquea el ingreso
            }
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

            // Ejemplo: "Juan Pérez - Administrador"
            string rol = Sesion.Rol == "Admin" ? "Administrador" : (Sesion.Rol ?? string.Empty);
            string texto = string.IsNullOrWhiteSpace(Sesion.NombreCompleto)
                ? rol
                : $"{Sesion.NombreCompleto} - {rol}";

            // Si el texto es largo, que haga salto de línea dentro del encabezado en vez de cortarse
            lblRolUsuario.MaximumSize = new Size(pnlSidebarHeader.Width - 20, 0);
            lblRolUsuario.Text = texto;
            lblUsuario.Text = texto;
        }

        // Configura la visibilidad de los botones del menú según el rol del usuario, mostrando solo las opciones permitidas para cada rol
        private void ConfigurarMenuSegunRol()
        {
            bool esAdmin = Sesion.Rol == "Admin";
            btnStock.Visible = esAdmin;
            btnCompras.Visible = esAdmin; // Compras mueve stock y costos, por eso es solo para administradores
            btnCanchas.Visible = esAdmin;
            btnProveedores.Visible = esAdmin;
            btnEmpleados.Visible = esAdmin;
            btnReportes.Visible = esAdmin;
        }

        // Al ingresar al sistema, la primera pantalla es siempre la Caja (para abrirla o ver su estado)
        private void CargarModuloPorDefecto()
        {
            AbrirCaja();
        }

        // Muestra el módulo Caja en el panel de contenido
        private void AbrirCaja()
        {
            Navegar("Caja");
            AbrirEnPanel(new cantinaPadel.UI.FrmGestionCaja());
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

        // Evita salir del programa con la X (o Alt+F4) mientras el usuario tenga su caja abierta
        // Solo se bloquea el cierre iniciado por el usuario: un apagado de Windows no se frena
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
                return;
            }

            // Confirmación antes de salir del sistema con la X (o Alt+F4)
            var confirmacion = MessageBox.Show(
                this, "¿Está seguro de que desea salir?", "Confirmar salida",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
                e.Cancel = true;
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

        // Verifica que el usuario logueado tenga su caja abierta antes de entrar al Punto de Venta
        private bool PuedeUsarPuntoVenta()
        {
            try
            {
                var logicaCaja = new LogicaCaja();
                if (logicaCaja.ObtenerCajaAbierta(Sesion.IdUsuario) != null)
                    return true;

                MessageBox.Show(this, "No tenés una caja abierta. Abrí tu caja para poder usar el Punto de Venta.",
                    "Caja cerrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show(this, "No se pudo verificar el estado de la caja. Intente nuevamente.",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void AvisarCajaAbierta(string accion)
        {
            MessageBox.Show(this,
                $"La caja está abierta. Cerrala desde el módulo Caja antes de {accion}.\n\nSi no podés cerrarla, pedile a un administrador que la cierre.",
                "Caja abierta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Método para navegar a un módulo específico, actualizando el título del módulo y el título de la ventana
        private void Navegar(string modulo)
        {
            lblTituloModulo.Text = modulo;
            this.Text = "Cantina y Pádel - " + modulo;
        }

        // Eventos de clic para los botones del menú, cada uno llama al método Navegar con el nombre del módulo correspondiente
        // => es una expresión lambda que simplifica la sintaxis del método, permitiendo escribirlo en una sola línea
        private void btnClientes_Click(object sender, EventArgs e)
        {
            Navegar("Clientes");
            cantinaPadel.UI.FrmListadoClientes frm = new cantinaPadel.UI.FrmListadoClientes();
            AbrirEnPanel(frm);
        }
        private void btnPuntoVenta_Click(object sender, EventArgs e)
        {
            // Sin caja abierta no se puede vender: se avisa y se lleva al usuario a la pantalla de Caja
            if (!PuedeUsarPuntoVenta())
            {
                AbrirCaja();
                return;
            }

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
        private void btnCompras_Click(object sender, EventArgs e)
        {
            Navegar("Compras");
            cantinaPadel.UI.FrmListadoCompras frm = new cantinaPadel.UI.FrmListadoCompras();
            AbrirEnPanel(frm);
        }
        private void btnTurnos_Click(object sender, EventArgs e)
        {
            Navegar("Turnos");
            cantinaPadel.UI.FrmAlquilerDia frm = new cantinaPadel.UI.FrmAlquilerDia();
            AbrirEnPanel(frm);
        }
        private void btnCanchas_Click(object sender, EventArgs e)
        {
            Navegar("Canchas");
            cantinaPadel.UI.FrmCanchas frm = new cantinaPadel.UI.FrmCanchas();
            AbrirEnPanel(frm);
        }
        private void btnCaja_Click(object sender, EventArgs e) => AbrirCaja();
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
