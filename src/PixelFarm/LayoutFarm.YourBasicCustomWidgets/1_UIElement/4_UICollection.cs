//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    public class UICollection
    {
        List<UIElement> _uiList = new List<UIElement>();
        UIElement _owner;
        public UICollection(UIElement owner)
        {
            _owner = owner;
        }
        public int Count
        {
            get { return this._uiList.Count; }
        }
        public void AddUI(UIElement ui)
        {
#if DEBUG
            if (this._owner == ui)
                throw new Exception("cyclic!");
#endif
            _uiList.Add(ui);
            ui.ParentUI = this._owner;
        }
        public bool RemoveUI(UIElement ui)
        {
            //remove specific ui 
            if (_uiList.Remove(ui))
            {
                ui.ParentUI = null;//clear parent
                return true;
            }
            else
            {
                return false;
            }
        }
        public void RemoveAt(int index)
        {
            UIElement ui = _uiList[index];
            _uiList.RemoveAt(index);
            ui.ParentUI = null;
        }
        public void Clear()
        {
            //clear all parent relation
            for (int i = _uiList.Count - 1; i >= 0; --i)
            {
                _uiList[i].ParentUI = null;
            }
            this._uiList.Clear();
        }
        public UIElement GetElement(int index)
        {
            return this._uiList[index];
        }
    }
}