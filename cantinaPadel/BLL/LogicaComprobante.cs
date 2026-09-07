using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Mail;
using System.Text;

namespace cantinaPadel.BLL
{
    // Una línea del carrito, tal como se necesita para imprimir el
    // comprobante (nombre + cantidad + precio, nada más).
    public class DetalleComprobante
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    // Todo lo que este módulo necesita de "la venta", sin depender del
    // modelo Venta (todavía no existe: lo arman US-13/US-14). Cuando esa
    // tabla esté lista, este objeto se termina de armar a partir de la
    // Venta real y el IdVenta pasa a ser el de esa venta ya guardada.
    public class DatosVentaParaComprobante
    {
        public int IdVenta { get; set; }
        public decimal Total { get; set; }
        public List<DetalleComprobante> Items { get; set; } = new();
        public string NombreCliente { get; set; } = "Consumidor Final";
        public string? EmailCliente { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
    }

    public class LogicaComprobante
    {
        // Único punto de venta por ahora (una sola caja). Si en el futuro
        // hay más de una, esto deja de ser una constante.
        private const int PUNTO_VENTA = 1;

        private readonly IComprobanteRepository _repo;

        public LogicaComprobante()
            : this(new ComprobanteRepository())
        {
        }

        public LogicaComprobante(IComprobanteRepository repo)
        {
            _repo = repo;
        }

        // Numeración correlativa por tipo de comprobante (sin AFIP: es un
        // correlativo propio del sistema, no un CAE).
        public long GenerarProximoNumero(TipoComprobante tipo)
            => _repo.ObtenerUltimoNumero(tipo, PUNTO_VENTA) + 1;

        // Punto de entrada de US-15: valida, arma el Comprobante, lo
        // persiste con su número correlativo y resuelve la entrega elegida
        // (imprimir / mandar por email / no emitir nada).
        public Comprobante ConfirmarEmision(DatosVentaParaComprobante datos, TipoComprobante tipo, FormaEntrega formaEntrega)
        {
            if (datos == null)
                throw new ArgumentException("Los datos de la venta son obligatorios.");

            if (formaEntrega == FormaEntrega.Email && string.IsNullOrWhiteSpace(datos.EmailCliente))
                throw new ArgumentException("Para enviar el comprobante por email hace falta el email del cliente.");

            var comprobante = new Comprobante
            {
                IdVenta = datos.IdVenta,
                Tipo = tipo,
                PuntoVenta = PUNTO_VENTA,
                Numero = GenerarProximoNumero(tipo),
                FormaEntrega = formaEntrega,
                Total = datos.Total,
                IdUsuario = Sesion.IdUsuario,
                FechaEmision = DateTime.Now
            };

            _repo.Agregar(comprobante);

            switch (formaEntrega)
            {
                case FormaEntrega.Imprimir:
                    Imprimir(comprobante, datos);
                    break;
                case FormaEntrega.Email:
                    EnviarPorEmail(comprobante, datos);
                    break;
                case FormaEntrega.NoEmitir:
                    // No hay nada más que hacer: queda registrado igual,
                    // solo que no se imprime ni se manda.
                    break;
            }

            return comprobante;
        }

        // Arma el texto del comprobante (formato ticket angosto). Se usa
        // tanto para imprimir como base del cuerpo del email.
        public string GenerarTexto(Comprobante comprobante, DatosVentaParaComprobante datos)
        {
            var sb = new StringBuilder();
            sb.AppendLine(EtiquetaTipo(comprobante.Tipo).ToUpper());
            sb.AppendLine($"Comprobante Nº {comprobante.NumeroFormateado}");
            sb.AppendLine($"Fecha: {comprobante.FechaEmision:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Cliente: {datos.NombreCliente}");
            sb.AppendLine(new string('-', 32));

            foreach (var item in datos.Items)
                sb.AppendLine($"{item.Cantidad,3} x {item.Nombre,-18} {item.Subtotal,7:C}");

            sb.AppendLine(new string('-', 32));
            if (!string.IsNullOrWhiteSpace(datos.MetodoPago))
                sb.AppendLine($"Método de pago: {datos.MetodoPago}");
            sb.AppendLine($"TOTAL: {comprobante.Total:C}");

            return sb.ToString();
        }

        public static string EtiquetaTipo(TipoComprobante tipo) => tipo switch
        {
            TipoComprobante.Ticket => "Ticket",
            TipoComprobante.FacturaA => "Factura A",
            TipoComprobante.FacturaB => "Factura B",
            TipoComprobante.FacturaC => "Factura C",
            _ => tipo.ToString()
        };

        // Impresión directa vía PrintDocument (impresora del sistema por
        // defecto). No se agregó ninguna librería de PDF: si el usuario deja
        // configurada "Microsoft Print to PDF" como impresora predeterminada
        // (viene instalada por default en Windows), esto mismo termina
        // exportando a PDF sin código extra.
        private void Imprimir(Comprobante comprobante, DatosVentaParaComprobante datos)
        {
            string texto = GenerarTexto(comprobante, datos);

            using var doc = new PrintDocument();
            doc.PrintPage += (s, e) =>
            {
                e.Graphics!.DrawString(texto, new Font("Consolas", 10), Brushes.Black, 10, 10);
            };
            doc.Print();
        }

        // TODO: por ahora usa la configuración de SMTP que tenga la máquina
        // (SmtpClient sin parámetros lee app.config). Falta definir de dónde
        // sale esa configuración real (usuario/contraseña de la cantina).
        private void EnviarPorEmail(Comprobante comprobante, DatosVentaParaComprobante datos)
        {
            string texto = GenerarTexto(comprobante, datos);

            using var mensaje = new MailMessage();
            mensaje.To.Add(datos.EmailCliente!);
            mensaje.Subject = $"{EtiquetaTipo(comprobante.Tipo)} Nº {comprobante.NumeroFormateado} - Cantina Padel";
            mensaje.Body = texto;

            using var smtp = new SmtpClient();
            smtp.Send(mensaje);
        }
    }
}