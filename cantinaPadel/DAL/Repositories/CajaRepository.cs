using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        public Caja? ObtenerCajaAbierta(int idEmpleado)
        {
            using var ctx = new AppDbContext();
            return ctx.Cajas
                .Where(c => c.IdEmpleado == idEmpleado && c.Estado == Caja.EstadoAbierta)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefault();
        }

        public Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado)
        {
            using var ctx = new AppDbContext();

            var caja = ctx.Cajas
                .Where(c => c.IdEmpleado == idEmpleado && c.Estado == Caja.EstadoAbierta)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefault();

            if (caja != null)
                return caja;

            caja = new Caja
            {
                IdEmpleado = idEmpleado,
                FechaApertura = DateTime.Now,
                Estado = Caja.EstadoAbierta
            };

            ctx.Cajas.Add(caja);
            ctx.SaveChanges();

            return caja;
        }
    }
}
