using System;
using System.Collections.Generic;
using System.Text;
using cantinaPadel.Models;
using cantinaPadel.DAL.Repositories;

namespace cantinaPadel.BLL
{
    //TODO VER SI ELIMINAR MÉTODOS QUE NO SE USAN MÁS POR REFACTOR LogicaPersonaRoles
    public class LogicaCliente
    {

        // Instancia del repositorio de clientes
        private readonly IClienteRepository _clienteRepository;

        // Constructor que inicializa el repositorio de clientes
        public LogicaCliente()
        {
            _clienteRepository = new ClienteRepository();
        }
        // Método para obtener todos los clientes

        public List<Cliente> ObtenerTodos()
        {
            return _clienteRepository.ObtenerTodos();
        }

        // Método para obtener un cliente por su ID

        public Cliente? ObtenerPorId(int id)
        {
            return _clienteRepository.ObtenerPorId(id);
        }

        // Método para dar de baja un cliente por su ID
        public void Baja(int id)
        {
            _clienteRepository.Bajalogica(id);
        }

        // Método privado para validar los datos de un cliente antes de agregarlo o modificarlo.
        private void Validar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentException("Los datos del cliente son obligatorios.");

            cliente.Validar();
        }
    }
}