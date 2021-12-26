using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Harris<int> test = new Harris<int>();

            //Thread kek = new Thread(() =>
            //{
            //    for (int i = 0; i < 3; i++)
            //        test.Push(i);
            //});
            //Thread kek1 = new Thread(() =>
            //{
            //    for (int i = 3; i < 5; i++)
            //        test.Push(i);
            //});
            //Thread kek2 = new Thread(() =>
            //{
            //    for (int i = 5; i < 8; i++)
            //        test.Push(i);
            //});

            //kek.Start();
            //kek1.Start();
            //kek2.Start();

            //while (kek.IsAlive || kek1.IsAlive || kek2.IsAlive)
            //{ }
            //foreach (var a in test)
            //{
            //    Console.Write(a + "->");
            //}
            //Console.WriteLine();
            //Console.WriteLine(new string('*', 30));
            //Task<bool> task = Task.Factory.StartNew<bool>(() => test.Find(4));
            //Console.WriteLine(task.Result);
            //Console.WriteLine(new string('*', 30));
            //Thread kek3 = new Thread(() => test.Delete(4));
            //kek3.Start();
            //while (kek3.IsAlive)
            //{ }
            //foreach (var a in test)
            //{
            //    Console.Write(a + "->");
            //}
            //Console.WriteLine();
            //Console.WriteLine(new string('*', 30));
            //task = Task.Factory.StartNew<bool>(() => test.Find(4));
            //Console.WriteLine(task.Result);
            //Console.WriteLine(new string('*', 30));
            SkipList<int> skipList = new SkipList<int>();
            Random random = new Random();
            List<int> pusher = new List<int>();
            List<int> deleter = new List<int>();
            ConcurrentStack<SkipList<int>.Node> elements = new ConcurrentStack<SkipList<int>.Node>();
            object randomLock = new object();
            int key;
            lock (randomLock)
            {
                key = random.Next(0, 100000);
            }
            SkipList<int>.Node node = new SkipList<int>.Node(10,key);
            var push = skipList.Push(node);
            if (push)
            {
                elements.Push(node);
                pusher.Add(node.Value);
            }
            if (!elements.TryPop(out var output)) return;
            var delete = skipList.Delete(output);
            if (delete)
            {
                deleter.Add(output.Value);
            }

        }
    }
}
