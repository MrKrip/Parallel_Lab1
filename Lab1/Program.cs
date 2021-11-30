using System;
using System.Threading;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            SkipList<int> test = new SkipList<int>(5);
            for(int i=10;i>=0;i--)
            {
                test.Push(i);
            }

            foreach(var a in test)
            {
                Console.WriteLine(a);
            }
            var a1= test.Contains(5);
            test.delete(5);
            a1 = test.Contains(5);
            test.delete(0);
        }
    }
}
