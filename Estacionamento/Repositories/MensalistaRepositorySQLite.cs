using Estacionamento.Models;
using Estacionamento.Infrastructure;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Estacionamento.Repositories
{
    public class MensalistaRepositorySQLite : IMensalistaRepository
    {
        private static string ConnectionString => $"Data Source={AppPaths.GetDataFilePath("estacionamento.db")}";

        public MensalistaRepositorySQLite()
        {
            CriarTabelaSeNaoExistir();
        }

        private void CriarTabelaSeNaoExistir()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Mensalistas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    DependenteDe TEXT,
                    CpfCnpj TEXT,
                    Matricula TEXT,
                    Chave TEXT,
                    Grupo TEXT,
                    TabelaPrecos TEXT,
                    Vagas INTEGER NOT NULL DEFAULT 1,
                    Valor REAL NOT NULL DEFAULT 0.0,
                    DiaVencimento INTEGER NOT NULL DEFAULT 1,
                    Recorrencias INTEGER NOT NULL DEFAULT 1,
                    RegraTempoExcedente TEXT,
                    TempoExcedente INTEGER NOT NULL DEFAULT 0,
                    ControlaVagaPorCartao INTEGER NOT NULL DEFAULT 0,
                    ValidadeLiberacao TEXT,
                    Ativo INTEGER NOT NULL DEFAULT 1
                );";
            cmd.ExecuteNonQuery();
        }

        public void Adicionar(Mensalista m)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO Mensalistas (
                    Nome, DependenteDe, CpfCnpj, Matricula, Chave, Grupo, TabelaPrecos,
                    Vagas, Valor, DiaVencimento, Recorrencias, RegraTempoExcedente,
                    TempoExcedente, ControlaVagaPorCartao, ValidadeLiberacao, Ativo
                ) VALUES (
                    $nome, $dependenteDe, $cpfCnpj, $matricula, $chave, $grupo, $tabelaPrecos,
                    $vagas, $valor, $diaVencimento, $recorrencias, $regraTempoExcedente,
                    $tempoExcedente, $controlaVagaPorCartao, $validadeLiberacao, $ativo
                )";
            cmd.Parameters.AddWithValue("$nome", m.Nome ?? string.Empty);
            cmd.Parameters.AddWithValue("$dependenteDe", m.DependenteDe ?? string.Empty);
            cmd.Parameters.AddWithValue("$cpfCnpj", m.CpfCnpj ?? string.Empty);
            cmd.Parameters.AddWithValue("$matricula", m.Matricula ?? string.Empty);
            cmd.Parameters.AddWithValue("$chave", m.Chave ?? string.Empty);
            cmd.Parameters.AddWithValue("$grupo", m.Grupo ?? string.Empty);
            cmd.Parameters.AddWithValue("$tabelaPrecos", m.TabelaPrecos ?? string.Empty);
            cmd.Parameters.AddWithValue("$vagas", m.Vagas);
            cmd.Parameters.AddWithValue("$valor", (double)m.Valor);
            cmd.Parameters.AddWithValue("$diaVencimento", m.DiaVencimento);
            cmd.Parameters.AddWithValue("$recorrencias", m.Recorrencias);
            cmd.Parameters.AddWithValue("$regraTempoExcedente", m.RegraTempoExcedente ?? string.Empty);
            cmd.Parameters.AddWithValue("$tempoExcedente", m.TempoExcedente);
            cmd.Parameters.AddWithValue("$controlaVagaPorCartao", m.ControlaVagaPorCartao ? 1 : 0);
            cmd.Parameters.AddWithValue("$validadeLiberacao", m.ValidadeLiberacao ?? string.Empty);
            cmd.Parameters.AddWithValue("$ativo", m.Ativo ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyList<Mensalista> Listar()
        {
            var list = new List<Mensalista>();
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Mensalistas ORDER BY Nome COLLATE NOCASE";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(LerMensalista(reader));
            }
            return list;
        }

        public Mensalista? ObterPorId(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Mensalistas WHERE Id = $id LIMIT 1";
            cmd.Parameters.AddWithValue("$id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return LerMensalista(reader);
            }
            return null;
        }

        public void Atualizar(Mensalista m)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE Mensalistas
                SET Nome = $nome,
                    DependenteDe = $dependenteDe,
                    CpfCnpj = $cpfCnpj,
                    Matricula = $matricula,
                    Chave = $chave,
                    Grupo = $grupo,
                    TabelaPrecos = $tabelaPrecos,
                    Vagas = $vagas,
                    Valor = $valor,
                    DiaVencimento = $diaVencimento,
                    Recorrencias = $recorrencias,
                    RegraTempoExcedente = $regraTempoExcedente,
                    TempoExcedente = $tempoExcedente,
                    ControlaVagaPorCartao = $controlaVagaPorCartao,
                    ValidadeLiberacao = $validadeLiberacao,
                    Ativo = $ativo
                WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", m.Id);
            cmd.Parameters.AddWithValue("$nome", m.Nome ?? string.Empty);
            cmd.Parameters.AddWithValue("$dependenteDe", m.DependenteDe ?? string.Empty);
            cmd.Parameters.AddWithValue("$cpfCnpj", m.CpfCnpj ?? string.Empty);
            cmd.Parameters.AddWithValue("$matricula", m.Matricula ?? string.Empty);
            cmd.Parameters.AddWithValue("$chave", m.Chave ?? string.Empty);
            cmd.Parameters.AddWithValue("$grupo", m.Grupo ?? string.Empty);
            cmd.Parameters.AddWithValue("$tabelaPrecos", m.TabelaPrecos ?? string.Empty);
            cmd.Parameters.AddWithValue("$vagas", m.Vagas);
            cmd.Parameters.AddWithValue("$valor", (double)m.Valor);
            cmd.Parameters.AddWithValue("$diaVencimento", m.DiaVencimento);
            cmd.Parameters.AddWithValue("$recorrencias", m.Recorrencias);
            cmd.Parameters.AddWithValue("$regraTempoExcedente", m.RegraTempoExcedente ?? string.Empty);
            cmd.Parameters.AddWithValue("$tempoExcedente", m.TempoExcedente);
            cmd.Parameters.AddWithValue("$controlaVagaPorCartao", m.ControlaVagaPorCartao ? 1 : 0);
            cmd.Parameters.AddWithValue("$validadeLiberacao", m.ValidadeLiberacao ?? string.Empty);
            cmd.Parameters.AddWithValue("$ativo", m.Ativo ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Mensalistas WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        private static Mensalista LerMensalista(SqliteDataReader r)
        {
            return new Mensalista
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Nome = r.GetString(r.GetOrdinal("Nome")),
                DependenteDe = r.IsDBNull(r.GetOrdinal("DependenteDe")) ? string.Empty : r.GetString(r.GetOrdinal("DependenteDe")),
                CpfCnpj = r.IsDBNull(r.GetOrdinal("CpfCnpj")) ? string.Empty : r.GetString(r.GetOrdinal("CpfCnpj")),
                Matricula = r.IsDBNull(r.GetOrdinal("Matricula")) ? string.Empty : r.GetString(r.GetOrdinal("Matricula")),
                Chave = r.IsDBNull(r.GetOrdinal("Chave")) ? string.Empty : r.GetString(r.GetOrdinal("Chave")),
                Grupo = r.IsDBNull(r.GetOrdinal("Grupo")) ? string.Empty : r.GetString(r.GetOrdinal("Grupo")),
                TabelaPrecos = r.IsDBNull(r.GetOrdinal("TabelaPrecos")) ? string.Empty : r.GetString(r.GetOrdinal("TabelaPrecos")),
                Vagas = r.GetInt32(r.GetOrdinal("Vagas")),
                Valor = (decimal)r.GetDouble(r.GetOrdinal("Valor")),
                DiaVencimento = r.GetInt32(r.GetOrdinal("DiaVencimento")),
                Recorrencias = r.GetInt32(r.GetOrdinal("Recorrencias")),
                RegraTempoExcedente = r.IsDBNull(r.GetOrdinal("RegraTempoExcedente")) ? string.Empty : r.GetString(r.GetOrdinal("RegraTempoExcedente")),
                TempoExcedente = r.GetInt32(r.GetOrdinal("TempoExcedente")),
                ControlaVagaPorCartao = r.GetInt32(r.GetOrdinal("ControlaVagaPorCartao")) != 0,
                ValidadeLiberacao = r.IsDBNull(r.GetOrdinal("ValidadeLiberacao")) ? string.Empty : r.GetString(r.GetOrdinal("ValidadeLiberacao")),
                Ativo = r.GetInt32(r.GetOrdinal("Ativo")) != 0
            };
        }
    }
}
