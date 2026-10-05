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
        // Constantes para los nombres de los métodos de pago que requieren un tratamiento especial
        private const string MetodoPagoCuentaCorriente = "Cuenta Corriente";

        private readonly LogicaComprobante _logica;
        private readonly DatosVentaParaComprobante _datos;

        // Propiedad pública para acceder al comprobante generado después de la confirmación
        public Comprobante? ComprobanteGenerado { get; private set; }

        // Constructor por defecto que inicializa el formulario con datos de venta vacíos
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

            // Deshabilitar opciones de comprobante según el método de pago
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

        // Evento del botón "Confirmar" que valida la selección de tipo de comprobante y forma de entrega, y genera el comprobante
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
    }
}