using System.Globalization;

namespace Estacionamento.UI
{
    internal static class MoneyHelper
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public static string Formatar(decimal valor) => valor.ToString("C", PtBr);

        public static string FormatarNumero(decimal valor) => valor.ToString("N2", PtBr);

        public static string FormatarParaEntrada(decimal valor) => valor.ToString("N2", PtBr);

        public static bool TryParse(string? input, out decimal value)
        {
            input = (input ?? string.Empty).Trim().Replace("R$", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            if (decimal.TryParse(input, NumberStyles.Number, PtBr, out value))
            {
                return true;
            }

            if (decimal.TryParse(input.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            value = 0m;
            return false;
        }
    }
}
