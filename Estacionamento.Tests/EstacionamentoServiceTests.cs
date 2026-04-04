using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Repositories;
using Estacionamento.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Estacionamento.Tests;

public class EstacionamentoServiceTests
{
    [Fact]
    public void RegistrarEntrada_DeveCriarVeiculoAtivo()
    {
        var repo = new InMemoryVeiculoRepository();
        var service = new EstacionamentoService(repo, new FixedClock(new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Utc)));

        service.RegistrarEntrada("ABC1234", TipoVeiculo.Carro, 10m);

        var ativo = repo.ObterAtivoPorPlaca("ABC1234");
        Assert.NotNull(ativo);
        Assert.Equal(TipoVeiculo.Carro, ativo!.Tipo);
        Assert.Equal(10m, ativo.ValorHora);
    }

    [Fact]
    public void ConfirmarSaida_DeveDefinirSaida()
    {
        var repo = new InMemoryVeiculoRepository();
        var clock = new FixedClock(new DateTime(2026, 4, 4, 10, 0, 0, DateTimeKind.Utc));
        var service = new EstacionamentoService(repo, clock);
        service.RegistrarEntrada("DEF1G23", TipoVeiculo.Moto, 5m);

        clock.UtcNowValue = new DateTime(2026, 4, 4, 11, 10, 0, DateTimeKind.Utc);
        var preview = service.SimularSaida("DEF1G23");
        var finalizado = service.ConfirmarSaida(preview.Id, preview.Saida!.Value);

        Assert.NotNull(finalizado.Saida);
        Assert.True(finalizado.CalcularValor() >= 10m);
    }

    private sealed class FixedClock : IClock
    {
        public FixedClock(DateTime utcNow) => UtcNowValue = utcNow;
        public DateTime UtcNowValue { get; set; }
        public DateTime UtcNow => UtcNowValue;
    }

    private sealed class InMemoryVeiculoRepository : IVeiculoRepository
    {
        private readonly List<Veiculo> _items = new();
        private int _nextId = 1;

        public void Adicionar(Veiculo veiculo)
        {
            veiculo.Id = _nextId++;
            _items.Add(veiculo);
        }

        public void Atualizar(Veiculo veiculo)
        {
            var idx = _items.FindIndex(v => v.Id == veiculo.Id);
            if (idx >= 0) _items[idx] = veiculo;
        }

        public void AtualizarDados(string placa, TipoVeiculo tipo, decimal valorHora)
        {
            var ativo = ObterAtivoPorPlaca(placa);
            if (ativo == null) return;
            ativo.Tipo = tipo;
            ativo.ValorHora = valorHora;
        }

        public void RemoverAtivoPorPlaca(string placa)
            => _items.RemoveAll(v => v.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase) && v.Saida == null);

        public void RemoverFinalizadoPorPlaca(string placa)
            => _items.RemoveAll(v => v.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase) && v.Saida != null);

        public Veiculo? ObterAtivoPorPlaca(string placa)
            => _items.LastOrDefault(v => v.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase) && v.Saida == null);

        public Veiculo? ObterPorId(int id) => _items.FirstOrDefault(v => v.Id == id);

        public List<Veiculo> ObterTodos() => _items.ToList();

        public void FinalizarSaida(int id, DateTime saidaUtc)
        {
            var item = ObterPorId(id);
            if (item != null && item.Saida == null)
            {
                item.Saida = saidaUtc;
            }
        }
    }
}
