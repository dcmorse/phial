using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            // Trials.Run(10000, 1);
            RunOne();
        }

        static void RunOne() {
            var logger = new ConsoleLogger();
            var q = new Quest(1, logger);
            q.FromRivendell();
            logger.Log(q.ToString());
        }
    }
}
