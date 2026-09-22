using cantinaPadel.BLL;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cantinaPadel.UI
{
    public partial class FrmSeleccionComprobante : Form
    {
        // Mismo texto que usa PagoVenta.FormaPago (LogicaVenta.cs) y
        // LogicaComprobante para Cuenta Corriente. Comparación sin
        // distinguir mayúsculas, igual que en LogicaComprobante.
        private const string MetodoPagoCuentaCorriente = "Cuenta Corriente";

        private readonly LogicaComprobante _logica;
        private readonly DatosVentaParaComprobante _datos;

        // Se completa recién al confirmar; la pantalla que abrió este form
        // (FrmMetodoPago, US-14) la puede leer después de que ShowDialog()
        // devuelva DialogResult.OK.
        public Comprobante? ComprobanteGenerado { get; private set; }

        // Constructor sin parámetros: lo pide el Diseñador de Windows Forms
        // para poder abrir este form en modo diseño. No usar en tiempo de
        // ejecución real; para eso está el constructor de abajo.
        public FrmSeleccionComprobante()
            : this(new DatosVentaParaComprobante())
        {
        }

        public FrmSeleccionComprobante(DatosVentaParaComprobante datos)
            : this(datos, new LogicaComprobante())
        {
        }

        public FrmSeleccionComprobante(DatosVentaParaComprobante datos, LogicaComprobante logica)
        {
            InitializeComponent();

            _datos = datos ?? throw new ArgumentException("Los datos de la venta son obligatorios.");
            _logica = logica;

            lblTotalValor.Text = _datos.Total.ToString("C");

            btnCancelar.Click += btnCancelar_Click;

            // Punto 4: una venta pagada con Cuenta Corriente no se factura,
            // como máximo se emite un remito. Se deja Remito preseleccionado
            // y fijo, y se deshabilitan el resto de las opciones para que el
            // empleado no pueda elegirlas (la validación de fondo también
            // vive en LogicaComprobante.ConfirmarEmision, por si algo llega
            // a este form con el radio mal seteado).
            // OJO: rbRemito se deja Enabled = true a propósito. Un
            // RadioButton ya marcado no se desmarca haciendo clic de nuevo
            // (no es un checkbox), así que no hace falta deshabilitarlo
            // para "fijarlo"; y si lo deshabilitamos, WinForms lo pinta en
            // gris junto con los demás y el empleado no distingue de un
            // vistazo que Remito sigue marcado.
            if (string.Equals(_datos.MetodoPago, MetodoPagoCuentaCorriente, StringComparison.OrdinalIgnoreCase))
            {
                rbTicket.Enabled = false;
                rbFacturaA.Enabled = false;
                rbFacturaB.Enabled = false;
                rbFacturaC.Enabled = false;

                rbRemito.Checked = true;
            }
        }

        private void panelCuerpo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            TipoComprobante? tipo = ObtenerTipoSeleccionado();
            if (tipo == null)
            {
                MessageBox.Show(this, "Elegí un tipo de comprobante.", "Falta un dato",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormaEntrega? formaEntrega = ObtenerFormaEntregaSeleccionada();
            if (formaEntrega == null)
            {
                MessageBox.Show(this, "Elegí cómo entregar el comprobante.", "Falta un dato",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ComprobanteGenerado = _logica.ConfirmarEmision(_datos, tipo.Value, formaEntrega.Value);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "No se pudo emitir el comprobante",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

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
    }
}