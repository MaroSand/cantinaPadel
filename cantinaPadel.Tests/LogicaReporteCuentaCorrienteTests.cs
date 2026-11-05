using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;

namespace cantinaPadel.Tests;

[TestClass]
public class LogicaReporteCuentaCorrienteTests
{
    [TestMethod]
    public void ObtenerClientesConActividad_DevuelveLosDatosDelRepositorio()
    {
        var cliente = new ClienteCuentaCorrienteReporte
        {
            IdCliente = 7,
            NombreCompleto = "Ana Paz",
            Dni = "30111222",
            MontoAdeudado = 1500m
        };
        var repositorio = new ReporteCuentaCorrienteRepositoryFake();
        repositorio.Clientes.Add(cliente);
        var logica = new LogicaReporteCuentaCorriente(repositorio);

        var resultado = logica.ObtenerClientesConActividad();

        Assert.AreSame(repositorio.Clientes, resultado);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(1500m, resultado[0].MontoAdeudado);
    }

    [TestMethod]
    public void ObtenerMovimientos_ConsultaElHistorialDelClienteSeleccionado()
    {
        var movimiento = new MovimientoCuentaCorrienteReporte
        {
            Fecha = new DateTime(2026, 10, 2, 10, 0, 0),
            Tipo = "Pago",
            Referencia = "Venta #12",
            MedioPago = "Efectivo",
            Monto = 500m,
            CreditoPosterior = 1000m,
            Responsable = "Empleado Uno"
        };
        var repositorio = new ReporteCuentaCorrienteRepositoryFake();
        repositorio.Movimientos.Add(movimiento);
        var logica = new LogicaReporteCuentaCorriente(repositorio);

        var resultado = logica.ObtenerMovimientos(7);

        Assert.AreEqual(7, repositorio.IdClienteConsultado);
        Assert.AreSame(repositorio.Movimientos, resultado);
        Assert.AreEqual("Venta #12", resultado.Single().Referencia);
        Assert.AreEqual(1000m, resultado.Single().CreditoPosterior);
    }

    private sealed class ReporteCuentaCorrienteRepositoryFake : IReporteCuentaCorrienteRepository
    {
        public List<ClienteCuentaCorrienteReporte> Clientes { get; } = new();
        public List<MovimientoCuentaCorrienteReporte> Movimientos { get; } = new();
        public int? IdClienteConsultado { get; private set; }

        public List<ClienteCuentaCorrienteReporte> ObtenerClientesConActividad() => Clientes;

        public List<MovimientoCuentaCorrienteReporte> ObtenerMovimientos(int idCliente)
        {
            IdClienteConsultado = idCliente;
            return Movimientos;
        }
    }
}
