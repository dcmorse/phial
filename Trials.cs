using System.Diagnostics;
using System;
using static System.Math;

namespace phial
{
    class Trials
    {
        int TrialCount { get; } = 500000;
        Histogram<int> FPRVs { get; } = new Histogram<int>();
        Histogram<int> SARVs { get; } = new Histogram<int>();
        const int ReportHeightPixels = 100;

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
            (int minTurn, int maxTurn) = Turns();
            (int fpMaxHeight, int saMaxHeight) = Range();
            int trialsPerPixel = Max(fpMaxHeight, saMaxHeight) / ReportHeightPixels;
            var viewport = new RectangleInt(
                minTurn - 1,
                fpMaxHeight / trialsPerPixel + 2,
                maxTurn + 1,
                -(saMaxHeight / trialsPerPixel + 1));
            var r = new Raster<string>(viewport, "");
            DrawAxisAndLabels(r, minTurn, maxTurn);
            // FPRV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 1, 1, FPRVs.Fetch(turn) / trialsPerPixel), " ###");
            // SARV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawRectangle(RectangleInt.XYWH(turn, 0, 1, -SARVs.Fetch(turn) / trialsPerPixel), " @@@");
            for (var turn = minTurn; turn <= maxTurn; turn++)
            {
                int y = FPRVs.Fetch(turn);
                r.DrawPoint(turn, 1 + y / trialsPerPixel, y.ToString().PadLeft(ReportPixelWidth - 1));
            }
            for (var turn = minTurn; turn <= maxTurn; turn++)
            {
                int y = SARVs.Fetch(turn);
                r.DrawPoint(turn, -1 - y / trialsPerPixel, y.ToString().PadLeft(ReportPixelWidth - 1));
            }
            Console.WriteLine(r.ToAsciiArt(ReportPixelWidth));
        }

        const int ReportPixelWidth = 5;

        (int, int) Turns()
        {
            var (fmin, fmax) = FPRVs.Domain();
            var (smin, smax) = SARVs.Domain();
            var minTurn = Min(fmin, smin);
            var maxTurn = Max(fmax, smax);
            return (minTurn, maxTurn);
        }
        (int, int) Range()
        {
            int fpMax = FPRVs.MedianHeight();
            int saMax = SARVs.MedianHeight();
            return (fpMax, saMax);
        }

        static void DrawAxisAndLabels(Raster<string> r, int minTurn, int maxTurn)
        {
            r.DrawPoint(minTurn - 1, 0, "Turn");
            r.DrawPoint(minTurn - 1, 1, "FPRV");
            r.DrawPoint(minTurn - 1, -1, "SARV");
            for (var turn = minTurn; turn <= maxTurn; turn++)
                r.DrawPoint(turn, 0, turn.ToString().PadLeft(ReportPixelWidth - 1));
        }
        public static void ReportDifference(Trials plus, Trials minus)
        {
            (int pMinTurn, int pMaxTurn) = plus.Turns();
            (int mMinTurn, int mMaxTurn) = minus.Turns();
            var minTurn = Min(pMinTurn, pMaxTurn);
            var maxTurn = Max(pMaxTurn, mMaxTurn);
            (int pFPMax, int pSAMax) = plus.Range();
            (int mFPMax, int mSAMax) = minus.Range();
            var fpMaxHeight = Max(pFPMax, mFPMax);
            var saMaxHeight = Max(pSAMax, mSAMax);
            int trialsPerPixel = Max(fpMaxHeight, saMaxHeight) / ReportHeightPixels;
            var viewport = new RectangleInt(
                minTurn - 1,
                fpMaxHeight / trialsPerPixel + 2,
                maxTurn + 1,
                -(saMaxHeight / trialsPerPixel + 1));
            var r = new Raster<string>(viewport, "");
            DrawAxisAndLabels(r, minTurn, maxTurn);
            // FPRV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
            {
                var pHeight = plus.FPRVs.Fetch(turn) / trialsPerPixel;
                var mHeight = minus.FPRVs.Fetch(turn) / trialsPerPixel;
                var sharedTop = Min(pHeight, mHeight);
                var peakedTop = Max(pHeight, mHeight);
                if (sharedTop > 0)
                {
                    r.DrawRectangle(new RectangleInt(turn, 1, turn + 1, sharedTop + 1), " ###");
                }
                if (peakedTop > sharedTop)
                {
                    string color = pHeight > mHeight ? " +++" : " ---";
                    r.DrawRectangle(new RectangleInt(turn, sharedTop + 1, turn + 1, peakedTop + 1), color);
                }
            }
            // SAMV bars
            for (var turn = minTurn; turn <= maxTurn; turn++)
            {
                var pHeight = plus.SARVs.Fetch(turn) / trialsPerPixel;
                var mHeight = minus.SARVs.Fetch(turn) / trialsPerPixel;
                var sharedTop = Min(pHeight, mHeight);
                var peakedTop = Max(pHeight, mHeight);
                if (sharedTop > 0)
                {
                    r.DrawRectangle(new RectangleInt(turn, 0, turn + 1, -sharedTop), " @@@");
                }
                if (peakedTop > sharedTop)
                {
                    string color = pHeight > mHeight ? " +++" : " ---";
                    r.DrawRectangle(new RectangleInt(turn, -sharedTop, turn + 1, -peakedTop), color);
                }
            }
            Console.WriteLine(r.ToAsciiArt(5));
        }
    }
}