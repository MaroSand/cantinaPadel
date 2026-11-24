using System.Drawing;
using System.Windows.Forms;

namespace cantinaPadel.UI;

internal static class ConvencionCrudPersonas
{
    private const int AnchoVentana = 1000;
    private const int AltoVentana = 680;
    private const int AltoEncabezado = 56;
    private const int AnchoBoton = 180;
    private const int AltoBoton = 42;
    private const int SeparacionBotones = 16;
    private const int MargenInferior = 24;

    public static void Configurar(
        Form formulario,
        Panel encabezado,
        Label titulo,
        Control contenedorBotones,
        Button cancelar,
        Button guardar)
    {
        formulario.ClientSize = new Size(AnchoVentana, AltoVentana);
        formulario.FormBorderStyle = FormBorderStyle.FixedDialog;
        formulario.StartPosition = FormStartPosition.CenterParent;
        formulario.MaximizeBox = false;
        formulario.MinimizeBox = false;

        encabezado.Dock = DockStyle.Top;
        encabezado.Height = AltoEncabezado;
        encabezado.BackColor = Color.Gold;
        encabezado.BorderStyle = BorderStyle.None;

        titulo.AutoSize = false;
        titulo.Location = new Point(24, 0);
        titulo.Size = new Size(encabezado.ClientSize.Width - 48, AltoEncabezado);
        titulo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        titulo.TextAlign = ContentAlignment.MiddleLeft;
        titulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);

        void AcomodarTitulo(object? sender, System.EventArgs e)
        {
            titulo.Width = Math.Max(0, encabezado.ClientSize.Width - 48);
        }

        encabezado.Resize += AcomodarTitulo;

        ConfigurarBotones(formulario, contenedorBotones, cancelar, guardar);
        formulario.Shown += (_, _) =>
        {
            AcomodarTitulo(null, System.EventArgs.Empty);
        };
        AcomodarTitulo(null, System.EventArgs.Empty);
    }

    public static void ConfigurarBotones(
        Form formulario,
        Control contenedorBotones,
        Button cancelar,
        Button guardar,
        bool centrarVerticalmente = false)
    {
        ConfigurarBoton(cancelar, Color.IndianRed);
        ConfigurarBoton(guardar, Color.ForestGreen);

        if (guardar.TabIndex > cancelar.TabIndex)
        {
            int tabIndexGuardar = guardar.TabIndex;
            guardar.TabIndex = cancelar.TabIndex;
            cancelar.TabIndex = tabIndexGuardar;
        }

        void AcomodarBotones(object? sender, System.EventArgs e)
        {
            int anchoGrupo = AnchoBoton * 2 + SeparacionBotones;
            int izquierda = Math.Max(16, (contenedorBotones.ClientSize.Width - anchoGrupo) / 2);
            int arriba = centrarVerticalmente
                ? Math.Max(0, (contenedorBotones.ClientSize.Height - AltoBoton) / 2)
                : Math.Max(0, contenedorBotones.ClientSize.Height - MargenInferior - AltoBoton);

            guardar.Location = new Point(izquierda, arriba);
            guardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cancelar.Location = new Point(izquierda + AnchoBoton + SeparacionBotones, arriba);
            cancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        }

        contenedorBotones.Resize += AcomodarBotones;
        formulario.Shown += (_, _) => AcomodarBotones(null, System.EventArgs.Empty);
        AcomodarBotones(null, System.EventArgs.Empty);
    }

    private static void ConfigurarBoton(Button boton, Color color)
    {
        boton.Size = new Size(AnchoBoton, AltoBoton);
        boton.BackColor = color;
        boton.ForeColor = Color.White;
        boton.FlatStyle = FlatStyle.Flat;
        boton.FlatAppearance.BorderSize = 0;
        boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        boton.Margin = Padding.Empty;
        boton.UseVisualStyleBackColor = false;
    }
}
