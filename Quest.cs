using System;

using System.Collections.Generic;

namespace phial
{
    class Quest
    {
        public Quest(int shadowHuntAllocated)
        {
            ShadowHuntAllocated = shadowHuntAllocated;
            ShadowRolled = 7 - shadowHuntAllocated;
            HuntBag = new HuntBag(Tile.GreyTiles);
        }

        private int FreeDice { get; } = 4;
        private int ShadowHuntAllocated { get; }
        private int ShadowRolled { get; }
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
            return MordorTrackStep >= 5;
        }
        public int Turns { get; set; } = 0;
        public int Corruption { get; set; } = 0;
        public int MordorTrackStep { get; set; } = 0;
        public bool Revealed { get; set; } = false;
        public Fellowship Fellowship { get; set; } = new Fellowship();
        public Strategy Strategy { get; set; } = new KillGuide();

        public Quest FromRivendell()
        {
            Turns = 1;
            return MordorTrack();
            //     for (Turns = 1; ; Turns++)
            //     {
            //         int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
            //         bool movedOrHidThisTurn = false;
            //         Console.WriteLine($"Turn {Turns}: {eyes} eyes");
            //         for (int swords = D6.CountHits(FreeDice, 4); swords > 0; swords--)
            //         {
            //             if (Revealed)
            //             {
            //                 Revealed = false;
            //                 Console.WriteLine($"  hide");
            //             }
            //             else
            //             {
            //                 var tile = HuntBag.DrawTile();
            //                 int huntValue = tile.Value(eyes);
            //                 Console.WriteLine($"  from step {MordorTrackStep} <{tile}> = {huntValue}");
            //                 Strategy.Hunt(huntValue, tile.Reveal(), tile, this);
            //                 if (!tile.Stop()) MordorTrackStep++;
            //                 eyes++;
            //                 Console.WriteLine($"    corruption {Corruption}, {eyes} eyes");
            //                 if (IsOver())
            //                     return this;
            //             }
            //             movedOrHidThisTurn = true;
            //         }
            //         if (!movedOrHidThisTurn)
            //         {
            //             Corruption++;
            //             Console.WriteLine("  lazy hobbit corruption");
            //         }
            //     }
        }
        public Quest MordorTrack()
        {
            for (; ; Turns++)
            {
                int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
                bool movedOrHidThisTurn = false;
                Console.WriteLine($"Turn {Turns}: {eyes} eyes");
                for (int swords = D6.CountHits(FreeDice, 4); swords > 0; swords--)
                {
                    if (Revealed)
                    {
                        Revealed = false;
                        Console.WriteLine($"  hide");
                    }
                    else
                    {
                        var tile = HuntBag.DrawTile();
                        int huntValue = tile.Value(eyes);
                        Console.WriteLine($"  from step {MordorTrackStep} <{tile}> = {huntValue}");
                        Strategy.Hunt(huntValue, tile.Reveal(), tile, this);
                        if (!tile.Stop()) MordorTrackStep++;
                        eyes++;
                        Console.WriteLine($"    corruption {Corruption}, {eyes} eyes");
                        if (IsOver())
                            return this;
                    }
                    movedOrHidThisTurn = true;
                }
                if (!movedOrHidThisTurn)
                {
                    Corruption++;
                    Console.WriteLine("  lazy hobbit corruption");
                }
            }
        }

        public void TakeGuideCasualty(int tileValue, bool reveal, Tile tile)
        {
            Revealed = Revealed || reveal;
            if (tileValue > 0)
            {
                int damage = Math.Max(0, tileValue - Fellowship.Guide.Level());
                Corruption += damage;
                Console.WriteLine($"    {Fellowship.Guide} falls against {tileValue} damage");
                Fellowship = Fellowship.RemoveCompanion(Fellowship.Guide);
            }
        }

        public override string ToString()
        {
            if (IsCorrupted())
                return $"Corruption win at step {MordorTrackStep}";
            else if (IsRingDestroyed())
                return $"Ring Destroyed at corruption {Corruption}";
            else
                return $"Step {MordorTrackStep} Corruption {Corruption}{(Revealed ? " revealed" : "")}";
        }
    }
}