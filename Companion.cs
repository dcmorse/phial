namespace phial
{
    abstract class Companion
    {
        public abstract int Level();
        public virtual bool IsRemovable()
        {
            return true;
        }
    }
    class Level3 : Companion
    {
        public override int Level() { return 3; }
    }
    class Level2 : Companion
    {
        public override int Level() { return 2; }
    }
    class Level1 : Companion
    {
        public override int Level() { return 1; }
    }

    class Gandalf : Level3
    {

    }
    class Strider : Level3
    {

    }

    class Legolas : Level2
    {

    }
    class Gimli : Level2
    {

    }
    class Boromir : Level2
    {

    }
    class Merry : Level1
    {

    }
    class Pippin : Level1
    {

    }
    class Gollum : Companion
    {
        public override int Level() { return 0; }
        public override bool IsRemovable()
        {
            return false;
        }
    }
}