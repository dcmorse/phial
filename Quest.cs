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
            ShadowRolled = 7 - shadowHuntAllocated;
            HuntBag = new HuntBag();
            Log = log;
        }

        private FreeStrategy FreeStrategy { get; }
        private int ShadowHuntAllocated { get; }
        private int ShadowRolled { get; }
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

        public bool AtTheGatesOfMordor()
        {
            int moves = EffectiveDistanceFromRivendell;
            int movesNeeded = 10 + (TookMoria ?? true ? 0 : 1);
            return moves >= movesNeeded;
        }
        public int Turns { get; set; } = 0;
        public int Corruption { get; set; } = 0;
        public int MordorTrackStep { get; set; } = 0;
        public int Progress { get; set; } = 0;
        public int LastKnownDistanceFromRivendell { get; set; } = 0;
        public bool? TookMoria { get; set; }
        public bool Revealed { get; set; } = false;
        public Fellowship Fellowship { get; set; } = new Fellowship();
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
                int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
                int freeHuntBoxDiceCount = 0;
                var freeDice = new FreeActionDiceRoll(FreeActionDiceCount);
                Log.Log($"Turn {Turns}: {eyes} eyes, {freeDice}");

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
                        int hits = D6.CountHits(eyes, 6 - freeHuntBoxDiceCount);
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
                            Log.Log($"    corruption {Corruption}, {eyes} eyes");
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
                int eyes = ShadowHuntAllocated + D6.CountHits(ShadowRolled, 6);
                bool movedOrHidThisTurn = false;
                var freeDice = new FreeActionDiceRoll(FreeActionDiceCount);

                Log.Log($"Turn {Turns}: {eyes} eyes, {freeDice}");

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
                        int huntValue = tile.Value(eyes);
                        Log.Log($"  from step {MordorTrackStep} {tile} = {huntValue}");
                        FreeStrategy.Hunt(huntValue, tile.Reveal(), tile, this);
                        if (!tile.Stop()) MordorTrackStep++;
                        eyes++;
                        Log.Log($"    corruption {Corruption}, {eyes} eyes");
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

        public void ResolveTileWithGuideCasualty(int tileValue, bool reveal, Tile tile)
        {
            ResolveTileWithCasualty(Fellowship.Guide, tileValue, reveal, tile);
        }
        public void ResolveTileWithRandomCasualty(int tileValue, bool reveal, Tile tile)
        {
            ResolveTileWithCasualty(Fellowship.Random(), tileValue, reveal, tile);
        }
        public void ResolveTileWithCasualty(Companion companion, int tileValue, bool reveal, Tile tile)
        {
            Revealed = Revealed || reveal;
            if (tileValue > 0)
            {
                int damage = Math.Max(0, tileValue - companion.Level());
                Corruption += damage;
                Log.Log($"    {companion} falls to {tileValue} damage");
                TakeCasualty(companion);
            }
            else
            {
                Corruption = Math.Max(0, Corruption + tileValue); // heal from hunt tiles
                if (tileValue < 0)
                    Log.Log($"    Frodo heals to {Corruption} corruption");
            }
        }

        public void ResolveTileWithCorruption(int tileValue, bool reveal, Tile tile)
        {
            Revealed = Revealed || reveal;
            Corruption = Math.Max(0, Corruption + tileValue);
            Log.Log($"    taking {tileValue} corruption");
        }

        public void ResolveTileWithGollum(int tileValue, bool reveal, Tile tile, bool reduceDamageByRevealing)
        {
            if (!(tile.IsEye() || tile.IsShadowSpecial()))
            {
                reveal = false;
                if (reduceDamageByRevealing && tileValue > 0)
                {
                    --tileValue;
                    reveal = true;
                }
            }
            ResolveTileWithCorruption(tileValue, reveal, tile);
        }

        void TakeCasualty(Companion c)
        {
            if (c is Gandalf)
                GandalfDeadTheFirstTime = true;
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