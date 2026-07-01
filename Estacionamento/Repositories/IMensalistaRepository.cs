using Estacionamento.Models;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public interface IMensalistaRepository
    {
        void Adicionar(Mensalista mensalista);
        IReadOnlyList<Mensalista> Listar();
        Mensalista? ObterPorId(int id);
        void Atualizar(Mensalista mensalista);
        void Excluir(int id);
    }
}
