using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            Quest q = new Quest(1).MordorTrack();
            Console.WriteLine(q);
        }
    }
}
