using System.Diagnostics;
using System;

namespace phial
{
    class Trials
    {
        int TrialCount { get; } = 10000;
        Histogram<int> FPRVs { get; } = new Histogram<int>();
        Histogram<int> SARVs { get; } = new Histogram<int>();

        public Trials(FreeStrategy strategy)
        {
            var logger = new NullLogger();
            for (var i = 0; i < TrialCount; i++)
            {
                var q = new Quest(strategy, 1, logger).FromRivendell();
                if (q.IsCorrupted())
                    SARVs.Increment(q.Turns);
                else if (q.IsRingDestroyed())
                    FPRVs.Increment(q.Turns);
                else
                    Debug.Assert(false, "This game isn't finished. Why?");
            }
        }

        public void Report()
        {
            Console.WriteLine($"{TrialCount} trials");
            Console.WriteLine($"FPRV median is {FPRVs.MedianIndex()} with {FPRVs.Fetch(FPRVs.MedianIndex())}");
            var (fmin, fmax) = FPRVs.Domain();
            var (smin, smax) = SARVs.Domain();
            var minTurn = Math.Min(fmin, smin);
            var maxTurn = Math.Max(fmax, smax);
            const int heightPx = 30;
            int fpMaxHeight = FPRVs.MedianHeight();
            int saMaxHeight = SARVs.MedianHeight();
            int trialsPerPixel = Math.Max(fpMaxHeight, saMaxHeight) / heightPx;
            var viewport = new RectangleInt(
                minTurn - 1,
                fpMaxHeight / trialsPerPixel + 2,
                maxTurn + 1,
                -(saMaxHeight / trialsPerPixel + 1));
            var r = new Raster<string>(viewport, "");
            r.DrawPoint(minTurn - 1, 0, "Turn");
            r.DrawPoint(minTurn - 1, 1, "FPRV");
            r.DrawPoint(minTurn - 1, -1, "SARV");
            // horizontal axis
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawPoint(turn, 0, turn.ToString().PadLeft(4));
            // FPRV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 1, 1, FPRVs.Fetch(turn) / trialsPerPixel), " ###");
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 0, 1, -SARVs.Fetch(turn) / trialsPerPixel), " @@@");
            // for (var turn = minTurn; turn <= maxTurn; turn++)
            // {
            //     int y = FPRVs.Fetch(turn);
            //     r.DrawPoint(turn, 1 + y / trialsPerPixel, y.ToString().PadLeft(4));
            // }
            // for (var turn = minTurn; turn <= maxTurn; turn++)
            // {
            //     int y = SARVs.Fetch(turn);
            //     r.DrawPoint(turn, -1 - y / trialsPerPixel, y.ToString().PadLeft(4));
            // }
            Console.WriteLine(r.ToAsciiArt(5));
        }

        // (int, int) Turns()
        // {
        //     var (fmin, fmax) = FPRVs.Domain();
        //     var (smin, smax) = SARVs.Domain();
        //     var minTurn = Math.Min(fmin, smin);
        //     var maxTurn = Math.Max(fmax, smax);
        //     return (minTurn, maxTurn);
        // }
        // public static void ReportDifference(Trials plus, Trials minus)
        // {

        // }
    }
}