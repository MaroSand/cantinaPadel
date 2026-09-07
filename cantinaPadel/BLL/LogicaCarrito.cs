using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    // Una línea del carrito tiene un producto elegido, con la cantidad que se está por vender
    // No tiene tabla propia en la base porque vive en memoria mientras se arma la venta en FrmPuntoVenta
    public class ItemCarrito
    {
        public Producto Producto { get; }
        public int Cantidad { get; internal set; }

        public ItemCarrito(Producto producto, int cantidad)
        {
            Producto = producto;
            Cantidad = cantidad;
        }

        // Precio unitario CON IVA: es lo que efectivamente paga el cliente por unidad
        public decimal PrecioUnitario => Producto.PrecioConIva;

        public decimal Subtotal => Math.Round(PrecioUnitario * Cantidad, 2);
    }

    // Carrito de venta de FrmPuntoVenta
    // Agrupa los productos elegidos con su cantidad, valida que no se pida más del stock disponible y calcula el total
    // Esta clase no depende de ningún repositorio, solo necesita los Producto ya obtenidos (por LogicaProducto.Buscar / ObtenerPorCodigoBarras)
    // para validar stock contra lo cargado
    public class LogicaCarrito
    {
        private readonly List<ItemCarrito> _items = new();

        public IReadOnlyList<ItemCarrito> Items => _items;

        public decimal Total => _items.Sum(i => i.Subtotal);

        public int CantidadItems => _items.Sum(i => i.Cantidad);

        // Agrega el producto con la cantidad indicada. Si ya estaba en el carrito, suma a la cantidad existente (ej: escanear dos veces el
        // mismo código de barras). Valida que la cantidad total resultante no supere el stock disponible del producto
        public void AgregarProducto(Producto producto, int cantidad = 1)
        {
            if (producto == null)
                throw new ArgumentException("El producto es obligatorio.");

            if (!producto.Activo)
                throw new ArgumentException($"El producto '{producto.Nombre}' está dado de baja.");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            var existente = _items.FirstOrDefault(i => i.Producto.IdProducto == producto.IdProducto);
            int cantidadFinal = (existente?.Cantidad ?? 0) + cantidad;

            ValidarStock(producto, cantidadFinal);

            if (existente != null)
                existente.Cantidad = cantidadFinal;
            else
                _items.Add(new ItemCarrito(producto, cantidad));
        }

        // Cambia la cantidad de un producto ya agregado (ej: el usuario edita la celda "Cantidad" en la grilla). Si Cantidad 0 o negativa, lo quita
        public void ActualizarCantidad(int idProducto, int cantidadNueva)
        {
            var item = _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto)
                ?? throw new ArgumentException("El producto no está en el carrito.");

            if (cantidadNueva <= 0)
            {
                _items.Remove(item);
                return;
            }

            ValidarStock(item.Producto, cantidadNueva);
            item.Cantidad = cantidadNueva;
        }

        public void QuitarProducto(int idProducto)
        {
            var item = _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);
            if (item != null)
                _items.Remove(item);
        }

        public void Vaciar() => _items.Clear();

        private static void ValidarStock(Producto producto, int cantidadPedida)
        {
            if (cantidadPedida > producto.StockActual)
                throw new ArgumentException(
                    $"No hay stock suficiente de '{producto.Nombre}'. Disponible: {producto.StockActual}, pedido: {cantidadPedida}.");
        }
    }
}