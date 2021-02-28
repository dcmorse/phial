using System.Collections.Generic;
using System;

namespace phial
{
    class HuntBag
    {
        public HuntBag(Tile[] tiles)
        {
            Tiles = new List<Tile>(tiles);
        }
        private List<Tile> Tiles { get; }

        private static Random random = new Random();
        public Tile DrawTile()
        {
            var i = random.Next(0, Tiles.Count);
            var tile = Tiles[i];
            Tiles.RemoveAt(i);
            return tile;
        }
    }
}