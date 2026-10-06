using cantinaPadel.BLL;

namespace cantinaPadel.Tests;

[TestClass]
public class CalculadorCajaTests
{
    [TestMethod]
    public void CalcularEfectivoDisponible_SumaFondosIngresadosVentasYCobrosYDescuentaRetiros()
    {
        Assert.AreEqual(13250m, CalculadorCaja.CalcularEfectivoDisponible(5000m, 7000m, 2250m, 1000m));
    }

    [TestMethod]
    public void CalcularEfectivoDisponible_RechazaMontosNegativos()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCaja.CalcularEfectivoDisponible(100m, -1m, 0m, 0m));
    }

    [TestMethod]
    public void CalcularResumen_AgrupaMercadoPagoConTransferencia()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new cantinaPadel.Models.MovimientoCajaDato("Transferencia", 1200m),
            new cantinaPadel.Models.MovimientoCajaDato("MercadoPago", 800m)
        });

        Assert.AreEqual(2000m, resumen.Transferencia);
    }

    [TestMethod]
    public void CalcularResumen_CobroCuentaCorriente_SumaAlMetodoYSeIdentificaEnSuColumna()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new cantinaPadel.Models.MovimientoCajaDato("Efectivo", 900m, EsPagoCuentaCorriente: true)
        });

        Assert.AreEqual(900m, resumen.Efectivo);
        Assert.AreEqual(900m, resumen.CobrosCuentaCorrienteEfectivo);
    }
}
