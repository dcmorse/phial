namespace phial
{
    abstract class Tile
    {
        public Tile(bool reveal)
        {
            _Reveal = reveal;
        }
        public abstract int Value(int huntHits);
        private bool _Reveal { get; }
        public bool Reveal()
        {
            return _Reveal;
        }
        public virtual bool Stop()
        {
            return false;
        }
        public virtual bool IsEye()
        {
            return false;
        }
        public bool IsSpecial()
        {
            return IsFellowshipSpecial() || IsShadowSpecial();
        }
        public virtual bool IsFellowshipSpecial()
        {
            return false;
        }
        public virtual bool IsShadowSpecial()
        {
            return false;
        }
        public override string ToString()
        {
            return $"[{ToStringHelper()}]";
        }
        public virtual string ToStringHelper()
        {
            return $"{GetType().Name} {ToStringSuffix()}";
        }
        public string ToStringSuffix()
        {
            return $"{(Reveal() ? "r" : "")}{(Stop() ? " stop" : "")}";
        }
        public static Tile[] GreyTiles = new Tile[] {
            new NumericTile(1, false),
            new NumericTile(1, false),
            new NumericTile(2, false),
            new NumericTile(2, false),
            new NumericTile(3, false),
            new NumericTile(3, false),
            new NumericTile(3, false),
            new NumericTile(0, true),
            new NumericTile(0, true),
            new NumericTile(1, true),
            new NumericTile(1, true),
            new NumericTile(2, true),
            new EyeTile(),
            new EyeTile(),
            new EyeTile(),
            new EyeTile(),
        };

        public static Tile[] RedTiles = new Tile[] {
            new TheRingIsMine(),
            new ShelobsLair(),
            new ShadowSpecialNumericTile(1, true),
            new ShadowSpecialNumericTile(3, false)
        };

        public static Tile[] BlueTiles = new Tile[] {
            new FellowshipSpecialTile(-2),
            new FellowshipSpecialTile(-1),
            new FellowshipSpecialTile(0),
            new FellowshipSpecialTile(0)
        };
    }

    class NumericTile : Tile
    {
        public NumericTile(int n, bool reveal) : base(reveal)
        {
            Number = n;
        }
        public int Number { get; }
        public override int Value(int huntHits)
        {
            return Number;
        }
        public override string ToStringHelper()
        {
            return $"{Number}{ToStringSuffix()}";
        }
    }

    class EyeTile : Tile
    {
        public EyeTile() : base(true) { }
        public override bool IsEye()
        {
            return true;
        }
        public override int Value(int huntHits)
        {
            return huntHits;
        }
        public override string ToStringHelper()
        {
            return "Eye";
        }
    }

    class FellowshipSpecialTile : NumericTile
    {
        public FellowshipSpecialTile(int n) : base(n, false) { }
        public override bool IsFellowshipSpecial()
        {
            return true;
        }

        public override string ToStringHelper()
        {
            return $"Blue {base.ToStringHelper()}";
        }
    }

    class TheRingIsMine : EyeTile
    {
        public override bool IsShadowSpecial()
        {
            return true;
        }
        public override bool Stop()
        {
            return true;
        }

        public override string ToStringHelper()
        {
            return $"Red Eye {ToStringSuffix()}";
        }
    }

    class ShadowSpecialNumericTile : NumericTile
    {
        public ShadowSpecialNumericTile(int n, bool reveal) : base(n, reveal) { }
        public override bool IsShadowSpecial()
        {
            return true;
        }
        public override bool Stop()
        {
            return true;
        }

        public override string ToStringHelper()
        {
            return $"Red {base.ToStringHelper()}";
        }
    }


    class ShelobsLair : Tile
    {
        public ShelobsLair() : base(false) { }
        public override bool IsShadowSpecial()
        {
            return true;
        }
        public override bool Stop()
        {
            return true;
        }
        public override int Value(int huntHits)
        {
            return D6.roll();
        }

        public override string ToStringHelper()
        {
            return $"Red D6 {ToStringSuffix()}";
        }
    }
}