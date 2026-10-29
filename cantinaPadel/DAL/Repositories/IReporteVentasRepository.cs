using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

public interface IReporteVentasRepository
{
    // Lee las ventas con sus detalles que caen entre desde y hasta (fechas inclusivas, se compara solo el día).
    // formaPago e idEmpleado son opcionales: si vienen null, ese filtro no se aplica.
    // No descarta anuladas ni ventas "hijas": esas reglas las aplica CalculadorReporteVentas
    DatosReporteVentas ObtenerDatos(DateTime desde, DateTime hasta, string? formaPago, int? idEmpleado);
}
