using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cantinaPadel.Models;

[Table("retiros_caja")]
public class RetiroCaja
{
    [Key, Column("id_retiro")]
    public int IdRetiro { get; set; }
    [Column("id_caja")]
    public int IdTurnoCaja { get; set; }
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }
    [Column("monto")]
    public decimal Monto { get; set; }
    [Column("motivo")]
    public string? Motivo { get; set; }
    [Column("fecha_retiro")]
    public DateTime FechaRetiro { get; set; } = DateTime.Now;
}
