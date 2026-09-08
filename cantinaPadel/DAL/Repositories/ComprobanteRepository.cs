using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public class ComprobanteRepository : IComprobanteRepository
    {
        // Nota: Max()+1 sobre la tabla asume que las ventas se confirman de
        // a una (un solo punto de venta / caja activa). Si en algún momento
        // se necesita más de una caja emitiendo en simultáneo, esto hay que
        // pasarlo a un contador con lock/transacción para evitar números
        // repetidos.
        public long ObtenerUltimoNumero(TipoComprobante tipo, int puntoVenta)
        {
            using var ctx = new AppDbContext();
            return ctx.Comprobantes
                .Where(c => c.Tipo == tipo && c.PuntoVenta == puntoVenta)
                .Select(c => (long?)c.Numero)
                .Max() ?? 0;
        }

        public void Agregar(Comprobante comprobante)
        {
            using var ctx = new AppDbContext();
            ctx.Comprobantes.Add(comprobante);
            ctx.SaveChanges();
        }

        public Comprobante? ObtenerPorId(int idComprobante)
        {
            using var ctx = new AppDbContext();
            return ctx.Comprobantes.FirstOrDefault(c => c.IdComprobante == idComprobante);
        }
    }
}