using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

var outputRoot = args.Length > 0
    ? args[0]
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Estacionamento.Package", "Images"));

var appResources = Path.GetFullPath(Path.Combine(outputRoot, "..", "..", "Estacionamento", "Resources"));
Directory.CreateDirectory(outputRoot);
Directory.CreateDirectory(appResources);

var assets = new (string Name, int Width, int Height)[]
{
    ("StoreLogo.png", 50, 50),
    ("Square44x44Logo.png", 44, 44),
    ("Square150x150Logo.png", 150, 150),
    ("Wide310x150Logo.png", 310, 150),
    ("SplashScreen.png", 620, 300),
    ("Logo300.png", 300, 300),
    ("logo_parksystem.png", 256, 256)
};

foreach (var (name, width, height) in assets)
{
    var targetDir = name == "logo_parksystem.png" ? appResources : outputRoot;
    using var bitmap = RenderBrandImage(width, height, name != "logo_parksystem.png");
    bitmap.Save(Path.Combine(targetDir, name), ImageFormat.Png);
    Console.WriteLine($"Gerado: {Path.Combine(targetDir, name)}");
}

CreateMultiSizeIcon(Path.Combine(appResources, "app.ico"));
Console.WriteLine($"Gerado: {Path.Combine(appResources, "app.ico")}");

static Bitmap RenderBrandImage(int width, int height, bool includeTitle)
{
    var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
    using var graphics = Graphics.FromImage(bitmap);
    graphics.SmoothingMode = SmoothingMode.AntiAlias;
    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    graphics.Clear(Color.Transparent);

    var background = new RectangleF(0, 0, width, height);
    using (var brush = new LinearGradientBrush(
               background,
               Color.FromArgb(41, 128, 185),
               Color.FromArgb(31, 97, 141),
               LinearGradientMode.ForwardDiagonal))
    {
        graphics.FillRectangle(brush, background);
    }

    var iconSize = Math.Min(width, height) * 0.42f;
    var iconX = includeTitle && width > height
        ? width * 0.12f
        : (width - iconSize) / 2f;
    var iconY = (height - iconSize) / 2f;

    using var iconFont = new Font("Segoe MDL2 Assets", iconSize * 0.85f, FontStyle.Regular, GraphicsUnit.Pixel);
    using var iconBrush = new SolidBrush(Color.White);
    var iconFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
    graphics.DrawString("\uECE4", iconFont, iconBrush, new RectangleF(iconX, iconY, iconSize, iconSize), iconFormat);

    if (includeTitle && width >= 200)
    {
        var titleSize = Math.Max(14f, height * 0.18f);
        using var titleFont = new Font("Segoe UI Semibold", titleSize, FontStyle.Bold, GraphicsUnit.Pixel);
        var titleRect = new RectangleF(iconX + iconSize + width * 0.04f, height * 0.28f, width * 0.55f, height * 0.28f);
        graphics.DrawString("ParkSystem", titleFont, iconBrush, titleRect, new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        });

        using var subtitleFont = new Font("Segoe UI", titleSize * 0.55f, FontStyle.Regular, GraphicsUnit.Pixel);
        var subtitleRect = new RectangleF(titleRect.X, height * 0.52f, width * 0.55f, height * 0.22f);
        graphics.DrawString("Estacionamento", subtitleFont, iconBrush, subtitleRect, new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        });
    }

    return bitmap;
}

static void CreateMultiSizeIcon(string path)
{
    var sizes = new[] { 16, 32, 48, 256 };
    var images = sizes.Select(CreateIconBitmap).ToArray();
    using var stream = File.Create(path);
    WriteIco(stream, images);
    foreach (var image in images)
    {
        image.Dispose();
    }
}

static Bitmap CreateIconBitmap(int size)
{
    using var source = RenderBrandImage(size, size, false);
    return new Bitmap(source);
}

static void WriteIco(Stream output, IReadOnlyList<Bitmap> images)
{
    using var writer = new BinaryWriter(output, System.Text.Encoding.UTF8, leaveOpen: true);
    writer.Write((short)0);
    writer.Write((short)1);
    writer.Write((short)images.Count);

    var offset = 6 + (16 * images.Count);
    var imageData = new List<byte[]>();

    for (var i = 0; i < images.Count; i++)
    {
        var image = images[i];
        using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        var data = ms.ToArray();
        imageData.Add(data);

        var size = image.Width >= 256 ? (byte)0 : (byte)image.Width;
        writer.Write(size);
        writer.Write(size);
        writer.Write((byte)0);
        writer.Write((byte)0);
        writer.Write((short)1);
        writer.Write((short)32);
        writer.Write(data.Length);
        writer.Write(offset);
        offset += data.Length;
    }

    foreach (var data in imageData)
    {
        writer.Write(data);
    }
}
