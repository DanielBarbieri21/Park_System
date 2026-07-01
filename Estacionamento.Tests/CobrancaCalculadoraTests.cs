using Estacionamento.Services;
using System;
using Xunit;

namespace Estacionamento.Tests;

public class CobrancaCalculadoraTests
{
    [Theory]
    [InlineData(45, 12, 8, 12)]
    [InlineData(60, 12, 8, 12)]
    [InlineData(90, 12, 8, 16)]
    [InlineData(150, 12, 8, 24)]
    public void Calcular_DeveAplicarPrimeiraHoraMinimaEHoraAdicionalProporcional(
        int minutos,
        decimal primeiraHora,
        decimal horaAdicional,
        decimal esperado)
    {
        var valor = CobrancaCalculadora.Calcular(
            TimeSpan.FromMinutes(minutos),
            primeiraHora,
            horaAdicional);

        Assert.Equal(esperado, valor);
    }

    [Fact]
    public void Calcular_ExemploNegocio_DuasHorasEMeia_DeveRetornar24()
    {
        var valor = CobrancaCalculadora.Calcular(
            TimeSpan.FromHours(2.5),
            valorPrimeiraHora: 12m,
            valorHoraAdicional: 8m);

        Assert.Equal(24m, valor);
    }
}
