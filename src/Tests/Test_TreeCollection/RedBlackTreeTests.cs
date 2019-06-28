using System;
using System.Collections.Generic;
using PixelFarm.TreeCollection;
using System.Diagnostics;

namespace Test_TreeCollection
{
    public class RedBlackTreeTests
    {
        class TestNode : IRedBlackTreeNode<TestNode>, IComparable
        {
            int val;

            public TestNode(int val)
            {
                this.val = val;
            }


            public void UpdateAugmentedData()
            {
            }

            public TestNode Parent
            {
                get;
                set;
            }

            public TestNode Left
            {
                get;
                set;
            }

            public TestNode Right
            {
                get;
                set;
            }

            public RedBlackColor Color
            {
                get;
                set;
            }
            public int CompareTo(object obj)
            {
                return val.CompareTo(((TestNode)obj).val);
            }
            public int CompareTo(TestNode obj)
            {
                return this.val.CompareTo(obj.val);
            }

        }


        public void TestRemoveBug()
        {
            var tree = new RedBlackTree<TestNode>();
            TestNode t1 = new TestNode(1);
            TestNode t2 = new TestNode(2);
            TestNode t3 = new TestNode(3);

            tree.Add(t1);
            tree.InsertRight(t1, t2);
            tree.InsertLeft(t1, t3);
            tree.Remove(t1);

            Debug.Assert(2 == tree.Count);
        }


        public void TestAddBug()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var tree = new RedBlackTree<TestNode>();
            sw.Reset();
            sw.Start();

            var list1 = new List<TestNode>();
            for (int i = 100000; i >= 0; --i)
            {
                list1.Add(new TestNode(i));
            }
            for (int i = 100000; i >= 0; --i)
            {
                list1.Insert(100000, new TestNode(-i));
            }
            sw.Stop();
            long ms2 = sw.ElapsedMilliseconds;
            //
            sw.Reset();
            sw.Start();
            TestNode latest = null;
            for (int i = 100000; i >= 0; --i)
            {
                tree.Add(latest = new TestNode(0));
            }

            for (int i = 100000; i >= 0; --i)
            {
                tree.InsertBefore(latest, new TestNode(i));
            }

            sw.Stop();
            long ms1 = sw.ElapsedMilliseconds;
            //TestNode t1 = new TestNode(1);
            //TestNode t2 = new TestNode(2);
            //TestNode t3 = new TestNode(3);
            //tree.Add(t1);
            //tree.Add(t2);
            //tree.Add(t3);

            //test with linked
            //test with linked list node


            LinkedList<TestNode> linkedList = new LinkedList<TestNode>();
            sw.Reset();
            sw.Start();
            for (int i = 100000; i >= 0; --i)
            {
                linkedList.AddLast(new TestNode(i));
            }
            var lastNode = linkedList.Last;
            for (int i = 100000; i >= 0; --i)
            {
                linkedList.AddBefore(lastNode, new TestNode(i));
            }

            sw.Stop();
            long ms0 = sw.ElapsedMilliseconds;
            List<TestNode> list2 = new List<TestNode>();
            sw.Reset();
            sw.Start();
            for (int i = 100000; i >= 0; --i)
            {
                list2.Add(new TestNode(i));
            }
            for (int i = 100000; i >= 0; --i)
            {
                list2.Insert(100000, new TestNode(-i));
            }
            sw.Stop();
            long ms5 = sw.ElapsedMilliseconds;
        }
    }

}