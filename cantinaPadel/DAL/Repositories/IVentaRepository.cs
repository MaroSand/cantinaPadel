using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories;

public interface IVentaRepository
{
    Venta Registrar(Venta venta, IReadOnlyCollection<DetalleVenta> detalles);
    void ActualizarTipoComprobante(int idVenta, string tipoComprobante);
}
