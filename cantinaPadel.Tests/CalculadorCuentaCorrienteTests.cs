using cantinaPadel.BLL;

namespace cantinaPadel.Tests;

[TestClass]
public class CalculadorCuentaCorrienteTests
{
    private static List<decimal> Unidades(int cantidad, decimal precio) => Enumerable.Repeat(precio, cantidad).ToList();

    [TestMethod]
    public void AplicarCredito_PagoParcial_SoloSaldaLasUnidadesQueCubreCompletas()
    {
        // Pedro debe 10 cocas de $1.500 y entrega $14.000.
        var resultado = CalculadorCuentaCorriente.AplicarCredito(Unidades(10, 1500m), 14000m);

        Assert.AreEqual(9, resultado.CantidadSaldada);
        Assert.AreEqual(500m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_PagoExacto_SaldaTodoYNoDejaRemanente()
    {
        var resultado = CalculadorCuentaCorriente.AplicarCredito(Unidades(10, 1500m), 15000m);

        Assert.AreEqual(10, resultado.CantidadSaldada);
        Assert.AreEqual(0m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_NoAlcanzaParaNinguna_NoSaldaNada()
    {
        var resultado = CalculadorCuentaCorriente.AplicarCredito(Unidades(3, 1500m), 1499.99m);

        Assert.AreEqual(0, resultado.CantidadSaldada);
        Assert.AreEqual(1499.99m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_UnidadCaraBloqueaALasSiguientes_RespetaOrdenFifo()
    {
        // La unidad más vieja ($2.000) no se cubre: no se saltea para pagar la más barata.
        var resultado = CalculadorCuentaCorriente.AplicarCredito(new List<decimal> { 2000m, 1500m }, 1600m);

        Assert.AreEqual(0, resultado.CantidadSaldada);
        Assert.AreEqual(1600m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_AumentoDePrecio_LaUnidadImpagaSeCobraAlPrecioNuevoDescontandoLoEntregado()
    {
        // Tras pagar $14.000 quedan $500 de crédito y una coca impaga.
        // La coca sube a $2.000: sigue impaga y faltan $1.500.
        var resultado = CalculadorCuentaCorriente.AplicarCredito(new List<decimal> { 2000m }, 500m);

        Assert.AreEqual(0, resultado.CantidadSaldada);
        Assert.AreEqual(1500m, CalculadorCuentaCorriente.CalcularDeudaNeta(2000m, resultado.CreditoRemanente));
    }

    [TestMethod]
    public void AplicarCredito_BajaDePrecio_ElCreditoPuedeSaldarLaUnidad()
    {
        var resultado = CalculadorCuentaCorriente.AplicarCredito(new List<decimal> { 400m }, 500m);

        Assert.AreEqual(1, resultado.CantidadSaldada);
        Assert.AreEqual(100m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_SinPendientes_TodoElCreditoQuedaRemanente()
    {
        var resultado = CalculadorCuentaCorriente.AplicarCredito(new List<decimal>(), 300m);

        Assert.AreEqual(0, resultado.CantidadSaldada);
        Assert.AreEqual(300m, resultado.CreditoRemanente);
    }

    [TestMethod]
    public void AplicarCredito_CreditoNegativo_LanzaExcepcion()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCuentaCorriente.AplicarCredito(Unidades(1, 100m), -1m));
    }

    [TestMethod]
    public void CalcularDeudaNeta_DescuentaElCredito()
    {
        Assert.AreEqual(1000m, CalculadorCuentaCorriente.CalcularDeudaNeta(1500m, 500m));
        Assert.AreEqual(0m, CalculadorCuentaCorriente.CalcularDeudaNeta(500m, 800m));
    }

    [TestMethod]
    public void CalcularSaldoAFavor_SoloExisteSiElCreditoSuperaLaDeuda()
    {
        // Debe $1.500 y entregó $500: NO tiene saldo a favor.
        Assert.AreEqual(0m, CalculadorCuentaCorriente.CalcularSaldoAFavor(1500m, 500m));
        Assert.AreEqual(300m, CalculadorCuentaCorriente.CalcularSaldoAFavor(0m, 300m));
    }

    [TestMethod]
    public void ResumenCuentaCorriente_ClienteConPagoParcial_NoMuestraSaldoAFavor()
    {
        var resumen = new ResumenCuentaCorriente
        {
            Pendientes = new List<ItemDeudaCliente> { new() { IdDetalle = 1, Monto = 1500m } },
            Credito = 500m
        };

        Assert.AreEqual(1500m, resumen.DeudaBruta);
        Assert.AreEqual(1000m, resumen.DeudaNeta);
        Assert.AreEqual(500m, resumen.PagosParcialesAcreditados);
        Assert.AreEqual(0m, resumen.SaldoAFavor);
    }
}
