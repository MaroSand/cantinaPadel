using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models
{
    [Table("cajas")]
    public class Caja
    {
        [Key]
        [Column("id_caja")]
        public int IdCaja { get; set; }

        [Column("id_empleado")]
        public int IdEmpleado { get; set; }

    }
}
