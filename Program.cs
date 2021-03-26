using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            var oldTrial = new Trials(new StriderSprint(7, true));
            oldTrial.Report();
            // for (var i = 1; i < 11; i++)
            // {
            //     var newTrial = new Trials(new StriderSprint(i, true));
            //     Console.WriteLine($"{i}_________");
            //     Trials.ReportDifference(newTrial, oldTrial);
            //     oldTrial = newTrial;
            // }


            // RunOne(new StriderSprint(6, true));
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
