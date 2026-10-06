using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

[TestClass]
public class PagosCuentaCorrienteTests
{
    private const int IdPedro = 1;
    private const int IdLucia = 2;
    private const int IdEmpleado = 5;

    private static (LogicaCuentaCorriente Logica, CuentaCorrienteEnMemoria Repo) Crear()
    {
        var repo = new CuentaCorrienteEnMemoria();
        var logica = new LogicaCuentaCorriente(repo, new ClienteRepositoryFake(), new CajaRepositoryAbiertaFake());
        return (logica, repo);
    }

    private static List<MovimientoCuentaCorriente> Pagos(CuentaCorrienteEnMemoria repo)
        => repo.Movimientos.Where(m => m.Tipo == MovimientoCuentaCorriente.TipoPago).ToList();

    private static Cliente CrearCliente(int id) =>
        new() { IdCliente = id, Email = "cliente@test.com", Persona = new Persona { Nombre = "Cliente", Apellido = id.ToString(), Activo = true } };

    // Pedro debe 10 cocas de $1.500
    private static (LogicaCuentaCorriente Logica, CuentaCorrienteEnMemoria Repo) CrearConDiezCocas()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, Enumerable.Repeat(1500m, 10).ToArray());
        return (logica, repo);
    }

    // Equivale a ProductoRepository cuando cambia el precio de un producto con unidades impagas del cliente
    private static void CambiarPrecioPendientes(CuentaCorrienteEnMemoria repo, int idCliente, decimal nuevoMonto)
    {
        foreach (var idDetalle in repo.ObtenerPendientes(idCliente).Select(p => p.IdDetalle).ToList())
            repo.CambiarMontoPendiente(idDetalle, nuevoMonto);
    }

    // Pago parcial

    [TestMethod]
    public void PagoParcial_SaldaSoloLasUnidadesQueCubreCompletas()
    {
        var (logica, repo) = CrearConDiezCocas();

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);

        Assert.AreEqual(14000m, resultado.MontoRecibido);
        Assert.HasCount(9, resultado.ItemsPagados);
        Assert.HasCount(1, resultado.ItemsPendientes);
        Assert.AreEqual(13500m, resultado.MontoAplicado);
        Assert.AreEqual(1500m, resultado.DeudaPendiente);
        Assert.AreEqual(500m, resultado.CreditoResultante);
        Assert.AreEqual(0m, resultado.SaldoAFavor);
    }

    [TestMethod]
    public void PagoParcial_ElResumenMuestraLaDeudaNetaDescontandoLoEntregado()
    {
        var (logica, _) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);

        var resumen = logica.ObtenerResumen(IdPedro);

        Assert.HasCount(1, resumen.Pendientes);
        Assert.AreEqual(1500m, resumen.DeudaBruta);
        Assert.AreEqual(500m, resumen.Credito);
        Assert.AreEqual(1000m, resumen.DeudaNeta);
        Assert.AreEqual(500m, resumen.PagosParcialesAcreditados);
        Assert.AreEqual(0m, resumen.SaldoAFavor);
    }

    [TestMethod]
    public void PagoParcial_MenorAUnaUnidad_NoSaldaNadaPeroQuedaAcreditado()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m, 1500m);

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 1000m, IdEmpleado);

        Assert.IsEmpty(resultado.ItemsPagados);
        Assert.HasCount(3, resultado.ItemsPendientes);
        Assert.AreEqual(1000m, resultado.CreditoResultante);
        Assert.AreEqual(3500m, logica.ObtenerResumen(IdPedro).DeudaNeta);
    }

    [TestMethod]
    public void PagoParcial_SaldaLasUnidadesMasViejasPrimero()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m, 1500m);
        var idsEnOrden = repo.ObtenerPendientes(IdPedro).Select(p => p.IdDetalle).ToList();

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 3000m, IdEmpleado);

        CollectionAssert.AreEqual(idsEnOrden.Take(2).ToList(), resultado.ItemsPagados.Select(i => i.IdDetalle).ToList());
        CollectionAssert.AreEqual(idsEnOrden.Skip(2).ToList(), resultado.ItemsPendientes.Select(i => i.IdDetalle).ToList());
    }

    [TestMethod]
    public void PagoParcial_UnidadCaraMasVieja_NoSeSalteaParaPagarLaMasBarata()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 2000m, 1500m);

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 1600m, IdEmpleado);

        Assert.IsEmpty(resultado.ItemsPagados);
        Assert.AreEqual(1600m, resultado.CreditoResultante);
    }

    // Pagos sucesivos

    [TestMethod]
    public void DosPagosParciales_ElCreditoDelPrimeroSeSumaAlSegundo()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m);

        var primero = logica.RegistrarPago(CrearCliente(IdPedro), 1000m, IdEmpleado);
        var segundo = logica.RegistrarPago(CrearCliente(IdPedro), 500m, IdEmpleado);

        Assert.IsEmpty(primero.ItemsPagados);
        Assert.HasCount(1, segundo.ItemsPagados);
        Assert.AreEqual(0m, segundo.CreditoResultante);
        Assert.AreEqual(1500m, logica.ObtenerResumen(IdPedro).DeudaNeta);
    }

    [TestMethod]
    public void PagosParcialesSucesivos_HastaCancelarToda_LaDeudaQuedaEnCero()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m, 1500m);

        logica.RegistrarPago(CrearCliente(IdPedro), 2000m, IdEmpleado);
        logica.RegistrarPago(CrearCliente(IdPedro), 2500m, IdEmpleado);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.IsEmpty(resumen.Pendientes);
        Assert.AreEqual(0m, resumen.DeudaNeta);
        Assert.AreEqual(0m, resumen.Credito);
        Assert.AreEqual(0m, resumen.SaldoAFavor);
    }

    [TestMethod]
    public void PagoParcialYLuegoPagoPorLaDeudaNetaRestante_CancelaTodo()
    {
        var (logica, _) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);

        // Quedan $1.500 de deuda bruta con $500 acreditados: faltan $1.000.
        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 1000m, IdEmpleado);

        Assert.HasCount(1, resultado.ItemsPagados);
        Assert.IsEmpty(resultado.ItemsPendientes);
        Assert.AreEqual(0m, resultado.CreditoResultante);
        Assert.IsEmpty(logica.ObtenerPendientes(IdPedro));
    }

    // Pago total

    [TestMethod]
    public void PagoTotal_SaldaTodasLasUnidadesYNoDejaCredito()
    {
        var (logica, _) = CrearConDiezCocas();

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 15000m, IdEmpleado);

        Assert.HasCount(10, resultado.ItemsPagados);
        Assert.IsEmpty(resultado.ItemsPendientes);
        Assert.AreEqual(15000m, resultado.MontoAplicado);
        Assert.AreEqual(0m, resultado.DeudaPendiente);
        Assert.AreEqual(0m, resultado.CreditoResultante);
        Assert.AreEqual(0m, resultado.SaldoAFavor);
    }

    [TestMethod]
    public void PagoTotal_ElResumenQuedaEnCero()
    {
        var (logica, _) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 15000m, IdEmpleado);

        var resumen = logica.ObtenerResumen(IdPedro);

        Assert.IsEmpty(resumen.Pendientes);
        Assert.AreEqual(0m, resumen.DeudaBruta);
        Assert.AreEqual(0m, resumen.DeudaNeta);
        Assert.AreEqual(0m, resumen.SaldoAFavor);
    }

    // Pagos rechazados

    [TestMethod]
    public void Pago_QueSuperaLaDeuda_SeRechazaYNoCambiaNada()
    {
        var (logica, repo) = CrearConDiezCocas();

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 15000.01m, IdEmpleado));

        Assert.HasCount(10, logica.ObtenerPendientes(IdPedro));
        Assert.AreEqual(0m, logica.ObtenerResumen(IdPedro).Credito);
        Assert.IsEmpty(Pagos(repo));
    }

    [TestMethod]
    public void Pago_QueSuperaLaDeudaNeta_AunqueNoLaBruta_SeRechaza()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 500m, IdEmpleado);

        // Deuda bruta $1.500, crédito $500 => deuda neta $1.000. Cobrar $1.001 dejaría saldo a favor.
        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 1001m, IdEmpleado));
        Assert.HasCount(1, Pagos(repo));
    }

    [TestMethod]
    public void Pago_ClienteSinDeuda_SeRechaza()
    {
        var (logica, repo) = Crear();

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 100m, IdEmpleado));
        Assert.IsEmpty(Pagos(repo));
    }

    [TestMethod]
    public void Pago_ClienteQueYaCancelaTodo_SeRechaza()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 1500m, IdEmpleado);

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 1m, IdEmpleado));
        Assert.HasCount(1, Pagos(repo));
    }

    // Movimientos (auditoría)

    [TestMethod]
    public void Pago_RegistraUnMovimientoDeTipoPagoConLaCajaYElEmpleado()
    {
        var (logica, repo) = CrearConDiezCocas();

        logica.RegistrarPago(
            CrearCliente(IdPedro),
            14000m,
            IdEmpleado,
            MovimientoCuentaCorriente.TipoPagoTransferencia);

        var movimiento = Pagos(repo).Single();
        Assert.AreEqual(IdPedro, movimiento.IdCliente);
        Assert.AreEqual(MovimientoCuentaCorriente.TipoPago, movimiento.Tipo);
        Assert.AreEqual(MovimientoCuentaCorriente.TipoPagoTransferencia, movimiento.TipoDePago);
        Assert.AreEqual(14000m, movimiento.Monto);
        Assert.AreEqual(500m, movimiento.SaldoPosterior);
        Assert.AreEqual(4, movimiento.IdTurnoCaja); // turno de caja abierto simulado
        Assert.AreEqual(IdEmpleado, movimiento.IdEmpleado);
        Assert.IsNotNull(movimiento.IdVenta);
    }

    [TestMethod]
    public void Pago_CreaVentaDePagoYAsociaElMovimientoALosDetallesSaldados()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m, 1500m);

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 3000m, IdEmpleado);

        var movimiento = Pagos(repo).Single();
        var ventaPago = repo.Ventas.Single(v => v.IdVenta == resultado.IdVentaPago);
        Assert.AreEqual(movimiento.IdVenta, ventaPago.IdVenta);
        Assert.AreEqual(1, ventaPago.IdVentaPadre);
        CollectionAssert.AreEqual(
            resultado.ItemsPagados.Select(i => i.IdDetalle).ToArray(),
            repo.MovimientoPorDetalle
                .Where(kv => kv.Value == movimiento.IdMovimiento)
                .Select(kv => kv.Key)
                .OrderBy(id => id)
                .ToArray());
    }

    [TestMethod]
    public void VariosPagos_GeneranUnMovimientoCadaUnoConSuSaldoPosterior()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m);

        logica.RegistrarPago(CrearCliente(IdPedro), 1000m, IdEmpleado);
        logica.RegistrarPago(CrearCliente(IdPedro), 500m, IdEmpleado);

        CollectionAssert.AreEqual(new[] { 1000m, 500m }, Pagos(repo).Select(m => m.Monto).ToArray());
        CollectionAssert.AreEqual(new[] { 1000m, 0m }, Pagos(repo).Select(m => m.SaldoPosterior).ToArray());
    }

    [TestMethod]
    public void VentaACuenta_RegistraUnMovimientoDeTipoCargoQueNoMueveElSaldo()
    {
        var (_, repo) = Crear();

        repo.VenderACuenta(IdPedro, 1500m, 1500m);

        var movimiento = repo.Movimientos.Single();
        Assert.AreEqual(MovimientoCuentaCorriente.TipoCargo, movimiento.Tipo);
        Assert.AreEqual(3000m, movimiento.Monto);
        Assert.AreEqual(0m, movimiento.SaldoPosterior);
    }

    // Cambios de precio y nuevas ventas con crédito previo

    [TestMethod]
    public void AumentoDePrecio_LaUnidadImpagaSeCobraAlPrecioNuevoDescontandoLoEntregado()
    {
        var (logica, repo) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);

        // La coca que sigue impaga sube a $2.000: con $500 acreditados faltan $1.500.
        repo.CambiarMontoPendiente(repo.ObtenerPendientes(IdPedro).Single().IdDetalle, 2000m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.AreEqual(2000m, resumen.DeudaBruta);
        Assert.AreEqual(1500m, resumen.DeudaNeta);

        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 1500m, IdEmpleado);
        Assert.HasCount(1, resultado.ItemsPagados);
        Assert.AreEqual(0m, resultado.CreditoResultante);
    }

    // US-18: propagación del aumento de precio a la deuda pendiente

    [TestMethod]
    public void AumentoDePrecio_SeAplicaATodasLasUnidadesPendientes()
    {
        var (logica, repo) = CrearConDiezCocas();

        CambiarPrecioPendientes(repo, IdPedro, 2000m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.HasCount(10, resumen.Pendientes);
        Assert.IsTrue(resumen.Pendientes.All(p => p.Monto == 2000m));
        Assert.AreEqual(20000m, resumen.DeudaBruta);
    }

    [TestMethod]
    public void AumentoDePrecio_NoModificaLoQueYaFueCobrado()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 1500m, IdEmpleado); // salda 1 unidad

        CambiarPrecioPendientes(repo, IdPedro, 2000m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.HasCount(2, resumen.Pendientes);
        Assert.AreEqual(4000m, resumen.DeudaBruta);
        Assert.AreEqual(1500m, Pagos(repo).Single().Monto); // el pago histórico no cambia
    }

    [TestMethod]
    public void AumentoDePrecio_RespetaElCreditoDeCadaClienteIndependientemente()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m);
        repo.VenderACuenta(IdLucia, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 500m, IdEmpleado); // Pedro: crédito $500

        CambiarPrecioPendientes(repo, IdPedro, 2000m);
        CambiarPrecioPendientes(repo, IdLucia, 2000m);

        Assert.AreEqual(1500m, logica.ObtenerResumen(IdPedro).DeudaNeta);
        Assert.AreEqual(2000m, logica.ObtenerResumen(IdLucia).DeudaNeta);
    }

    [TestMethod]
    public void AumentoDePrecio_PagoPosterior_ActualizaElSaldoPendienteSobreElPrecioNuevo()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m);
        CambiarPrecioPendientes(repo, IdPedro, 2000m);

        // $2.500 saldan 1 unidad de $2.000 y quedan $500 de crédito sobre la siguiente
        var resultado = logica.RegistrarPago(CrearCliente(IdPedro), 2500m, IdEmpleado);

        Assert.HasCount(1, resultado.ItemsPagados);
        Assert.HasCount(1, resultado.ItemsPendientes);
        Assert.AreEqual(500m, resultado.CreditoResultante);
        Assert.AreEqual(1500m, logica.ObtenerResumen(IdPedro).DeudaNeta);
        // y un pago mayor a la deuda neta nueva se rechaza
        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 1500.01m, IdEmpleado));
    }

    [TestMethod]
    public void BajaDePrecio_SiElCreditoAlcanza_LaUnidadSeSaldaSolaYQuedaSaldoAFavor()
    {
        var (logica, repo) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);

        // Con $500 de crédito y la coca bajando a $400, queda saldada y sobran $100.
        repo.CambiarMontoPendiente(repo.ObtenerPendientes(IdPedro).Single().IdDetalle, 400m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.IsEmpty(resumen.Pendientes);
        Assert.AreEqual(100m, resumen.Credito);
        Assert.AreEqual(100m, resumen.SaldoAFavor);
        Assert.AreEqual(0m, resumen.DeudaNeta);
    }

    [TestMethod]
    public void BajaDePrecio_ConSaldoAFavorYSinDeuda_NoSePuedeCobrarMas()
    {
        var (logica, repo) = CrearConDiezCocas();
        logica.RegistrarPago(CrearCliente(IdPedro), 14000m, IdEmpleado);
        repo.CambiarMontoPendiente(repo.ObtenerPendientes(IdPedro).Single().IdDetalle, 400m);

        Assert.ThrowsExactly<InvalidOperationException>(() => logica.RegistrarPago(CrearCliente(IdPedro), 50m, IdEmpleado));
    }

    [TestMethod]
    public void NuevaVentaACuenta_ConSaldoAFavorPrevio_ConsumeElCredito()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 1500m, IdEmpleado);
        repo.AcreditarSaldoAFavor(IdPedro, 300m);

        repo.VenderACuenta(IdPedro, 200m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.IsEmpty(resumen.Pendientes);
        Assert.AreEqual(100m, resumen.Credito);
        Assert.AreEqual(100m, resumen.SaldoAFavor);
    }

    [TestMethod]
    public void NuevaVentaACuenta_ConPagoParcialPrevio_SumaALaDeudaYRespetaElOrden()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m);
        logica.RegistrarPago(CrearCliente(IdPedro), 1000m, IdEmpleado); // crédito $1.000, nada saldado

        repo.VenderACuenta(IdPedro, 1500m);

        var resumen = logica.ObtenerResumen(IdPedro);
        Assert.HasCount(3, resumen.Pendientes);
        Assert.AreEqual(4500m, resumen.DeudaBruta);
        Assert.AreEqual(3500m, resumen.DeudaNeta);
    }

    // Clientes independientes

    [TestMethod]
    public void PagoDeUnCliente_NoAfectaLaCuentaDeOtro()
    {
        var (logica, repo) = Crear();
        repo.VenderACuenta(IdPedro, 1500m, 1500m);
        repo.VenderACuenta(IdLucia, 2000m);

        logica.RegistrarPago(CrearCliente(IdPedro), 3000m, IdEmpleado);

        Assert.IsEmpty(logica.ObtenerPendientes(IdPedro));
        Assert.HasCount(1, logica.ObtenerPendientes(IdLucia));
        Assert.AreEqual(2000m, logica.ObtenerResumen(IdLucia).DeudaNeta);
        Assert.AreEqual(0m, logica.ObtenerResumen(IdLucia).Credito);
    }

    // Replica en memoria las reglas de CuentaCorrienteRepository y ConciliacionCuentaCorriente
    private sealed class CuentaCorrienteEnMemoria : ICuentaCorrienteRepository
    {
        private readonly List<(int IdCliente, ItemDeudaCliente Item)> _pendientes = new();
        private readonly Dictionary<int, decimal> _credito = new();
        private int _proximoDetalle = 1;
        private int _proximaVenta = 1;
        private int _proximoMovimiento = 1;

        public List<MovimientoCuentaCorriente> Movimientos { get; } = new();
        public List<Venta> Ventas { get; } = new();
        public Dictionary<int, int?> MovimientoPorDetalle { get; } = new();

        // Equivale a VentaRepository.Registrar con una venta a Cuenta Corriente
        public void VenderACuenta(int idCliente, params decimal[] montosPorUnidad)
        {
            int idVenta = _proximaVenta++;
            Ventas.Add(new Venta
            {
                IdVenta = idVenta,
                IdCliente = idCliente,
                FormaPago = "Cuenta Corriente",
                Total = montosPorUnidad.Sum(),
                Pagado = false
            });

            foreach (decimal monto in montosPorUnidad)
            {
                var item = new ItemDeudaCliente
                {
                    IdDetalle = _proximoDetalle++,
                    IdVenta = idVenta,
                    FechaVenta = new DateTime(2026, 1, 1).AddMinutes(_proximoDetalle),
                    NombreProducto = "Coca Cola",
                    Monto = monto
                };
                MovimientoPorDetalle[item.IdDetalle] = null;
                _pendientes.Add((idCliente, item));
            }

            Conciliar(idCliente);
            Movimientos.Add(new MovimientoCuentaCorriente
            {
                IdMovimiento = _proximoMovimiento++,
                IdCliente = idCliente,
                IdVenta = idVenta,
                Tipo = MovimientoCuentaCorriente.TipoCargo,
                Monto = montosPorUnidad.Sum(),
                SaldoPosterior = Credito(idCliente)
            });
        }

        // Equivale a ProductoRepository cuando cambia el precio de una unidad que sigue pendiente
        public void CambiarMontoPendiente(int idDetalle, decimal nuevoMonto)
        {
            int indice = _pendientes.FindIndex(p => p.Item.IdDetalle == idDetalle);
            _pendientes[indice].Item.Monto = nuevoMonto;
            Conciliar(_pendientes[indice].IdCliente);
        }

        public void AcreditarSaldoAFavor(int idCliente, decimal monto) => _credito[idCliente] = Credito(idCliente) + monto;

        public List<ItemDeudaCliente> ObtenerPendientes(int idCliente) => PendientesDe(idCliente);

        public ResumenCuentaCorriente ObtenerResumen(int idCliente)
            => new() { Pendientes = PendientesDe(idCliente), Credito = Credito(idCliente) };

        public ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado, string tipoPago)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto a cobrar debe ser mayor a cero.");
            if (!LogicaCuentaCorriente.EsTipoPagoValido(tipoPago))
                throw new ArgumentException("Seleccione un tipo de pago válido.");

            monto = Math.Round(monto, 2);
            var pendientes = PendientesDe(idCliente);

            decimal deudaNeta = CalculadorCuentaCorriente.CalcularDeudaNeta(pendientes.Sum(p => p.Monto), Credito(idCliente));
            if (deudaNeta == 0m)
                throw new InvalidOperationException("El cliente no tiene deuda pendiente en cuenta corriente.");
            if (monto > deudaNeta)
                throw new InvalidOperationException($"El monto ({monto:C2}) supera la deuda pendiente del cliente ({deudaNeta:C2}).");

            _credito[idCliente] = Credito(idCliente) + monto;
            var saldados = Conciliar(idCliente);

            int idVentaPago = _proximaVenta++;
            Ventas.Add(new Venta
            {
                IdVenta = idVentaPago,
                IdVentaPadre = saldados.Select(s => (int?)s.IdVenta).FirstOrDefault()
                    ?? pendientes.Select(p => (int?)p.IdVenta).FirstOrDefault(),
                IdCliente = idCliente,
                FormaPago = tipoPago,
                Total = monto,
                Pagado = true
            });

            int idMovimiento = _proximoMovimiento++;
            Movimientos.Add(new MovimientoCuentaCorriente
            {
                IdMovimiento = idMovimiento,
                IdCliente = idCliente,
                IdTurnoCaja = idCaja,
                IdVenta = idVentaPago,
                IdEmpleado = idEmpleado,
                Tipo = MovimientoCuentaCorriente.TipoPago,
                TipoDePago = tipoPago,
                Monto = monto,
                SaldoPosterior = Credito(idCliente)
            });

            foreach (var saldado in saldados)
                MovimientoPorDetalle[saldado.IdDetalle] = idMovimiento;

            return new ResultadoPagoCuentaCorriente
            {
                IdMovimiento = idMovimiento,
                IdVentaPago = idVentaPago,
                MontoRecibido = monto,
                ItemsPagados = saldados,
                ItemsPendientes = PendientesDe(idCliente),
                CreditoResultante = Credito(idCliente)
            };
        }

        private decimal Credito(int idCliente) => _credito.GetValueOrDefault(idCliente);

        private List<ItemDeudaCliente> PendientesDe(int idCliente) => _pendientes
            .Where(p => p.IdCliente == idCliente)
            .Select(p => p.Item)
            .OrderBy(i => i.FechaVenta).ThenBy(i => i.IdDetalle)
            .ToList();

        // Equivale a ConciliacionCuentaCorriente.Aplicar: devuelve las unidades que quedaron saldadas
        private List<ItemDeudaCliente> Conciliar(int idCliente)
        {
            var pendientes = PendientesDe(idCliente);
            var aplicacion = CalculadorCuentaCorriente.AplicarCredito(pendientes.Select(p => p.Monto).ToList(), Credito(idCliente));
            var saldados = pendientes.Take(aplicacion.CantidadSaldada).ToList();

            _pendientes.RemoveAll(p => saldados.Contains(p.Item));
            _credito[idCliente] = aplicacion.CreditoRemanente;
            return saldados;
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

    private sealed class CajaRepositoryAbiertaFake : ICajaRepository
    {
        public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => new() { IdTurnoCaja = 4, IdEmpleado = idEmpleado, Estado = TurnoCaja.EstadoAbierta };
        public TurnoCaja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado) => ObtenerCajaAbierta(idEmpleado)!;
    }
}
