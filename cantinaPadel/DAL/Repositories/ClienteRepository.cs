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
        public List<Cliente> Buscar(string texto)
        {
            texto = texto?.Trim().ToLower() ?? string.Empty;

            using var context = new AppDbContext();
            var query = context.Clientes
                .Include(c => c.Persona)
                .Where(c => c.Persona.Activo);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(c =>
                    c.Persona.Nombre.ToLower().Contains(texto) ||
                    c.Persona.Apellido.ToLower().Contains(texto) ||
                    (c.Persona.Dni != null && c.Persona.Dni.ToLower().Contains(texto)) ||
                    c.Email.ToLower().Contains(texto));
            }

            return query
                .OrderBy(c => c.Persona.Apellido)
                .ThenBy(c => c.Persona.Nombre)
                .Take(25)
                .ToList();
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
