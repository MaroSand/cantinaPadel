using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("id_marca")]
        public int? IdMarca { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("codigo_barras")]
        public string? CodigoBarras { get; set; }

        [Column("precio_costo")]
        public decimal PrecioCosto { get; set; } = 0;

        // Precio SIN IVA. El precio final se calcula en tiempo real
        [Column("precio_venta")]
        public decimal PrecioVenta { get; set; } = 0;

        [Column("stock_actual")]
        public int StockActual { get; set; } = 0;

        [Column("stock_minimo")]
        public int StockMinimo { get; set; } = 0;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey("IdMarca")]
        public Marca? Marca { get; set; }
        
        [NotMapped]
        public decimal PrecioConIva => Math.Round(PrecioVenta * 1.21m, 2);

        [NotMapped]
        public bool StockBajo => StockActual <= StockMinimo;
    }
}