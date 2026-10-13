using cantinaPadel.BLL;
using cantinaPadel.Models;

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
    public void CalcularEfectivoDisponible_RechazaRetiroNegativo()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCaja.CalcularEfectivoDisponible(100m, 0m, 0m, -1m));
    }

    [TestMethod]
    public void CalcularEfectivoDisponible_SinMovimientos_EsLaApertura()
    {
        Assert.AreEqual(5000m, CalculadorCaja.CalcularEfectivoDisponible(5000m, 0m, 0m, 0m));
    }

    [TestMethod]
    public void CalcularEfectivoDisponible_AperturaEnCero_SoloCuentanLasVentas()
    {
        Assert.AreEqual(2000m, CalculadorCaja.CalcularEfectivoDisponible(0m, 2000m, 0m, 0m));
    }

    [TestMethod]
    public void CalcularEfectivoDisponible_AperturaIngresoVentaYRetiro()
    {
        // Apertura 5000 + ingreso 1000 = 6000, venta en efectivo 2000, retiro 1500
        Assert.AreEqual(6500m, CalculadorCaja.CalcularEfectivoDisponible(6000m, 2000m, 0m, 1500m));
    }

    [TestMethod]
    public void CalcularEfectivoDisponible_SumaCobroDeCuentaCorrienteEnEfectivo()
    {
        Assert.AreEqual(8000m, CalculadorCaja.CalcularEfectivoDisponible(5000m, 2000m, 1000m, 0m));
    }

    [TestMethod]
    public void CalcularDiferenciaEfectivo_RestaEsperadoAlContado()
    {
        Assert.AreEqual(-250m, CalculadorCaja.CalcularDiferenciaEfectivo(10000m, 9750m));
        Assert.AreEqual(250m, CalculadorCaja.CalcularDiferenciaEfectivo(10000m, 10250m));
    }

    [TestMethod]
    public void CalcularDiferenciaEfectivo_ContadoIgualAlEsperado_EsCero()
    {
        Assert.AreEqual(0m, CalculadorCaja.CalcularDiferenciaEfectivo(7000m, 7000m));
    }

    [TestMethod]
    public void CalcularDiferenciaEfectivo_RechazaContadoNegativo()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCaja.CalcularDiferenciaEfectivo(7000m, -1m));
    }

    [TestMethod]
    public void CalcularDiferenciaEfectivo_RechazaEsperadoNegativo()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCaja.CalcularDiferenciaEfectivo(-1m, 100m));
    }

    [TestMethod]
    public void CalcularResumen_AgrupaMercadoPagoConTransferencia()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Transferencia", 1200m),
            new MovimientoCajaDato("MercadoPago", 800m)
        });

        Assert.AreEqual(2000m, resumen.Transferencia);
    }

    [TestMethod]
    public void CalcularResumen_CobroCuentaCorriente_SumaAlMetodoYSeIdentificaEnSuColumna()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", 900m, EsPagoCuentaCorriente: true)
        });

        Assert.AreEqual(900m, resumen.Efectivo);
        Assert.AreEqual(900m, resumen.CobrosCuentaCorrienteEfectivo);
    }

    [TestMethod]
    public void CalcularResumen_SinMovimientos_DevuelveTodoEnCero()
    {
        var resumen = CalculadorCaja.CalcularResumen(Array.Empty<MovimientoCajaDato>());

        Assert.AreEqual(0m, resumen.Efectivo);
        Assert.AreEqual(0m, resumen.Tarjeta);
        Assert.AreEqual(0m, resumen.Transferencia);
        Assert.AreEqual(0m, resumen.CobrosCuentaCorrienteEfectivo);
        Assert.AreEqual(0m, resumen.CobrosCuentaCorrienteTarjeta);
        Assert.AreEqual(0m, resumen.CobrosCuentaCorrienteTransferencia);
    }

    [TestMethod]
    public void CalcularResumen_SeparaEfectivoTarjetaYTransferencia()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", 2000m),
            new MovimientoCajaDato("Tarjeta", 3000m),
            new MovimientoCajaDato("Transferencia", 1500m),
            new MovimientoCajaDato("MercadoPago", 500m)
        });

        Assert.AreEqual(2000m, resumen.Efectivo);
        Assert.AreEqual(3000m, resumen.Tarjeta);
        Assert.AreEqual(2000m, resumen.Transferencia);
    }

    [TestMethod]
    public void CalcularResumen_SumaVariosMovimientosDelMismoMetodo()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", 1000m),
            new MovimientoCajaDato("Efectivo", 250m),
            new MovimientoCajaDato("Efectivo", 750m)
        });

        Assert.AreEqual(2000m, resumen.Efectivo);
    }

    [TestMethod]
    public void CalcularResumen_CobrosDeCuentaCorrienteTarjetaYTransferencia_SeIdentificanPorMetodo()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Tarjeta", 1000m, EsPagoCuentaCorriente: true),
            new MovimientoCajaDato("Tarjeta", 500m),
            new MovimientoCajaDato("MercadoPago", 700m, EsPagoCuentaCorriente: true),
            new MovimientoCajaDato("Transferencia", 300m)
        });

        Assert.AreEqual(1500m, resumen.Tarjeta);
        Assert.AreEqual(1000m, resumen.CobrosCuentaCorrienteTarjeta);
        Assert.AreEqual(1000m, resumen.Transferencia);
        Assert.AreEqual(700m, resumen.CobrosCuentaCorrienteTransferencia);
    }

    [TestMethod]
    public void CalcularResumen_VentaYCobroDeCuentaCorrienteEnEfectivo_NoMezclaLasColumnas()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", 2000m),
            new MovimientoCajaDato("Efectivo", 1000m, EsPagoCuentaCorriente: true)
        });

        Assert.AreEqual(3000m, resumen.Efectivo);
        Assert.AreEqual(1000m, resumen.CobrosCuentaCorrienteEfectivo);
    }

    [TestMethod]
    public void CalcularResumen_MedioDePagoNoReconocido_NoSumaEnNingunTotal()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Cuenta Corriente", 5000m)
        });

        Assert.AreEqual(0m, resumen.Efectivo);
        Assert.AreEqual(0m, resumen.Tarjeta);
        Assert.AreEqual(0m, resumen.Transferencia);
    }

    [TestMethod]
    public void CalcularResumen_RedondeaADosDecimales()
    {
        var resumen = CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", 10.004m),
            new MovimientoCajaDato("Efectivo", 10.004m)
        });

        Assert.AreEqual(20.01m, resumen.Efectivo);
    }

    [TestMethod]
    public void CalcularResumen_RechazaMontosNegativos()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CalculadorCaja.CalcularResumen(new[]
        {
            new MovimientoCajaDato("Efectivo", -1m)
        }));
    }

    [TestMethod]
    public void CalcularResumen_ListaNula_Lanza()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => CalculadorCaja.CalcularResumen(null!));
    }
}