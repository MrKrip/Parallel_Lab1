using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Node CurrentNode = Levels[MaxLevel - 1];
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
                    if (tempNode != null)
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
                }

                if (CurrentNode.next == null && CurrentNode.Value.CompareTo(Val) < 0)
                {
                    do
                    {
                        CurrentNode = tempNode;
                        while (CurrentNode == tempNode && CurrentNode.next != null && CurrentNode.next.Value.CompareTo(NewNode.Value) < 0)
                        {
                            CurrentNode = CurrentNode.next;
                            tempNode = tempNode.next;
                        }
                        var next = CurrentNode.next;
                        NewNode.next = next;
                        NewNode.previous = CurrentNode;
                    } while (!CAS(ref CurrentNode.next, tempNode.next, NewNode));

                }
                else
                {
                    do
                    {
                        CurrentNode = tempNode;

                        var prev = CurrentNode.previous;
                        NewNode.previous = prev;
                        CurrentNode.previous = NewNode;
                        NewNode.next = CurrentNode;
                        if (prev != null)
                        {
                            prev.next = NewNode;
                        }
                    } while (!CAS(ref CurrentNode.previous,tempNode.previous,NewNode));
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
                    return;
                }

                Node temp = new Node(Val);
                tempNode.above = temp;
                temp.below = tempNode;
                if (Levels[i] == null || TempNodes[i] == null)
                {
                    Levels[i] = temp;
                    tempNode = temp;
                }
                else if (TempNodes[i].Value.CompareTo(Val) >= 0)
                {
                    temp.next = TempNodes[i];
                    temp.previous = TempNodes[i].previous;
                    TempNodes[i].previous = temp;
                    if (temp.previous == null)
                    {
                        Levels[i] = temp;
                    }
                    tempNode = temp;
                }
                else
                {
                    temp.next = TempNodes[i].next;
                    temp.previous = TempNodes[i];
                    TempNodes[i].next = temp;
                    tempNode = temp;
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
                        if (tempNode.Value.CompareTo(Value) == 0 && tempNode.below != null)
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
                                while (tempNode.Value.CompareTo(Value) > 0 && tempNode.previous != null)
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

                if (tempNode.Value.CompareTo(Value) == 0)
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
                            tempNode.next.previous = tempNode.previous;
                            tempNode = tempNode.above;
                        }
                    }
                    else
                    {

                        for (int i = 0; i < MaxLevel; i++)
                        {
                            if (tempNode == null)
                            {
                                return;
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

        public IEnumerator<T> GetEnumerator()
        {
            Node cur = Levels[0];
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.next;
            }
        }

        public bool Contains(T Value)
        {
            if (Levels[0] == null)
            {
                return false;
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
                        if (tempNode.Value.CompareTo(Value) == 0)
                        {
                            return true;
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
                                    if (tempNode.Value.CompareTo(Value) == 0)
                                    {
                                        return true;
                                    }
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
                return false;
            }
        }

        private bool CAS(ref Node location, Node comparand, Node newValue)
        {
            return comparand == Interlocked.CompareExchange<Node>(ref location, newValue, comparand);
        }
    }


}
