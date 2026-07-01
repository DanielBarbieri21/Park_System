using System;

namespace Estacionamento.Services
{
    public static class CobrancaCalculadora
    {
        public static decimal Calcular(
            TimeSpan permanencia,
            decimal valorPrimeiraHora,
            decimal valorHoraAdicional,
            int toleranciaMinutos = 0)
        {
            if (valorPrimeiraHora <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valorPrimeiraHora));
            }

            if (valorHoraAdicional <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(valorHoraAdicional));
            }

            var minutos = (decimal)permanencia.TotalMinutes;
            if (minutos <= toleranciaMinutos)
            {
                return 0m;
            }

            if (minutos <= 60m)
            {
                return valorPrimeiraHora;
            }

            var minutosAdicionais = minutos - 60m;
            var valorAdicional = (minutosAdicionais / 60m) * valorHoraAdicional;
            return Math.Round(valorPrimeiraHora + valorAdicional, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalcularValorAdicional(TimeSpan permanencia, decimal valorHoraAdicional)
        {
            var minutos = (decimal)permanencia.TotalMinutes;
            if (minutos <= 60m)
            {
                return 0m;
            }

            return Math.Round(((minutos - 60m) / 60m) * valorHoraAdicional, 2, MidpointRounding.AwayFromZero);
        }
    }
}
