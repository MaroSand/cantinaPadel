using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class LogicaVentaTests
{
    [TestMethod]
    public void ConfirmarVenta_SinCliente_UsaConsumidorFinalYCalculaIva()
    {
        var repoVentas = new VentaRepositoryFake();
        var consumidor = CrearCliente(7, "Consumidor", "Final");
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(consumidor), new CajaRepositoryFake());
        var items = new[] { new ItemCarrito(CrearProducto(3, 100m), 2) }; // 242 con IVA

        var venta = logica.ConfirmarVenta(items, null, new PagoVenta { Metodo = MetodoPago.Efectivo }, 5);

        Assert.AreEqual(7, venta.IdCliente);
        Assert.AreEqual(242m, venta.Total);
        Assert.AreEqual(200m, venta.Subtotal);
        Assert.AreEqual(42m, venta.Iva);
        Assert.AreEqual("Efectivo", venta.FormaPago);
        // US-16: cada unidad vendida es su propia fila (2 cocas => 2 filas), y
        // al no ser Cuenta Corriente quedan pagadas en el momento.
        Assert.AreEqual(2, repoVentas.Detalles.Count);
        Assert.IsTrue(repoVentas.Detalles.All(d => d.Pagado));
    }

    [TestMethod]
    public void ConfirmarVenta_BilleteraVirtual_SeRegistraConEsaFormaDePago()
    {
        var repoVentas = new VentaRepositoryFake();
        var cliente = CrearCliente(8, "Ana", "Paz");
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(cliente), new CajaRepositoryFake());
        var items = new[] { new ItemCarrito(CrearProducto(3, 100m), 1) }; // 121 con IVA

        var venta = logica.ConfirmarVenta(items, cliente, new PagoVenta { Metodo = MetodoPago.BilleteraVirtual }, 5);

        Assert.AreEqual("MercadoPago", venta.FormaPago);
    }

    [TestMethod]
    public void ConfirmarVenta_CuentaCorrienteSinCliente_NoRegistra()
    {
        var repoVentas = new VentaRepositoryFake();
        var cliente = CrearCliente(8, "Ana", "Paz");
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(cliente), new CajaRepositoryFake());
        var items = new[] { new ItemCarrito(CrearProducto(3, 100m), 1) };

        Assert.ThrowsExactly<ArgumentException>(() => logica.ConfirmarVenta(items, null, new PagoVenta { Metodo = MetodoPago.CuentaCorriente }, 5));
        Assert.IsNull(repoVentas.VentaRegistrada);
    }

    [TestMethod]
    public void ConfirmarVenta_CuentaCorrienteConCliente_Registra()
    {
        var repoVentas = new VentaRepositoryFake();
        var cliente = CrearCliente(8, "Ana", "Paz");
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(cliente), new CajaRepositoryFake());
        var items = new[] { new ItemCarrito(CrearProducto(3, 100m), 1) };

        var venta = logica.ConfirmarVenta(items, cliente, new PagoVenta { Metodo = MetodoPago.CuentaCorriente }, 5);

        Assert.AreEqual("Cuenta Corriente", venta.FormaPago);
        // Queda pendiente de pago: no se marca como pagada al momento de vender.
        Assert.IsTrue(repoVentas.Detalles.All(d => !d.Pagado));
    }

    [DataTestMethod]
    [DataRow(TipoComprobante.Ticket, "T")]
    [DataRow(TipoComprobante.FacturaA, "A")]
    [DataRow(TipoComprobante.FacturaB, "B")]
    [DataRow(TipoComprobante.FacturaC, "C")]
    [DataRow(TipoComprobante.Remito, "R")]
    public void ActualizarTipoComprobante_CadaTipo_GuardaSuPropiaLetra(TipoComprobante tipo, string letraEsperada)
    {
        var repoVentas = new VentaRepositoryFake();
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(CrearCliente(1, "Test", "Test")), new CajaRepositoryFake());

        logica.ActualizarTipoComprobante(10, tipo);

        Assert.AreEqual(letraEsperada, repoVentas.UltimoTipoComprobanteGuardado);
    }

    private static Producto CrearProducto(int id, decimal precioVenta) => new() { IdProducto = id, Nombre = "Producto", PrecioVenta = precioVenta, StockActual = 10, Activo = true };
    private static Cliente CrearCliente(int id, string nombre, string apellido) => new() { IdCliente = id, Email = "cliente@test.com", Persona = new Persona { Nombre = nombre, Apellido = apellido, Activo = true } };

    private sealed class VentaRepositoryFake : IVentaRepository
    {
        public Venta? VentaRegistrada { get; private set; }
        public List<DetalleVenta> Detalles { get; } = new();
        public Venta Registrar(Venta venta, IReadOnlyCollection<DetalleVenta> detalles, int idCliente)
        {
            venta.IdVenta = 10;
            VentaRegistrada = venta;
            Detalles.AddRange(detalles);
            return venta;
        }
        public string? UltimoTipoComprobanteGuardado { get; private set; }
        public void ActualizarTipoComprobante(int idVenta, string tipoComprobante) => UltimoTipoComprobanteGuardado = tipoComprobante;
    }

    private sealed class ClienteRepositoryFake(params Cliente[] clientes) : IClienteRepository
    {
        public List<Cliente> ObtenerTodos() => clientes.ToList();
        public Cliente? ObtenerPorId(int id) => clientes.FirstOrDefault(c => c.IdCliente == id);
        public List<Cliente> Buscar(string texto) => clientes.ToList();
        public void Agregar(Cliente cliente) { }
        public void Modificar(Cliente cliente) { }
        public void Bajalogica(int id) { }
        public Persona? BuscarPersonaPorDni(string dni) => null;
    }

    private sealed class CajaRepositoryFake : ICajaRepository
    {
        public Caja? ObtenerCajaAbierta(int idEmpleado) => new() { IdCaja = 4, IdEmpleado = idEmpleado, Estado = Caja.EstadoAbierta };
        public Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado) => ObtenerCajaAbierta(idEmpleado)!;
    }
}