using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface ICajaRepository
    {
        Caja? ObtenerCajaAbierta(int idEmpleado);
        Caja? ObtenerCajaAbiertaGeneral() => null;
        Caja? ObtenerUltimaCajaCerrada() => null;
        List<Caja> ObtenerHistorial(int idEmpleado) => new();
        List<Caja> ObtenerTodoHistorial() => new();
        List<MovimientoEfectivoHistorial> ObtenerHistorialEfectivo(int? idEmpleado) => new();
        Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado);
        Caja Abrir(int idEmpleado, decimal montoInicial) => throw new NotSupportedException();
        Caja Obtener(int idCaja) => throw new NotSupportedException();
        CajaResumenDatos ObtenerResumen(int idCaja) => throw new NotSupportedException();
        List<MovimientoCajaDato> ObtenerMovimientosResumen(int idCaja) => throw new NotSupportedException();
        void Cerrar(int idCaja, CajaResumenDatos resumen, decimal efectivoFinal) => throw new NotSupportedException();
        EfectivoCajaDatos ObtenerDatosEfectivo(int idCaja) => throw new NotSupportedException();
        void RegistrarRetiro(RetiroCaja retiro) => throw new NotSupportedException();
        void RegistrarIngreso(IngresoCaja ingreso) => throw new NotSupportedException();
    }
}
