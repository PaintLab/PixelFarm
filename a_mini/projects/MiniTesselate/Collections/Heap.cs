//BSD 2014 WinterDev

using System;
using System.Collections.Generic;

namespace MiniCollection
{

    public class IPriorityQueueHandle<T> { } 


    public class MaxFirstQueue<T>
        where T : IComparable<T>
    {
        List<T> innerList = new List<T>();
        bool isSorted = false;
        public MaxFirstQueue()
        {

        }
        public bool IsEmpty
        {
            get
            {
                return innerList.Count == 0;
            }
        }
        static int MaxFirstSort(T t1, T t2)
        {
            return t2.CompareTo(t1);
        }
        public T DeleteMin()
        {
            //find min and delete 
            if (!isSorted)
            {
                innerList.Sort(MaxFirstSort);
                isSorted = true;
            }
            int last = innerList.Count - 1;
            var tmp = innerList[last];
            innerList.RemoveAt(last);
            return tmp;
        }
        public T FindMin()
        {
            if (!isSorted)
            {
                innerList.Sort(MaxFirstSort);
                isSorted = true;
            }
            return innerList[innerList.Count - 1];
        }
        public void Add(ref IPriorityQueueHandle<T> handle, T data)
        {
            innerList.Add(data);
            handle = null;
            isSorted = false;
        }
        public void Add(T data)
        {
            innerList.Add(data);
            isSorted = false;
        }
        public void Delete(IPriorityQueueHandle<T> handle)
        {
            //delete specfic node

        }

    }




}