using Estacionamento.Models;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public interface IUsuarioRepository
    {
        void Adicionar(Usuario usuario);
        IReadOnlyList<Usuario> Listar();
        Usuario? ObterPorId(int id);
        Usuario? ObterPorEmail(string email);
        Usuario? ObterPorLogin(string login);
        void Atualizar(Usuario usuario);
        void AtualizarCredenciais(int id, string senhaHash, string senhaSalt);
        void Excluir(int id);
        bool ExisteAdmin();
        int ContarAdmins();
    }
}
