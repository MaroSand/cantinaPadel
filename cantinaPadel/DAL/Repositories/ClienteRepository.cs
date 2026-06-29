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
                .Where(c => c.Persona.Activo)
                .ToList();
        }
        public Cliente? ObtenerPorId(int id)
        {
            using var context = new AppDbContext();
            return context.Clientes
                .Include(c => c.Persona)
                .FirstOrDefault(c => c.IdCliente == id && c.Persona.Activo);
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
            var cliente = context.Clientes.Find(id);
            if (cliente != null)
            {
                cliente.Persona.Activo = false;
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
