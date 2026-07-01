using System;

namespace Estacionamento.Models
{
    public class Mensalista
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string DependenteDe { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Chave { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string TabelaPrecos { get; set; } = string.Empty;
        public int Vagas { get; set; } = 1;
        public decimal Valor { get; set; }
        public int DiaVencimento { get; set; } = 1;
        public int Recorrencias { get; set; } = 1;
        public string RegraTempoExcedente { get; set; } = string.Empty;
        public int TempoExcedente { get; set; }
        public bool ControlaVagaPorCartao { get; set; }
        public string ValidadeLiberacao { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
