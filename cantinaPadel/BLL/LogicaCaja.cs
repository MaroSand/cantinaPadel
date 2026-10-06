using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public class LogicaCaja
{
    private readonly ICajaRepository _cajas;
    private readonly IEmpleadoRepository _empleados;

    public LogicaCaja() : this(new CajaRepository(), new EmpleadoRepository()) { }
    public LogicaCaja(ICajaRepository cajas, IEmpleadoRepository empleados) { _cajas = cajas; _empleados = empleados; }

    public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => _cajas.ObtenerCajaAbierta(idEmpleado);
    public TurnoCaja? ObtenerCajaAbiertaGeneral() => _cajas.ObtenerCajaAbiertaGeneral();
    public List<TurnoCaja> ObtenerHistorial(int idEmpleado) => _cajas.ObtenerHistorial(idEmpleado);
    public List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int idUsuario, string? rol)
    {
        if (idUsuario <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        return _cajas.ObtenerHistorialEfectivo(rol == "Admin" ? null : idUsuario);
    }
    public List<TurnoCaja> ObtenerTodoHistorial(string? rol)
    {
        if (rol != "Admin") throw new UnauthorizedAccessException("Solo un administrador puede consultar el historial completo.");
        return _cajas.ObtenerTodoHistorial();
    }
    public TurnoCaja AbrirCaja(int idEmpleado)
    {
        if (idEmpleado <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        if (_cajas.ObtenerCajaAbiertaGeneral() != null) throw new InvalidOperationException("La caja física ya está abierta por otro usuario. Cerrala antes de iniciar el siguiente turno.");
        return _cajas.Abrir(idEmpleado, ObtenerMontoSiguienteApertura());
    }

    public decimal ObtenerMontoSiguienteApertura()
    {
        var ultimaCerrada = _cajas.ObtenerUltimaCajaCerrada();
        return ultimaCerrada == null ? 0m : ultimaCerrada.EfectivoFinal ?? ObtenerEfectivoDisponible(ultimaCerrada.IdTurnoCaja);
    }

    public CajaResumenDatos ObtenerResumen(int idCaja) => CalculadorCaja.CalcularResumen(_cajas.ObtenerMovimientosResumen(idCaja));

    // Un empleado solo cierra su propia caja. Un Admin puede cerrar la de cualquiera (por ejemplo, tras un corte de luz
    // que dejó abierta la caja de un empleado). El rol llega por parámetro, igual que en RetirarEfectivo
    public void CerrarCaja(int idCaja, int idEmpleado, decimal efectivoContado, string? motivoDiferencia, string? rol = null)
    {
        var caja = _cajas.Obtener(idCaja);
        if (caja.IdEmpleado != idEmpleado && rol != "Admin") throw new InvalidOperationException("Solo podés cerrar tu propio turno de caja.");
        var efectivoEsperado = ObtenerEfectivoDisponible(idCaja);
        var diferencia = CalculadorCaja.CalcularDiferenciaEfectivo(efectivoEsperado, efectivoContado);
        if (diferencia != 0m && string.IsNullOrWhiteSpace(motivoDiferencia))
            throw new ArgumentException("Indicá el motivo de la diferencia de efectivo.");
        _cajas.Cerrar(idCaja, ObtenerResumen(idCaja), efectivoEsperado, efectivoContado, diferencia, motivoDiferencia?.Trim());
    }

    public decimal ObtenerEfectivoDisponible(int idCaja)
    {
        var d = _cajas.ObtenerDatosEfectivo(idCaja);
        return CalculadorCaja.CalcularEfectivoDisponible(d.AperturaMasIngresos, d.VentasEfectivo, d.PagosCuentaCorrienteEfectivo, d.Retiros);
    }

    public void RetirarEfectivo(int idTurnoCaja, decimal monto, string contrasenaAdmin)
    {
        if (monto <= 0) throw new ArgumentException("El monto del retiro debe ser mayor a cero.");
        var admin = ObtenerAdminAutorizado(contrasenaAdmin);
        var caja = _cajas.Obtener(idTurnoCaja);
        if (caja.Estado != TurnoCaja.EstadoAbierta || _cajas.ObtenerCajaAbiertaGeneral()?.IdTurnoCaja != idTurnoCaja)
            throw new InvalidOperationException("Solo se puede retirar efectivo de la caja física abierta.");
        if (ObtenerEfectivoDisponible(idTurnoCaja) < monto) throw new InvalidOperationException("El retiro supera el efectivo disponible en caja.");
        _cajas.RegistrarRetiro(new RetiroCaja { IdTurnoCaja = idTurnoCaja, IdEmpleado = admin.IdEmpleado, Monto = monto, FechaRetiro = DateTime.Now });
    }

    public void AgregarEfectivo(int idTurnoCaja, decimal monto, string contrasenaAdmin)
    {
        if (monto <= 0m) throw new ArgumentException("El monto a agregar debe ser mayor a cero.");
        var admin = ObtenerAdminAutorizado(contrasenaAdmin);
        var caja = _cajas.Obtener(idTurnoCaja);
        if (caja.Estado != TurnoCaja.EstadoAbierta || _cajas.ObtenerCajaAbiertaGeneral()?.IdTurnoCaja != idTurnoCaja)
            throw new InvalidOperationException("Solo se puede agregar efectivo a la caja física abierta.");
        _cajas.RegistrarIngreso(new IngresoCaja { IdTurnoCaja = idTurnoCaja, IdAdmin = admin.IdEmpleado, Monto = monto, FechaIngreso = DateTime.Now });
    }

    private Empleado ObtenerAdminAutorizado(string contrasena)
    {
        var admin = _empleados.ObtenerTodos().FirstOrDefault(e =>
            e.Activo && e.Rol == "Admin" && !string.IsNullOrEmpty(contrasena) && e.Contrasena == contrasena);
        if (admin == null)
            throw new UnauthorizedAccessException("La contraseña del administrador es incorrecta.");
        return admin;
    }
}   