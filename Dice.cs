using System;

namespace phial
{
    static class Dice
    {
        public static int CountHits(int ndice, int toHit)
        {
            int hits = 0;
            for (; ndice > 0; ndice--)
                if (D6() >= 4)
                    hits++;
            return hits;
        }


        public static int D6()
        {
            return random.Next(1, 7);
        }

        private static Random random = new Random();

    }
}