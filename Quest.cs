namespace phial
{
    class Quest
    {
        public Quest(int freeDice, int shadowHuntAllocated, int shadowRolled, Tile[] tiles)
        {
            FreeDice = freeDice;
            ShadowHuntAllocated = shadowHuntAllocated;
            ShadowRolled = shadowRolled;
            HuntPool = tiles;
        }

        private int FreeDice { get; }
        private int ShadowHuntAllocated { get; }
        private int ShadowRolled { get; }
        private Tile[] HuntPool { get; }

        // a fairly dumb read/write statebag for reporting quest statistics and recording who won
        public bool isOver()
        {
            return isCorrupted() || isRingDestroyed();
        }
        public bool isCorrupted()
        {
            return Corruption >= 12;
        }
        public bool isRingDestroyed()
        {
            return Step >= 5;
        }
        public int Turns { get; set; } = 0;
        public int Corruption { get; set; } = 0;
        public int Step { get; set; } = 0;
        public bool Revealed { get; set; } = false;

        public Quest run()
        {
            for (Turns = 1; ; Turns++)
            {
                for (int swords = Dice.CountHits(FreeDice, 4); swords > 0; swords--)
                {
                    Step++;
                    if (isOver())
                        return this;
                }
            }
        }

        public override string ToString()
        {
            if (isCorrupted())
                return $"Corruption win at step {Step}";
            else if (isRingDestroyed())
                return $"Ring Destroyed at corruption {Corruption}";
            else
                return $"Step {Step} Corruption {Corruption}{(Revealed ? " revealed" : "")}";
        }
    }
}