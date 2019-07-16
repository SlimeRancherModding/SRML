using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Utils
{
    public class Tuple<T1,T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple() { }
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2> tuple &&
                   Item1.Equals(tuple.Item1) &&
                   Item2.Equals(tuple.Item2);
        }

        public override int GetHashCode()
        {
            var hashCode = -1030903623;
            hashCode = hashCode * -1521134295 + Item1.GetHashCode();
            hashCode = hashCode * -1521134295 + Item2.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tuple<T1, T2> left, Tuple<T1, T2> right)
        {
            return !(left == right);
        }
    }
}
