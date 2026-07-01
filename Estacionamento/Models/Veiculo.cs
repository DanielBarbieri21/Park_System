using System;

namespace Estacionamento.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public TipoVeiculo Tipo { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        public decimal ValorHora { get; set; }
        public decimal ValorHoraAdicional { get; set; }

        public TimeSpan TempoPermanencia => (Saida ?? DateTime.UtcNow) - Entrada;

        public decimal CalcularValor()
        {
            return Services.CobrancaCalculadora.Calcular(TempoPermanencia, ValorHora, ValorHoraAdicional);
        }
    }
}
