namespace cantinaPadel.Models;

public sealed class MovimientoEfectivoHistorial
{
    public int IdCaja { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Usuario { get; set; } = string.Empty;
}
