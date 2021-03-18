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

        public void Report() {
            Console.WriteLine($"{TrialCount} trials");
            Console.WriteLine($"FPRV median is {FPRVs.Median()} with {FPRVs.Fetch(FPRVs.Median())}");
            var (fmin, fmax) = FPRVs.Domain();
            var (smin, smax) = SARVs.Domain();
            var min = Math.Min(fmin, smin);
            var max = Math.Max(fmax, smax);
            var rows = 12;
            var rowChunk = FPRVs.Fetch(FPRVs.Median()) / rows;
            Console.WriteLine($"rowChunk={rowChunk}");
            for (var row = rows; row > 0; row--) {
                var rowThreshold = rowChunk * row;
                var annotationThreshold = rowChunk * (row + 1);
                for (var turn = min; turn <= max; turn++) {
                    var wins = FPRVs.Fetch(turn);
                    string s = wins >= rowThreshold ? "##  " : (wins >= annotationThreshold ? wins.ToString().PadLeft(4, ' '): "    ");
                    Console.Write(s);
                }
                Console.WriteLine();
            }
            for (var i = min; i <= max; i++)
                Console.Write($"{i.ToString().PadLeft(2, ' ')}  ");
            Console.WriteLine();
            for (var row = 0; row < rows; row++) {
                var rowThreshold = rowChunk * row;
                for (var turn = min; turn <= max; turn++)
                    Console.Write(SARVs.Fetch(turn) >= rowThreshold ? "@@  " : "    ");
                Console.WriteLine();
            }
            Console.WriteLine($"{FPRVs} fprvs");
            Console.WriteLine($"{SARVs} sarvs");
        }
    }
}