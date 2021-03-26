using System;
using System.Text;

namespace phial
{
    class ShadowActionDiceRoll
    {
        public ShadowActionDiceRoll(int rolled, int eyesPlaced)
        {
            Eyes = eyesPlaced;
            for (; rolled > 0; rolled--)
                switch (D6.roll())
                {
                    case 1:
                        ++Armies;
                        break;
                    case 2:
                        ++Characters;
                        break;
                    case 3:
                        ++Musters;
                        break;
                    case 4:
                        ++ArmyMusters;
                        break;
                    case 5:
                        ++Events;
                        break;
                    case 6:
                        ++Eyes;
                        break;
                }
        }

        public int Characters { get; set; } = 0;
        public int Armies { get; set; } = 0;
        public int Musters { get; set; } = 0;
        public int ArmyMusters { get; set; } = 0;
        public int Events { get; set; } = 0;
        public int Eyes { get; set; }

        public int Count
        {
            get
            {
                return Armies + Characters + Musters + ArmyMusters + Events + Eyes;
            }
        }

        public int Mustery { get { return Musters + ArmyMusters; } }

        public void SpendMustery()
        {
            if (Musters > 0)
                --Musters;
            else if (ArmyMusters > 0)
                --ArmyMusters;
            else throw new InvalidOperationException("Attempt to spend action dice that don't exist.");
        }


        public override string ToString()
        {
            var sb = new StringBuilder("", 6);
            (int, string)[] a = { (Characters, "C"), (Armies, "A"), (Musters, "M"), (ArmyMusters, "H"), (Events, "E") };
            foreach (var (n, ch) in a)
            {
                for (var i = 0; i < n; i++)
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}