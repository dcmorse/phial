using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            var seven = new Trials(new StriderSprint(7));
            var four = new Trials(new StriderSprint(4));
            Trials.ReportDifference(four, seven);
            // RunOne();
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
