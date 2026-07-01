using Estacionamento.Models;
using System;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public interface IVeiculoRepository
    {
        void Adicionar(Veiculo veiculo);
        void Atualizar(Veiculo veiculo);
        void AtualizarDados(string placa, TipoVeiculo tipo, decimal valorHora, decimal valorHoraAdicional);
        void RemoverAtivoPorPlaca(string placa);
        void RemoverFinalizadoPorPlaca(string placa);
        Veiculo? ObterAtivoPorPlaca(string placa);
        Veiculo? ObterPorId(int id);
        List<Veiculo> ObterTodos();
        void FinalizarSaida(int id, DateTime saidaUtc);
    }
}
