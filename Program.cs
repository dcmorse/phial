using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest q = new Quest(1).FromRivendell();
            Console.WriteLine(q);
        }
    }
}
