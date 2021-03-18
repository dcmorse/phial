using System;

namespace phial
{
    class RectangleInt
    {
        public RectangleInt(int x0, int y0, int x1, int y1)
        {
            var xx1 = Math.Max(x0, x1);
            var yy1 = Math.Max(y0, y1);
            var xx0 = Math.Min(x0, x1);
            var yy0 = Math.Min(y0, y1);
            X0 = xx0;
            Y0 = yy0;
            X1 = xx1;
            Y1 = yy1;
        }

        public static RectangleInt XYWH(int x, int y, int w, int h)
        {
            return new RectangleInt(x, y, x + w, y + h);
        }

        public int X0 { get; }
        public int Y0 { get; }
        public int X1 { get; }
        public int Y1 { get; }
        public int X { get { return X0; } }
        public int Y { get { return Y0; } }
        public int W { get { return X1 - X0; } }
        public int H { get { return Y1 - Y0; } }
        public int Width { get { return X1 - X0; } }
        public int Height { get { return Y1 - Y0; } }
    }
}