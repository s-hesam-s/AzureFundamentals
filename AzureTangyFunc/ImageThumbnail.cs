using System;
using System.IO;
using SkiaSharp;

namespace AzureTangyFunc;

public static class ImageThumbnail
{
    private const int maxInputBytes = 8 * 1024 * 1024;
    private const long maxPixels = 16_000_000;

    public static byte[] Resize(Stream input)
    {
        using var encoded = new MemoryStream();
        var buffer = new byte[81920];
        int count;
        while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            if (encoded.Length + count > maxInputBytes)
                throw new InvalidDataException("Image exceeds the 8 MiB input limit.");
            encoded.Write(buffer, 0, count);
        }

        using var data = SKData.CreateCopy(encoded.ToArray());
        using var codec = SKCodec.Create(data);
        if (codec is null || (codec.EncodedFormat != SKEncodedImageFormat.Png && codec.EncodedFormat != SKEncodedImageFormat.Jpeg))
            throw new InvalidDataException("Only PNG and JPEG images are supported.");
        if (codec.Info.Width <= 0 || codec.Info.Height <= 0 || (long)codec.Info.Width * codec.Info.Height > maxPixels)
            throw new InvalidDataException("Image exceeds the 16 million pixel limit.");

        using var bitmap = new SKBitmap(codec.Info);
        if (codec.GetPixels(bitmap.Info, bitmap.GetPixels()) != SKCodecResult.Success)
            throw new InvalidDataException("Image could not be decoded completely.");
        using var resized = bitmap.Resize(new SKImageInfo(300, 200), new SKSamplingOptions(SKFilterMode.Linear));
        if (resized is null) throw new InvalidDataException("Image could not be resized.");
        using var image = SKImage.FromBitmap(resized);
        using var output = image.Encode(codec.EncodedFormat, 90);
        if (output is null) throw new InvalidDataException("Image could not be encoded.");
        return output.ToArray();
    }
}
