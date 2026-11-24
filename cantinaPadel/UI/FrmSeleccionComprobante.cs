using cantinaPadel.BLL;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace cantinaPadel.UI
{
    public partial class FrmSeleccionComprobante : Form
    {
        // Constantes para los nombres de los métodos de pago que requieren un tratamiento especial
        private const string MetodoPagoCuentaCorriente = "Cuenta Corriente";

        private readonly DatosVentaParaComprobante _datos;

        public TipoComprobante? TipoSeleccionado { get; private set; }
        public FormaEntrega? FormaEntregaSeleccionada { get; private set; }
        public string? RutaPdfImpresionSeleccionada { get; private set; }

        // Constructor por defecto que inicializa el formulario con datos de venta vacíos
        public FrmSeleccionComprobante()
            : this(new DatosVentaParaComprobante())
        {
        }

        public FrmSeleccionComprobante(DatosVentaParaComprobante datos)
        {
            InitializeComponent();

            _datos = datos ?? throw new ArgumentException("Los datos de la venta son obligatorios.");

            lblTotalValor.Text = _datos.Total.ToString("C");
            rbNoEmitir.Checked = true;

            btnCancelar.Click += btnCancelar_Click;

            // Deshabilitar opciones de comprobante según el método de pago
            if (string.Equals(_datos.MetodoPago, MetodoPagoCuentaCorriente, StringComparison.OrdinalIgnoreCase))
            {
                rbTicket.Enabled = false;
                rbFacturaA.Enabled = false;
                rbFacturaB.Enabled = false;
                rbFacturaC.Enabled = false;

                rbRemito.Checked = true;

                // En cuenta corriente el total es solo un estimado (puede cambiar si suben los precios)
                lblTotalTexto.Text = "Total estimado :";
            }
            else
            {
                ConfigurarTiposPorCondicionIva();
            }
        }

        private void ConfigurarTiposPorCondicionIva()
        {
            rbFacturaA.Enabled = LogicaComprobante.TipoPermitidoParaCliente(TipoComprobante.FacturaA, _datos.CondicionIvaCliente);
            rbFacturaB.Enabled = LogicaComprobante.TipoPermitidoParaCliente(TipoComprobante.FacturaB, _datos.CondicionIvaCliente);
            rbFacturaC.Enabled = LogicaComprobante.TipoPermitidoParaCliente(TipoComprobante.FacturaC, _datos.CondicionIvaCliente);

            var predeterminado = LogicaComprobante.TipoPredeterminadoParaCliente(_datos.CondicionIvaCliente);
            switch (predeterminado)
            {
                case TipoComprobante.FacturaA: rbFacturaA.Checked = true; break;
                case TipoComprobante.FacturaB: rbFacturaB.Checked = true; break;
                case TipoComprobante.FacturaC: rbFacturaC.Checked = true; break;
                case TipoComprobante.Ticket: rbTicket.Checked = true; break;
            }
        }

        private void panelCuerpo_Paint(object sender, PaintEventArgs e)
        {

        }

        // Este diálogo solo captura la selección. La venta debe persistirse antes de emitir el comprobante.
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            TipoComprobante? tipo = ObtenerTipoSeleccionado();
            if (tipo == null)
            {
                MessageBox.Show(this, "Elegí un tipo de comprobante.", "Falta un dato",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Validar la selección de la forma de entrega
            FormaEntrega? formaEntrega = ObtenerFormaEntregaSeleccionada();
            if (formaEntrega == null)
            {
                MessageBox.Show(this, "Elegí cómo entregar el comprobante.", "Falta un dato",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (formaEntrega == FormaEntrega.Imprimir && ImpresoraPredeterminadaEsPdf())
            {
                var comprobanteSugerido = new Comprobante { Tipo = tipo.Value };
                using var dialogo = new SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = "pdf",
                    FileName = $"{LogicaComprobante.NombreArchivoSugerido(comprobanteSugerido, _datos)}.pdf",
                    Filter = "Documento PDF (*.pdf)|*.pdf",
                    OverwritePrompt = true,
                    Title = "Guardar comprobante impreso en PDF"
                };
                if (dialogo.ShowDialog(this) != DialogResult.OK)
                    return;

                RutaPdfImpresionSeleccionada = dialogo.FileName;
            }

            try
            {
                TipoSeleccionado = tipo.Value;
                FormaEntregaSeleccionada = formaEntrega.Value;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo emitir el comprobante",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Evento del botón "Cancelar" que cierra el formulario sin generar un comprobante
        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Método privado que obtiene el tipo de comprobante seleccionado por el usuario
        private TipoComprobante? ObtenerTipoSeleccionado()
        {
            if (rbTicket.Checked) return TipoComprobante.Ticket;
            if (rbFacturaA.Checked) return TipoComprobante.FacturaA;
            if (rbFacturaB.Checked) return TipoComprobante.FacturaB;
            if (rbFacturaC.Checked) return TipoComprobante.FacturaC;
            if (rbRemito.Checked) return TipoComprobante.Remito;
            return null;
        }

        private FormaEntrega? ObtenerFormaEntregaSeleccionada()
        {
            if (rbImprimir.Checked) return FormaEntrega.Imprimir;
            if (rbEmail.Checked) return FormaEntrega.Email;
            if (rbNoEmitir.Checked) return FormaEntrega.NoEmitir;
            return null;
        }

        private static bool ImpresoraPredeterminadaEsPdf()
        {
            string nombreImpresora = new PrinterSettings().PrinterName;
            return nombreImpresora.Contains("PDF", StringComparison.OrdinalIgnoreCase);
        }
    }
}
