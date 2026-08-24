using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace cantinaPadel.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("nombre")]
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Column("activa")]
        public bool Activa { get; set; } = true;

        [Column("porcentaje_ganancia")]
        public decimal PorcentajeGanancia { get; set; } = 0;

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

            if (Nombre.Any(char.IsDigit))
                throw new ArgumentException("El nombre de la categoría no puede contener números.");

            if (PorcentajeGanancia < 0)
                throw new ArgumentException("El porcentaje de ganancia no puede ser negativo.");
        }
    }
}