using System;
using System.Collections.Generic;
using System.Linq;
using cantinaPadel.DAL;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaMarca
    {
        private readonly AppDbContext _context;

        public LogicaMarca(AppDbContext context)
        {
            _context = context;
        }

        // Lista marcas activas
        public List<Marca> ListarActivas()
        {
            return _context.Marcas
                           .Where(m => m.Activa)
                           .OrderBy(m => m.Nombre)
                           .ToList();
        }

        // Trae todas las marcas
        public List<Marca> ListarTodas()
        {
            return _context.Marcas
                           .OrderBy(m => m.Nombre)
                           .ToList();
        }

        // Guarda la marca controlando que cumpla el formato y no esté repetida
        public void Guardar(Marca marca)
        {
            // Limpia los espacios y valida que el nombre sea obligatorio
            marca.Validar();

            // Validar nombre único para Marcas
            bool yaExiste = _context.Marcas.Any(m =>
                m.IdMarca != marca.IdMarca &&
                m.Nombre.ToLower().Trim() == marca.Nombre.ToLower().Trim() &&
                m.Activa);

            if (yaExiste)
                throw new ArgumentException($"Ya existe una marca activa con el nombre '{marca.Nombre}'.");

            // Si el id es 0 es un registro nuevo, sino impacta la modificación
            if (marca.IdMarca == 0)
            {
                _context.Marcas.Add(marca);
            }
            else
            {
                _context.Marcas.Update(marca);
            }

            _context.SaveChanges();
        }

        // Modifica el bit de activo para dar de alta o de baja la marca
        public void CambiarEstado(int idMarca, bool nuevoEstado)
        {
            var marca = _context.Marcas.Find(idMarca);
            if (marca != null)
            {
                marca.Activa = nuevoEstado;
                _context.SaveChanges();
            }
        }
    }
}