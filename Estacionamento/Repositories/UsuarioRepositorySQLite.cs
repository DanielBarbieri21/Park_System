using Estacionamento.Models;
using Estacionamento.Infrastructure;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public class UsuarioRepositorySQLite : IUsuarioRepository
    {
        private static string ConnectionString => $"Data Source={AppPaths.GetDataFilePath("estacionamento.db")}";

        public UsuarioRepositorySQLite()
        {
            CriarTabelaSeNaoExistir();
            MigrarEsquemaSeNecessario();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Login TEXT NOT NULL,
                    Senha TEXT,
                    SenhaHash TEXT,
                    SenhaSalt TEXT,
                    Tipo INTEGER NOT NULL
                );
                CREATE UNIQUE INDEX IF NOT EXISTS UX_Usuarios_Email ON Usuarios(Email);
                CREATE UNIQUE INDEX IF NOT EXISTS UX_Usuarios_Login ON Usuarios(Login);";
            cmd.ExecuteNonQuery();
        }

        private void MigrarEsquemaSeNecessario()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            if (!ColunaExiste(conn, "Usuarios", "SenhaHash"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Usuarios ADD COLUMN SenhaHash TEXT;";
                cmd.ExecuteNonQuery();
            }

            if (!ColunaExiste(conn, "Usuarios", "SenhaSalt"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Usuarios ADD COLUMN SenhaSalt TEXT;";
                cmd.ExecuteNonQuery();
            }

            if (!ColunaExiste(conn, "Usuarios", "Login"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Usuarios ADD COLUMN Login TEXT;";
                cmd.ExecuteNonQuery();

                using var fillCmd = conn.CreateCommand();
                fillCmd.CommandText = "UPDATE Usuarios SET Login = Email WHERE Login IS NULL OR Login = '';";
                fillCmd.ExecuteNonQuery();
            }
        }

        private static bool ColunaExiste(SqliteConnection conn, string tabela, string coluna)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info({tabela});";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), coluna, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public void Adicionar(Usuario usuario)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Usuarios (Nome, Email, Login, Senha, SenhaHash, SenhaSalt, Tipo)
                VALUES ($nome, $email, $login, $senha, $senhaHash, $senhaSalt, $tipo)";
            cmd.Parameters.AddWithValue("$nome", usuario.Nome);
            cmd.Parameters.AddWithValue("$email", usuario.Email);
            cmd.Parameters.AddWithValue("$login", usuario.Login);
            cmd.Parameters.AddWithValue("$senha", string.IsNullOrWhiteSpace(usuario.Senha) ? DBNull.Value : usuario.Senha);
            cmd.Parameters.AddWithValue("$senhaHash", string.IsNullOrWhiteSpace(usuario.SenhaHash) ? DBNull.Value : usuario.SenhaHash);
            cmd.Parameters.AddWithValue("$senhaSalt", string.IsNullOrWhiteSpace(usuario.SenhaSalt) ? DBNull.Value : usuario.SenhaSalt);
            cmd.Parameters.AddWithValue("$tipo", (int)usuario.Tipo);
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyList<Usuario> Listar()
        {
            var usuarios = new List<Usuario>();
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Nome, Email, Login, Senha, SenhaHash, SenhaSalt, Tipo
                FROM Usuarios
                ORDER BY Nome COLLATE NOCASE, Login COLLATE NOCASE";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(LerUsuario(reader));
            }

            return usuarios;
        }

        public Usuario? ObterPorId(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Nome, Email, Login, Senha, SenhaHash, SenhaSalt, Tipo
                FROM Usuarios
                WHERE Id = $id
                LIMIT 1";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? LerUsuario(reader) : null;
        }

        public Usuario? ObterPorEmail(string email)
        {
            return ObterPorCampo("Email", email);
        }

        public Usuario? ObterPorLogin(string login)
        {
            return ObterPorCampo("Login", login);
        }

        public void Atualizar(Usuario usuario)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Usuarios
                SET Nome = $nome,
                    Email = $email,
                    Login = $login,
                    Tipo = $tipo
                WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", usuario.Id);
            cmd.Parameters.AddWithValue("$nome", usuario.Nome);
            cmd.Parameters.AddWithValue("$email", usuario.Email);
            cmd.Parameters.AddWithValue("$login", usuario.Login);
            cmd.Parameters.AddWithValue("$tipo", (int)usuario.Tipo);
            cmd.ExecuteNonQuery();
        }

        public void AtualizarCredenciais(int id, string senhaHash, string senhaSalt)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Usuarios
                SET SenhaHash = $senhaHash,
                    SenhaSalt = $senhaSalt,
                    Senha = NULL
                WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$senhaHash", senhaHash);
            cmd.Parameters.AddWithValue("$senhaSalt", senhaSalt);
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Usuarios WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        public bool ExisteAdmin()
        {
            return ContarAdmins() > 0;
        }

        public int ContarAdmins()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Usuarios WHERE Tipo = $tipo";
            cmd.Parameters.AddWithValue("$tipo", (int)TipoUsuario.Admin);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private Usuario? ObterPorCampo(string campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                SELECT Id, Nome, Email, Login, Senha, SenhaHash, SenhaSalt, Tipo
                FROM Usuarios
                WHERE {campo} = $valor
                LIMIT 1";
            cmd.Parameters.AddWithValue("$valor", valor);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? LerUsuario(reader) : null;
        }

        private static Usuario LerUsuario(SqliteDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Email = reader.GetString(2),
                Login = reader.IsDBNull(3) ? reader.GetString(2) : reader.GetString(3),
                Senha = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                SenhaHash = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                SenhaSalt = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                Tipo = (TipoUsuario)reader.GetInt32(7)
            };
        }
    }
}
