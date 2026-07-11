using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("nombre_categoria")]
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Column("activa")]
        public bool Activa { get; set; } = true;

        // HASTA QUE FACU CREES EL MODELO PRODUCTO
        // public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public void Normalizar()
        {
            Nombre = Nombre?.Trim() ?? string.Empty;
        }

        public void Validar()
        {
            Normalizar();

            if (string.IsNullOrWhiteSpace(Nombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.");

            if (Nombre.Length > 100)
                throw new ArgumentException("El nombre de la categoría no puede superar los 100 caracteres.");
        }
    }
}