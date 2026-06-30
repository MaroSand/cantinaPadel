using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;
namespace cantinaPadel.DAL.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        public List<Cliente> ObtenerTodos()
        {

            using var context = new AppDbContext();
            return context.Clientes
                .Include(c => c.Persona)
                .ToList();
        }
        public Cliente? ObtenerPorId(int id)
        {
            using var context = new AppDbContext();
            return context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefault(c => c.IdCliente == id);
        }
        public void Agregar(Cliente cliente)
        {
            using var context = new AppDbContext();
            context.Clientes.Add(cliente);
            context.SaveChanges();
        }
        public void Modificar(Cliente cliente)
        {
            using var context = new AppDbContext();
            context.Clientes.Update(cliente);
            context.SaveChanges();
        }
        public void Bajalogica(int id)
        {
            using var context = new AppDbContext();
            var cliente = context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefault(c => c.IdCliente == id);

            if (cliente != null)
            {
                cliente.Persona.Activo = !cliente.Persona.Activo;
                context.SaveChanges();
            }
        }
        public Persona? BuscarPersonaPorDni(string dni)
        {
            using var context = new AppDbContext();
            return context.Personas.FirstOrDefault(p => p.Dni == dni);
        }
    }
}
