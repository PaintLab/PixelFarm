using System;
using System.Collections.Generic;
using PixelFarm.TreeCollection;
using System.Diagnostics;

namespace Test_TreeCollection
{
    public class RedBlackTreeTests
    {
        class TestNode : IRedBlackTreeNode, IComparable
        {
            int val;

            public TestNode(int val)
            {
                this.val = val;
            }

            #region IRedBlackTreeNode implementation
            public void UpdateAugmentedData()
            {
            }

            public IRedBlackTreeNode Parent
            {
                get;
                set;
            }

            public IRedBlackTreeNode Left
            {
                get;
                set;
            }

            public IRedBlackTreeNode Right
            {
                get;
                set;
            }

            public RedBlackColor Color
            {
                get;
                set;
            }
            #endregion

            #region IComparable implementation
            public int CompareTo(object obj)
            {
                return val.CompareTo(((TestNode)obj).val);
            }
            #endregion
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
                tree.InsertBefore(latest, new TestNode(-i));
            }

            sw.Stop();
            long ms1 = sw.ElapsedMilliseconds;
            //TestNode t1 = new TestNode(1);
            //TestNode t2 = new TestNode(2);
            //TestNode t3 = new TestNode(3);
            //tree.Add(t1);
            //tree.Add(t2);
            //tree.Add(t3);


        }
    }

}