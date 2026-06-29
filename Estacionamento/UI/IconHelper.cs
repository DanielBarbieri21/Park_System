using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Estacionamento.UI
{
    /// <summary>
    /// Renderiza ícones Segoe MDL2 Assets (Fluent Design) para uso nos botões WinForms.
    /// </summary>
    internal static class IconHelper
    {
        private const string IconFontName = "Segoe MDL2 Assets";

        // Glifos oficiais do Segoe MDL2 Assets
        private const string GlyphEntrada = "\uECE4";       // Car
        private const string GlyphSaida = "\uE8BB";         // Leave
        private const string GlyphAlterar = "\uE70F";       // Edit
        private const string GlyphCancelar = "\uE711";      // Cancel
        private const string GlyphRelatorio = "\uE8A5";     // ReportDocument
        private const string GlyphExcluir = "\uE74D";       // Delete

        private static readonly Lazy<Image> EntradaIcon = new(() => CreateIcon(GlyphEntrada, 22, Color.White));
        private static readonly Lazy<Image> SaidaIcon = new(() => CreateIcon(GlyphSaida, 22, Color.White));
        private static readonly Lazy<Image> AlterarIcon = new(() => CreateIcon(GlyphAlterar, 22, Color.FromArgb(52, 73, 94)));
        private static readonly Lazy<Image> CancelarIcon = new(() => CreateIcon(GlyphCancelar, 22, Color.White));
        private static readonly Lazy<Image> RelatorioIcon = new(() => CreateIcon(GlyphRelatorio, 34, Color.White));
        private static readonly Lazy<Image> ExcluirIcon = new(() => CreateIcon(GlyphExcluir, 22, Color.FromArgb(52, 73, 94)));

        public static Image Entrada => EntradaIcon.Value;
        public static Image Saida => SaidaIcon.Value;
        public static Image Alterar => AlterarIcon.Value;
        public static Image Cancelar => CancelarIcon.Value;
        public static Image Relatorio => RelatorioIcon.Value;
        public static Image Excluir => ExcluirIcon.Value;

        private static Bitmap CreateIcon(string glyph, int size, Color color)
        {
            var bitmap = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(Color.Transparent);

            var fontSize = size * 0.72f;
            using var font = new Font(IconFontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            using var brush = new SolidBrush(color);
            using var format = new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.NoClip
            };

            graphics.DrawString(glyph, font, brush, new RectangleF(0, 0, size, size), format);
            return bitmap;
        }
    }
}
