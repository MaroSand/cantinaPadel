using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    // PLACEHOLDER: contrato mínimo que necesita Alquiler por Día
    public interface ICajaRepository
    {
        Caja? ObtenerCajaAbierta(int idEmpleado);
        Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado);
    }
}
