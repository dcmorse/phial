using System.Collections.Generic;
using System;
namespace phial
{
    class Quest
    {
        public Quest(FreeStrategy strategy, int shadowHuntAllocated, ILogger log)
        {
            FreeStrategy = strategy;
            ShadowHuntAllocated = shadowHuntAllocated;
            HuntBag = new HuntBag();
            Log = log;
        }

        private FreeStrategy FreeStrategy { get; }
        private int ShadowHuntAllocated { get; }
        private int Minions { get; set; }
        private HuntBag HuntBag { get; }
        private ILogger Log { get; }

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
            return MordorTrackStep >= 5 && !IsCorrupted();
        }

        public int DistanceToGatesOfMordor()
        {
            int moves = EffectiveDistanceFromRivendell;
            int movesNeeded = 10 + (TookMoria ?? true ? 0 : 1);
            return Math.Max(movesNeeded - moves, 0);
        }
        public bool AtTheGatesOfMordor()
        {
            return 0 == DistanceToGatesOfMordor();
        }
        public int Turns { get; private set; } = 0;
        public int Corruption { get; private set; } = 0;
        public int MordorTrackStep { get; private set; } = 0;
        public bool IsInMordor { get; private set; } = false;
        public int Progress { get; private set; } = 0;
        public int LastKnownDistanceFromRivendell { get; private set; } = 0;
        public bool? TookMoria { get; private set; }
        public bool Revealed { get; private set; } = false;
        public Fellowship Fellowship { get; private set; } = new Fellowship();
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
            IsInMordor = true;
            Log.Log($"____Enter Mordor____  took moria = {TookMoria}");
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
                    Log.Log("    In Hollin");
                    TookMoria = true;
                }
                else if (3 == EffectiveDistanceFromRivendell)
                {
                    Log.Log("    In Goblin's Gate");
                    TookMoria = false;
                }
                else if (3 < EffectiveDistanceFromRivendell)
                {
                    Log.Log("    Taking the Moria route");
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
                Log.Log($"    stronghold tile {tile}");
                FreeStrategy.Hunt(tile.Value(0), false, tile, this);
            }
        }

        bool GandalfDeadTheFirstTime { get; set; } = false; // set to true when he leaves fsp
        int FreeActionDiceCount { get; set; } = 4;

        bool PromoteGandalfIfAble(FreeActionDiceRoll freeDice)
        {
            if (GandalfDeadTheFirstTime && freeDice.WillOfTheWests > 0)
            {
                GandalfDeadTheFirstTime = false;
                --freeDice.WillOfTheWests;
                ++FreeActionDiceCount;
                Log.Log("  ____Gandalf the White____");
                return true;
            }
            else
                return false;
        }

        public Quest FromRivendell()
        {
            for (Turns = 1; ; Turns++)
            {
                if (AtTheGatesOfMordor())
                {
                    EnterMordor();
                    return MordorTrack();
                }
                ShadowActionDiceRoll shadowDice = new ShadowActionDiceRoll(7 + Minions, ShadowHuntAllocated);
                int freeHuntBoxDiceCount = 0;
                var freeDice = new FreeActionDiceRoll(FreeActionDiceCount);
                Log.Log($"Turn {Turns}: {shadowDice.Eyes} eyes, {freeDice}");

                while (freeDice.CharacterOrWills > 0 || StriderCanHide(freeDice))
                {
                    if (PromoteGandalfIfAble(freeDice))
                    {
                        // do nothing
                    }
                    else if (StriderCanHide(freeDice))
                    {
                        Revealed = false;
                        Log.Log($"  Strider hide");
                        freeDice.SpendMostUselessDie();
                    }
                    else if (Revealed)
                    {
                        Revealed = false;
                        Log.Log($"  hide");
                        freeDice.SpendCharacterOrWill();
                    }
                    else if (AtTheGatesOfMordor())
                    {
                        EnterMordor();
                        return MordorTrack();
                    }
                    else
                    {
                        int hits = D6.CountHits(shadowDice.Eyes, 6 - freeHuntBoxDiceCount);
                        if (hits > 0)
                        {
                            var tile = HuntBag.DrawTile();
                            int huntValue = tile.Value(hits);
                            bool wasRevealed = Revealed;
                            Log.Log($"  walk {EffectiveDistanceFromRivendell + 1} {Pluralize("step", EffectiveDistanceFromRivendell + 1)} from Rivendell  - {hits} {Pluralize("hit", hits)} - {tile}");
                            FreeStrategy.Hunt(huntValue, tile.Reveal(), tile, this);
                            ++Progress;
                            bool freshlyRevealed = (!wasRevealed) && Revealed;
                            if (freshlyRevealed) RevealFellowshipOutsideMordorAndResolveStrongholdTiles();
                            ++freeHuntBoxDiceCount;
                            Log.Log($"    corruption {Corruption}, {shadowDice.Eyes} eyes");
                        }
                        else
                        {
                            Progress++;
                            Log.Log($"  walk {EffectiveDistanceFromRivendell} {Pluralize("step", EffectiveDistanceFromRivendell)} from Rivendell");
                        }
                        freeDice.SpendCharacterOrWill();
                    }
                    if (IsOver())
                        return this;
                }
            }
        }

        public Quest MordorTrack()
        {
            for (; ; Turns++)
            {
                // int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
                ShadowActionDiceRoll shadowDice = new ShadowActionDiceRoll(7 + Minions, ShadowHuntAllocated);
                bool movedOrHidThisTurn = false;
                var freeDice = new FreeActionDiceRoll(FreeActionDiceCount);
                int freeHuntBox = 0;

                Log.Log($"Turn {Turns}: {shadowDice.Eyes} eyes, {freeDice}");

                while (freeDice.CharacterOrWills > 0 || StriderCanHide(freeDice))
                {
                    if (StriderCanHide(freeDice))
                    {
                        Revealed = false;
                        Log.Log($"  Strider hide");
                        freeDice.SpendMostUselessDie();
                    }
                    else if (Revealed)
                    {
                        Revealed = false;
                        Log.Log($"  hide");
                        freeDice.SpendCharacterOrWill();
                    }
                    else
                    {
                        var tile = HuntBag.DrawTile();
                        int huntValue = tile.Value(shadowDice.Eyes + freeHuntBox);
                        Log.Log($"  from step {MordorTrackStep} {tile} = {huntValue}");
                        FreeStrategy.Hunt(huntValue, tile.Reveal(), tile, this);
                        if (!tile.Stop()) MordorTrackStep++;
                        freeHuntBox++;
                        Log.Log($"    corruption {Corruption}, {shadowDice.Eyes} eyes");
                        freeDice.SpendCharacterOrWill();
                        if (IsOver())
                            return this;
                    }
                    movedOrHidThisTurn = true;
                }
                if (!movedOrHidThisTurn)
                {
                    Corruption++;
                    Log.Log("  lazy hobbit corruption");
                }
            }
        }

        private bool StriderCanHide(FreeActionDiceRoll freeDice)
        {
            return Revealed && (Fellowship.Guide is Strider) && freeDice.Count > 0;
        }

        public void ResolveTileWithGuideCasualty(int damage, bool reveal, Tile tile)
        {
            (damage, reveal) = ApplyGuidePowers(damage, reveal, tile);
            ResolveTileWithCasualty(Fellowship.Guide, damage, reveal, tile);
        }

        public void ResolveTileWithRandomCasualty(int damage, bool reveal, Tile tile)
        {
            (damage, reveal) = ApplyGuidePowers(damage, reveal, tile);
            ResolveTileWithCasualty(Fellowship.Random(), damage, reveal, tile);
        }

        private (int, bool) ApplyGuidePowers(int tileValue, bool reveal, Tile tile)
        {
            List<Companion> separatedCompanions;
            (tileValue, reveal, separatedCompanions) = FreeStrategy.CalculateGuidePowers(tileValue, reveal, tile, this);
            if (separatedCompanions.Count > 0)
                SeparateCompanions(separatedCompanions);
            return (tileValue, reveal);
        }
        public void ResolveTileWithCasualty(Companion companion, int tileValue, bool reveal, Tile tile)
        {
            if (tileValue > 0)
            {
                int damage = Math.Max(0, tileValue - companion.Level());
                // more companion separation here. 
                Corruption += damage;
                if (companion.IsRemovable())
                {
                    Log.Log($"    {companion} falls to {tileValue} damage");
                    TakeCasualty(companion);
                }
            }
            else
            {
                Corruption = Math.Max(0, Corruption + tileValue); // heal from hunt tiles
                if (tileValue < 0)
                    Log.Log($"    Frodo heals to {Corruption} corruption");
            }
            Revealed = Revealed || reveal;
        }

        public void ResolveTileWithCorruption(int tileValue, bool reveal, Tile tile)
        {
            Revealed = Revealed || reveal;
            Corruption = Math.Max(0, Corruption + tileValue);
            Log.Log($"    taking {tileValue} corruption");
        }


        public void TakeCasualty(Companion c)
        {
            if (c is Gandalf)
                GandalfDeadTheFirstTime = true;
            Fellowship = Fellowship.RemoveCompanion(c);
        }

        public void SeparateCompanions(List<Companion> companions)
        {
            Log.Log($"    {String.Join(", ", companions)} separate");
            foreach (Companion c in companions)
                Fellowship = Fellowship.RemoveCompanion(c);
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