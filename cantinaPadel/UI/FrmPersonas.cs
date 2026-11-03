using System.Drawing;
using System.Windows.Forms;

namespace cantinaPadel.UI
{
    // Pantalla "Personas": junta en una sola ventana, con pestañas, los listados de Clientes, Empleados y Proveedores
    public class FrmPersonas : Form
    {
        private readonly TabControl tabControlMain = new();
        private readonly TabPage tabPageClientes = new("Clientes");
        private readonly TabPage tabPageEmpleados = new("Empleados");
        private readonly TabPage tabPageProveedores = new("Proveedores");

        public FrmPersonas()
        {
            Text = "Personas";
            Font = new Font("Segoe UI", 9F);
            BackColor = SystemColors.Info;

            foreach (var pestana in new[] { tabPageClientes, tabPageEmpleados, tabPageProveedores })
                pestana.BackColor = SystemColors.Info;

            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Controls.Add(tabPageClientes);

            // Empleados y Proveedores son solo para el Administrador
            if (EsAdministrador())
            {
                tabControlMain.Controls.Add(tabPageEmpleados);
                tabControlMain.Controls.Add(tabPageProveedores);
            }

            Controls.Add(tabControlMain);

            Load += FrmPersonas_Load;
            tabControlMain.SelectedIndexChanged += (_, _) => CargarPestanaSeleccionada();
        }

        // La primera pestaña (Clientes) ya está seleccionada al abrir: no dispara SelectedIndexChanged, así que se carga acá
        private void FrmPersonas_Load(object? sender, EventArgs e) => CargarPestanaSeleccionada();

        private void CargarPestanaSeleccionada()
        {
            if (tabControlMain.SelectedTab == tabPageClientes)
                CargarFormularioEnTab<FrmListadoClientes>(tabPageClientes);
            else if (tabControlMain.SelectedTab == tabPageEmpleados && EsAdministrador())
                CargarFormularioEnTab<FrmListadoEmpleados>(tabPageEmpleados);
            else if (tabControlMain.SelectedTab == tabPageProveedores && EsAdministrador())
                CargarFormularioEnTab<FrmListadoProveedores>(tabPageProveedores);
        }

        private static void CargarFormularioEnTab<T>(TabPage tab) where T : Form, new()
        {
            // Ya está cargado
            if (tab.Controls.Count > 0) return;

            var formulario = new T
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            tab.Controls.Add(formulario);
            formulario.Show();
        }

        private static bool EsAdministrador()
            => string.Equals(global::cantinaPadel.Sesion.Rol, "Admin", StringComparison.Ordinal);
    }
}