using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface ICajaRepository
    {
        TurnoCaja? ObtenerCajaAbierta(int idEmpleado);
        TurnoCaja? ObtenerCajaAbiertaGeneral() => null;
        TurnoCaja? ObtenerUltimaCajaCerrada() => null;
        List<TurnoCaja> ObtenerHistorial(int idEmpleado) => new();
        List<TurnoCaja> ObtenerTodoHistorial() => new();
        List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int? idEmpleado) => new();
        TurnoCaja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado);
        TurnoCaja Abrir(int idEmpleado, decimal montoInicial) => throw new NotSupportedException();
        TurnoCaja Obtener(int idTurnoCaja) => throw new NotSupportedException();
        CajaResumenDatos ObtenerResumen(int idCaja) => throw new NotSupportedException();
        List<MovimientoCajaDato> ObtenerMovimientosResumen(int idTurnoCaja) => throw new NotSupportedException();
        void Cerrar(int idTurnoCaja, CajaResumenDatos resumen, decimal efectivoEsperado, decimal efectivoContado, decimal diferencia, string? motivoDiferencia) => throw new NotSupportedException();
        EfectivoCajaDatos ObtenerDatosEfectivo(int idTurnoCaja) => throw new NotSupportedException();
        void RegistrarRetiro(RetiroCaja retiro) => throw new NotSupportedException();
        void RegistrarIngreso(IngresoCaja ingreso) => throw new NotSupportedException();
    }
}
