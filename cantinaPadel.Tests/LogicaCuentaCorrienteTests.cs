using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class LogicaCuentaCorrienteTests
{
    [TestMethod]
    public void RegistrarPago_SinCliente_LanzaExcepcion()
    {
        var logica = new LogicaCuentaCorriente(new CuentaCorrienteRepositoryFake(), new ClienteRepositoryFake(), new CajaRepositoryFake());

        Assert.ThrowsExactly<ArgumentException>(() => logica.RegistrarPago(null!, 100m, 5));
    }

    [TestMethod]
    public void RegistrarPago_MontoCero_LanzaExcepcion()
    {
        var logica = new LogicaCuentaCorriente(new CuentaCorrienteRepositoryFake(), new ClienteRepositoryFake(), new CajaRepositoryFake());
        var cliente = CrearCliente(1, "Ana", "Paz");

        Assert.ThrowsExactly<ArgumentException>(() => logica.RegistrarPago(cliente, 0m, 5));
    }

    [TestMethod]
    public void RegistrarPago_SinCajaAbierta_LanzaExcepcion()
    {
        var logica = new LogicaCuentaCorriente(new CuentaCorrienteRepositoryFake(), new ClienteRepositoryFake(), new CajaRepositoryFake(sinCaja: true));
        var cliente = CrearCliente(1, "Ana", "Paz");

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(cliente, 100m, 5));
    }

    [TestMethod]
    public void RegistrarPago_ConCajaYMontoValido_DelegaEnElRepositorioConLaCajaAbierta()
    {
        var repo = new CuentaCorrienteRepositoryFake();
        var logica = new LogicaCuentaCorriente(repo, new ClienteRepositoryFake(), new CajaRepositoryFake());
        var cliente = CrearCliente(1, "Ana", "Paz");

        var resultado = logica.RegistrarPago(cliente, 150m, 5);

        Assert.AreEqual(1, repo.IdClienteRecibido);
        Assert.AreEqual(150m, repo.MontoRecibido);
        Assert.AreEqual(4, repo.IdCajaRecibido); // caja abierta simulada
        Assert.AreSame(repo.ResultadoAEntregar, resultado);
    }

    [TestMethod]
    public void ObtenerPendientes_DelegaEnElRepositorio()
    {
        var repo = new CuentaCorrienteRepositoryFake();
        repo.Pendientes.Add(new ItemDeudaCliente { IdDetalle = 1, NombreProducto = "Coca Cola", Monto = 1500m });
        var logica = new LogicaCuentaCorriente(repo, new ClienteRepositoryFake(), new CajaRepositoryFake());

        var pendientes = logica.ObtenerPendientes(1);

        Assert.AreEqual(1, pendientes.Count);
        Assert.AreEqual("Coca Cola", pendientes[0].NombreProducto);
    }

    private static Cliente CrearCliente(int id, string nombre, string apellido) =>
        new() { IdCliente = id, Email = "cliente@test.com", Persona = new Persona { Nombre = nombre, Apellido = apellido, Dni = "30111222", Activo = true } };

    private sealed class CuentaCorrienteRepositoryFake : ICuentaCorrienteRepository
    {
        public List<ItemDeudaCliente> Pendientes { get; } = new();
        public int? IdClienteRecibido { get; private set; }
        public decimal? MontoRecibido { get; private set; }
        public int? IdCajaRecibido { get; private set; }
        public ResultadoPagoCuentaCorriente ResultadoAEntregar { get; } = new();

        public List<ItemDeudaCliente> ObtenerPendientes(int idCliente) => Pendientes;

        public ResumenCuentaCorriente ObtenerResumen(int idCliente)
            => new() { Pendientes = Pendientes, Credito = 0m };

        public ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado)
        {
            IdClienteRecibido = idCliente;
            MontoRecibido = monto;
            IdCajaRecibido = idCaja;
            return ResultadoAEntregar;
        }
    }

    private sealed class ClienteRepositoryFake : IClienteRepository
    {
        public List<Cliente> ObtenerTodos() => new();
        public Cliente? ObtenerPorId(int id) => null;
        public List<Cliente> Buscar(string texto) => new();
        public void Agregar(Cliente cliente) { }
        public void Modificar(Cliente cliente) { }
        public void Bajalogica(int id) { }
        public Persona? BuscarPersonaPorDni(string dni) => null;
    }

    private sealed class CajaRepositoryFake(bool sinCaja = false) : ICajaRepository
    {
        public Caja? ObtenerCajaAbierta(int idEmpleado) => sinCaja ? null : new() { IdCaja = 4, IdEmpleado = idEmpleado, Estado = Caja.EstadoAbierta };
        public Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado) => ObtenerCajaAbierta(idEmpleado)!;
    }
}
