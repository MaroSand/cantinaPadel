using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    public enum TipoComprobante
    {
        Ticket,
        FacturaA,
        FacturaB,
        FacturaC,
        // Punto 4/5: única opción válida cuando la venta se paga con Cuenta
        // Corriente (no se factura hasta que el cliente cancele la deuda).
        // Se agrega al FINAL del enum a propósito: EF Core guarda los enums
        // como su valor ordinal (int), así que insertarlo en el medio
        // corriería los números de los tipos que ya existan en la tabla
        // "comprobantes" el día que se conecte el repositorio real en vez
        // del repositorio en memoria que se usa hoy.
        Remito
    }

    public enum FormaEntrega
    {
        Imprimir,
        Email,
        NoEmitir
    }

    [Table("comprobantes")]
    public class Comprobante
    {
        [Key]
        [Column("id_comprobante")]
        public int IdComprobante { get; set; }

        // FK a la venta que originó este comprobante. Todavía no existe el
        // modelo/tabla Venta (US-13/US-14 en desarrollo), así que por ahora
        // se guarda solo el id, sin [ForeignKey] ni navegación. Cuando el
        // modelo Venta esté listo: agregar la relación acá y en
        // AppDbContext.OnModelCreating.
        [Column("id_venta")]
        public int IdVenta { get; set; }

        [Column("tipo")]
        public TipoComprobante Tipo { get; set; }

        [Column("punto_venta")]
        public int PuntoVenta { get; set; } = 1;

        // Numeración correlativa propia del sistema (sin AFIP: no es un CAE,
        // es solo un correlativo interno por tipo de comprobante).
        [Column("numero")]
        public long Numero { get; set; }

        [Column("forma_entrega")]
        public FormaEntrega FormaEntrega { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [NotMapped]
        public string NumeroFormateado => $"{PuntoVenta:D4}-{Numero:D8}";
    }
}