using static System.Math;
using System.Collections.Generic;

namespace phial
{
    abstract class FreeStrategy
    {
        public abstract void Hunt(int tileValue, bool reveal, Tile tile, Quest quest);
        protected abstract bool DoesGollumReveal(int damage, Quest readonlyQuest);

        public (int, bool, List<Companion>) CalculateGuidePowers(int damage, bool reveal, Tile tile, Quest readonlyQuest)
        {
            var fellowship = readonlyQuest.Fellowship;
            List<Companion> separatedCompanions = new List<Companion>();
            while (fellowship.Guide.GuideSeperateable() && damage > 0)
            {
                var guide = fellowship.Guide;
                fellowship = fellowship.RemoveCompanion(guide);
                separatedCompanions.Add(guide);
                damage = Max(0, damage - guide.Level());
            }
            if (fellowship.Guide is Gollum)
            {
                if (reveal && (tile is NumericTile) && !tile.IsSpecial())
                {
                    reveal = false;
                }
                if (!reveal && !readonlyQuest.Revealed && damage > 0 && DoesGollumReveal(damage, readonlyQuest))
                {
                    reveal = true;
                    --damage;
                }
            }
            return (damage, reveal, separatedCompanions);
        }
    }

    // The idea here is to have pluggable strategy instances (KillGandalf being one of many) that
    // handle the judgement of how to resolve hunt tiles, but delegate the bolts/nuts of updating the
    // board state back to the quest via Quest#ResolveTileWithGuideCasualty, #TakeRandomCasualty, and #TakeCorruption. 

    class StriderSprint : FreeStrategy
    {
        public StriderSprint(int randomCasualtyCorruption, bool carefulGollum)
        {
            RandomCasualtyCorruption = randomCasualtyCorruption;
            CarefulGollum = carefulGollum;
        }

        public int RandomCasualtyCorruption { get; }

        // never uses character cards
        // never uses guide powers
        public override void Hunt(int tileValue, bool reveal, Tile tile, Quest quest)
        {
            var guide = quest.Fellowship.Guide;
            if (guide is Gandalf)
            {
                quest.ResolveTileWithGuideCasualty(tileValue, reveal, tile);
            }
            else if (guide is Strider)
            {
                if (quest.Corruption + tileValue < RandomCasualtyCorruption)
                    quest.ResolveTileWithCorruption(tileValue, reveal, tile);
                else
                    quest.ResolveTileWithRandomCasualty(tileValue, reveal, tile);
            }
            else if (guide.Level() <= tileValue)
            {
                quest.ResolveTileWithGuideCasualty(tileValue, reveal, tile);
            }
            else
            {
                quest.ResolveTileWithRandomCasualty(tileValue, reveal, tile);
            }

            // fancier versions would first apply guide powers before calling a final tile resolution function.
            // Thats why we pass the reveal, even though we could read it off the tile - smeagol might toggle the reveal. 
        }
        private bool CarefulGollum { get; }
        protected override bool DoesGollumReveal(int damage, Quest readonlyQuest)
        {
            if (CarefulGollum)
            {
                var corruptionLeft = 12 - damage - readonlyQuest.Corruption;
                var mordorSpacesToGo = readonlyQuest.IsInMordor ? 5 - readonlyQuest.MordorTrackStep : 5;
                var wildernessSpacesToGo = readonlyQuest.IsInMordor ? 0 : readonlyQuest.DistanceToGatesOfMordor();
                return corruptionLeft < 2 * mordorSpacesToGo + wildernessSpacesToGo;
            }
            else
            {
                // even reckless gollum reveals to prevent losing the game.
                return damage + readonlyQuest.Corruption == 12;
            }
        }
    }
}