using System;
using System.Text;

namespace phial
{
    class FreeActionDiceRoll
    {
        public FreeActionDiceRoll(int n)
        {
            for (; n > 0; n--)
                switch (D6.roll())
                {
                    case 1:
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
                        ++WillOfTheWests;
                        break;
                }
        }

        public int Characters { get; set; } = 0;
        public int Musters { get; set; } = 0;
        public int ArmyMusters { get; set; } = 0;
        public int Events { get; set; } = 0;
        public int WillOfTheWests { get; set; } = 0;

        public int CharacterOrWills
        {
            get
            {
                return Characters + WillOfTheWests;
            }
        }

        public void SpendCharacterOrWill()
        {
            if (Characters > 0)
                --Characters;
            else if (WillOfTheWests > 0)
                --WillOfTheWests;
            else throw new InvalidOperationException("Attempt to spend action dice that don't exist.");
        }

        public override string ToString()
        {
            var sb = new StringBuilder("", 6);
            (int, string)[] a = { (Characters, "C"), (Musters, "M"), (ArmyMusters, "H"), (Events, "E"), (WillOfTheWests, "W") };
            foreach (var (n, ch) in a)
            {
                for (var i = 0; i < n; i++)
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}