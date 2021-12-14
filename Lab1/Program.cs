using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Harris<int> test = new Harris<int>();

            Thread kek = new Thread(() =>
            {
                for (int i = 0; i < 3; i++)
                    test.Push(i);
            });
            Thread kek1 = new Thread(() =>
            {
                for (int i = 3; i < 5; i++)
                    test.Push(i);
            });
            Thread kek2 = new Thread(() =>
            {
                for (int i = 5; i < 8; i++)
                    test.Push(i);
            });

            kek.Start();
            kek1.Start();
            kek2.Start();

            while (kek.IsAlive || kek1.IsAlive || kek2.IsAlive)
            { }
            foreach (var a in test)
            {
                Console.Write(a + "->");
            }
            Console.WriteLine();
            Console.WriteLine(new string('*', 30));
            Task<bool> task = Task.Factory.StartNew<bool>(() => test.Find(4));
            Console.WriteLine(task.Result);
            Console.WriteLine(new string('*', 30));
            Thread kek3 = new Thread(() => test.Delete(4));
            kek3.Start();
            while (kek3.IsAlive)
            { }
            foreach (var a in test)
            {
                Console.Write(a + "->");
            }
            Console.WriteLine();
            Console.WriteLine(new string('*', 30));
            task = Task.Factory.StartNew<bool>(() => test.Find(4));
            Console.WriteLine(task.Result);
            Console.WriteLine(new string('*', 30));
        }
    }
}
