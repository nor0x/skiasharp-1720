using System.Collections.Generic;
using SkiaSharp;

namespace Topten.RichTextKit
{
    public class FontMetrics
    {
        public static readonly Dictionary<SKTypeface, FontMetrics> Metrics = new Dictionary<SKTypeface, FontMetrics>();
        public float Ascent { get; }
        public float Descent { get; }
        public float Leading { get; }
        public FontMetrics(float ascent, float descent, float leading)
        {
            Ascent = ascent;
            Descent = descent;
            Leading = leading;
        }
    }
}