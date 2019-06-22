//Apache2, 2014-present, WinterDev

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
        int _viewportLeft, _viewportTop;
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

            _panel = simpleBox;
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
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        //
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
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

                foreach (UIElement ui in _uiList.GetIter())
                {
                    renderE.AddChild(ui);
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
        public override bool NeedContentLayout => _panel.NeedContentLayout;
        //----------------------------------------------------
        public void AddItem(ListItem ui)
        {
            _items.Add(ui);
            _panel.AddChild(ui);
        }
        //
        public int ItemCount => _items.Count;
        //
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
            _selectedIndex = -1;
            _items.Clear();
            _panel.ClearChildren();
        }
        //----------------------------------------------------

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value < this.ItemCount)
                {
                    if (value < 0)
                    {
                        value = -1;
                    }
                    //-----------------------------
                    if (_selectedIndex != value)
                    {
                        //1. current item
                        if (_selectedIndex > -1)
                        {
                            //switch back    
                            GetItem(_selectedIndex).BackColor = Color.LightGray;
                        }

                        _selectedIndex = value;
                        if (value == -1)
                        {
                            //no selection
                            _selectedItem = null;
                        }
                        else
                        {
                            //highlight selection item
                            _selectedItem = GetItem(value);
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
            MouseDown?.Invoke(this, e);
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {

            MouseUp?.Invoke(this, e);
            base.OnMouseUp(e);
        }
        //
        public override int ViewportLeft => _viewportLeft;
        public override int ViewportTop => _viewportTop;
        //
        public override void SetViewport(int left, int top, object reqBy)
        {
            _viewportLeft = left;
            _viewportTop = top;
            if (this.HasReadyRenderElement)
            {
                _panel.SetViewport(left, top, reqBy);
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
            if (_selectedIndex > -1)
            {
                //find the item height
                int topPos = _selectedItem.Top;
                SetViewport(_viewportLeft, topPos);
            }
        }
        public void EnsureSelectedItemVisibleToTopItem()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                int newtop = _selectedItem.Top;
                SetViewport(_viewportLeft, newtop);
            }

        }
        public void EnsureSelectedItemVisible()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                if (_selectedItem.Top < _viewportTop)
                {
                    //must see entire item
                    int newtop = _selectedItem.Top - (Height / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(_viewportLeft, newtop);
                }
                else if (_selectedItem.Bottom > _viewportTop + Height)
                {
                    int newtop = _selectedItem.Top - (Height * 2 / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(_viewportLeft, newtop);
                }
            }

        }

        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;

    }


    public class ListItem : AbstractRectUI
    {
        CustomContainerRenderBox _primElement;
        CustomTextRun _listItemText;
        string _itemText;
        Color _backColor;
        RequestFont _font;
        //
        public ListItem(int width, int height)
            : base(width, height)
        {
            this.TransparentAllMouseEvents = true;

        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                //1.
                var element = new CustomContainerRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = _backColor;
                element.SetController(this);
                //
                _listItemText = new CustomTextRun(rootgfx, 200, this.Height);
                if (_font != null)
                {
                    _listItemText.RequestFont = _font;
                    //TODO: review how to find 
                    int blankLineHeight = (int)rootgfx.TextServices.MeasureBlankLineHeight(_font);
                    _listItemText.SetHeight(blankLineHeight);
                    element.SetHeight(blankLineHeight);
                }


                element.AddChild(_listItemText);
                _listItemText.TransparentForAllEvents = true;
                if (_itemText != null)
                {                     
                    _listItemText.Text = _itemText;
                }
                _primElement = element;
            }
            return _primElement;
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
                }
            }
        }
        public string Text
        {
            get => _itemText;
            set
            {
                //set content has some effect to its layout
                _itemText = value;
                if (_listItemText != null)
                {
                    _listItemText.Text = value;
                }
            }
        }
        public override void SetFont(RequestFont font)
        {
            //set content has some effect to its layout
            _font = font;
            if (font != null && HasReadyRenderElement)
            {
                _listItemText.RequestFont = font;
            }
        }
    }
}