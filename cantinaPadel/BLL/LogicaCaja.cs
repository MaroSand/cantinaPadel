using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public class LogicaCaja
{
    private readonly ICajaRepository _cajas;
    private readonly IEmpleadoRepository _empleados;

    public LogicaCaja() : this(new CajaRepository(), new EmpleadoRepository()) { }
    public LogicaCaja(ICajaRepository cajas, IEmpleadoRepository empleados) { _cajas = cajas; _empleados = empleados; }

    public Caja? ObtenerCajaAbierta(int idEmpleado) => _cajas.ObtenerCajaAbierta(idEmpleado);
    public Caja? ObtenerCajaAbiertaGeneral() => _cajas.ObtenerCajaAbiertaGeneral();
    public List<Caja> ObtenerHistorial(int idEmpleado) => _cajas.ObtenerHistorial(idEmpleado);
    public List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int idUsuario, string? rol)
    {
        if (idUsuario <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        return _cajas.ObtenerHistorialEfectivo(rol == "Admin" ? null : idUsuario);
    }
    public List<Caja> ObtenerTodoHistorial(string? rol)
    {
        if (rol != "Admin") throw new UnauthorizedAccessException("Solo un administrador puede consultar el historial completo.");
        return _cajas.ObtenerTodoHistorial();
    }
    public Caja AbrirCaja(int idEmpleado)
    {
        if (idEmpleado <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        if (_cajas.ObtenerCajaAbiertaGeneral() != null) throw new InvalidOperationException("La caja física ya está abierta por otro usuario. Cerrala antes de iniciar el siguiente turno.");
        return _cajas.Abrir(idEmpleado, ObtenerMontoSiguienteApertura());
    }

    public decimal ObtenerMontoSiguienteApertura()
    {
        var ultimaCerrada = _cajas.ObtenerUltimaCajaCerrada();
        return ultimaCerrada == null ? 0m : ultimaCerrada.EfectivoFinal ?? ObtenerEfectivoDisponible(ultimaCerrada.IdCaja);
    }

    public CajaResumenDatos ObtenerResumen(int idCaja) => CalculadorCaja.CalcularResumen(_cajas.ObtenerMovimientosResumen(idCaja));

    public void CerrarCaja(int idCaja, int idEmpleado)
    {
        var caja = _cajas.Obtener(idCaja);
        if (caja.IdEmpleado != idEmpleado) throw new InvalidOperationException("Solo podés cerrar tu propia caja.");
        var efectivoFinal = ObtenerEfectivoDisponible(idCaja);
        _cajas.Cerrar(idCaja, ObtenerResumen(idCaja), efectivoFinal);
    }

    public decimal ObtenerEfectivoDisponible(int idCaja)
    {
        var d = _cajas.ObtenerDatosEfectivo(idCaja);
        return CalculadorCaja.CalcularEfectivoDisponible(d.AperturaMasIngresos, d.VentasEfectivo, d.PagosCuentaCorrienteEfectivo, d.Retiros);
    }

    public void RetirarEfectivo(int idCaja, int idAdmin, string? rol, decimal monto, string contrasena)
    {
        if (rol != "Admin") throw new UnauthorizedAccessException("Solo un administrador puede retirar efectivo.");
        if (monto <= 0) throw new ArgumentException("El monto del retiro debe ser mayor a cero.");
        var admin = _empleados.ObtenerTodos().FirstOrDefault(e => e.IdEmpleado == idAdmin && e.Activo && e.Rol == "Admin");
        if (admin == null || string.IsNullOrEmpty(contrasena) || admin.Contrasena != contrasena)
            throw new UnauthorizedAccessException("La contraseña del administrador es incorrecta.");
        var caja = _cajas.Obtener(idCaja);
        if (caja.Estado != Caja.EstadoAbierta) throw new InvalidOperationException("La caja está cerrada.");
        if (ObtenerEfectivoDisponible(idCaja) < monto) throw new InvalidOperationException("El retiro supera el efectivo disponible en caja.");
        _cajas.RegistrarRetiro(new RetiroCaja { IdCaja = idCaja, IdEmpleado = idAdmin, Monto = monto, FechaRetiro = DateTime.Now });
    }

    public void AgregarEfectivo(int idCaja, decimal monto, string usuarioAdmin, string contrasenaAdmin)
    {
        if (monto <= 0m) throw new ArgumentException("El monto a agregar debe ser mayor a cero.");
        var admin = _empleados.ObtenerPorUsuario(usuarioAdmin.Trim());
        if (admin == null || !admin.Activo || admin.Rol != "Admin" || string.IsNullOrEmpty(contrasenaAdmin) || admin.Contrasena != contrasenaAdmin)
            throw new UnauthorizedAccessException("El usuario o la contraseña del administrador son incorrectos.");
        var caja = _cajas.Obtener(idCaja);
        if (caja.Estado != Caja.EstadoAbierta || _cajas.ObtenerCajaAbiertaGeneral()?.IdCaja != idCaja)
            throw new InvalidOperationException("Solo se puede agregar efectivo a la caja física abierta.");
        _cajas.RegistrarIngreso(new IngresoCaja { IdCaja = idCaja, IdAdmin = admin.IdEmpleado, Monto = monto, FechaIngreso = DateTime.Now });
    }
}
