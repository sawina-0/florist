using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;

namespace florist.AppData
{
    public class MyFontResolver : IFontResolver
    {
        private readonly string _fontPath;

        public MyFontResolver()
        {
            string projectDir = Path.GetFullPath(Path.Combine(
         AppDomain.CurrentDomain.BaseDirectory,
         @"..\..\")); // bin/Debug → bin → project root

            _fontPath = Path.Combine(projectDir, "Font", "ArialRegular.ttf");

            if (!File.Exists(_fontPath))
                throw new FileNotFoundException($"Font file not found: {_fontPath}");
        }

        public byte[] GetFont(string faceName) => File.ReadAllBytes(_fontPath);

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            => new FontResolverInfo(_fontPath, isBold, isItalic);
    }
}
