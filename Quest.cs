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
            HuntBag = new HuntBag();
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

        public bool AtTheGatesOfMordor()
        {
            int moves = Progress + LastKnownDistanceFromRivendell;
            int movesNeeded = 10 + (TookMoria ?? true ? 0 : 1);
            return moves > movesNeeded;
        }
        public int Turns { get; set; } = 0;
        public int Corruption { get; set; } = 0;
        public int MordorTrackStep { get; set; } = 0;
        public int Progress { get; set; } = 0;
        public int LastKnownDistanceFromRivendell { get; set; } = 0;
        public bool? TookMoria { get; set; }
        public bool Revealed { get; set; } = false;
        public Fellowship Fellowship { get; set; } = new Fellowship();
        public Strategy Strategy { get; set; } = new KillGuide();
        public int EffectiveDistanceFromRivendell
        {
            get
            {
                return Progress + LastKnownDistanceFromRivendell;
            }
        }
        private void EnterMordor()
        {
            HuntBag.EnterMordor();
            if (null == TookMoria)
            {
                TookMoria = 10 == EffectiveDistanceFromRivendell;
            }
            Progress = 0;
            Console.WriteLine("____Enter Mordor____");
        }

        // returns the number of stronghold tiles to take
        private int RevealFellowshipOutsideMordor()
        {
            int strongholdTiles = 0;
            if (null == TookMoria)
            {
                // Take Moria unless you'll eat two stronghold tiles for it. 
                // Need some testing. 
                if (2 == EffectiveDistanceFromRivendell)
                {
                    Console.WriteLine("    In Hollin");
                    TookMoria = true;
                }
                else if (3 == EffectiveDistanceFromRivendell)
                {
                    Console.WriteLine("    In Goblin's Gate");
                    TookMoria = false;
                }
                else if (3 < EffectiveDistanceFromRivendell)
                {
                    Console.WriteLine("    Taking the Moria route");
                    TookMoria = true;
                }
            }
            if (TookMoria ?? false)
            {
                bool touchedMoria = EffectiveDistanceFromRivendell >= 3 && LastKnownDistanceFromRivendell <= 3;
                if (touchedMoria)
                    strongholdTiles +=
                        (LastKnownDistanceFromRivendell <= 3 ? 1 : 0) +
                        (LastKnownDistanceFromRivendell >= 3 ? 1 : 0);
            }
            LastKnownDistanceFromRivendell = EffectiveDistanceFromRivendell;
            Progress = 0;
            Revealed = true;
            if (AtTheGatesOfMordor())
                strongholdTiles++;
            return strongholdTiles;
        }

        void RevealFellowshipOutsideMordorAndResolveStrongholdTiles()
        {
            int ntiles = RevealFellowshipOutsideMordor();
            for (; ntiles > 0; ntiles--)
            {
                var tile = HuntBag.DrawTile();
                Console.WriteLine($"    stronghold tile {tile}");
                Strategy.Hunt(tile.Value(0), false, tile, this);
            }
        }

        public Quest FromRivendell()
        {
            for (Turns = 1; ; Turns++)
            {
                int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
                int freeHuntBoxDiceCount = 0;
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
                        int hits = D6.CountHits(eyes, 6 - freeHuntBoxDiceCount);
                        if (hits > 0)
                        {
                            var tile = HuntBag.DrawTile();
                            int huntValue = tile.Value(hits);
                            bool wasRevealed = Revealed;
                            int newEffectiveDistance = Progress + 1;
                            Console.WriteLine($"  walk {newEffectiveDistance} {Pluralize("step", newEffectiveDistance)} from Rivendell - {huntValue} {Pluralize("hit", huntValue)} - {tile}");
                            Strategy.Hunt(huntValue, tile.Reveal(), tile, this);
                            ++Progress;
                            bool freshlyRevealed = (!wasRevealed) && Revealed;
                            if (freshlyRevealed) RevealFellowshipOutsideMordorAndResolveStrongholdTiles();
                            ++freeHuntBoxDiceCount;
                            Console.WriteLine($"    corruption {Corruption}, {eyes} eyes");
                        }
                        else
                        {
                            Progress++;
                            Console.WriteLine($"  walk {EffectiveDistanceFromRivendell} {Pluralize("step", EffectiveDistanceFromRivendell)} from Rivendell");
                        }
                    }
                    if (IsOver())
                        return this;
                    if (AtTheGatesOfMordor())
                    {
                        EnterMordor();
                        return MordorTrack();
                    }
                }
            }
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
                        Console.WriteLine($"  from step {MordorTrackStep} {tile} = {huntValue}");
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
                Console.WriteLine($"    {Fellowship.Guide} falls to {tileValue} damage");
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

        private string Pluralize(string s, int n)
        {
            return n == 1 ? s : $"{s}s";
        }
    }
}