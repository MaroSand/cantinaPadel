using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.BLL;

public class LogicaCaja
{
    private readonly ICajaRepository _cajas;
    private readonly IEmpleadoRepository _empleados;

    public LogicaCaja() : this(new CajaRepository(), new EmpleadoRepository())
    {
    }

    public LogicaCaja(ICajaRepository cajas, IEmpleadoRepository empleados)
    {
        _cajas = cajas;
        _empleados = empleados;
    }

    public TurnoCaja? ObtenerCajaAbierta(int idEmpleado) => _cajas.ObtenerCajaAbierta(idEmpleado);
    public TurnoCaja? ObtenerCajaAbiertaGeneral() => _cajas.ObtenerCajaAbiertaGeneral();

    // Caja que el empleado dejó abierta en una sesión anterior (cierre abrupto del programa, corte de luz, etc.).
    // Como el sistema no deja cerrar sesión ni salir con la caja abierta, si al loguearse tiene una abierta
    // es porque quedó colgada: debe cerrarla antes de hacer cualquier otra cosa.
    public TurnoCaja? ObtenerCajaPendienteDeCierre(int idEmpleado) => _cajas.ObtenerCajaAbierta(idEmpleado);

    // Todas las cajas abiertas (paralelas). Solo el administrador puede verlas y cerrarlas
    public List<TurnoCaja> ObtenerCajasAbiertas(string? rol)
    {
        if (rol != "Admin")
            throw new UnauthorizedAccessException("Solo un administrador puede consultar todas las cajas abiertas.");
        return _cajas.ObtenerCajasAbiertas();
    }
    public List<TurnoCaja> ObtenerHistorial(int idEmpleado) => _cajas.ObtenerHistorial(idEmpleado);

    // El historial de ingresos y retiros de efectivo es solo para el administrador
    public List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int idUsuario, string? rol)
    {
        if (idUsuario <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        if (rol != "Admin")
            throw new UnauthorizedAccessException("Solo un administrador puede consultar el historial de ingresos y retiros.");
        return _cajas.ObtenerHistorialEfectivo(null);
    }

    public List<TurnoCaja> ObtenerTodoHistorial(string? rol)
    {
        if (rol != "Admin")
            throw new UnauthorizedAccessException("Solo un administrador puede consultar el historial completo.");
        return _cajas.ObtenerTodoHistorial();
    }

    // El monto inicial lo ingresa quien abre la caja (por defecto 0).
    public TurnoCaja AbrirCaja(int idEmpleado, decimal montoInicial = 0m)
    {
        if (idEmpleado <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        if (montoInicial < 0m) throw new ArgumentException("El monto inicial no puede ser negativo.");
        // Cajas paralelas: cada empleado puede tener su propia caja abierta, pero solo una a la vez
        if (_cajas.ObtenerCajaAbierta(idEmpleado) != null)
            throw new InvalidOperationException("Ya tenés una caja abierta. Cerrala antes de abrir otra.");
        return _cajas.Abrir(idEmpleado, montoInicial);
    }

    public decimal ObtenerMontoSiguienteApertura()
    {
        var ultimaCerrada = _cajas.ObtenerUltimaCajaCerrada();
        return ultimaCerrada == null
            ? 0m
            : ultimaCerrada.EfectivoFinal ?? ObtenerEfectivoDisponible(ultimaCerrada.IdTurnoCaja);
    }

    public CajaResumenDatos ObtenerResumen(int idCaja) =>
        CalculadorCaja.CalcularResumen(_cajas.ObtenerMovimientosResumen(idCaja));

    // Un empleado solo cierra su propia caja. Un Admin puede cerrar la de cualquiera (por ejemplo, tras un corte de luz
    // que dejó abierta la caja de un empleado).
    public void CerrarCaja(int idCaja, int idEmpleado, decimal efectivoContado, string? motivoDiferencia,
        string? rol = null)
    {
        var caja = _cajas.Obtener(idCaja);
        if (caja.IdEmpleado != idEmpleado && rol != "Admin")
            throw new InvalidOperationException("Solo podés cerrar tu propio turno de caja.");
        var efectivoEsperado = ObtenerEfectivoDisponible(idCaja);
        var diferencia = CalculadorCaja.CalcularDiferenciaEfectivo(efectivoEsperado, efectivoContado);
        if (diferencia != 0m && string.IsNullOrWhiteSpace(motivoDiferencia))
            throw new ArgumentException("Indicá el motivo de la diferencia de efectivo.");
        _cajas.Cerrar(idCaja, ObtenerResumen(idCaja), efectivoEsperado, efectivoContado, diferencia,
            motivoDiferencia?.Trim());
    }

    // Total retirado de la caja en el turno (se muestra en el cierre de caja)
    public decimal ObtenerRetiros(int idCaja) => _cajas.ObtenerDatosEfectivo(idCaja).Retiros;

    // Efectivo ingresado a la caja en el turno, sin contar el fondo inicial
    public decimal ObtenerIngresos(int idCaja) =>
        _cajas.ObtenerDatosEfectivo(idCaja).AperturaMasIngresos - _cajas.Obtener(idCaja).MontoApertura;

    public decimal ObtenerEfectivoDisponible(int idCaja)
    {
        var d = _cajas.ObtenerDatosEfectivo(idCaja);
        return CalculadorCaja.CalcularEfectivoDisponible(d.AperturaMasIngresos, d.VentasEfectivo,
            d.PagosCuentaCorrienteEfectivo, d.Retiros);
    }

    public void RetirarEfectivo(int idTurnoCaja, decimal monto, string contrasenaAdmin)
    {
        if (monto <= 0) throw new ArgumentException("El monto del retiro debe ser mayor a cero.");
        var admin = ObtenerAdminAutorizado(contrasenaAdmin);
        var caja = _cajas.Obtener(idTurnoCaja);
        if (!EstaAbierta(caja))
            throw new InvalidOperationException("Solo se puede retirar efectivo de una caja abierta.");
        if (ObtenerEfectivoDisponible(idTurnoCaja) < monto)
            throw new InvalidOperationException("El retiro supera el efectivo disponible en caja.");
        _cajas.RegistrarRetiro(new RetiroCaja
        { IdTurnoCaja = idTurnoCaja, IdEmpleado = admin.IdEmpleado, Monto = monto, FechaRetiro = DateTime.Now });
    }

    // Ingreso de efectivo a la caja: no requiere contraseña y se permite una sola vez por turno de caja
    // Queda registrado el empleado que lo realizó y el monto
    public void AgregarEfectivo(int idTurnoCaja, decimal monto, int idEmpleado)
    {
        if (idEmpleado <= 0) throw new ArgumentException("No hay un usuario autenticado.");
        if (monto <= 0m) throw new ArgumentException("El monto a agregar debe ser mayor a cero.");
        var caja = _cajas.Obtener(idTurnoCaja);
        if (!EstaAbierta(caja))
            throw new InvalidOperationException("Solo se puede agregar efectivo a una caja abierta.");
        if (_cajas.ExisteIngreso(idTurnoCaja))
            throw new InvalidOperationException("Ya se registró un ingreso de efectivo en este turno de caja.");
        _cajas.RegistrarIngreso(new IngresoCaja
        { IdTurnoCaja = idTurnoCaja, IdAdmin = idEmpleado, Monto = monto, FechaIngreso = DateTime.Now });
    }

    public bool ExisteIngreso(int idTurnoCaja) => _cajas.ExisteIngreso(idTurnoCaja);

    // El turno sigue abierto: figura como abierto y es la caja abierta actual de su empleado
    private bool EstaAbierta(TurnoCaja caja) =>
        caja.Estado == TurnoCaja.EstadoAbierta &&
        _cajas.ObtenerCajaAbierta(caja.IdEmpleado)?.IdTurnoCaja == caja.IdTurnoCaja;

    private Empleado ObtenerAdminAutorizado(string contrasena)
    {
        var admin = _empleados.ObtenerTodos().FirstOrDefault(e =>
            e.Activo && e.Rol == "Admin" && !string.IsNullOrEmpty(contrasena) && e.Contrasena == contrasena);
        if (admin == null)
            throw new UnauthorizedAccessException("La contraseña del administrador es incorrecta.");
        return admin;
    }
}