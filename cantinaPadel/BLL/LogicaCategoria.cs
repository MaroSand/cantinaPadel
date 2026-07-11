using System;
using System.Collections.Generic;
using System.Linq;
using cantinaPadel.DAL;
using cantinaPadel.Models;

namespace cantinaPadel.BLL
{
    public class LogicaCategoria
    {
        private readonly AppDbContext _context;

        public LogicaCategoria(AppDbContext context)
        {
            _context = context;
        }

        // Trae solo las categorías que están operativas en la cantina
        public List<Categoria> ListarActivas()
        {
            return _context.Categorias
                           .Where(c => c.Activa)
                           .OrderBy(c => c.Nombre)
                           .ToList();
        }

        // Listado completo para la grilla del ABM (activos e inactivos)
        public List<Categoria> ListarTodas()
        {
            return _context.Categorias
                           .OrderBy(c => c.Nombre)
                           .ToList();
        }

        // Inserta una nueva categoría o actualiza los cambios de una existente
        public void Guardar(Categoria categoria)
        {
            // Ejecuta los Trim y chequea que el nombre no venga vacío o muy largo
            categoria.Validar();

            // Validar nombre único ignorando mayúsculas/minúsculas
            bool yaExiste = _context.Categorias.Any(c =>
                c.IdCategoria != categoria.IdCategoria &&   // Si es modificación, no se compara consigo misma
                c.Nombre.ToLower().Trim() == categoria.Nombre.ToLower().Trim() &&
                c.Activa);  // Solo choca si la categoría duplicada está activa

            if (yaExiste)
                throw new ArgumentException($"Ya existe una categoría activa con el nombre '{categoria.Nombre}'.");

            // Define si va un Add o un Update según el id
            if (categoria.IdCategoria == 0)
            {
                _context.Categorias.Add(categoria);
            }
            else
            {
                _context.Categorias.Update(categoria);
            }

            _context.SaveChanges();
        }

        // Manejo de la baja lógica o reactivación desde la grilla
        public void CambiarEstado(int idCategoria, bool nuevoEstado)
        {
            var categoria = _context.Categorias.Find(idCategoria);
            if (categoria != null)
            {
                categoria.Activa = nuevoEstado;
                _context.SaveChanges();
            }
        }
    }
}