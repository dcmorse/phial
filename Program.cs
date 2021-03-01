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
            Quest q = new Quest(5, 1, 9, Tile.Tiles).MordorTrack();
            Console.WriteLine(q);
        }
    }
}
