//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
using System;

namespace LayoutFarm.CustomWidgets
{
    public class UIFloatWindow : AbstractBox, ITopWindowBox, ISimpleContainerUI
    {
        IPlatformWindowBox _platformWindowBox;
        UIList<UIElement> _list = new UIList<UIElement>();
        UIElement _content;
        public UIFloatWindow(int w, int h)
            : base(w, h)
        {

        }
        protected override IUICollection<UIElement> GetDefaultChildrenIter() => _list;
        public void AddContent(UIElement content)
        {
            _list.Clear(this);  //clear existing elem
            _content = content;
            if (content != null)
            {
                _list.Add(this, content);
            }
        }

        IPlatformWindowBox ITopWindowBox.PlatformWinBox
        {
            get => _platformWindowBox;
            set
            {
                bool isFirstTime = _platformWindowBox == null;
                _platformWindowBox = value;
                if (isFirstTime)
                {
                    _platformWindowBox.SetLocation(this.Left, this.Top);
                    _platformWindowBox.SetSize(this.Width, this.Height);
                    _platformWindowBox.Visible = this.Visible;
                }
            }
        }
        public override void SetLocation(int left, int top)
        {
            if (_platformWindowBox != null)
            {
                //use location relative to top parent/control
                _platformWindowBox.SetLocation(left, top);
            }
            else
            {
                base.SetLocation(left, top);
            }
        }
        public override void SetSize(int width, int height)
        {
            _platformWindowBox?.SetSize(width, height);

        }
        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (_platformWindowBox != null)
                {
                    _platformWindowBox.Visible = value;
                }
                base.Visible = value;
            }
        }
    }
}