using cantinaPadel.BLL;
using cantinaPadel.Models;

namespace cantinaPadel.Tests;

// BLL: LogicaCarritoCompra - líneas de la compra en armado (US-25)
[TestClass]
public class LogicaCarritoCompraTests
{
    private static Producto CrearProducto(int id, string nombre = "Coca Cola 500ml", bool activo = true)
        => new() { IdProducto = id, Nombre = nombre, Activo = activo };

    [TestMethod]
    public void AgregarProducto_ProductoNuevo_QuedaComoLineaYCalculaSubtotalYTotal()
    {
        var carrito = new LogicaCarritoCompra();

        bool sumado = carrito.AgregarProducto(CrearProducto(1), 10, 800m);
        carrito.AgregarProducto(CrearProducto(2, "Agua"), 5, 400.50m);

        Assert.IsFalse(sumado);
        Assert.HasCount(2, carrito.Items);
        Assert.AreEqual(8000m, carrito.Items[0].Subtotal);
        Assert.AreEqual(2002.50m, carrito.Items[1].Subtotal);
        Assert.AreEqual(10002.50m, carrito.Total);
    }

    [TestMethod]
    public void AgregarProducto_ProductoRepetido_SumaLaCantidadEnLaMismaLinea()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), 10, 800m);

        bool sumado = carrito.AgregarProducto(CrearProducto(1), 5, 800m);

        Assert.IsTrue(sumado);
        Assert.HasCount(1, carrito.Items);
        Assert.AreEqual(15, carrito.Items[0].Cantidad);
        Assert.AreEqual(12000m, carrito.Total);
    }

    [TestMethod]
    public void AgregarProducto_ProductoRepetidoConOtroCosto_QuedaConElUltimoCostoIngresado()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), 10, 800m);

        carrito.AgregarProducto(CrearProducto(1), 10, 900m);

        Assert.AreEqual(20, carrito.Items[0].Cantidad);
        Assert.AreEqual(900m, carrito.Items[0].PrecioCostoUnitario);
        Assert.AreEqual(18000m, carrito.Total);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-3)]
    public void AgregarProducto_CantidadCeroONegativa_LanzaArgumentException(int cantidad)
    {
        var carrito = new LogicaCarritoCompra();

        Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(CrearProducto(1), cantidad, 800m));
        Assert.HasCount(0, carrito.Items);
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("-10")]
    [DataRow("800.555")]
    public void AgregarProducto_CostoCeroNegativoOConMasDeDosDecimales_LanzaArgumentException(string costo)
    {
        var carrito = new LogicaCarritoCompra();
        decimal valor = decimal.Parse(costo, System.Globalization.CultureInfo.InvariantCulture);

        Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(CrearProducto(1), 1, valor));
        Assert.HasCount(0, carrito.Items);
    }

    [TestMethod]
    public void AgregarProducto_ProductoDeBaja_LanzaArgumentException()
    {
        var carrito = new LogicaCarritoCompra();

        Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(CrearProducto(1, activo: false), 1, 800m));
    }

    [TestMethod]
    public void AgregarProducto_SumaQueSuperaElMaximo_LanzaArgumentExceptionYNoModificaLaLinea()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), DetalleCompra.CantidadMaxima, 1m);

        Assert.ThrowsExactly<ArgumentException>(() => carrito.AgregarProducto(CrearProducto(1), 1, 1m));
        Assert.AreEqual(DetalleCompra.CantidadMaxima, carrito.Items[0].Cantidad);
    }

    [TestMethod]
    public void ActualizarLinea_CambiaCantidadYCosto()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), 10, 800m);

        carrito.ActualizarLinea(1, 4, 1000m);

        Assert.AreEqual(4, carrito.Items[0].Cantidad);
        Assert.AreEqual(4000m, carrito.Total);
    }

    [TestMethod]
    public void ActualizarLinea_ValorInvalido_LanzaArgumentExceptionYConservaLoAnterior()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), 10, 800m);

        Assert.ThrowsExactly<ArgumentException>(() => carrito.ActualizarLinea(1, 0, 800m));
        Assert.ThrowsExactly<ArgumentException>(() => carrito.ActualizarLinea(1, 10, 0m));
        Assert.AreEqual(10, carrito.Items[0].Cantidad);
        Assert.AreEqual(800m, carrito.Items[0].PrecioCostoUnitario);
    }

    [TestMethod]
    public void ActualizarLinea_ProductoQueNoEstaEnLaCompra_LanzaArgumentException()
    {
        var carrito = new LogicaCarritoCompra();

        Assert.ThrowsExactly<ArgumentException>(() => carrito.ActualizarLinea(99, 1, 100m));
    }

    [TestMethod]
    public void QuitarProductoYVaciar_ActualizanLasLineasYElTotal()
    {
        var carrito = new LogicaCarritoCompra();
        carrito.AgregarProducto(CrearProducto(1), 1, 100m);
        carrito.AgregarProducto(CrearProducto(2), 1, 200m);

        carrito.QuitarProducto(1);
        Assert.HasCount(1, carrito.Items);
        Assert.AreEqual(200m, carrito.Total);

        carrito.Vaciar();
        Assert.HasCount(0, carrito.Items);
        Assert.AreEqual(0m, carrito.Total);
    }
}