using System;
using System.Linq;

namespace cantinaPadel.BLL
{
    internal static class CoincidenciaBusqueda
    {
        public static bool EmpiezaConAlgunCampo(string texto, params string?[] campos)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return false;

            return campos.Any(campo =>
                campo?.StartsWith(texto, StringComparison.CurrentCultureIgnoreCase) == true);
        }
    }
}
