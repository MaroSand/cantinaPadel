using cantinaPadel.DAL;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.BLL
{
    public class LogicaPersonaRoles
    {
        public Cliente? ObtenerClientePorPersonaId(int idPersona)
        {
            using var ctx = new AppDbContext();
            return ctx.Clientes
                .Include(c => c.Persona)
                .FirstOrDefault(c => c.IdPersona == idPersona);
        }

        public Proveedor? ObtenerProveedorPorPersonaId(int idPersona)
        {
            using var ctx = new AppDbContext();
            return ctx.Proveedores
                .Include(p => p.Persona)
                .FirstOrDefault(p => p.IdPersona == idPersona);
        }

        public Empleado? ObtenerEmpleadoPorPersonaId(int idPersona)
        {
            using var ctx = new AppDbContext();
            return ctx.Empleados
                .Include(e => e.Persona)
                .FirstOrDefault(e => e.IdPersona == idPersona);
        }

        public void GuardarRoles(Persona persona, Cliente? cliente, Proveedor? proveedor, Empleado? empleado)
        {
            if (persona == null)
                throw new ArgumentException("Los datos de la persona son obligatorios.");

            bool requiereDni = empleado != null;
            persona.ValidarDatosComunes(dniObligatorio: requiereDni);

            if (cliente != null)
            {
                cliente.Persona = persona;
                cliente.Validar();
            }

            if (proveedor != null)
            {
                proveedor.Persona = persona;
                proveedor.ValidarNombreEmpresa();
            }

            if (empleado != null)
            {
                empleado.Persona = persona;
                empleado.ValidarFormato(dniObligatorio: true);
            }

            using var ctx = new AppDbContext();
            using var tx = ctx.Database.BeginTransaction();

            Persona personaDb = ObtenerOAgregarPersona(ctx, persona);
            ActualizarPersona(personaDb, persona);

            ctx.SaveChanges();

            if (cliente != null)
                GuardarCliente(ctx, personaDb, cliente);

            if (proveedor != null)
                GuardarProveedor(ctx, personaDb, proveedor);

            if (empleado != null)
                GuardarEmpleado(ctx, personaDb, empleado);

            ctx.SaveChanges();
            tx.Commit();
        }

        private static Persona ObtenerOAgregarPersona(AppDbContext ctx, Persona persona)
        {
            Persona? personaDb = null;

            if (persona.IdPersona > 0)
            {
                personaDb = ctx.Personas.FirstOrDefault(p => p.IdPersona == persona.IdPersona);
            }

            if (personaDb == null && !string.IsNullOrWhiteSpace(persona.Dni))
            {
                personaDb = ctx.Personas.FirstOrDefault(p => p.Dni == persona.Dni);
            }

            if (personaDb != null)
                return personaDb;

            ctx.Personas.Add(persona);
            return persona;
        }

        private static void ActualizarPersona(Persona destino, Persona origen)
        {
            destino.Nombre = origen.Nombre;
            destino.Apellido = origen.Apellido;
            destino.Dni = origen.Dni;
            destino.Cuit = origen.Cuit;
            destino.CondicionIva = origen.CondicionIva;
            destino.Telefono = origen.Telefono;
            destino.Direccion = origen.Direccion;
            destino.Activo = origen.Activo;
        }

        private static void GuardarCliente(AppDbContext ctx, Persona personaDb, Cliente cliente)
        {
            var clienteDb = ctx.Clientes.FirstOrDefault(c => c.IdPersona == personaDb.IdPersona);

            if (clienteDb == null)
            {
                cliente.IdPersona = personaDb.IdPersona;
                cliente.Persona = personaDb;
                personaDb.EsCliente = true;
                ctx.Clientes.Add(cliente);
                return;
            }

            clienteDb.Email = cliente.Email;
            personaDb.EsCliente = true;
        }

        private static void GuardarProveedor(AppDbContext ctx, Persona personaDb, Proveedor proveedor)
        {
            var proveedorDb = ctx.Proveedores.FirstOrDefault(p => p.IdPersona == personaDb.IdPersona);

            if (proveedorDb == null)
            {
                proveedor.IdPersona = personaDb.IdPersona;
                proveedor.Persona = personaDb;
                personaDb.EsProveedor = true;
                ctx.Proveedores.Add(proveedor);
                return;
            }

            proveedorDb.NombreEmpresa = proveedor.NombreEmpresa;
            personaDb.EsProveedor = true;
        }

        private static void GuardarEmpleado(AppDbContext ctx, Persona personaDb, Empleado empleado)
        {
            var empleadoDb = ctx.Empleados.FirstOrDefault(e => e.IdPersona == personaDb.IdPersona);

            bool usuarioDuplicado = ctx.Empleados.Any(e =>
                e.NombreUsuario == empleado.NombreUsuario &&
                e.IdEmpleado != (empleadoDb == null ? empleado.IdEmpleado : empleadoDb.IdEmpleado));

            if (usuarioDuplicado)
                throw new ArgumentException("El nombre de usuario ya está registrado en el sistema.");

            bool dniDuplicado = !string.IsNullOrWhiteSpace(personaDb.Dni) && ctx.Empleados
                .Include(e => e.Persona)
                .Any(e =>
                    e.Persona.Dni == personaDb.Dni &&
                    e.IdPersona != personaDb.IdPersona);

            if (dniDuplicado)
                throw new ArgumentException("El DNI ingresado ya pertenece a otra persona registrada como empleado.");

            if (empleadoDb == null)
            {
                empleado.IdPersona = personaDb.IdPersona;
                empleado.Persona = personaDb;
                empleado.Activo = true;
                personaDb.EsEmpleado = true;
                ctx.Empleados.Add(empleado);
                return;
            }

            empleadoDb.NombreUsuario = empleado.NombreUsuario;
            empleadoDb.Contrasena = empleado.Contrasena;
            empleadoDb.Rol = empleado.Rol;
            if (empleado.IdEmpleado > 0)
                empleadoDb.Activo = empleado.Activo;
            personaDb.EsEmpleado = true;
        }
    }
}
