namespace phial
{
    abstract class Strategy
    {
        public abstract void Hunt(int tileValue, bool reveal, Tile tile, Quest quest);
    }

    // The idea here is to have pluggable strategy instances (KillGandalf being one of many) that
    // handle the judgement of how to resolve hunt tiles, but delegate the bolts/nuts of updating the
    // board state back to the quest via Quest#TakeGuideCasualty, #TakeRandomCasualty, and #TakeCorruption. 

    class KillGuide : Strategy
    {
        // never uses character cards
        // never uses guide powers
        // directs all damage onto the guide. 
        public override void Hunt(int tileValue, bool reveal, Tile tile, Quest quest)
        {
            // fancier versions would first apply guide powers before calling a final tile resolution function.
            // Thats why we pass the reveal, even though we could read it off the tile - smeagol might toggle the reveal. 
            quest.TakeGuideCasualty(tileValue, reveal, tile);
        }
    }
}