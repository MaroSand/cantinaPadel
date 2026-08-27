using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    // PLACEHOLDER: contrato mínimo que necesita Alquiler por Día.
    // Si en paralelo aparece un módulo de Caja más completo, fusionar
    // este método ahí en vez de mantener dos repos de la misma tabla.
    public interface ICajaRepository
    {
        Caja? ObtenerCajaAbierta(int idEmpleado);
        Caja ObtenerOCrearCajaTecnicaParaPruebas(int idEmpleado);
    }
}
