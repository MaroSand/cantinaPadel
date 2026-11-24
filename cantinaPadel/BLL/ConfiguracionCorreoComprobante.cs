using System.Net.Mail;
using System.Configuration;

namespace cantinaPadel.BLL;

internal sealed class ConfiguracionCorreoComprobante
{
    public string Host { get; }
    public int Puerto { get; }
    public string Usuario { get; }
    public string Contrasena { get; }
    public string Remitente { get; }
    public bool UsarSsl { get; }

    private ConfiguracionCorreoComprobante(string host, int puerto, string usuario, string contrasena, string remitente, bool usarSsl)
    {
        Host = host;
        Puerto = puerto;
        Usuario = usuario;
        Contrasena = contrasena;
        Remitente = remitente;
        UsarSsl = usarSsl;
    }

    public static ConfiguracionCorreoComprobante Cargar()
    {
        string host = Leer("SMTP_HOST");
        string puertoTexto = Leer("SMTP_PORT");
        string usuario = Leer("SMTP_USER");
        string contrasena = Leer("SMTP_PASSWORD", trim: false);
        string remitente = Leer("SMTP_FROM");
        string sslTexto = Leer("SMTP_SSL");

        if (EsPlaceholder(host))
            throw new InvalidOperationException("Configure SMTP_HOST en App.config con el servidor SMTP real.");
        if (!int.TryParse(puertoTexto, out int puerto) || puerto is < 1 or > 65535)
            throw new InvalidOperationException("SMTP_PORT debe ser un puerto entre 1 y 65535.");
        if (EsPlaceholder(remitente) || !MailAddress.TryCreate(remitente, out _))
            throw new InvalidOperationException("Configure SMTP_FROM en App.config con una dirección de correo válida.");
        if (EsPlaceholder(usuario) || EsPlaceholder(contrasena))
            throw new InvalidOperationException("Reemplace SMTP_USER y SMTP_PASSWORD en App.config, o deje ambos vacíos si el servidor no requiere autenticación.");
        if (string.IsNullOrWhiteSpace(usuario) != string.IsNullOrWhiteSpace(contrasena))
            throw new InvalidOperationException("Configure tanto SMTP_USER como SMTP_PASSWORD, o deje ambos vacíos si el servidor no requiere autenticación.");
        if (!bool.TryParse(sslTexto, out bool usarSsl))
            throw new InvalidOperationException("SMTP_SSL debe ser true o false.");

        return new ConfiguracionCorreoComprobante(host, puerto, usuario, contrasena, remitente, usarSsl);
    }

    private static string Leer(string clave, bool trim = true)
    {
        string valor = ConfigurationManager.AppSettings[clave] ?? string.Empty;
        return trim ? valor.Trim() : valor;
    }

    private static bool EsPlaceholder(string valor)
        => string.IsNullOrWhiteSpace(valor) || valor.StartsWith("<", StringComparison.Ordinal) || valor.StartsWith("TU_", StringComparison.OrdinalIgnoreCase);
}
