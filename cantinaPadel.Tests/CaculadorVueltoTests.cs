using cantinaPadel.BLL;

namespace cantinaPadel.Tests;

[TestClass]
public class CalculadorVueltoTests
{
    [TestMethod]
    public void Calcular_PagaConMasDelTotal_DevuelveLaDiferencia()
        => Assert.AreEqual(2000m, CalculadorVuelto.Calcular(3000m, 5000m));

    [TestMethod]
    public void Calcular_PagoExacto_NoHayVuelto()
        => Assert.AreEqual(0m, CalculadorVuelto.Calcular(3000m, 3000m));

    [TestMethod]
    public void Calcular_SinInformarPagaCon_NoHayVuelto()
        => Assert.AreEqual(0m, CalculadorVuelto.Calcular(3000m, 0m));

    [TestMethod]
    public void Validar_PagaConMenosDelTotal_DevuelveMensaje()
        => Assert.IsNotNull(CalculadorVuelto.Validar(3000m, 2000m));

    [TestMethod]
    public void Validar_PagaConDeMas_OSinInformar_NoDevuelveError()
    {
        Assert.IsNull(CalculadorVuelto.Validar(3000m, 5000m));
        Assert.IsNull(CalculadorVuelto.Validar(3000m, 0m));
    }

    [TestMethod]
    public void TryParsearMonto_Vacio_EsValidoYVale0()
    {
        Assert.IsTrue(CalculadorVuelto.TryParsearMonto("  ", out var monto));
        Assert.AreEqual(0m, monto);
    }

    [TestMethod]
    public void TryParsearMonto_TextoInvalidoONegativo_Falla()
    {
        Assert.IsFalse(CalculadorVuelto.TryParsearMonto("abc", out _));
        Assert.IsFalse(CalculadorVuelto.TryParsearMonto("-5", out _));
    }

    [TestMethod]
    public void TryParsearMonto_NumeroSimple_Parsea()
    {
        Assert.IsTrue(CalculadorVuelto.TryParsearMonto("5000", out var monto));
        Assert.AreEqual(5000m, monto);
    }
}