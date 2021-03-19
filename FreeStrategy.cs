namespace phial
{
    abstract class FreeStrategy
    {
        public abstract void Hunt(int tileValue, bool reveal, Tile tile, Quest quest);
    }

    // The idea here is to have pluggable strategy instances (KillGandalf being one of many) that
    // handle the judgement of how to resolve hunt tiles, but delegate the bolts/nuts of updating the
    // board state back to the quest via Quest#ResolveTileWithGuideCasualty, #TakeRandomCasualty, and #TakeCorruption. 

    class StriderSprint : FreeStrategy
    {
        public StriderSprint(int randomCasualtyCorruption)
        {
            RandomCasualtyCorruption = randomCasualtyCorruption;
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
            else if (guide is Gollum)
            {
                quest.ResolveTileWithGollum(tileValue, reveal, tile, false);
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
    }
}