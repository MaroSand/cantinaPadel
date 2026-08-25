using cantinaPadel.Models;

namespace cantinaPadel.DAL.Repositories
{
    public interface ICanchaRepository
    {
        List<Cancha> ObtenerTodas(bool? activa = true);
        Cancha? ObtenerPorId(int idCancha);
        bool ExisteNombre(string nombre, int? idCanchaExcluir = null);

        // Devuelve el id de la categoría "Hora Padel" (sembrada por el script SQL), usada para crear el producto asociado a una cancha nueva
        int ObtenerIdCategoriaHoraPadel();

        void Agregar(Cancha cancha);
        void Modificar(Cancha cancha);
        void CambiarEstado(int idCancha, bool nuevoEstado);
    }
}