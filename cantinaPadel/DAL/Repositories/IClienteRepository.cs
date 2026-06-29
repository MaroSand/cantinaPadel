using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace cantinaPadel.DAL.Repositories
{
    public interface IClienteRepository
    {
        List<Cliente> ObtenerTodos();
         Cliente? ObtenerPorId(int id);
         void Agregar(Cliente cliente);
         void Modificar(Cliente cliente);
         void Bajalogica(int id);
        Persona? BuscarPersonaPorDni(string dni);
    }
}
