using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    class CasAtomic<T>
    {
        private static object locker = new object();

        public class CasObj
        {
            public T Val { get; set; }
            public T Compare { get; set; }
            public T NewVal { get; set; }
        }

        public static bool CAS(ref object casObj)
        {
            bool acquiredLock = false;

                CasObj temp = (CasObj)casObj;                
                Monitor.Enter(locker, ref acquiredLock);
                Console.WriteLine($"\nValue : {temp.Val}\nValue to compare : {temp.Compare}\nNew value(if Value == ValueToCompre) : {temp.NewVal}\n");
                if (EqualityComparer<T>.Default.Equals(temp.Val, temp.Compare))
                {
                    temp.Val = temp.Compare;
                    if (acquiredLock) Monitor.Exit(locker);
                    return true;
                }
                if (acquiredLock) Monitor.Exit(locker);
                return false;
        }       
    }
}
