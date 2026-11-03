namespace cantinaPadel.BLL;

public sealed class ClienteCuentaCorrienteReporte
{
    public int IdCliente { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Dni { get; init; } = string.Empty;
    public decimal MontoAdeudado { get; init; }
}

public sealed class MovimientoCuentaCorrienteReporte
{
    public DateTime Fecha { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Referencia { get; init; } = string.Empty;
    public string MedioPago { get; init; } = string.Empty;
    public decimal Monto { get; init; }
    public decimal CreditoPosterior { get; init; }
    public string Responsable { get; init; } = string.Empty;
}
