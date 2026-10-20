using cantinaPadel.Models;

namespace cantinaPadel.BLL;

// Una línea de la compra que se está armando: un producto, la cantidad que se compra y su costo unitario
// No tiene tabla propia: vive en memoria mientras se arma la compra en FrmAltaCompra y recién al confirmar pasa a ser un DetalleCompra
public class ItemCompra
{
    public Producto Producto { get; }
    public int Cantidad { get; internal set; }
    public decimal PrecioCostoUnitario { get; internal set; }

    public ItemCompra(Producto producto, int cantidad, decimal precioCostoUnitario)
    {
        Producto = producto;
        Cantidad = cantidad;
        PrecioCostoUnitario = precioCostoUnitario;
    }

    public decimal Subtotal => DetalleCompra.CalcularSubtotal(Cantidad, PrecioCostoUnitario);
}

// Carrito de la pantalla de alta de compras (equivalente a LogicaCarrito del Punto de Venta, pero acá se ingresa mercadería)
// No depende de ningún repositorio: trabaja con los Producto ya obtenidos por el buscador
public class LogicaCarritoCompra
{
    private readonly List<ItemCompra> _items = new();

    public IReadOnlyList<ItemCompra> Items => _items;

    public decimal Total => decimal.Round(_items.Sum(i => i.Subtotal), 2);

    public bool ContieneProducto(int idProducto) => _items.Any(i => i.Producto.IdProducto == idProducto);

    // Agrega el producto con su cantidad y costo unitario. Un producto no puede quedar repetido en la compra: si ya estaba cargado, se SUMA la cantidad
    // a su línea y el costo unitario pasa a ser el que se acaba de ingresar (es el dato más reciente que tipeó el usuario; si fue un error, lo corrige en la grilla)
    // Devuelve true si se sumó a una línea que ya existía, para que la pantalla pueda avisarlo
    public bool AgregarProducto(Producto producto, int cantidad, decimal precioCostoUnitario)
    {
        if (producto == null)
            throw new ArgumentException("El producto es obligatorio.");

        if (!producto.Activo)
            throw new ArgumentException($"El producto '{producto.Nombre}' está dado de baja.");

        DetalleCompra.ValidarCantidad(cantidad);
        DetalleCompra.ValidarCosto(precioCostoUnitario);

        var existente = _items.FirstOrDefault(i => i.Producto.IdProducto == producto.IdProducto);
        if (existente == null)
        {
            _items.Add(new ItemCompra(producto, cantidad, precioCostoUnitario));
            return false;
        }

        int cantidadFinal = existente.Cantidad + cantidad;
        DetalleCompra.ValidarCantidad(cantidadFinal);

        existente.Cantidad = cantidadFinal;
        existente.PrecioCostoUnitario = precioCostoUnitario;
        return true;
    }

    // El usuario edita la cantidad y/o el costo de una línea ya cargada directamente en la grilla
    public void ActualizarLinea(int idProducto, int cantidad, decimal precioCostoUnitario)
    {
        var item = _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto)
            ?? throw new ArgumentException("El producto no está en la compra.");

        DetalleCompra.ValidarCantidad(cantidad);
        DetalleCompra.ValidarCosto(precioCostoUnitario);

        item.Cantidad = cantidad;
        item.PrecioCostoUnitario = precioCostoUnitario;
    }

    public void QuitarProducto(int idProducto)
    {
        var item = _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);
        if (item != null)
            _items.Remove(item);
    }

    public void Vaciar() => _items.Clear();
}