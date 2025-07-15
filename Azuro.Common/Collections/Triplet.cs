using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azuro.Common.Collections
{
    public class Triplet<T1, T2, T3>
    {
        public Triplet( T1 a, T2 b, T3 c )
        {
            A = a;
            B = b;
            C = c;
        }

        public T1 A { get; set; }
        public T2 B { get; set; }
        public T3 C { get; set; }
    }
}
