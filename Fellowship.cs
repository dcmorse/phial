using System.Linq;
using System;

namespace phial
{
    class Fellowship
    {
        public Fellowship()
        {
            Companion[] companions = { new Gandalf(), new Strider(), new Legolas(), new Gimli(), new Boromir(), new Merry(), new Pippin() };
            Companions = companions;
        }

        public Fellowship(Companion[] companions)
        {
            Companions = companions;
        }

        public Fellowship RemoveCompanion(Companion casualty)
        {
            if (casualty.IsRemovable())
            {
                if (Companions.Length == 1)
                {
                    Companion[] gollums = { new Gollum() };
                    return new Fellowship(gollums);
                }
                else
                    return new Fellowship(Companions.Where(companion => companion != casualty).ToArray());
            }
            else return this;
        }

        private Companion[] Companions { get; }
        public Companion Guide { get { return Companions[0]; } }
        public Companion Random()
        {
            return Companions[RandomState.Next(Companions.Length)];
        }
        private Random RandomState = new Random();
    }
}