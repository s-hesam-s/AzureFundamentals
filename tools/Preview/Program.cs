using AzureTangyFunc;
using SkiaSharp;

var root = PreviewSetup.Root("AzureTangyFunc"); var builder = PreviewSetup.Builder(root, 8844);
using var bitmap = new SKBitmap(900, 600); using (var canvas = new SKCanvas(bitmap))
{
    canvas.Clear(new SKColor(13, 17, 23)); using var paint = new SKPaint { IsAntialias = true };
    for (var x = 0; x < 900; x += 75) { paint.Color = new SKColor((byte)(30 + x / 6), 110, 180); canvas.DrawRect(x, 0, 70, 600, paint); }
    paint.Color = new SKColor(245, 176, 65); canvas.DrawCircle(450, 300, 180, paint); paint.Color = new SKColor(25, 50, 75); canvas.DrawRoundRect(new SKRect(150, 440, 750, 520), 20, 20, paint);
}
using var image = SKImage.FromBitmap(bitmap); using var encoded = image.Encode(SKEncodedImageFormat.Png, 90); var original = encoded.ToArray();
var resized = ImageThumbnail.Resize(new MemoryStream(original)); using var codec = SKCodec.Create(new MemoryStream(resized));
string rejected(byte[] bytes) { try { ImageThumbnail.Resize(new MemoryStream(bytes)); return "Unexpected acceptance"; } catch (InvalidDataException ex) { return ex.Message; } }
var evidence = new { implementation = "ImageThumbnail.Resize", input = new { width = 900, height = 600, format = "PNG", bytes = original.Length }, output = new { width = codec.Info.Width, height = codec.Info.Height, format = codec.EncodedFormat.ToString(), bytes = resized.Length }, invalidTiff = rejected(new byte[] { 0x49, 0x49, 0x2a, 0, 8, 0, 0, 0 }), oversized = rejected(new byte[8 * 1024 * 1024 + 1]), functionsHostStarted = false, cloudCalls = 0 };
var app = builder.Build(); PreviewSetup.Guard(app); app.MapGet("/input.png", () => Results.File(original, "image/png")); app.MapGet("/output.png", () => Results.File(resized, "image/png")); app.MapGet("/evidence.json", () => evidence);
app.MapGet("/", () => Results.Content(EvidencePage.Html("Azure sample · local image processing", "The existing image transform executes directly with a synthetic reference image. Functions listeners, storage, SQL and email are not started.", evidence, "<section class='card'><figure><h2>Input · 900 × 600</h2><img src='/input.png' alt='Synthetic geometric reference input'></figure><figure><h2>Actual output · 300 × 200</h2><img src='/output.png' alt='Output returned by ImageThumbnail.Resize'></figure></section>"), "text/html")); app.Run();
