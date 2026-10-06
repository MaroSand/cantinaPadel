using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("ingresos_caja")]
public class IngresoCaja
{
    [Key, Column("id_ingreso")]
    public int IdIngreso { get; set; }
    [Column("id_caja")]
    public int IdTurnoCaja { get; set; }
    [Column("id_admin")]
    public int IdAdmin { get; set; }
    [Column("monto")]
    public decimal Monto { get; set; }
    [Column("fecha_ingreso")]
    public DateTime FechaIngreso { get; set; } = DateTime.Now;
}
