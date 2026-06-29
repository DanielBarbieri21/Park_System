using System.Drawing;
using System.Windows.Forms;

namespace Estacionamento.UI
{
    internal static class ButtonStyleHelper
    {
        public static void AplicarBotaoAcao(Button botao, string texto, Color corFundo, Image icone, Color? corTexto = null)
        {
            botao.Text = texto;
            botao.BackColor = corFundo;
            botao.ForeColor = corTexto ?? Color.White;
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = ControlPaint.Light(corFundo, 0.08f);
            botao.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(corFundo, 0.08f);
            botao.Image = icone;
            botao.ImageAlign = ContentAlignment.MiddleLeft;
            botao.TextAlign = ContentAlignment.MiddleCenter;
            botao.TextImageRelation = TextImageRelation.ImageBeforeText;
            botao.Padding = new Padding(10, 0, 10, 0);
            botao.Margin = new Padding(0, 3, 0, 3);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }

        public static void AlinharColunaBotoes(params Button[] botoes)
        {
            if (botoes.Length == 0)
            {
                return;
            }

            var largura = botoes.Max(b => b.Width);
            foreach (var botao in botoes)
            {
                botao.Width = largura;
                botao.Height = 34;
                botao.ImageAlign = ContentAlignment.MiddleLeft;
                botao.TextAlign = ContentAlignment.MiddleCenter;
                botao.Padding = new Padding(10, 0, 10, 0);
            }
        }

        public static void AplicarBotaoRelatorio(Button botao)
        {
            botao.Text = "Gerar Relatório (PDF)";
            botao.BackColor = Color.FromArgb(52, 152, 219);
            botao.ForeColor = Color.White;
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185);
            botao.FlatAppearance.MouseDownBackColor = Color.FromArgb(36, 113, 163);
            botao.Image = IconHelper.Relatorio;
            botao.ImageAlign = ContentAlignment.TopCenter;
            botao.TextAlign = ContentAlignment.BottomCenter;
            botao.TextImageRelation = TextImageRelation.ImageAboveText;
            botao.Padding = new Padding(4, 8, 4, 6);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }

        public static void AplicarBotaoSecundario(Button botao, string texto, Image icone)
        {
            botao.Text = $"  {texto}";
            botao.BackColor = Color.FromArgb(236, 240, 241);
            botao.ForeColor = Color.FromArgb(52, 73, 94);
            botao.FlatStyle = FlatStyle.Flat;
            botao.FlatAppearance.BorderSize = 0;
            botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 227, 230);
            botao.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 210, 215);
            botao.Image = icone;
            botao.ImageAlign = ContentAlignment.MiddleLeft;
            botao.TextImageRelation = TextImageRelation.ImageBeforeText;
            botao.Padding = new Padding(10, 0, 0, 0);
            botao.Cursor = Cursors.Hand;
            botao.UseVisualStyleBackColor = false;
        }
    }
}
