//Apache2, 2014-2017, WinterDev

using PaintLab;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;

namespace LayoutFarm
{
    //---------------
    struct MyUIElement : IUIElement, IUIBoxElement, System.IEquatable<MyUIElement>
    {
        internal readonly UI.UIElement uiElem;
        public MyUIElement(UI.UIElement uiElem)
        {
            this.uiElem = uiElem;
        }

        public int Width
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Width : 0;
            }
        }

        public int Height
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Height : 0;
            }
        }

        public int Top
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Top : 0;
            }
        }

        public int Left
        {
            get
            {
                UI.UIBox box = uiElem as UI.UIBox;
                return (box != null) ? box.Left : 0;
            }
        }


        public bool Equals(MyUIElement other)
        {
            return this.uiElem == other.uiElem;
        }

        public void SetLocation(int left, int top)
        {
            UI.UIBox box = uiElem as UI.UIBox;
            if (box == null) return;

            box.SetLocation(left, top);
        }

        public void SetSize(int w, int h)
        {
            UI.UIBox box = uiElem as UI.UIBox;
            if (box == null) return;

            box.SetSize(w, h);
        }
    }
    class UIRootElement : IUIRootElement
    {
        internal SampleViewport _viewport;
        public void AddContent(IUIElement uiElement)
        {
            var myUI = (MyUIElement)uiElement;
            _viewport.AddContent(myUI.uiElem);
        }
        public IUIElement CreateElement(string elemName)
        {
            switch (elemName)
            {
                default:
                    return Wrap(new CustomWidgets.SimpleBox(10, 10));
                case UIElemNameConst.simple_box:
                    return Wrap(new CustomWidgets.SimpleBox(10, 10));
                case UIElemNameConst.h_scroll_bar:
                    {
                        var scBar = new CustomWidgets.ScrollBar(10, 10);
                        scBar.ScrollBarType = CustomWidgets.ScrollBarType.Horizontal;
                        return Wrap(scBar);
                    }
                case UIElemNameConst.v_scroll_bar:
                    {
                        var scBar = new CustomWidgets.ScrollBar(10, 10);
                        scBar.ScrollBarType = CustomWidgets.ScrollBarType.Vertical;
                        return Wrap(scBar);
                    }
                case UIElemNameConst.textbox:
                    {
                        var textBox = new CustomWidgets.TextBox(10, 10, false);
                        return Wrap(textBox);
                    }
            }
        }
        static MyUIElement Wrap(LayoutFarm.UI.UIElement ui)
        {
            return new MyUIElement(ui);
        }
    }
    class MyAppHost : IAppHost
    {
        internal IViewport clientViewport;
    }
}