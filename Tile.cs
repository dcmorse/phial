namespace phial
{
    abstract class Tile
    {
        public Tile(bool reveal) {
            _Reveal = reveal;
        }
        public abstract int Value(int huntHits);
        private bool _Reveal { get; }
        public bool Reveal() {
            return _Reveal;
        }
        public virtual bool Stop() {
            return false;
        }
        public virtual bool isEye() {
            return false;
        }
        public virtual bool isSpecial() {
            return false;
        }
        public virtual bool isFellowshipSpecial() {
            return false;
        }
        public virtual bool isShadowSpecial() {
            return false;
        }
        public override string ToString() {
            return $"{GetType().Name} {ToStringSuffix()}";
        }
        public string ToStringSuffix() {
            return $"{(Reveal() ? "r" : "")}{(Stop() ? " stop" : "")}";
        }
    }

    class NumericTile : Tile
    {
        public NumericTile(int n, bool reveal) : base(reveal)
        {
            Number = n;
        }
        public int Number { get; }
        public override int Value(int huntHits) {
            return Number;
        }
        public override string ToString() {
            return $"{GetType().Name} {Number}{ToStringSuffix()}";
        }
    }

    class EyeTile : Tile
    {
        public EyeTile() : base(true) {}
        public override bool isEye() {
            return true;
        }
        public override int Value(int huntHits) {
            return huntHits;
        }
    }

    class FellowshipSpecialTile : NumericTile {
        public FellowshipSpecialTile(int n) : base(n, false) {}
        public override bool isSpecial() {
            return true;
        }
        public override bool isFellowshipSpecial() {
            return true;
        }
    }

    class TheRingIsMine : EyeTile {
        public override bool isSpecial() {
            return true;
        }
        public override bool isShadowSpecial() {
            return true;
        }
        public override bool Stop() {
            return true;
        }

        public override string ToString() {
            return $"{GetType().Name} Eye {ToStringSuffix()}";
        }
    }

    class ShadowSpecialNumericTile : NumericTile {
        public ShadowSpecialNumericTile(int n, bool reveal) : base(n, reveal) {}
        public override bool isSpecial() {
            return true;
        }
        public override bool isShadowSpecial() {
            return true;
        }
        public override bool Stop() {
            return true;
        }
    }


    class ShelobsLair : Tile {
        public ShelobsLair() : base(false) {}
        public override bool isSpecial() {
            return true;
        }
        public override bool isShadowSpecial() {
            return true;
        }
        public override bool Stop() {
            return true;
        }
        public override int Value(int huntHits) {
            return Dice.D6();
        }
    }
}