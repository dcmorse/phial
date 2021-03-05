
using System.Linq;
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

        public Fellowship LoseCompanion(Companion casualty)
        {
            return new Fellowship(Companions.Where(companion => companion != casualty).ToArray());
        }

        private Companion[] Companions { get; }
        public Companion Guide { get { return Companions[0]; } }
    }
}