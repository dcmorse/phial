using System.Collections.Generic;
using System;

namespace phial
{
    class HuntBag
    {
        public HuntBag()
        {
            Tiles = new List<Tile>(Tile.GreyTiles);
        }

        private List<Tile> Tiles { get; }
        private List<Tile> SetAsideTiles { get; } = new List<Tile>(12);

        private static Random random = new Random();
        public Tile DrawTile()
        {
            if (0 == Tiles.Count)
                Tiles.AddRange(Tile.GreyTiles); // wow, the bag was empty
            var i = random.Next(0, Tiles.Count);
            var tile = Tiles[i];
            Tiles.RemoveAt(i);
            if (tile.isEye())
                SetAsideTiles.Add(tile);
            return tile;
        }

        public void EnterMordor()
        {

        }
    }
}