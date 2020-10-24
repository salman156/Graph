using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Node
    {
        public int i;
        public int weight;
        public Node next;
        public Node(int ti, int tweight, Node tnext = null)
        {
            i = ti;
            weight = tweight;
            next = tnext;
        }
    }
    class Versh
    {
        public int x;
        public int y;
        public Versh vnext;
        public Node next;
        public void InsertEdge(int ti, int tweight = 0)
        {
            if (next == null && tweight != 0)
            {
                next = new Node(ti, tweight);
            }
            else
            {
                if (tweight != 0)
                {
                    Node p = next;
                    while (p.next != null && p.i < ti)
                    {
                        p = p.next;
                    }
                    if (p.i < ti)
                    {
                        p.next = new Node(ti, tweight);
                    }
                    else if (p.i == ti)
                    {
                        p.weight = tweight;
                    }
                    else
                    {
                        Node s = new Node(p.i, p.weight, p.next);
                        p.i = ti;
                        p.weight = tweight;
                        p.next = s;
                    }
                }
                else if (next != null)
                {
                    Node p = next;
                    if (next != null && p.i == ti)
                        next = next.next;
                    while (p.next != null && p.next.i != ti)
                    {
                        p = p.next;
                    }
                    if (p.next != null)
                    {
                        p.next = p.next.next;
                    }
                }
            }
        }
        public Versh(int tx, int ty, Node tnext = null)
        {
            x = tx;
            y = ty;
            vnext = null;
            next = tnext;
        }
    }

    class PathsNode
    {
        public int[] path;
        public int kpath;
        public int weight;
        public PathsNode next;
        public PathsNode(int[] tpath, int tkpath, int tweight = 0, PathsNode tnext = null)
        {
            path = tpath;
            kpath = tkpath;
            weight = tweight;
            next = tnext;
        }
    }
    class StackNode
    {
        public int i;
        public StackNode next;
        public StackNode(int ti, StackNode tnext = null)
        {
            i = ti;
            next = tnext;
        }
    }
    class Stack
    {
        public StackNode head;
        public int size;
        public Stack() { head = null; size = 0; }
        public void Push(int i)
        {
            head = new StackNode(i, head);
        }
        public int Pop()
        {
            int s=-1;
            if (head != null)
            {
                s = head.i;
                head = head.next;
            }
            return s;
        }
        public int Top()
        {
            return head.i;
        }
        public bool IsEmpty()
        {
            return head == null;
        }
        ~Stack()
        {
            head = null;
        }
    }

    abstract class Graph
    {
        protected Stack St;
        public PathsNode Paths { get; set; }
        public int size;
        public Graph()
        {
            St = new Stack();
            size = 0;
            Paths = null;
        }
        public bool IsPathFound()
        {
            return Paths != null;
        }
        public abstract bool Lies(ref int x, ref int y);
        public abstract void AddVersh(int x, int y);
        public abstract void AddRebro(int t, int j, int weight = 0);
        public abstract void GetCoord(int i, out int x, out int y);
        public bool DFS(int Vn, int Vk, int V1, int V2)
        {
            int[] R = new int[size];
            for (int i = 0; i < size; i++) R[i] = -1;
            int t, j;
            St.Push(Vn); R[Vn] = 0;
            j = -1;
            do
            {
                t = St.Top();
                j = OtherVersh(t, j);
                if (j == -1)
                {
                    j = t; R[t] = -1; St.Pop();
                }
                else if (j != Vk && R[j] < 0)
                {
                    St.Push(j); R[j] = 1; j = -1;
                }
                else if (j == Vk)
                {
                    St.Push(j);
                    Stack s = new Stack();
                    int[] path = new int[size + 1];
                    int kpath = 0;
                    while (!St.IsEmpty())
                    {
                        path[kpath] = St.Pop();
                        s.Push(path[kpath]);
                        kpath++;
                    }
                    while (!s.IsEmpty())
                        St.Push(s.Pop());
                    Condition(path, kpath, V1, V2);
                    St.Pop();
                }
            } while (!St.IsEmpty());
            return false;
        }
        abstract public int OtherVersh(int start, int end = -1);
        public abstract int GetWeight(int t, int j);
        public abstract int Get(int x, int y);
        public abstract void Clear();
        abstract public int Condition(int[] path, int kpath, int V1, int V2);
    }
    class GraphList : Graph
    {
        public Versh head;
        public GraphList()
        {
            head = null;
        }
        public override int Get(int x, int y)
        {
            Versh p = head;
            int i = 0;
            while (p != null)
            {
                if (x == p.x && y == p.y)
                    return i;
                i++;
                p = p.vnext;
            }
            return -1;
        }
        public override void GetCoord(int i, out int x, out int y)
        {
            Versh p = head;
            while (i > 0)
            {
                p = p.vnext;
                i--;
            }
            x = p.x;
            y = p.y;
        }
        public override bool Lies(ref int x, ref int y)
        {
            Versh p = head;
            for (; p != null; p = p.vnext)
            {
                if (Math.Abs(x - p.x) <= 20 && Math.Abs(y - p.y) <= 20)
                {
                    x = p.x;
                    y = p.y;
                    return true;
                }
            }
            return false;
        }
        public override void AddVersh(int x, int y)
        {
            Versh p = head;
            if (head == null)
            {
                head = new Versh(x, y);
                size++;
                return;
            }
            while (p.vnext != null)
                p = p.vnext;
            p.vnext = new Versh(x, y);
            size++;
        }
        public override void AddRebro(int t, int j, int weight = 0)
        {
            Versh p = head;
            int t1 = t, j1 = j;
            while (p != null && (t >= 0 || j >= 0))
            {
                if (t1 == 0)
                {
                    p.InsertEdge(j, weight);
                }
                if (j1 == 0)
                {
                    p.InsertEdge(t, weight);
                }
                t1--;
                j1--;
                p = p.vnext;
            }
        }
        public override int GetWeight(int t, int j)
        {
            Versh p = head;
            while (p != null && t != 0)
            {
                p = p.vnext;
                t--;
            }
            if (p != null)
            {
                Node s = p.next;
                while (s != null && s.i != j)
                {
                    s = s.next;
                }
                if (s != null)
                    return s.weight;
                else
                    return -1;
            }
            else
                return -1;
        }
        public override int Condition(int[] path, int kpath, int A, int B)
        {
            bool rebro = true;
            int weight = 0;
            for (int i = 0; i < kpath - 1; i++)
            {
                if (path[i]==A &&path[i+1]==B || path[i]==B && path[i+1]==A)
                    rebro = false;
                weight += GetWeight(path[i], path[i + 1]);
            }
            if (rebro)
            {
                if(Paths==null || weight<Paths.weight)
                    Paths = new PathsNode(path, kpath, weight);
                else if(weight==Paths.weight)
                {
                    Paths = new PathsNode(path, kpath, weight, Paths);
                }
                return 1;
            }
            else return 0;
        }
        public bool ContainsPath(int[] path, int kpath, int A, int B)
        {
            for (int i = 0; i < kpath - 1; i++)
                if (path[i] == A && path[i + 1] == B || path[i] == B && path[i + 1] == A)
                    return true;
            return false;
        }
        public bool Sravnenie(int[] path1, int kpath1, int[] path2, int kpath2)
        {
            if (kpath1 != kpath2)
            {
                return true;
            }
            else
            {
                bool flag = false;
                for (int i = 0; i < kpath1 - 1; i++)
                    if (!ContainsPath(path2, kpath2, path1[i], path1[i + 1]))
                        return true;
                return false;
            }
        }
        public override int OtherVersh(int start, int end = -1)
        {
            if (end == -1)
            {
                Versh p = head;
                while (p != null && start != 0)
                {
                    p = p.vnext;
                    start--;
                }
                if (p != null)
                    return p.next.i;
                else return -1;
            }
            else
            {
                Versh p = head;
                while (p != null && start != 0)
                {
                    p = p.vnext;
                    start--;
                }
                if (p != null)
                {
                    Node s = p.next;
                    while (s != null && s.i != end)
                    {
                        s = s.next;
                    }
                    if (s == null)
                        return -1;
                    else
                    if (s.next != null)
                        return s.next.i;
                    else return -1;
                }
                else return -1;
            }
        }
        public override void Clear()
        {
            Versh p = head;
            while (p != null)
            {
                Versh t = p;
                Node s = p.next, s1 = p.next;
                while (s != null)
                {
                    s = s.next;
                    s1.next = null;
                    s1 = s;
                }
                p = p.vnext;
                if (t != null)
                    t.vnext = null;
                t = p;
            }
            size = 0;
        }
    }
}
