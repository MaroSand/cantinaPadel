using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("canchas")]
    public class Cancha
    {
        [Key]
        [Column("id_cancha")]
        public int IdCancha { get; set; }

        [Column("nombre")]
        [Required]
        public string Nombre { get; set; } = string.Empty;

        // FK al producto "Hora Padel" de esta cancha (ver comentario en el script SQL:
        // precio_venta del producto = precio por hora, fijo, no varía según horario del día)
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Column("activa")]
        public bool Activa { get; set; } = true;

        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }

        // la pantalla de Canchas solo conoce "precio por hora"
        [NotMapped]
        public decimal PrecioHora
        {
            get => Producto?.PrecioVenta ?? 0m;
            set
            {
                if (Producto == null)
                    Producto = new Producto();

                Producto.PrecioVenta = value;
            }
        }

        public void Normalizar()
        {
            Nombre = Nombre?.Trim() ?? string.Empty;
        }

        public void Validar()
        {
            Normalizar();

            if (string.IsNullOrWhiteSpace(Nombre))
                throw new ArgumentException("El nombre de la cancha es obligatorio.");

            if (Nombre.Length > 100)
                throw new ArgumentException("El nombre de la cancha no puede superar los 100 caracteres.");

            if (PrecioHora <= 0)
                throw new ArgumentException("El precio por hora debe ser mayor a cero.");
        }
    }
}