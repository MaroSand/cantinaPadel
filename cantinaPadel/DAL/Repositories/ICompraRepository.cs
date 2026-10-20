using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

public interface ICompraRepository
{
    // Guarda la compra junto con todas sus líneas de detalle en una sola transacción: si algo falla, no se guarda nada
    Compra Registrar(Compra compra, IReadOnlyDictionary<int, decimal> preciosVentaNuevos);

    // Anula una compra y revierte su impacto en productos dentro de una transacción.
    // Lanza InvalidOperationException si no existe, ya está anulada o el stock no alcanza.
    Compra Anular(int idCompra);

    // Búsqueda "todo en uno": cada parámetro es opcional y, si viene null, ese filtro no se aplica.
    // desde / hasta son fechas inclusivas (se compara solo el día). producto busca por nombre o código de barras en cualquier línea de la compra.
    // Si incluirAnuladas es false, solo se devuelven las compras activas. Se ordena de la más reciente a la más antigua
    List<Compra> Buscar(int? idProveedor, DateTime? desde, DateTime? hasta, string? producto, bool incluirAnuladas = false);

    // Compra completa (proveedor, empleado y detalle con sus productos), o null si no existe
    Compra? ObtenerPorId(int idCompra);
}
