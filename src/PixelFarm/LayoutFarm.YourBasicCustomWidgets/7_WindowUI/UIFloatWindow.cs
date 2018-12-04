//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class UIFloatWindow : AbstractBox, ITopWindowBox
    {
        IPlatformWindowBox _platformWindowBox;
        public UIFloatWindow(int w, int h)
            : base(w, h)
        {

        }
        IPlatformWindowBox ITopWindowBox.PlatformWinBox
        {
            get { return _platformWindowBox; }
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
            get
            {
                return base.Visible;
            }
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