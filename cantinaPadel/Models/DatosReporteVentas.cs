namespace cantinaPadel.Models;

// Lo que el repositorio le entrega a la lógica del reporte de ventas: datos "crudos" ya leídos de la base.
// Todo el armado y los cálculos se hacen después en CalculadorReporteVentas, sin tocar la base
public sealed class DatosReporteVentas
{
    // Ventas dentro del rango de fechas (y del método de pago / empleado elegidos), con sus Detalles cargados
    public List<Venta> Ventas { get; init; } = new();

    // Ids de los productos de la categoría "Hora Padel" (el producto asociado a cada cancha)
    public HashSet<int> IdsProductosPadel { get; init; } = new();

    // id_producto -> precio_costo actual. OJO: es el costo de HOY, no el que tenía el producto cuando se vendió
    public Dictionary<int, decimal> CostosProductos { get; init; } = new();

    // id_cliente -> "Nombre Apellido"
    public Dictionary<int, string> Clientes { get; init; } = new();

    // id_empleado -> "Nombre Apellido"
    public Dictionary<int, string> Empleados { get; init; } = new();
}
