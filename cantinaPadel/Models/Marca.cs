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
        public string NombreMarca { get; set; } = string.Empty;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // HASTA QUE FACU CREES EL MODELO PRODUCTO
        // public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

        public void Normalizar()
        {
            NombreMarca = NombreMarca?.Trim() ?? string.Empty;
        }

        public void Validar()
        {
            Normalizar();

            if (string.IsNullOrWhiteSpace(NombreMarca))
                throw new ArgumentException("El nombre de la marca es obligatorio.");

            if (NombreMarca.Length > 50)
                throw new ArgumentException("El nombre de la marca no puede superar los 50 caracteres.");
        }
    }
}