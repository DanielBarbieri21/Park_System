using Estacionamento.Models;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public interface IUsuarioRepository
    {
        void Adicionar(Usuario usuario);
        Usuario? ObterPorEmail(string email);
        Usuario? ObterPorLogin(string login);
        void AtualizarCredenciais(int id, string senhaHash, string senhaSalt);
        bool ExisteAdmin();
    }
}
