using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    // Una línea del carrito tiene un producto elegido, con la cantidad que se está por vender
    // No tiene tabla propia en la base porque vive en memoria mientras se arma la venta en FrmPuntoVenta
    public class ItemCarrito
    {
        public Producto Producto { get; }
        public int Cantidad { get; internal set; }
        public int CantidadEscaneada { get; internal set; }
        public int CantidadManual => Cantidad - CantidadEscaneada;
        public decimal? PrecioUnitarioManual { get; internal set; }

        public ItemCarrito(Producto producto, int cantidad)
            : this(producto, cantidad, cantidadEscaneada: 0)
        {
        }

        internal ItemCarrito(Producto producto, int cantidad, int cantidadEscaneada)
        {
            if (cantidadEscaneada < 0 || cantidadEscaneada > cantidad)
                throw new ArgumentOutOfRangeException(nameof(cantidadEscaneada));

            Producto = producto;
            Cantidad = cantidad;
            CantidadEscaneada = cantidadEscaneada;
        }

        // Precio unitario CON IVA: es lo que efectivamente paga el cliente por unidad
        public decimal PrecioUnitario => PrecioUnitarioManual ?? Producto.PrecioConIva;

        public decimal Subtotal => Math.Round(PrecioUnitario * Cantidad, 2);
    }

    // Carrito de venta de FrmPuntoVenta
    // Agrupa los productos y valida stock para unidades ingresadas manualmente. Las horas de pádel no usan stock;
    // el punto de venta autoriza las unidades leídas por el escáner.
    // Esta clase no depende de ningún repositorio, solo necesita los Producto ya obtenidos (por LogicaProducto.Buscar / ObtenerPorCodigoBarras)
    // para validar stock contra lo cargado
    public class LogicaCarrito
    {
        private readonly List<ItemCarrito> _items = new();

        public IReadOnlyList<ItemCarrito> Items => _items;

        public decimal Total => _items.Sum(i => i.Subtotal);

        public int CantidadItems => _items.Sum(i => i.Cantidad);

        // Agrega el producto con la cantidad indicada. Si ya estaba en el carrito, suma a la cantidad existente (ej: escanear dos veces el
        // mismo código de barras). Las unidades cargadas manualmente deben caber en el stock disponible.
        public void AgregarProducto(Producto producto, int cantidad = 1)
            => AgregarProductoInterno(producto, cantidad, porEscaner: false);

        // Los productos "Varios" se cobran por el importe total de una porción o puñado, sin precio unitario fijo.
        public void AgregarProductoConPrecioManual(Producto producto, decimal importeTotal)
        {
            if (producto == null)
                throw new ArgumentException("El producto es obligatorio.");
            if (!producto.Activo)
                throw new ArgumentException($"El producto '{producto.Nombre}' está dado de baja.");
            if (!producto.EsVarios)
                throw new ArgumentException("El precio manual solo se permite para productos de la categoría Varios.");
            if (importeTotal <= 0m || importeTotal > Producto.PrecioVentaMaximo * 1.21m)
                throw new ArgumentException("El importe debe ser mayor a cero y estar dentro del máximo permitido.");

            var existente = _items.FirstOrDefault(i => i.Producto.IdProducto == producto.IdProducto);
            if (existente != null)
            {
                existente.PrecioUnitarioManual = Math.Round((existente.PrecioUnitarioManual ?? existente.PrecioUnitario) + importeTotal, 2);
                return;
            }

            _items.Add(new ItemCarrito(producto, 1) { PrecioUnitarioManual = Math.Round(importeTotal, 2) });
        }

        internal void AgregarProductoPorEscaner(Producto producto, int cantidad = 1)
            => AgregarProductoInterno(producto, cantidad, porEscaner: true);

        private void AgregarProductoInterno(Producto producto, int cantidad, bool porEscaner)
        {
            if (producto == null)
                throw new ArgumentException("El producto es obligatorio.");

            if (!producto.Activo)
                throw new ArgumentException($"El producto '{producto.Nombre}' está dado de baja.");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.");

            var existente = _items.FirstOrDefault(i => i.Producto.IdProducto == producto.IdProducto);
            int cantidadFinal = (existente?.Cantidad ?? 0) + cantidad;
            int cantidadEscaneadaFinal = (existente?.CantidadEscaneada ?? 0) + (porEscaner ? cantidad : 0);
            int cantidadManualFinal = (existente?.CantidadManual ?? 0) + (porEscaner ? 0 : cantidad);

            ValidarStock(producto, cantidadManualFinal);

            if (existente != null)
            {
                existente.Cantidad = cantidadFinal;
                existente.CantidadEscaneada = cantidadEscaneadaFinal;
            }
            else
                _items.Add(new ItemCarrito(producto, cantidad, porEscaner ? cantidad : 0));
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

            int cantidadEscaneadaNueva = Math.Min(item.CantidadEscaneada, cantidadNueva);
            ValidarStock(item.Producto, cantidadNueva - cantidadEscaneadaNueva);
            item.Cantidad = cantidadNueva;
            item.CantidadEscaneada = cantidadEscaneadaNueva;
        }

        public void QuitarProducto(int idProducto)
        {
            var item = _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto);
            if (item != null)
                _items.Remove(item);
        }

        // Cuánto de este producto ya está en el carrito (0 si no está). Lo usa la UI  para mostrar el stock disponible (stock real - lo ya cargado
        // en el carrito) en la grilla de resultados de búsqueda
        public int ObtenerCantidadEnCarrito(int idProducto)
            => _items.FirstOrDefault(i => i.Producto.IdProducto == idProducto)?.Cantidad ?? 0;

        public void Vaciar() => _items.Clear();

        private static void ValidarStock(Producto producto, int cantidadManualPedida)
        {
            if (producto.EsHoraPadel)
                return;

            int stockDisponible = Math.Max(0, producto.StockActual);
            if (cantidadManualPedida > stockDisponible)
                throw new ArgumentException(
                    $"No hay stock suficiente de '{producto.Nombre}'. Disponible: {stockDisponible}, pedido: {cantidadManualPedida}.");
        }
    }
}
