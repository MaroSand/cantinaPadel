using cantinaPadel.BLL;

namespace cantinaPadel.DAL.Repositories;

public interface IReporteCuentaCorrienteRepository
{
    List<ClienteCuentaCorrienteReporte> ObtenerClientesConActividad();
    List<MovimientoCuentaCorrienteReporte> ObtenerMovimientos(int idCliente);
}
