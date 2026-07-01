namespace Estacionamento.Models
{
    public static class TarifasPadrao
    {
        public readonly record struct Tarifa(decimal PrimeiraHora, decimal HoraAdicional);

        public static Tarifa Obter(TipoVeiculo tipo) => tipo switch
        {
            TipoVeiculo.Moto => new Tarifa(12m, 8m),
            TipoVeiculo.Carro => new Tarifa(15m, 8m),
            TipoVeiculo.Caminhao => new Tarifa(25m, 12m),
            _ => new Tarifa(15m, 8m)
        };
    }
}
