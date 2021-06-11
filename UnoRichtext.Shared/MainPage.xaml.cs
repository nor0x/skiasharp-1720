using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System;
using SkiaSharp.Views.UWP;
using SkiaSharp;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
using Topten.RichTextKit;

using TextBlock = Topten.RichTextKit.TextBlock;
using Style = Topten.RichTextKit.Style;
using TextAlignment = Topten.RichTextKit.TextAlignment;

namespace UnoRichtext
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            rand = new Random();
            renderButton.Click += (s, e) =>
            {
                Console.WriteLine("clicked");

                if(tb != null && canvas != null)
                {
                    skiaCanvas.Invalidate();
                }

            };


            // Create the text block
            tb = new TextBlock();

            // Configure layout properties
            tb.MaxWidth = 900;
            tb.Alignment = TextAlignment.Center;
            // Create normal style
            var styleNormal = new Style()
            {
                FontFamily = "Arial",
                FontSize = 14
            };

            // Create bold italic style
            var styleBoldItalic = new Style()
            {
                FontFamily = "Arial",
                FontSize = 14,
                FontWeight = 700,
                FontItalic = true,
            };

            // Add text to the text block
            tb.AddText("Hello World.  ", styleNormal);
            tb.AddText("Welcome to RichTextKit", styleBoldItalic);


        }

        TextBlock tb;
        SKCanvas canvas;
        Random rand;
        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs arg)
        {
            canvas = arg.Surface.Canvas;
            canvas.Clear(SKColor.Parse(String.Format("#{0:X6}", rand.Next(0x1000000))));

            if (tb != null)
            {
                tb.Paint(arg.Surface.Canvas, new SKPoint(100, 100));
            }

        }
    }
}
