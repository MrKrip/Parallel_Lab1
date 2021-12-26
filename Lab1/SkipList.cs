using Lab1.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class SkipList<T>
    {
        private Node head { get; } = new Node(int.MinValue);

        private Node tail { get; } = new Node(int.MaxValue);

        public SkipList()
        {
            for (var i = 0; i < head.Next.Length; ++i)
            {
                head.Next[i] = new MarkedReference<Node>(tail, false);
            }
        }


        public bool Push(Node node)
        {
            var PreviousNode = new Node[33];
            var NextNode = new Node[33];

            while (true)
            {
                if (Find(node, ref PreviousNode, ref NextNode))
                {
                    return false;
                }
                var highestPoint = node.HighestPoint;

                for (var level = 0; level <= highestPoint; level++)
                {
                    var tempElem = NextNode[level];
                    node.Next[level] = new MarkedReference<Node>(tempElem, false);
                }

                var currentPrevious = PreviousNode[0];
                var currentNext = NextNode[0];

                node.Next[0] = new MarkedReference<Node>(currentNext, false);


                for (var level = 1; level <= highestPoint; level++)
                {
                    while (true)
                    {
                        currentPrevious = PreviousNode[level];
                        currentNext = NextNode[level];

                        if (currentPrevious.Next[level].CompareAndExchange(node, false, currentNext, false))
                        {
                            break;
                        }

                        Find(node, ref PreviousNode, ref NextNode);
                    }
                }

                return true;
            }
        }
        public bool Delete(Node node)
        {
            var previousItem = new Node[33];
            var NextItem = new Node[33];

            while (true)
            {


                if (!Find(node, ref previousItem, ref NextItem))
                {
                    return false;
                }

                Node currentNext;
                for (var level = node.HighestPoint; level > 0; level--)
                {
                    var isMarked = false;
                    currentNext = node.Next[level].Get(ref isMarked);

                    while (!isMarked)
                    {
                        node.Next[level].CompareAndExchange(currentNext, true, currentNext, false);
                        currentNext = node.Next[level].Get(ref isMarked);
                    }
                }

                var marked = false;
                currentNext = node.Next[0].Get(ref marked);

                while (true)
                {
                    var iMarkedIt = node.Next[0].CompareAndExchange(currentNext, true, currentNext, false);
                    currentNext = NextItem[0].Next[0].Get(ref marked);

                    if (iMarkedIt)
                    {
                        Find(node, ref previousItem, ref NextItem);
                        return true;
                    }

                    if (marked)
                    {
                        return false;
                    }
                }
            }
        }
        private bool Find(Node node, ref Node[] previousItem, ref Node[] NextItem)
        {
            var marked = false;
            var isRetryNeeded = false;
            Node searchPoint = null;

            while (true)
            {
                var currentPrevious = head;
                for (var level = 32; level >= 0; level--)
                {
                    searchPoint = currentPrevious.Next[level].Value;
                    while (true)
                    {
                        var currentNext = searchPoint.Next[level].Get(ref marked);
                        while (marked)
                        {
                            var snip = currentPrevious.Next[level].CompareAndExchange(currentNext, false, searchPoint, false);
                            if (!snip)
                            {
                                isRetryNeeded = true;
                                break;
                            }

                            searchPoint = currentPrevious.Next[level].Value;
                            currentNext = searchPoint.Next[level].Get(ref marked);
                        }

                        if (isRetryNeeded)
                        {
                            break;
                        }

                        if (searchPoint.NodeKey < node.NodeKey)
                        {
                            currentPrevious = searchPoint;
                            searchPoint = currentNext;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (isRetryNeeded)
                    {
                        isRetryNeeded = false;
                        continue;
                    }

                    previousItem[level] = currentPrevious;
                    NextItem[level] = searchPoint;
                }

                return searchPoint != null && searchPoint.NodeKey == node.NodeKey;
            }
        }
        public class Node
        {
            private static uint _randomSeed;
            public T Value { get; }

            public int NodeKey { get; }

            public MarkedReference<Node>[] Next { get; }

            public int HighestPoint { get; }


            public Node(int key)
            {
                NodeKey = key;
                Next = new MarkedReference<Node>[33];
                for (var i = 0; i < Next.Length; ++i)
                {
                    Next[i] = new MarkedReference<Node>(null, false);
                }
                HighestPoint = 32;
            }

            public Node(T value, int key)
            {
                Value = value;
                NodeKey = key;
                var height = RandomLevel();
                Next = new MarkedReference<Node>[height + 1];
                for (var i = 0; i < Next.Length; ++i)
                {
                    Next[i] = new MarkedReference<Node>(null, false);
                }
                HighestPoint = height;
            }

            private static int RandomLevel()
            {
                var x = _randomSeed;
                x ^= x << 13;
                x ^= x >> 17;
                _randomSeed = x ^= x << 5;
                if ((x & 0x80000001) != 0)
                {
                    return 0;
                }

                var level = 1;
                while (((x >>= 1) & 1) != 0)
                {
                    level++;
                }

                return Math.Min(level, 32);
            }
        }
    }
}
