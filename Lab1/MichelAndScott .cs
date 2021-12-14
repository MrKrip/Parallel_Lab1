using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1
{
    class MichelAndScott<T>
    {
        private class Node
        {
            public T value;
            public Node next;
            public Node(T Val)
            {
                value = Val;
            }
        }

        private Node Head;
        private Node Tail;
        private Node Temp;

        public void Push(T Val)
        {
            Node NewNode = new Node(Val);
            do
            {
                Temp = Head;
                if (Temp != null)
                {
                    NewNode.next = Temp;
                }
                else
                {
                    CAS(ref Tail, Temp, NewNode);
                }
            } while (!CAS(ref Head, Temp, NewNode));
        }

        public void Pop()
        {
            Node CurrentNode = Head;
            do
            {
                do
                {
                    Temp = CurrentNode;
                } while (!CAS(ref CurrentNode, Temp, Temp.next));
            } while (!CAS(ref CurrentNode.next, Tail, null));
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node cur = Head;
            while (cur != null)
            {
                yield return cur.value;
                cur = cur.next;
            }
        }
        private bool CAS(ref Node location, Node comparand, Node newValue)
        {
            return comparand == Interlocked.CompareExchange<Node>(ref location, newValue, comparand);
        }
    }
}
