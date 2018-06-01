//Apache2, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    public class UICollection
    {
        List<UIElement> uiList = new List<UIElement>();
         
        public UICollection( )
        {
             
        }
        public void AddUI(UIElement ui)
        {
//#if DEBUG
//            if (this.owner == ui)
//                throw new Exception("cyclic!");
//#endif
           
            uiList.Add(ui);
        }
        public int Count
        {
            get { return this.uiList.Count; }
        }
        public bool RemoveUI(UIElement ui)
        {
            //remove specific ui
            return uiList.Remove(ui);
        }
        public void RemoveAt(int index)
        {
            uiList.RemoveAt(index);
        }
        public void Clear()
        {
            this.uiList.Clear();
        }
        public UIElement GetElement(int index)
        {
            return this.uiList[index];
        }
    }
}