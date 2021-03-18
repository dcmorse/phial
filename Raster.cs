using System.Text;

namespace phial
{
    class Raster<Pixel>
    {
        public Raster(RectangleInt viewport, Pixel initialPixel)
        {
            Viewport = viewport;
            Pixels = new Pixel[viewport.Width, viewport.Height];
            for (var i = 0; i < viewport.Width; i++)
                for (var j = 0; j < viewport.Height; j++)
                    Pixels[i, j] = initialPixel;
        }

        public RectangleInt Viewport { get; }
        private Pixel[,] Pixels { get; }

        public void DrawPoint(int x, int y, Pixel pixel)
        {
            Pixels[x - Viewport.X, y - Viewport.Y] = pixel;
        }

        public void DrawRectangle(RectangleInt r, Pixel pixel)
        {
            var vx = Viewport.X;
            var vy = Viewport.Y;
            for (var y = r.Y0; y < r.Y1; y++)
                for (var x = r.X0; x < r.X1; x++)
                    Pixels[x - vx, y - vy] = pixel;
        }

        private Pixel ReadPixel(int x, int y)
        {
            return Pixels[x - Viewport.X, y - Viewport.Y];
        }

        public string ToAsciiArt(int pixelWidth)
        {
            const int newlineWidth = 2; // upper bound for CR-LF on MSDOS
            StringBuilder sb = new StringBuilder("", (Viewport.W + newlineWidth) * Viewport.H * pixelWidth);
            for (var y = Viewport.Y1 - 1; y >= Viewport.Y0; y--)
            {
                for (var x = Viewport.X0; x < Viewport.X1; x++)
                    sb.Append(PadPixel(ReadPixel(x, y), pixelWidth));
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static string PadPixel<Pxl>(Pxl pixel, int width)
        {
            string s = pixel.ToString();
            if (s.Length < width)
                return s.PadRight(width, ' ');
            return s[..width];
        }
    }
}