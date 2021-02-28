using System;

namespace phial
{
    class Program
    {
        static void Main(string[] args)
        {
            NumericTile t = new NumericTile(2, true);
            EyeTile e = new EyeTile();
            FellowshipSpecialTile phial = new FellowshipSpecialTile(-2);
            TheRingIsMine theRingIsMine = new TheRingIsMine();
            ShadowSpecialNumericTile onOnTheyWent = new ShadowSpecialNumericTile(3, false);
            ShadowSpecialNumericTile weShallGetIt = new ShadowSpecialNumericTile(1, true);
            ShelobsLair shelobsLair = new ShelobsLair();
            Console.WriteLine(t);
            Console.WriteLine(e);
            Console.WriteLine(phial);
            Console.WriteLine(theRingIsMine);
            Console.WriteLine(onOnTheyWent);
            Console.WriteLine(weShallGetIt);
            Console.WriteLine(shelobsLair);
        }
    }
}
