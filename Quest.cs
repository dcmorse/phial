using System;

using System.Collections.Generic;

namespace phial
{
    class Quest
    {
        public Quest(int freeDice, int shadowHuntAllocated, int shadowRolled, Tile[] tiles)
        {
            FreeDice = freeDice;
            ShadowHuntAllocated = shadowHuntAllocated;
            ShadowRolled = shadowRolled;
            Tiles = tiles;
            HuntBag = new HuntBag(Tiles);
        }

        private int FreeDice { get; }
        private int ShadowHuntAllocated { get; }
        private int ShadowRolled { get; }
        private Tile[] Tiles { get; }
        private HuntBag HuntBag { get; }

        public bool IsOver()
        {
            return IsCorrupted() || IsRingDestroyed();
        }
        public bool IsCorrupted()
        {
            return Corruption >= 12;
        }
        public bool IsRingDestroyed()
        {
            return Step >= 5;
        }
        public int Turns { get; set; } = 0;
        public int Corruption { get; set; } = 0;
        public int Step { get; set; } = 0;
        public bool Revealed { get; set; } = false;

        public Quest MordorTrack()
        {
            for (Turns = 1; ; Turns++)
            {
                var eyes = ShadowHuntAllocated + Dice.CountHits(ShadowRolled, 6);
                bool movedOrHidThisTurn = false;
                Console.WriteLine($"Turn {Turns}: {eyes} eyes");
                for (int swords = Dice.CountHits(FreeDice, 4); swords > 0; swords--) {
                    if (Revealed)  {
                        Revealed = false;
                        Console.WriteLine($"  hide");
                    } else {
                        var tile = HuntBag.DrawTile();
                        Corruption += tile.Value(eyes);
                        Step++;
                        eyes++;
                        Revealed = tile.Reveal();
                        Console.WriteLine($"  step {Step} <{tile}> corruption {Corruption}, {eyes} eyes");
                        if (IsOver())
                            return this;
                    }
                    movedOrHidThisTurn = true;
                }
                if (!movedOrHidThisTurn) {
                    Corruption++;
                    Console.WriteLine("  lazy hobbit corruption");
                }
            }
        }


        public override string ToString()
        {
            if (IsCorrupted())
                return $"Corruption win at step {Step}";
            else if (IsRingDestroyed())
                return $"Ring Destroyed at corruption {Corruption}";
            else
                return $"Step {Step} Corruption {Corruption}{(Revealed ? " revealed" : "")}";
        }
    }
}