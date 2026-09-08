using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface IComprobanteRepository
    {
        // Último número usado para ese tipo de comprobante en ese punto de
        // venta. Devuelve 0 si todavía no se emitió ninguno (así el próximo
        // número, en la lógica, arranca en 1).
        long ObtenerUltimoNumero(TipoComprobante tipo, int puntoVenta);

        void Agregar(Comprobante comprobante);

        Comprobante? ObtenerPorId(int idComprobante);
    }
}