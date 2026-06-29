using Estacionamento.Models;
using Estacionamento.Infrastructure;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Estacionamento.Repositories
{
    public class VeiculoRepositorySQLite : IVeiculoRepository
    {
        private static string ConnectionString => $"Data Source={AppPaths.GetDataFilePath("estacionamento.db")}";

        public VeiculoRepositorySQLite()
        {
            CriarTabelaSeNaoExistir();
            MigrarEsquemaSeNecessario();
            CriarIndices();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Veiculos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Placa TEXT NOT NULL,
                    Tipo INTEGER NOT NULL,
                    Entrada TEXT NOT NULL,
                    Saida TEXT,
                    ValorHora REAL NOT NULL,
                    CreatedAtUtc TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
                    UpdatedAtUtc TEXT
                );";
            cmd.ExecuteNonQuery();
        }

        private void MigrarEsquemaSeNecessario()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            if (!ColunaExiste(conn, "Veiculos", "Id"))
            {
                using var tx = conn.BeginTransaction();
                var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Veiculos_new (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Placa TEXT NOT NULL,
    Tipo INTEGER NOT NULL,
    Entrada TEXT NOT NULL,
    Saida TEXT,
    ValorHora REAL NOT NULL,
    CreatedAtUtc TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    UpdatedAtUtc TEXT
);
INSERT INTO Veiculos_new (Placa, Tipo, Entrada, Saida, ValorHora)
    SELECT Placa, Tipo, Entrada, Saida, ValorHora FROM Veiculos;
DROP TABLE Veiculos;
ALTER TABLE Veiculos_new RENAME TO Veiculos;";
                cmd.ExecuteNonQuery();
                tx.Commit();
            }

            if (!ColunaExiste(conn, "Veiculos", "CreatedAtUtc"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Veiculos ADD COLUMN CreatedAtUtc TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP);";
                cmd.ExecuteNonQuery();
            }

            if (!ColunaExiste(conn, "Veiculos", "UpdatedAtUtc"))
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "ALTER TABLE Veiculos ADD COLUMN UpdatedAtUtc TEXT;";
                cmd.ExecuteNonQuery();
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

        private static void CriarIndices()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE UNIQUE INDEX IF NOT EXISTS UX_Veiculos_Placa_Ativa
                ON Veiculos(Placa)
                WHERE Saida IS NULL OR Saida = '';

                CREATE INDEX IF NOT EXISTS IX_Veiculos_Placa ON Veiculos(Placa);
                CREATE INDEX IF NOT EXISTS IX_Veiculos_Saida ON Veiculos(Saida);
                CREATE INDEX IF NOT EXISTS IX_Veiculos_Entrada ON Veiculos(Entrada);";
            cmd.ExecuteNonQuery();
        }

        public void Adicionar(Veiculo veiculo)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Veiculos (Placa, Tipo, Entrada, Saida, ValorHora, UpdatedAtUtc)
                VALUES ($placa, $tipo, $entrada, $saida, $valorHora, $updatedAtUtc);
                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$placa", veiculo.Placa);
            cmd.Parameters.AddWithValue("$tipo", (int)veiculo.Tipo);
            cmd.Parameters.AddWithValue("$entrada", ToIso(veiculo.Entrada));
            cmd.Parameters.AddWithValue("$saida", veiculo.Saida == null ? DBNull.Value : ToIso(veiculo.Saida.Value));
            cmd.Parameters.AddWithValue("$valorHora", veiculo.ValorHora);
            cmd.Parameters.AddWithValue("$updatedAtUtc", ToIso(DateTime.UtcNow));

            var newId = Convert.ToInt32(cmd.ExecuteScalar());
            veiculo.Id = newId;
        }

        public void Atualizar(Veiculo veiculo)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Veiculos
                SET Saida = $saida,
                    Tipo = $tipo,
                    ValorHora = $valorHora,
                    UpdatedAtUtc = $updatedAtUtc
                WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", veiculo.Id);
            cmd.Parameters.AddWithValue("$tipo", (int)veiculo.Tipo);
            cmd.Parameters.AddWithValue("$valorHora", veiculo.ValorHora);
            cmd.Parameters.AddWithValue("$saida", veiculo.Saida == null ? DBNull.Value : ToIso(veiculo.Saida.Value));
            cmd.Parameters.AddWithValue("$updatedAtUtc", ToIso(DateTime.UtcNow));
            cmd.ExecuteNonQuery();
        }

        public void FinalizarSaida(int id, DateTime saidaUtc)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Veiculos
                SET Saida = $saida,
                    UpdatedAtUtc = $updatedAtUtc
                WHERE Id = $id AND (Saida IS NULL OR Saida = '')";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$saida", ToIso(saidaUtc));
            cmd.Parameters.AddWithValue("$updatedAtUtc", ToIso(DateTime.UtcNow));
            cmd.ExecuteNonQuery();
        }

        public void AtualizarDados(string placa, TipoVeiculo tipo, decimal valorHora)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Veiculos
                SET Tipo = $tipo,
                    ValorHora = $valorHora,
                    UpdatedAtUtc = $updatedAtUtc
                WHERE UPPER(Placa) = UPPER($placa)
                  AND (Saida IS NULL OR Saida = '')";
            cmd.Parameters.AddWithValue("$placa", placa);
            cmd.Parameters.AddWithValue("$tipo", (int)tipo);
            cmd.Parameters.AddWithValue("$valorHora", valorHora);
            cmd.Parameters.AddWithValue("$updatedAtUtc", ToIso(DateTime.UtcNow));
            cmd.ExecuteNonQuery();
        }

        public void RemoverAtivoPorPlaca(string placa)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Veiculos WHERE UPPER(Placa) = UPPER($placa) AND (Saida IS NULL OR Saida = '')";
            cmd.Parameters.AddWithValue("$placa", placa);
            cmd.ExecuteNonQuery();
        }

        public void RemoverFinalizadoPorPlaca(string placa)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Veiculos WHERE UPPER(Placa) = UPPER($placa) AND Saida IS NOT NULL AND Saida <> ''";
            cmd.Parameters.AddWithValue("$placa", placa);
            cmd.ExecuteNonQuery();
        }

        public Veiculo? ObterAtivoPorPlaca(string placa)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Placa, Tipo, Entrada, Saida, ValorHora
                FROM Veiculos
                WHERE UPPER(Placa) = UPPER($placa)
                  AND (Saida IS NULL OR Saida = '')
                ORDER BY Entrada DESC
                LIMIT 1";
            cmd.Parameters.AddWithValue("$placa", placa);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public Veiculo? ObterPorId(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Placa, Tipo, Entrada, Saida, ValorHora
                FROM Veiculos
                WHERE Id = $id
                LIMIT 1";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public List<Veiculo> ObterTodos()
        {
            var lista = new List<Veiculo>();
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id, Placa, Tipo, Entrada, Saida, ValorHora
                FROM Veiculos
                ORDER BY Entrada DESC";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(Map(reader));
            }

            return lista;
        }

        private static Veiculo Map(SqliteDataReader reader)
        {
            return new Veiculo
            {
                Id = reader.GetInt32(0),
                Placa = reader.GetString(1),
                Tipo = (TipoVeiculo)reader.GetInt32(2),
                Entrada = ParseIso(reader.GetString(3)),
                Saida = reader.IsDBNull(4) || string.IsNullOrWhiteSpace(reader.GetString(4)) ? null : ParseIso(reader.GetString(4)),
                ValorHora = reader.GetDecimal(5)
            };
        }

        private static string ToIso(DateTime value)
        {
            var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
            return utc.ToString("O", CultureInfo.InvariantCulture);
        }

        private static DateTime ParseIso(string raw)
        {
            var parsed = DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return parsed.Kind == DateTimeKind.Utc ? parsed : parsed.ToUniversalTime();
        }
    }
}
