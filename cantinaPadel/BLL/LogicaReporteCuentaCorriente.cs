using cantinaPadel.DAL.Repositories;

namespace cantinaPadel.BLL;

public sealed class LogicaReporteCuentaCorriente
{
    private readonly IReporteCuentaCorrienteRepository _repositorio;

    public LogicaReporteCuentaCorriente() : this(new ReporteCuentaCorrienteRepository()) { }

    public LogicaReporteCuentaCorriente(IReporteCuentaCorrienteRepository repositorio)
        => _repositorio = repositorio;

    public List<ClienteCuentaCorrienteReporte> ObtenerClientesConActividad()
        => _repositorio.ObtenerClientesConActividad();

    public List<MovimientoCuentaCorrienteReporte> ObtenerMovimientos(int idCliente)
        => _repositorio.ObtenerMovimientos(idCliente);
}
