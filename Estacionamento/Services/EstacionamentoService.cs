using Estacionamento.Abstractions;
using Estacionamento.Models;
using Estacionamento.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Estacionamento.Services
{
    public class EstacionamentoService
    {
        private readonly IVeiculoRepository _repository;
        private readonly IClock _clock;

        public EstacionamentoService(IVeiculoRepository repository, IClock clock)
        {
            _repository = repository;
            _clock = clock;
        }

        public void RegistrarEntrada(string placa, TipoVeiculo tipo, decimal valorHora)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa nao pode ser vazia.");
            }

            if (valorHora <= 0)
            {
                throw new ArgumentException("O valor por hora deve ser positivo.");
            }

            var existenteAtivo = _repository.ObterAtivoPorPlaca(placa);
            if (existenteAtivo != null)
            {
                throw new InvalidOperationException("Ja existe um registro ativo com esta placa. Finalize ou cancele antes de registrar nova entrada.");
            }

            var novoVeiculo = new Veiculo
            {
                Placa = placa.ToUpperInvariant(),
                Tipo = tipo,
                Entrada = _clock.UtcNow,
                ValorHora = valorHora
            };

            _repository.Adicionar(novoVeiculo);
        }

        public Veiculo SimularSaida(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa nao pode ser vazia.");
            }

            var ativo = _repository.ObterAtivoPorPlaca(placa);
            if (ativo == null)
            {
                throw new InvalidOperationException("Veiculo nao encontrado ou ja foi registrada a saida.");
            }

            return new Veiculo
            {
                Id = ativo.Id,
                Placa = ativo.Placa,
                Tipo = ativo.Tipo,
                Entrada = ativo.Entrada,
                ValorHora = ativo.ValorHora,
                Saida = _clock.UtcNow
            };
        }

        public Veiculo ConfirmarSaida(int veiculoId, DateTime saidaUtc)
        {
            _repository.FinalizarSaida(veiculoId, saidaUtc);
            var atualizado = _repository.ObterPorId(veiculoId);
            if (atualizado == null || atualizado.Saida == null)
            {
                throw new InvalidOperationException("Nao foi possivel confirmar a saida.");
            }

            return atualizado;
        }

        public void AlterarDadosVeiculo(string placa, TipoVeiculo novoTipo, decimal novoValorHora)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa nao pode ser vazia.");
            }

            if (novoValorHora <= 0)
            {
                throw new ArgumentException("O valor por hora deve ser positivo.");
            }

            var veiculo = _repository.ObterAtivoPorPlaca(placa);
            if (veiculo == null)
            {
                throw new InvalidOperationException("Veiculo nao encontrado ou ja finalizado.");
            }

            _repository.AtualizarDados(placa, novoTipo, novoValorHora);
        }

        public void CancelarVeiculoAtivo(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa nao pode ser vazia.");
            }

            var veiculo = _repository.ObterAtivoPorPlaca(placa);
            if (veiculo == null)
            {
                throw new InvalidOperationException("Veiculo nao encontrado ou ja finalizado.");
            }

            _repository.RemoverAtivoPorPlaca(placa);
        }

        public List<Veiculo> ListarVeiculosEstacionados()
        {
            return _repository.ObterTodos();
        }

        public List<Veiculo> ListarTodosVeiculos()
        {
            return _repository.ObterTodos();
        }

        public List<Veiculo> ListarVeiculosAtivos()
        {
            return _repository.ObterTodos().Where(v => v.Saida == null).ToList();
        }

        public decimal ObterFaturamentoTotal()
        {
            return _repository.ObterTodos()
                .Where(v => v.Saida != null)
                .Sum(v => v.CalcularValor());
        }

        public void ExcluirVeiculoFinalizado(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
            {
                throw new ArgumentException("A placa nao pode ser vazia.");
            }

            var finalizado = _repository.ObterTodos()
                .FirstOrDefault(v => v.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase) && v.Saida != null);

            if (finalizado == null)
            {
                throw new InvalidOperationException("Registro finalizado nao encontrado.");
            }

            _repository.RemoverFinalizadoPorPlaca(placa);
        }
    }
}
