using System;
using System.Collections.Generic;
using System.Text;
using cantinaPadel.Models;
using cantinaPadel.DAL.Repositories;

namespace cantinaPadel.BLL
{
    // Clase que contiene la lógica de negocio relacionada con los clientes.
    public class LogicaCliente
    {
        private readonly IClienteRepository _clienteRepository;

        // Constructor que inicializa el repositorio de clientes.
        public LogicaCliente()
        {
            _clienteRepository = new ClienteRepository();
        }

        // Método que obtiene todos los clientes.
        public List<Cliente> ObtenerTodos()
        {
            return _clienteRepository.ObtenerTodos();
        }
        // Método que obtiene un cliente por su ID.
        public Cliente? ObtenerPorId(int id)
        {
            return _clienteRepository.ObtenerPorId(id);
        }

        public void Baja(int id)
        {
            _clienteRepository.Bajalogica(id);
        }

        private bool EsEmailValido(string email)
        {
            try 
            {
                var direccion = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public void Modificar(Cliente cliente)
        {
            if (!EsEmailValido(cliente.Email))
            {
                throw new ArgumentException("El Email ingresado no es válido.");
            }
            _clienteRepository.Modificar(cliente);  
        }

        public void Alta(Cliente cliente)
        {
            if (!EsEmailValido(cliente.Email))
            {
                throw new ArgumentException("El Email ingresado no es válido.");
            }
            Persona? personaExiste = null;
            if (!string.IsNullOrWhriteSpace(cliente.Persona.Dni))
            {
                personaExiste = _clienteRepository.BuscarPersonaPorDni(cliente.Persona.Dni);
            }
            if (personaExiste == null)
            {
                cliente.Persona.EsCliente = true;
            }
            else
            {

            }
        }

    }
}