using System;

namespace phial
{
    static class Dice
    {
        public static int CountHits(int ndice, int toHit)
        {
            int hits = 0;
            var ndice2 = ndice;
            for (; ndice > 0; ndice--)
                if (D6() >= toHit)
                    hits++;
            // Console.WriteLine($"    CountHits({ndice2}, {toHit}) => {hits}");
            return hits;
        }


        public static int D6()
        {
            var d6 = random.Next(1, 7);
            // Console.WriteLine($"      d6 {d6}");
            return d6;
        }

        private static Random random = new Random();

    }
}