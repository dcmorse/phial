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
            Console.WriteLine($"{FPRVs} fprvs");
            Console.WriteLine($"{SARVs} sarvs");
        }
    }
}