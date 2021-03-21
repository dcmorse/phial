using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            // var yesG = new Trials(new StriderSprint(6, true));
            // var noG = new Trials(new StriderSprint(6, false));
            // Trials.ReportDifference(yesG, noG);

            RunOne(new StriderSprint(6, true));
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
