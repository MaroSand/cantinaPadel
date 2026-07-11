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
        public string NombreCategoria { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // HASTA QUE FACU CREES EL MODELO PRODUCTO
        // public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public void Normalizar()
        {
            NombreCategoria = NombreCategoria?.Trim() ?? string.Empty;
            Descripcion = Descripcion?.Trim();
        }

        public void Validar()
        {
            Normalizar();

            if (string.IsNullOrWhiteSpace(NombreCategoria))
                throw new ArgumentException("El nombre de la categoría es obligatorio.");

            if (NombreCategoria.Length > 50)
                throw new ArgumentException("El nombre de la categoría no puede superar los 50 caracteres.");
        }
    }
}