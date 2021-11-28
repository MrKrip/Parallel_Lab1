using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    class SkipList<T> where T : IComparable<T>
    {
        private Node[] Levels;
        private Random _rand = new Random();
        private int MaxLevel;
        private class Node
        {
            public Node next;
            public Node previous;
            public Node above;
            public Node below;
            public T Value;
            public Node(T val)
            {
                Value = val;
            }
        }

        public SkipList(int Lvl)
        {
            Levels = new Node[Lvl];
            MaxLevel = Lvl;
        }

        public void Push(T Val)
        {
            Node NewNode = new Node(Val);
            Node tempNode = Levels[MaxLevel - 1];
            Node[] TempNodes = new Node[MaxLevel];
            if (Levels[0] == null)
            {
                Levels[0] = NewNode;
                tempNode = NewNode;
            }
            else
            {
                for (int i = MaxLevel - 1; i >= 0; i--)
                {
                    if (tempNode == null)
                    {
                        tempNode = Levels[i];
                    }
                    if (Levels[i] != null)
                    {
                        if (tempNode.Value.CompareTo(Val) >= 0)
                        {
                            if (i != 0)
                            {
                                if (tempNode.previous != null && tempNode.previous.Value.CompareTo(Val) > 0)
                                {
                                    tempNode = tempNode.previous;
                                }
                                TempNodes[i] = tempNode;
                                tempNode = tempNode.below;
                            }
                            else
                            {
                                while (tempNode.Value.CompareTo(Val) >= 0 && tempNode.previous != null)
                                {
                                    tempNode = tempNode.previous;
                                }
                            }
                        }
                        else
                        {
                            if (tempNode.next == null && i != 0)
                            {
                                TempNodes[i] = tempNode;
                                tempNode = tempNode.below;
                            }
                            else
                            {
                                while (tempNode.Value.CompareTo(Val) < 0 && tempNode.next != null)
                                {
                                    tempNode = tempNode.next;
                                }
                            }
                        }
                    }
                    else
                    {
                        tempNode = Levels[i];
                    }
                }

                if (tempNode.next == null)
                {
                    var next = tempNode.next;
                    NewNode.next = next;
                    NewNode.previous = tempNode;
                    tempNode.next = NewNode;
                }
                else
                {
                    var prev = tempNode.previous;
                    tempNode.previous = NewNode;
                    NewNode.previous = prev;
                    NewNode.next = tempNode;
                    if (NewNode.previous == null)
                    {
                        Levels[0] = NewNode;
                    }
                }
                tempNode = NewNode;
            }

            for (int i = 1; i < MaxLevel; i++)
            {
                if (_rand.Next(10) % 2 == 0)
                {
                    break;
                }

                Node temp = new Node(Val);
                tempNode.above = temp;
                temp.below = tempNode;
                if (Levels[i] == null)
                {
                    Levels[i] = temp;
                }
                else if (TempNodes[i].previous == null)
                {
                    temp.next = TempNodes[i];
                    TempNodes[i].previous = temp;
                    Levels[i] = temp;
                }
                else
                {
                    temp.next = TempNodes[i].next;
                    temp.previous = TempNodes[i];
                    TempNodes[i].next = temp;
                }
            }

        }

        public void delete(T Value)
        {
            if (Levels[0] == null)
            {
                return;
            }
            else
            {
                Node tempNode = Levels[MaxLevel - 1];
                for (int i = MaxLevel - 1; i >= 0; i--)
                {
                    if (Levels[i] != null)
                    {
                        if (tempNode == null)
                        {
                            tempNode = Levels[i];
                        }
                        if (tempNode.Value.CompareTo(Value) == 0 && i != 0)
                        {
                            tempNode = tempNode.below;
                        }
                        else if (tempNode.Value.CompareTo(Value) > 0)
                        {
                            if (i != 0)
                            {
                                while (tempNode.previous != null && tempNode.previous.Value.CompareTo(Value) > 0)
                                {
                                    tempNode = tempNode.previous;
                                }
                                tempNode = tempNode.below;
                            }
                            else
                            {
                                while (tempNode.Value.CompareTo(Value) >= 0 && tempNode.previous != null)
                                {
                                    tempNode = tempNode.previous;
                                }
                            }
                        }
                        else
                        {
                            if (tempNode.next == null && i != 0)
                            {
                                tempNode = tempNode.below;
                            }
                            else
                            {
                                while (tempNode.Value.CompareTo(Value) < 0 && tempNode.next != null)
                                {
                                    tempNode = tempNode.next;
                                }
                            }
                        }
                    }
                }

                if (tempNode.Value.CompareTo(Value)==0)
                {
                    if (tempNode.previous == null)
                    {
                        for (int i = 0; i < MaxLevel; i++)
                        {
                            if (tempNode == null)
                            {
                                break;
                            }
                            Levels[i] = tempNode.next;
                            tempNode.next.previous = null;
                            tempNode = tempNode.above;
                        }
                    }
                    else
                    {

                        for (int i = 0; i < MaxLevel; i++)
                        {
                            if (tempNode == null)
                            {
                                break;
                            }
                            var next = tempNode.next;
                            var prev = tempNode.previous;
                            next.previous = prev;
                            prev.next = next;
                            tempNode = tempNode.above;
                        }

                    }
                }

            }
        }
    }
}
