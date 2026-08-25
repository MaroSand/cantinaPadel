using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace cantinaPadel.Models
{
    [Table("Personas")]
    public class Persona
    {
        [Key]
        [Column("id_persona")]
        public int IdPersona { get; set; }

        [Column("nombre")]
        [Required]
        public string Nombre { get; set; }

        [Column("apellido")]
        [Required]
        public string Apellido { get; set; }

        [Column("dni")]
        public string? Dni { get; set; }
        
        [Column("cuit")]
        public string? Cuit { get; set; }
        
        [Column("condicion_iva")]
        public string? CondicionIva { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_alta")]
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        [Column("es_cliente")]
        public bool EsCliente { get; set; } = false;

        [Column("es_empleado")]
        public bool EsEmpleado { get; set; } = false;

        [Column("es_proveedor")]
        public bool EsProveedor { get; set; } = false;

        // @OneToOne
        public Empleado?   Empleado   { get; set; }
        public Cliente?    Cliente    { get; set; }
        public Proveedor?  Proveedor  { get; set; }

        public static readonly string[] CondicionesIvaValidas =
        {
            "Responsable Inscripto",
            "Responsable No Inscripto",
            "Inscripto Exterior",
            "No Responsable",
            "IVA Exento",
            "Consumidor Final",
            "Monotributista",
            "Sujeto No Categorizado",
            "Proveedor del Exterior",
            "Cliente del Exterior",
            "IVA Liberado",
            "Pequeño Contribuyente Eventual"
        };

        public void NormalizarDatosComunes()
        {
            Nombre = Nombre?.Trim() ?? string.Empty;
            Apellido = Apellido?.Trim() ?? string.Empty;
            Dni = Dni?.Trim();
            Cuit = Cuit?.Trim();
            CondicionIva = CondicionIva?.Trim();
            Telefono = Telefono?.Trim();
            Direccion = Direccion?.Trim();
        }

        public void ValidarDatosComunes(bool dniObligatorio = false)
        {
            NormalizarDatosComunes();

            ValidarNombre(Nombre);
            ValidarApellido(Apellido);
            ValidarDni(Dni, dniObligatorio);
            ValidarCuit(Cuit);
            ValidarTelefono(Telefono);
            ValidarCondicionIva(CondicionIva);
        }

        public static void ValidarNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El campo Nombre es obligatorio y no puede contener solo espacios.");

            if (nombre.Length > 100)
                throw new ArgumentException("El Nombre no puede exceder los 100 caracteres.");

            if (nombre.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
                throw new ArgumentException("El Nombre no puede contener números ni caracteres especiales.");
        }

        public static void ValidarApellido(string? apellido)
        {
            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El campo Apellido es obligatorio y no puede contener solo espacios.");

            if (apellido.Length > 100)
                throw new ArgumentException("El Apellido no puede exceder los 100 caracteres.");

            if (apellido.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c)))
                throw new ArgumentException("El Apellido no puede contener números ni caracteres especiales.");
        }

        public static void ValidarDni(string? dni, bool obligatorio = false)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                if (obligatorio)
                    throw new ArgumentException("El campo DNI es obligatorio y no puede contener solo espacios.");

                return;
            }

            if (!Regex.IsMatch(dni, @"^\d{7,8}$"))
                throw new ArgumentException("El DNI debe contener entre 7 y 8 dígitos numéricos.");
        }

        public static void ValidarCuit(string? cuit)
        {
            // Control de campo obligatorio
            if (string.IsNullOrWhiteSpace(cuit))
                throw new ArgumentException("El campo CUIT/CUIL es obligatorio.");

            // Extraer solo dígitos numéricos
            string cuitLimpio = cuit.Replace("-", "").Replace(" ", "").Trim();

            if (cuitLimpio.Length != 11 || !long.TryParse(cuitLimpio, out _))
                throw new ArgumentException("El CUIT debe contener 11 dígitos numéricos completos (formato XX-XXXXXXXX-X).");

            // Validación de prefijo válido de AFIP
            string prefijo = cuitLimpio.Substring(0, 2);
            string[] prefijosValidos = { "20", "23", "24", "27", "30", "33", "34" };
            if (!prefijosValidos.Contains(prefijo))
                throw new ArgumentException("El CUIT ingresado no posee un prefijo válido.");

            // Algoritmo Módulo 11 (Dígito verificador)
            int[] factores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += (cuitLimpio[i] - '0') * factores[i];
            }

            int resto = suma % 11;
            int digitoVerificadorCalculado = 11 - resto;

            if (digitoVerificadorCalculado == 11) digitoVerificadorCalculado = 0;
            if (digitoVerificadorCalculado == 10) digitoVerificadorCalculado = 9;

            int digitoVerificadorIngresado = cuitLimpio[10] - '0';

            if (digitoVerificadorCalculado != digitoVerificadorIngresado)
                throw new ArgumentException("El CUIT ingresado no es válido (falló el dígito verificador de AFIP).");
        }

        public static void ValidarTelefono(string? telefono)
        {
            if (!string.IsNullOrWhiteSpace(telefono) && !Regex.IsMatch(telefono, @"^[\d\s\-\+]{6,20}$"))
                throw new ArgumentException("El teléfono ingresado no es válido.");
        }

        public static void ValidarCondicionIva(string? condicionIva)
        {
            if (!string.IsNullOrWhiteSpace(condicionIva) && !CondicionesIvaValidas.Contains(condicionIva))
                throw new ArgumentException("La condición de IVA ingresada no es válida.");
        }
    }
}
