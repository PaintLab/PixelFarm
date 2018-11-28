﻿//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{


    public class ListView : AbstractRectUI
    {


        public delegate void ListItemMouseHandler(object sender, UIMouseEventArgs e);
        public delegate void ListItemKeyboardHandler(object sender, UIKeyEventArgs e);

        //composite          
        CustomRenderBox _primElement;//background
        Color _backColor = Color.LightGray;
        int _viewportX, _viewportY;
        UICollection _uiList;
        List<ListItem> _items = new List<ListItem>();
        int _selectedIndex = -1;//default = no selection
        ListItem _selectedItem = null;
        Box _panel;


        public event ListItemMouseHandler ListItemMouseEvent;
        public event ListItemKeyboardHandler ListItemKeyboardEvent;

        public ListView(int width, int height)
            : base(width, height)
        {
            _uiList = new UICollection(this);

            var simpleBox = new Box(width, height);
            simpleBox.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            simpleBox.BackColor = Color.LightGray;
            simpleBox.MouseDown += panel_MouseDown;
            simpleBox.MouseDoubleClick += panel_MouseDoubleClick;
            simpleBox.AcceptKeyboardFocus = true;
            simpleBox.KeyDown += simpleBox_KeyDown;
            simpleBox.NeedClipArea = true;

            this._panel = simpleBox;
            _uiList.AddUI(_panel);
        }
        public override void PerformContentLayout()
        {
            _panel.PerformContentLayout();
        }

        void simpleBox_KeyDown(object sender, UIKeyEventArgs e)
        {
            if (_selectedItem != null && ListItemKeyboardEvent != null)
            {
                e.UIEventName = UIEventName.KeyDown;
                ListItemKeyboardEvent(this, e);
            }
        }
        void panel_MouseDoubleClick(object sender, UIMouseEventArgs e)
        {
            //raise event mouse double click
            var src = e.SourceHitElement as ListItem;
            if (src != null && ListItemMouseEvent != null)
            {
                e.UIEventName = UIEventName.DblClick;
                ListItemMouseEvent(this, e);
            }
        }
        void panel_MouseDown(object sender, UIMouseEventArgs e)
        {
            //check what item is selected
            var src = e.SourceHitElement as ListItem;
            if (src != null)
            {
                //make this as current selected list item
                //find index ?
                //TODO: review, for faster find list item index method
                int found = -1;
                for (int i = _items.Count - 1; i >= 0; --i)
                {
                    if (_items[i] == src)
                    {
                        found = i;
                        break;
                    }
                }
                if (found > -1)
                {
                    SelectedIndex = found;
                }
                if (ListItemMouseEvent != null)
                {
                    e.UIEventName = UIEventName.MouseDown;
                    ListItemMouseEvent(this, e);
                }
            }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this._primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this._primElement; }
        }
        public Color BackColor
        {
            get { return this._backColor; }
            set
            {
                this._backColor = value;
                if (HasReadyRenderElement)
                {
                    this._primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = _backColor;
                renderE.SetController(this);
                renderE.HasSpecificWidthAndHeight = true;
                //------------------------------------------------
                //create visual layer

                int uiCount = this._uiList.Count;
                for (int m = 0; m < uiCount; ++m)
                {
                    renderE.AddChild(_uiList.GetElement(m));
                }

                //---------------------------------
                renderE.SetVisible(this.Visible);
                _primElement = renderE;
            }
            return _primElement;
        }

        protected override void OnContentLayout()
        {
            _panel.PerformContentLayout();
        }
        public override bool NeedContentLayout
        {
            get
            {
                return this._panel.NeedContentLayout;
            }
        }
        //----------------------------------------------------
        public void AddItem(ListItem ui)
        {
            _items.Add(ui);
            _panel.AddChild(ui);
        }
        public int ItemCount
        {
            get { return this._items.Count; }
        }
        public void RemoveAt(int index)
        {
            var item = _items[index];
            _panel.RemoveChild(item);
            _items.RemoveAt(index);
        }
        public ListItem GetItem(int index)
        {
            if (index < 0)
            {
                return null;
            }
            else
            {
                return _items[index];
            }
        }
        public void Remove(ListItem item)
        {
            _items.Remove(item);
            _panel.RemoveChild(item);
        }
        public void ClearItems()
        {
            this._selectedIndex = -1;
            this._items.Clear();
            this._panel.ClearChildren();
        }
        //----------------------------------------------------

        public int SelectedIndex
        {
            get { return this._selectedIndex; }
            set
            {
                if (value < this.ItemCount)
                {
                    if (value < 0)
                    {
                        value = -1;
                    }
                    //-----------------------------
                    if (this._selectedIndex != value)
                    {
                        //1. current item
                        if (_selectedIndex > -1)
                        {
                            //switch back    
                            GetItem(this._selectedIndex).BackColor = Color.LightGray;
                        }

                        this._selectedIndex = value;
                        if (value == -1)
                        {
                            //no selection
                            this._selectedItem = null;
                        }
                        else
                        {
                            //highlight selection item
                            this._selectedItem = GetItem(value);
                            _selectedItem.BackColor = Color.Yellow;
                        }
                    }
                }
                else
                {
                    //do nothing 
                }
            }
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }


        public override int ViewportX
        {
            get { return this._viewportX; }
        }
        public override int ViewportY
        {
            get { return this._viewportY; }
        }

        public override void SetViewport(int x, int y, object reqBy)
        {
            this._viewportX = x;
            this._viewportY = y;
            if (this.HasReadyRenderElement)
            {
                this._panel.SetViewport(x, y, reqBy);
            }
        }
        public override int InnerHeight
        {
            get
            {
                if (_items.Count > 0)
                {
                    ListItem lastOne = _items[_items.Count - 1];
                    return lastOne.Bottom;
                }
                return this.Height;
            }
        }

        public void ScrollToSelectedItem()
        {
            //EnsureSelectedItemVisible();
            if (this._selectedIndex > -1)
            {
                //find the item height
                int topPos = _selectedItem.Top;
                SetViewport(this._viewportX, topPos);
            }
        }
        public void EnsureSelectedItemVisibleToTopItem()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                int newtop = _selectedItem.Top;
                SetViewport(this._viewportX, newtop);
            }

        }
        public void EnsureSelectedItemVisible()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                if (_selectedItem.Top < _viewportY)
                {
                    //must see entire item
                    int newtop = _selectedItem.Top - (Height / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(this._viewportX, newtop);
                }
                else if (_selectedItem.Bottom > _viewportY + Height)
                {
                    int newtop = _selectedItem.Top - (Height * 2 / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(this._viewportX, newtop);
                }
            }

        }

        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listview");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }


    public class ListItem : AbstractRectUI
    {
        CustomContainerRenderBox primElement;
        CustomTextRun listItemText;
        string itemText;
        Color backColor;
        RequestFont font;


        public ListItem(int width, int height)
            : base(width, height)
        {
            this.TransparentAllMouseEvents = true;

        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return primElement != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                //1.
                var element = new CustomContainerRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = this.backColor;
                element.SetController(this);
                //
                listItemText = new CustomTextRun(rootgfx, 200, this.Height);
                if (font != null)
                {
                    listItemText.RequestFont = font;
                    //TODO: review how to find 
                    int blankLineHeight = (int)rootgfx.TextServices.MeasureBlankLineHeight(font);
                    listItemText.SetHeight(blankLineHeight);
                    element.SetHeight(blankLineHeight);
                }


                element.AddChild(listItemText);
                listItemText.TransparentForAllEvents = true;
                if (this.itemText != null)
                {
                    listItemText.NeedClipArea = true;
                    listItemText.Text = this.itemText;
                }

                element.NeedClipArea = true;
                this.primElement = element;
            }
            return primElement;
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        public string Text
        {
            get { return this.itemText; }
            set
            {
                //set content has some effect to its layout
                this.itemText = value;
                if (listItemText != null)
                {
                    listItemText.Text = value;
                }
            }
        }
        public override void SetFont(RequestFont font)
        {
            //set content has some effect to its layout
            this.font = font;
            if (font != null && HasReadyRenderElement)
            {
                listItemText.RequestFont = font;
            }
        }
        //-----------------  

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "listitem");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}