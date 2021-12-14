using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    class Harris<T> where T : IComparable<T>
    {
        private class Node
        {
            public T Value;
            public Node next;
            public Node(T Val)
            {
                Value = Val;
            }
        }

        private Node Head;
        private Node Tail;
        private Node Temp;

        public void Push(T Val)
        {
            Node NewNode = new Node(Val);
            Node LeftNode = default(Node), RightNode = default(Node);
            do
            {
                if (CAS(ref Head, null, NewNode) && CAS(ref Tail, null, NewNode))
                    return;
                RightNode = Search(Val, ref LeftNode);
                if (RightNode != null && RightNode.Value.CompareTo(Val) == 0)
                    return;
                NewNode.next = RightNode;
                if (LeftNode == null && CAS(ref Head, RightNode, NewNode))
                    return;
                if (CAS(ref LeftNode.next, RightNode, NewNode))
                    return;
            } while (true);
        }

        private Node Search(T val, ref Node leftNode)
        {
            Node RightNode = default(Node), leftNode_next = default(Node);
            do
            {
                Node t = Head;
                do
                {
                    if (t.Value.CompareTo(val) >= 0)
                        break;
                    leftNode = t;
                    leftNode_next = leftNode.next;
                    t = leftNode_next;


                } while (leftNode_next != null);
                RightNode = t;

                if (leftNode == null && CAS(ref RightNode, Head, Head))
                {
                    return RightNode;
                }

                if (leftNode.next == RightNode)
                {
                    return RightNode;
                }

                if (CAS(ref leftNode.next, leftNode_next, RightNode))
                {
                    return RightNode;
                }
            } while (true);
        }

        public void Delete(T Val)
        {
            if (Head == null)
                return;
            Node LeftNode = default(Node), RightNode = default(Node);
            RightNode = Search(Val, ref LeftNode);
            if (RightNode.Value.CompareTo(Val) != 0)
                return;
            if (LeftNode == null && RightNode.Value.CompareTo(Val) == 0)
            {
                do
                {
                    Temp = RightNode;
                } while (!CAS(ref RightNode, Temp, RightNode.next));
                Head = RightNode;
                return;
            }

            do
            {
                Temp = RightNode;
            } while (!CAS(ref RightNode, Temp, RightNode.next));
            LeftNode.next = RightNode;
            if (RightNode.next == null)
                Tail = LeftNode;
        }

        public bool Find(T Val)
        {
            if (Head == null)
                return false;
            Node LeftNode = default(Node), RightNode = default(Node);
            RightNode = Search(Val, ref LeftNode);
            if (RightNode.Value.CompareTo(Val) == 0)
                return true;

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node cur = Head;
            while (cur != null)
            {
                yield return cur.Value;
                cur = cur.next;
            }
        }
        private bool CAS(ref Node location, Node comparand, Node newValue)
        {
            return comparand == Interlocked.CompareExchange<Node>(ref location, newValue, comparand);
        }

    }
}
