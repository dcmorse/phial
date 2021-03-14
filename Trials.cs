using System.Diagnostics;
using System;

namespace phial
{
    static class Trials
    {
        public static void Run(int trialCount, int shadowDice)
        {
            var fprvs = new Histogram<int>();
            var sarvs = new Histogram<int>();
            var logger = new NullLogger();
            for (var i = 0; i < trialCount; i++)
            {
                var q = new Quest(shadowDice, logger).FromRivendell();
                if (q.IsCorrupted())
                {
                    sarvs.Increment(q.Turns);
                }
                else if (q.IsRingDestroyed())
                {
                    fprvs.Increment(q.Turns);
                }
                else
                {
                    Debug.Assert(false, "This game isn't finished. Why?");
                }
            }
            Console.WriteLine($"{trialCount} trials");
            Console.WriteLine($"{fprvs} fprvs");
            Console.WriteLine($"{sarvs} sarvs");
        }
    }
}