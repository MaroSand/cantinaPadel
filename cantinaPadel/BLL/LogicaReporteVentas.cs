using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Opción de un combo de filtro: el texto que se ve y el id que se usa para filtrar (null = "Todos")
public sealed record OpcionFiltro(int? Id, string Texto)
{
    public override string ToString() => Texto;
}

// Lógica del reporte de ventas (US-25): valida los filtros, pide los datos y arma las filas
public class LogicaReporteVentas
{
    // Mismos valores que acepta la columna ventas.forma_pago
    public static readonly IReadOnlyList<string> FormasPago = new[]
    {
        "Efectivo", "Tarjeta", "Transferencia", "MercadoPago", "Cuenta Corriente"
    };

    private readonly IReporteVentasRepository _reporte;
    private readonly IEmpleadoRepository _empleados;

    public LogicaReporteVentas() : this(new ReporteVentasRepository(), new EmpleadoRepository()) { }

    public LogicaReporteVentas(IReporteVentasRepository reporte, IEmpleadoRepository empleados)
    {
        _reporte = reporte;
        _empleados = empleados;
    }

    // Todos los empleados, también los dados de baja: pueden tener ventas históricas que se quieran consultar
    public List<OpcionFiltro> ObtenerEmpleados()
        => _empleados.ObtenerTodos()
            .Select(e =>
            {
                string nombre = e.Persona == null
                    ? e.NombreUsuario
                    : $"{e.Persona.Nombre} {e.Persona.Apellido}".Trim();
                return new OpcionFiltro(e.IdEmpleado, e.Activo ? nombre : $"{nombre} (inactivo)");
            })
            .OrderBy(o => o.Texto, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    public static void Validar(FiltroReporteVentas filtro)
    {
        if (filtro == null)
            throw new ArgumentException("Los filtros del reporte son obligatorios.");

        if (filtro.Desde.Date > filtro.Hasta.Date)
            throw new ArgumentException("La fecha inicial no puede ser posterior a la fecha final.");

        if (!string.IsNullOrWhiteSpace(filtro.FormaPago) && !FormasPago.Contains(filtro.FormaPago))
            throw new ArgumentException("El método de pago seleccionado no es válido.");

        if (!Enum.IsDefined(filtro.Tipo))
            throw new ArgumentException("El tipo de venta seleccionado no es válido.");

        if (filtro.IdEmpleado is <= 0)
            throw new ArgumentException("El empleado seleccionado no es válido.");
    }

    public ResumenReporteVentas CalcularResumen(IEnumerable<FilaReporteVentas> filas)
        => CalculadorReporteVentas.CalcularResumen(filas);

    public List<FilaReporteVentas> Generar(FiltroReporteVentas filtro)
    {
        Validar(filtro);

        var datos = _reporte.ObtenerDatos(filtro.Desde, filtro.Hasta, filtro.FormaPago, filtro.IdEmpleado);

        return CalculadorReporteVentas.ArmarFilas(
            datos.Ventas, datos.IdsProductosPadel, datos.CostosProductos, datos.Clientes, datos.Empleados, filtro.Tipo);
    }
}
