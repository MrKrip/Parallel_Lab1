using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.Util.Interfaces
{
    interface IBorderline<T>
    {
        T MinValue();
        T MaxValue();
        T Value { get; set; }
    }
}
