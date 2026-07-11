using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("Marcas")]
    public class Marca
    {
        [Key]
        [Column("id_marca")]
        public int IdMarca { get; set; }

        [Column("nombre_marca")]
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
                throw new ArgumentException("El nombre de la marca es obligatorio.");

            if (Nombre.Length > 100)
                throw new ArgumentException("El nombre de la marca no puede superar los 100 caracteres.");
        }
    }
}