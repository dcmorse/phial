using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace phial
{
    class Histogram<T> where T : IComparable
    {
        private Dictionary<T, int> D { get; } = new Dictionary<T, int>();

        public int Fetch(T key)
        {
            int n;
            if (D.TryGetValue(key, out n))
                return n;
            else return 0;
        }

        public void Increment(T key)
        {
            if (D.ContainsKey(key))
                ++D[key];
            else D[key] = 1;
        }

        public int Count() {
            return D.Sum(kv => kv.Value);
        }

        public (T, T) Domain() {
            var min = D.Keys.Aggregate((l, r) => l.CompareTo(r) <= 0 ? l : r);
            var max = D.Keys.Aggregate((l, r) => l.CompareTo(r) >= 0 ? l : r);
            return (min, max);
        }

        public T Median() {
            return D.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("");
            foreach (T key in D.Keys.ToList().OrderBy(n=>n))
                sb.Append($"{key}: {D[key]}  ");
            return sb.ToString();
        }
    }
}