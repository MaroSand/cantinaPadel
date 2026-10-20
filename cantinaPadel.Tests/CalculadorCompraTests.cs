using cantinaPadel.BLL;

namespace cantinaPadel.Tests;

[TestClass]
public class CalculadorCompraTests
{
    [TestMethod]
    public void CalcularPrecioVentaBase_AplicaMargenSobreCosto()
    {
        Assert.AreEqual(1350m, CalculadorCompra.CalcularPrecioVentaBase(1000m, 35m));
    }

    [TestMethod]
    public void CalcularPrecioVentaBase_RedondeaADosDecimales()
    {
        Assert.AreEqual(115.51m, CalculadorCompra.CalcularPrecioVentaBase(100.01m, 15.5m));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void CalcularPrecioVentaBase_CostoNoPositivo_Lanza(int costo)
    {
        Assert.ThrowsExactly<ArgumentException>(() => CalculadorCompra.CalcularPrecioVentaBase(costo, 20m));
    }

    [TestMethod]
    public void CalcularPrecioVentaBase_PorcentajeNegativo_Lanza()
    {
        Assert.ThrowsExactly<ArgumentException>(() => CalculadorCompra.CalcularPrecioVentaBase(100m, -1m));
    }

    [TestMethod]
    public void CalcularStockNuevo_SumaElIngresoAlStockActual()
    {
        Assert.AreEqual(17, CalculadorCompra.CalcularStockNuevo(12, 5));
    }

    [TestMethod]
    public void CalcularStockNuevo_Overflow_LanzaArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() => CalculadorCompra.CalcularStockNuevo(int.MaxValue, 1));
    }

    [TestMethod]
    public void CalcularStockLuegoDeAnular_RestaLaCantidadComprada()
    {
        Assert.AreEqual(7, CalculadorCompra.CalcularStockLuegoDeAnular(12, 5));
    }

    [TestMethod]
    public void CalcularStockLuegoDeAnular_SiStockNoAlcanza_LanzaMensajeClaro()
    {
        var ex = Assert.ThrowsExactly<InvalidOperationException>(
            () => CalculadorCompra.CalcularStockLuegoDeAnular(3, 5));

        StringAssert.Contains(ex.Message, "stock quedaría negativo");
    }
}
