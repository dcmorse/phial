using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            var seven = new Trials(new StriderSprint(7));
            var four = new Trials(new StriderSprint(4));
            four.Report();
            // RunOne();
            // var raster = new Raster<string>(RectangleInt.XYWH(2, 2, 10, 10), ".");
            // raster.DrawRectangle(RectangleInt.XYWH(2, 2, 5, 5), "#");
            // raster.DrawPoint(3, 5, "123");
            // Console.WriteLine(raster.ToAsciiArt(3));
        }

        static void RunOne(FreeStrategy strategy)
        {
            var logger = new ConsoleLogger();
            var q = new Quest(strategy, 1, logger);
            q.FromRivendell();
            logger.Log(q.ToString());
        }
    }
}
