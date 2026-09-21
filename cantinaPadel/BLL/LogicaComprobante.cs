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

    public class DatosVentaParaComprobante
    {
        public int IdVenta { get; set; }
        public decimal Total { get; set; }
        public List<DetalleComprobante> Items { get; set; } = new();
        public string NombreCliente { get; set; } = "Consumidor Final";
        public string? EmailCliente { get; set; }
        public string MetodoPago { get; set; } = string.Empty;

        // Datos impositivos del cliente, necesarios para decidir si se le puede emitir Factura A. Se toman de Persona.Cuit y
        // Persona.CondicionIva (ver Models/Persona.cs). Quedan null/vacíos para Consumidor Final, que no tiene por qué tener CUIT cargado
        public string? CuitCliente { get; set; }
        public string? CondicionIvaCliente { get; set; }

        // Saldo a favor del cliente en cuenta corriente al momento de emitir el comprobante (0 si no tiene crédito a favor)
        public decimal SaldoFavor { get; set; }

        // Crédito que el cliente ya tenía acumulado de un pago parcial anterior y que se sumó automáticamente a este pago para saldar
        // deuda (0 si no había crédito previo). Se muestra en el ticket para que quede constancia de que esa plata no se "perdió"
        public decimal CreditoPrevioAplicado { get; set; }
    }

    public class LogicaComprobante
    {
        // Único punto de venta por ahora (una sola caja). Si en el futuro hay más de una, esto deja de ser una constante
        private const int PUNTO_VENTA = 1;

        // En PagoVenta.FormaPago (LogicaVenta.cs) para Cuenta Corriente. Se compara sin distinguir mayúsculas por las dudas.
        private const string MetodoPagoCuentaCorriente = "Cuenta Corriente";

 
        // En Argentina, Factura A solo se le puede emitir a un cliente Responsable Inscripto (con CUIT); a Consumidor Final, o a
        // cualquier otra condición de IVA, corresponde Factura B o C
        private const string CondicionIvaResponsableInscripto = "Responsable Inscripto";

        private readonly IComprobanteRepository _repo;

        private static readonly IComprobanteRepository _repositorioEnMemoria = new ComprobanteRepositoryEnMemoria();

        public LogicaComprobante()
            : this(_repositorioEnMemoria)
        {
        }

        public LogicaComprobante(IComprobanteRepository repo)
        {
            _repo = repo;
        }

        // Numeración correlativa por tipo de comprobante
        public long GenerarProximoNumero(TipoComprobante tipo)
            => _repo.ObtenerUltimoNumero(tipo, PUNTO_VENTA) + 1;

        // Valida, arma el Comprobante, conserva su número correlativo en memoria y resuelve la entrega elegida
        // (imprimir / mandar por email / no emitir nada)
        public Comprobante ConfirmarEmision(DatosVentaParaComprobante datos, TipoComprobante tipo, FormaEntrega formaEntrega)
        {
            if (datos == null)
                throw new ArgumentException("Los datos de la venta son obligatorios.");

            // Una venta pagada con Cuenta Corriente no se factura; como máximo se emite un remito (queda pendiente de facturar
            // hasta que el cliente cancele la deuda). Esta validación es un resguardo de fondo: la UI (FrmSeleccionComprobante) ya debería
            // impedir elegir Factura A/B/C en ese caso
            bool esVentaCuentaCorriente = string.Equals(datos.MetodoPago, MetodoPagoCuentaCorriente, StringComparison.OrdinalIgnoreCase);
            bool esFactura = tipo is TipoComprobante.FacturaA or TipoComprobante.FacturaB or TipoComprobante.FacturaC;
            if (esVentaCuentaCorriente && esFactura)
                throw new ArgumentException("Una venta pagada con Cuenta Corriente no se puede facturar. Como máximo se emite un remito; la factura corresponde recién cuando el cliente cancela la deuda.");

            // Factura A requiere un cliente Responsable Inscripto con CUIT válido cargado
            // No alcanza con que el nombre no sea literalmente "Consumidor Final": cualquier otra condición de IVA (Monotributista, IVA Exento, etc.)
            // tampoco habilita Factura A, y sin CUIT no hay a quién facturarle A
            if (tipo == TipoComprobante.FacturaA)
            {
                bool esResponsableInscripto = string.Equals(datos.CondicionIvaCliente, CondicionIvaResponsableInscripto, StringComparison.OrdinalIgnoreCase);
                if (!esResponsableInscripto || string.IsNullOrWhiteSpace(datos.CuitCliente))
                    throw new ArgumentException("Para emitir Factura A el cliente debe estar cargado como Responsable Inscripto con CUIT. Elegí Factura B/C o cargá esos datos en la ficha del cliente.");
            }

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
                    break;
            }

            return comprobante;
        }

        // Arma el texto del comprobante (formato ticket angosto). Se usa tanto para imprimir como base del cuerpo del email
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

            if (datos.CreditoPrevioAplicado > 0)
                sb.AppendLine($"Crédito a favor aplicado (pago anterior): {datos.CreditoPrevioAplicado:C}");

            if (datos.SaldoFavor > 0)
                sb.AppendLine($"Saldo a favor en cuenta corriente: {datos.SaldoFavor:C}");

            return sb.ToString();
        }

        public static string EtiquetaTipo(TipoComprobante tipo) => tipo switch
        {
            TipoComprobante.Ticket => "Ticket",
            TipoComprobante.FacturaA => "Factura A",
            TipoComprobante.FacturaB => "Factura B",
            TipoComprobante.FacturaC => "Factura C",
            TipoComprobante.Remito => "Remito",
            _ => tipo.ToString()
        };

        // Impresión directa vía PrintDocument (impresora del sistema por defecto).
        // No se agregó ninguna librería de PDF: si el usuario deja configurada "Microsoft Print to PDF" como impresora predeterminada, esto mismo
        // exporta a PDF
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

    // Implementación deliberadamente en memoria: evita depender de una tabla "comprobantes" que aún no forma parte del esquema de ventas
    internal sealed class ComprobanteRepositoryEnMemoria : IComprobanteRepository
    {
        private readonly List<Comprobante> _comprobantes = new();
        private readonly object _sync = new();

        public long ObtenerUltimoNumero(TipoComprobante tipo, int puntoVenta)
        {
            lock (_sync)
                return _comprobantes.Where(c => c.Tipo == tipo && c.PuntoVenta == puntoVenta)
                    .Select(c => (long?)c.Numero).Max() ?? 0;
        }

        public void Agregar(Comprobante comprobante)
        {
            lock (_sync)
            {
                comprobante.IdComprobante = _comprobantes.Count + 1;
                _comprobantes.Add(comprobante);
            }
        }

        public Comprobante? ObtenerPorId(int idComprobante)
        {
            lock (_sync)
                return _comprobantes.FirstOrDefault(c => c.IdComprobante == idComprobante);
        }
    }
}