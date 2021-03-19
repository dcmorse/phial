using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            new Trials().Report();
            // RunOne();
            // var raster = new Raster<string>(RectangleInt.XYWH(2, 2, 10, 10), ".");
            // raster.DrawRectangle(RectangleInt.XYWH(2, 2, 5, 5), "#");
            // raster.DrawPoint(3, 5, "123");
            // Console.WriteLine(raster.ToAsciiArt(3));
        }

        static void RunOne()
        {
            var logger = new ConsoleLogger();
            var q = new Quest(1, logger);
            q.FromRivendell();
            logger.Log(q.ToString());
        }
    }
}
