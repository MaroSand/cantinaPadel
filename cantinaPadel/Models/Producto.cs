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
        
        [Column("id_proveedor")]
        public int? IdProveedor { get; set; }

        [Column("nombre")]
        [Required]
        public string Nombre { get; set; } = string.Empty;
        
        [Column("codigo_barras")]
        public string CodigoBarras { get; set; } = string.Empty;

        [Column("precio_costo")]
        public decimal PrecioCosto { get; set; } = 0;

        // Precio SIN IVA. El precio final se calcula en tiempo real
        // con PrecioConIva (ver más abajo), nunca se guarda el precio con IVA.
        [Column("precio_venta")]
        public decimal PrecioVenta { get; set; } = 0;

        [Column("stock_actual")]
        public int StockActual { get; set; } = 0;

        [Column("stock_minimo")]
        public int StockMinimo { get; set; } = 0;

        [Column("tipo")]
        public string Tipo { get; set; } = "Cantina";

        [Column("activo")]
        public bool Activo { get; set; } = true;
        
        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey("IdMarca")]
        public Marca? Marca { get; set; }

        [ForeignKey("IdProveedor")]
        public Proveedor? Proveedor { get; set; }

        // Propiedades calculadas, NO existen en la BD
        // [NotMapped] = @Transient en JPA. Se calculan en memoria cada vez que se lean
        [NotMapped]
        public decimal PrecioConIva => Math.Round(PrecioVenta * 1.21m, 2);

        [NotMapped]
        public bool StockBajo => StockActual <= StockMinimo;
    }
}