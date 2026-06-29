using System.Drawing;

namespace Estacionamento.UI
{
    internal static class AppBranding
    {
        public static readonly Color Primary = Color.FromArgb(41, 128, 185);
        public static readonly Color PrimaryDark = Color.FromArgb(31, 97, 141);
        public static readonly Color Background = Color.FromArgb(248, 249, 250);
        public static readonly Color TextPrimary = Color.FromArgb(52, 73, 94);
        public static readonly Color TextMuted = Color.FromArgb(127, 140, 141);
        public static readonly Color Success = Color.FromArgb(46, 204, 113);
        public static readonly Color Surface = Color.White;

        public static Icon? LoadAppIcon()
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "app.ico");
            return File.Exists(iconPath) ? new Icon(iconPath) : null;
        }

        public static Image? LoadLogo(int size = 64)
        {
            var logoPath = Path.Combine(AppContext.BaseDirectory, "Resources", "logo_parksystem.png");
            if (!File.Exists(logoPath))
            {
                return null;
            }

            using var source = Image.FromFile(logoPath);
            return new Bitmap(source, new Size(size, size));
        }
    }
}
