using AzureTangyFunc;
using SkiaSharp;
using Xunit;

public class ImageThumbnailTests
{
    [Theory]
    [InlineData(SKEncodedImageFormat.Png)]
    [InlineData(SKEncodedImageFormat.Jpeg)]
    public void ResizesSupportedImagesAndPreservesEncoding(SKEncodedImageFormat format)
    {
        using var bitmap = new SKBitmap(40, 30);
        bitmap.Erase(SKColors.CornflowerBlue);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(format, 90);
        using var input = new MemoryStream(encoded.ToArray());

        var bytes = ImageThumbnail.Resize(input);

        using var output = SKCodec.Create(new MemoryStream(bytes));
        Assert.Equal(300, output.Info.Width);
        Assert.Equal(200, output.Info.Height);
        Assert.Equal(format, output.EncodedFormat);
    }

    [Fact]
    public void RejectsInvalidOrTiffInput()
    {
        using var input = new MemoryStream(new byte[] { 0x49, 0x49, 0x2a, 0, 8, 0, 0, 0 });
        Assert.Throws<InvalidDataException>(() => ImageThumbnail.Resize(input));
    }

    [Fact]
    public void RejectsInputLargerThanEightMiB()
    {
        using var input = new MemoryStream(new byte[8 * 1024 * 1024 + 1]);
        Assert.Throws<InvalidDataException>(() => ImageThumbnail.Resize(input));
    }
}
