using System.Diagnostics;
using System;

namespace phial
{
    static class Trials
    {
        public static void Run(int trialCount, int shadowDice)
        {
            int fprvs = 0;
            int sarvs = 0;
            var logger = new NullLogger();
            for (var i = 0; i < trialCount; i++)
            {
                var q = new Quest(shadowDice, logger).FromRivendell();
                if (q.IsCorrupted())
                {
                    ++sarvs;
                }
                else if (q.IsRingDestroyed())
                {
                    ++fprvs;
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