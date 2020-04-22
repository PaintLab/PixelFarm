//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
using System;

namespace LayoutFarm.CustomWidgets
{
    public class UIFloatWindow : AbstractBox, ITopWindowBox
    {
        IPlatformWindowBox _platformWindowBox;
        AbstractRectUI _content;
        public UIFloatWindow(int w, int h)
            : base(w, h)
        {

        }
        public void SetContent(AbstractRectUI content)
        {
            //clear existing elem
            RenderElement primRenderE = CurrentPrimaryRenderElement;
            if (primRenderE != null)
            {
                primRenderE.ClearAllChildren();
                primRenderE.AddChild(content);
            }
            else
            {
                //??
                throw new NotSupportedException();
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