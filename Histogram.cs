using System.Collections.Generic;
using System.Text;

namespace phial
{
    class Histogram<T>
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

        public override string ToString()
        {
            var sb = new StringBuilder("");
            foreach (T key in D.Keys)
                sb.Append($" {key} => {D[key]}");
            return sb.ToString();
        }
    }
}