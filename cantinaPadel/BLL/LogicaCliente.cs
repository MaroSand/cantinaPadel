using System;
using System.Collections.Generic;
using System.Text;
using cantinaPadel.Models;
using cantinaPadel.DAL.Repositories;

namespace cantinaPadel.BLL
{
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

        // Método privado para validar el formato del email
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


        // Método para modificar un cliente existente
        public void Modificar(Cliente cliente)
        {
            if (!EsEmailValido(cliente.Email))
                throw new ArgumentException("El Email ingresado no es válido.");

            _clienteRepository.Modificar(cliente);
        }

        // Método para dar de alta un nuevo cliente

        public void Alta(Cliente cliente)
        {

            // Validar el email del cliente antes de agregarlo
            if (!EsEmailValido(cliente.Email))
                throw new ArgumentException("El Email ingresado no es válido.");


            Persona? personaExiste = null;
            // Verificar si la persona ya existe en la base de datos por su DNI
            if (!string.IsNullOrWhiteSpace(cliente.Persona.Dni))
                personaExiste = _clienteRepository.BuscarPersonaPorDni(cliente.Persona.Dni);

            // Si la persona no existe, se marca como cliente; si existe, se actualiza su estado a cliente y se asigna al cliente
            if (personaExiste == null)
            {
                cliente.Persona.EsCliente = true;
            }
            else
            {
                // Si la persona ya existe, se actualiza su estado a cliente y se asigna al cliente
                personaExiste.EsCliente = true;
                cliente.IdPersona = personaExiste.IdPersona;
                cliente.Persona = personaExiste;
            }
            // Agregar el cliente a la base de datos
            _clienteRepository.Agregar(cliente);
        }
    }
}