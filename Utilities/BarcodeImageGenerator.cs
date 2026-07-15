using System;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

using System.Net.NetworkInformation;

public static class BarcodeImageGenerator
{
    public static string GenerateBarcodeImageUrl(string data)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 100,
                Width = 300,
                Margin = 10
            }
        };

        using (var bitmap = writer.Write(data))
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                string base64 = Convert.ToBase64String(ms.ToArray());
                return $"data:image/png;base64,{base64}";
            }
        }
    }
}
