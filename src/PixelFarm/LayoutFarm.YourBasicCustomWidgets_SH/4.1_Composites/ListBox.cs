//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
using PixelFarm.CpuBlit;
namespace LayoutFarm.CustomWidgets
{
    public class ListBox : AbstractBox
    {
        public delegate void ListItemMouseHandler(object sender, UIMouseEventArgs e);
        public delegate void ListItemKeyboardHandler(object sender, UIKeyEventArgs e);
        //composite          

        List<ListItem> _items = new List<ListItem>();
        int _selectedIndex = -1;//default = no selection
        ListItem _selectedItem = null;
        UICollection _uiCollection;
        Box _bgBox;
        public event ListItemMouseHandler ListItemMouseEvent;
        public event ListItemKeyboardHandler ListItemKeyboardEvent;

        public ListBox(int width, int height)
            : base(width, height)
        {

#if DEBUG
            //dbugBreakMe = true;
#endif
            this.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            this.BackColor = KnownColors.LightGray;
            this.AcceptKeyboardFocus = true;
            this.NeedClipArea = true;

            ListItemNormalColor = KnownColors.LightGray;
            ListItemSelectedColor = KnownColors.Yellow;

            _uiCollection = new UICollection(this);
            _bgBox = new Box(width, height);
            _uiCollection.AddUI(_bgBox);
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            //check what item is selected
            if (e.SourceHitElement is ListItem src)
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

                ListItemMouseEvent?.Invoke(this, e);
            }
            base.OnMouseDown(e);
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            if (_selectedItem != null && ListItemKeyboardEvent != null)
            {

                ListItemKeyboardEvent(this, e);
            }
            base.OnKeyDown(e);
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            base.OnDoubleClick(e);
            //raise event mouse double click
            if (e.SourceHitElement is ListItem src && ListItemMouseEvent != null)
            {
                ListItemMouseEvent(this, e);
            }
        }


        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!HasReadyRenderElement)
            {
                CustomRenderBox newone = base.GetPrimaryRenderElement(rootgfx) as CustomRenderBox;
                newone.LayoutHint = this.ContentLayoutKind;
                newone.HasSpecificWidthAndHeight = true;

                foreach (UIElement child in _uiCollection.GetIter())
                {
                    newone.AddChild(child.GetPrimaryRenderElement(rootgfx));
                }

                return newone;
            }
            else
            {
                return base.GetPrimaryRenderElement(rootgfx);
            }
        }
        public int ItemCount => _items.Count;
        public void AddItem(ListItem item)
        {
            _items.Add(item); //logical collection
            if (CurrentPrimaryRenderElement is CustomRenderBox customPrim)
            {
                customPrim.AddChild(item);
                item.InvalidateGraphics();
            }
        }
        public void InsertItem(int index, ListItem item)
        {
            ListItem existing = _items[index];
            _items.Insert(index, item);
            if (CurrentPrimaryRenderElement is CustomRenderBox customPrim)
            {
                customPrim.InsertBefore(existing.CurrentPrimaryRenderElement, item.GetPrimaryRenderElement(customPrim.Root));
            }
        }
        public void RemoveAt(int index)
        {
            ListItem item = _items[index];
            if (CurrentPrimaryRenderElement is CustomRenderBox customPrim)
            {
                customPrim.RemoveChild(item.GetPrimaryRenderElement(customPrim.Root));
            }
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
            if (CurrentPrimaryRenderElement is CustomRenderBox customPrim)
            {
                customPrim.RemoveChild(item.GetPrimaryRenderElement(customPrim.Root));
                customPrim.InvalidateGraphics();
            }
        }
        public void ClearItems()
        {
            _selectedIndex = -1;
            _items.Clear();
            if (CurrentPrimaryRenderElement is CustomRenderBox customPrim)
            {
                customPrim.ClearAllChildren();
                customPrim.InvalidateGraphics();
            }

        }
        //----------------------------------------------------

        public Color ListItemNormalColor { get; set; }
        public Color ListItemSelectedColor { get; set; }
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
                            GetItem(_selectedIndex).BackColor = ListItemNormalColor;
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
                            _selectedItem.BackColor = ListItemSelectedColor;
                        }
                    }
                }
                else
                {
                    //do nothing 
                }
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
                SetViewport(ViewportLeft, topPos);
            }
        }
        public void EnsureSelectedItemVisibleToTopItem()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                int newtop = _selectedItem.Top;
                SetViewport(ViewportLeft, newtop);
            }

        }
        public void EnsureSelectedItemVisible()
        {
            if (_selectedIndex > -1)
            {
                //check if selected item is visible
                //if not bring them into view 
                if (_selectedItem.Top < ViewportTop)
                {
                    //must see entire item
                    int newtop = _selectedItem.Top - (Height / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(ViewportLeft, newtop);
                }
                else if (_selectedItem.Bottom > ViewportTop + Height)
                {
                    int newtop = _selectedItem.Top - (Height * 2 / 3);
                    if (newtop < 0)
                    {
                        newtop = 0;
                    }
                    SetViewport(ViewportLeft, newtop);
                }
            }

        }

    }


    public class ListItem : AbstractRectUI
    {
        CustomRenderBox _primElement;
        CustomTextRun _listItemText;
        string _itemText;
        Color _backColor;
        RequestFont _font;

        Color _normalStateColor; //TODO: add theme
        public ListItem(int width, int height)
            : base(width, height)
        {
            this.TransparentForMouseEvents = true;
            //TODO: add theme
            _normalStateColor = BackColor = KnownColors.LightGray;
        }
        public object Tag { get; set; }
        protected override void OnMouseEnter(UIMouseMoveEventArgs e)
        {
            _normalStateColor = BackColor;

            using (Tools.More.BorrowChromaTool(out var chroma))
            {
                BackColor = chroma.SetColor(_normalStateColor).Brighten(0.5);
            }
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
            BackColor = _normalStateColor; //switch back
            base.OnMouseLeave(e);
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                //1.
                var element = new CustomRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = _backColor;
                element.SetController(this);
                //
                _listItemText = new CustomTextRun(rootgfx, 200, this.Height);
                _listItemText.DrawTextTechnique = DrawTextTechnique.LcdSubPix;

                if (_font != null)
                {
                    _listItemText.RequestFont = _font;
                    //TODO: review how to find 
                    int blankLineHeight = (int)rootgfx.TextServices.MeasureBlankLineHeight(_font);
                    _listItemText.SetHeight(blankLineHeight);
                    element.SetHeight(blankLineHeight);
                }


                element.AddChild(_listItemText);
                _listItemText.TransparentForMouseEvents = true;
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
                //set current back color
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