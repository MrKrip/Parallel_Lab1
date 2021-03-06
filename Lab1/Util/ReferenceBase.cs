using Lab1.Util.Atomic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Util
{
    class ReferenceBase<T>
    {
        public readonly T Value;
        public readonly AtomicBool Marked;

        public ReferenceBase(T value, bool marked)
        {
            this.Value = value;
            this.Marked = new AtomicBool(marked);
        }
    }
}
