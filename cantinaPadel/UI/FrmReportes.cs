using System.Drawing;
using System.Windows.Forms;

namespace cantinaPadel.UI;

// Contenedor único de la sección Reportes. Las pestañas conservan el patrón usado en los módulos principales.
public sealed class FrmReportes : Form
{
    public FrmReportes()
    {
        Text = "Reportes";
        BackColor = SystemColors.Info;
        Font = new Font("Segoe UI", 9F);

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 5) };
        var ventas = new TabPage("Ventas") { BackColor = SystemColors.Info };
        var compras = new TabPage("Compras") { BackColor = SystemColors.Info };
        var caja = new TabPage("Caja") { BackColor = SystemColors.Info };
        tabs.TabPages.AddRange(new[] { ventas, compras, caja });
        Controls.Add(tabs);

        AgregarFormulario(ventas, new FrmReporteVentas());
        tabs.SelectedIndexChanged += (_, _) =>
        {
            var pagina = tabs.SelectedTab;
            if (pagina == null || pagina.Controls.Count > 0) return;
            if (pagina == compras) AgregarFormulario(compras, new FrmReporteCompras());
            else if (pagina == caja) AgregarFormulario(caja, new FrmReporteCajas());
        };
    }

    private static void AgregarFormulario(TabPage pagina, Form formulario)
    {
        formulario.TopLevel = false;
        formulario.FormBorderStyle = FormBorderStyle.None;
        formulario.Dock = DockStyle.Fill;
        pagina.Controls.Add(formulario);
        formulario.Show();
    }
}
