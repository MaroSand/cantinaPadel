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
        Assert.AreEqual(1, repoVentas.Detalles.Count);
        Assert.AreEqual(2, repoVentas.Detalles[0].Cantidad);
    }

    [TestMethod]
    public void ConfirmarVenta_BilleteraVirtual_SeRegistraConEsaFormaDePago()
    {
        var repoVentas = new VentaRepositoryFake();
        var cliente = CrearCliente(8, "Ana", "Paz");
        var logica = new LogicaVenta(repoVentas, new ClienteRepositoryFake(cliente), new CajaRepositoryFake());
        var items = new[] { new ItemCarrito(CrearProducto(3, 100m), 1) }; // 121 con IVA

        var venta = logica.ConfirmarVenta(items, cliente, new PagoVenta { Metodo = MetodoPago.BilleteraVirtual }, 5);

        Assert.AreEqual("Billetera Virtual", venta.FormaPago);
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
    }

    private static Producto CrearProducto(int id, decimal precioVenta) => new() { IdProducto = id, Nombre = "Producto", PrecioVenta = precioVenta, StockActual = 10, Activo = true };
    private static Cliente CrearCliente(int id, string nombre, string apellido) => new() { IdCliente = id, Email = "cliente@test.com", Persona = new Persona { Nombre = nombre, Apellido = apellido, Activo = true } };

    private sealed class VentaRepositoryFake : IVentaRepository
    {
        public Venta? VentaRegistrada { get; private set; }
        public List<DetalleVenta> Detalles { get; } = new();
        public Venta Registrar(Venta venta, IReadOnlyCollection<DetalleVenta> detalles)
        {
            venta.IdVenta = 10;
            VentaRegistrada = venta;
            Detalles.AddRange(detalles);
            return venta;
        }
        public void ActualizarTipoComprobante(int idVenta, string tipoComprobante) { }
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