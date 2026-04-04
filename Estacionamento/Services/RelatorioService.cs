using Estacionamento.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Estacionamento.Services
{
    public sealed class RelatorioService
    {
        public void GerarPdf(IReadOnlyCollection<Veiculo> veiculos, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, fs);
            doc.Open();

            var titulo = new iTextSharp.text.Paragraph("Relatorio de Estacionamento", new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 18, iTextSharp.text.Font.BOLD));
            titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            doc.Add(titulo);
            doc.Add(new iTextSharp.text.Paragraph($"Data: {DateTime.Now:dd/MM/yyyy HH:mm}", new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 12)));
            doc.Add(new iTextSharp.text.Paragraph("\n"));

            var tabela = new iTextSharp.text.pdf.PdfPTable(6) { WidthPercentage = 100 };
            tabela.AddCell("Placa");
            tabela.AddCell("Tipo");
            tabela.AddCell("Entrada");
            tabela.AddCell("Saida");
            tabela.AddCell("Valor/Hora");
            tabela.AddCell("Valor Total");

            decimal faturamento = 0;
            foreach (var v in veiculos)
            {
                var entradaLocal = v.Entrada.ToLocalTime();
                var saidaLocal = v.Saida?.ToLocalTime();
                tabela.AddCell(v.Placa);
                tabela.AddCell(v.Tipo.ToString());
                tabela.AddCell(entradaLocal.ToString("dd/MM/yyyy HH:mm"));
                tabela.AddCell(saidaLocal?.ToString("dd/MM/yyyy HH:mm") ?? "-");
                tabela.AddCell($"R$ {v.ValorHora:F2}");
                var valor = v.Saida != null ? v.CalcularValor() : 0;
                tabela.AddCell($"R$ {valor:F2}");
                faturamento += valor;
            }

            doc.Add(tabela);
            doc.Add(new iTextSharp.text.Paragraph("\n"));
            doc.Add(new iTextSharp.text.Paragraph($"Faturamento total: R$ {faturamento:F2}", new iTextSharp.text.Font(iTextSharp.text.Font.HELVETICA, 14, iTextSharp.text.Font.BOLD)));

            doc.Close();
            writer.Close();
        }

        public void GerarCsv(IReadOnlyCollection<Veiculo> veiculos, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Placa;Tipo;Entrada;Saida;ValorHora;ValorTotal");

            foreach (var v in veiculos.OrderBy(v => v.Entrada))
            {
                var entrada = v.Entrada.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var saida = v.Saida?.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) ?? "";
                var total = v.Saida != null ? v.CalcularValor() : 0;
                sb.AppendLine($"{v.Placa};{v.Tipo};{entrada};{saida};{v.ValorHora:F2};{total:F2}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
