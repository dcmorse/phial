using System.Diagnostics;
using System;

namespace phial
{
    class Trials
    {
        int TrialCount { get; } = 10000;
        Histogram<int> FPRVs { get; } = new Histogram<int>();
        Histogram<int> SARVs { get; } = new Histogram<int>();

        public Trials()
        {
            var logger = new NullLogger();
            for (var i = 0; i < TrialCount; i++)
            {
                var q = new Quest(1, logger).FromRivendell();
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
                minTurn,
                fpMaxHeight / trialsPerPixel + 1,
                maxTurn + 1,
                -(saMaxHeight / trialsPerPixel + 1));
            var r = new Raster<string>(viewport, "");
            // horizontal axis
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawPoint(turn, 0, turn.ToString());
            // FPRV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 1, 1, FPRVs.Fetch(turn) / trialsPerPixel), "##");
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 0, 1, -SARVs.Fetch(turn) / trialsPerPixel), "@@");
            Console.WriteLine(r.ToAsciiArt(4));
        }
    }
}