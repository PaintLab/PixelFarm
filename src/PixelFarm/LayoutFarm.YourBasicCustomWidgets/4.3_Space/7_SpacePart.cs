//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public sealed class SpacePart
    {
        NamedSpaceContainerOverlapMode _overlapMode;
        SpaceName _spaceName;
        AbstractRectUI _spaceContent;
        NinespaceController _ownerDockspaceController;
        int _spaceWidth;
        int _spaceHeight;
        int _spaceX;
        int _spaceY;
        bool _hidden;
        bool _hasCalculatedSize;
        internal SpacePart(NinespaceController ownerDockspaceController, int spaceWidth, int spaceHeight, SpaceName docSpacename)
        {
            this._ownerDockspaceController = ownerDockspaceController;
            this._spaceWidth = spaceWidth;
            this._spaceHeight = spaceHeight;
            this._spaceName = docSpacename;
        }
        public NinespaceController ParentSpaceSet
        {
            get
            {
                return this._ownerDockspaceController;
            }
        }
        public AbstractRectUI Content
        {
            get
            {
                return this._spaceContent;
            }
            set
            {
                this._spaceContent = value;
            }
        }
        public SpaceName SpaceName
        {
            get
            {
                return this._spaceName;
            }
        }
        public NamedSpaceContainerOverlapMode OverlapMode
        {
            get
            {
                return this._overlapMode;
            }
            set
            {
                this._overlapMode = value;
            }
        }
        public int X
        {
            get
            {
                return this._spaceX;
            }
        }
        public int Y
        {
            get
            {
                return this._spaceY;
            }
        }
        public int Width
        {
            get
            {
                return this._spaceWidth;
            }
        }
        public int Height
        {
            get
            {
                return this._spaceHeight;
            }
        }
        public bool Visible
        {
            get
            {
                return !_hidden;
            }
        }
        public int Right
        {
            get
            {
                return this._spaceX + this._spaceWidth;
            }
        }
        public int Bottom
        {
            get
            {
                return this._spaceY + this._spaceHeight;
            }
        }

        public int DesiredHeight
        {
            get { return this.Height; }//temp
        }
        public int DesiredWidth
        {
            get { return this.Width; }//temp
        }


        public void SetSize(int w, int h)
        {
            this._spaceWidth = w;
            this._spaceHeight = h;
        }
        public void SetLocation(int x, int y)
        {
            this._spaceX = x;
            this._spaceY = y;
        }
        public void SetBound(int x, int y, int w, int h)
        {
            this._spaceX = x;
            this._spaceY = y;
            this._spaceWidth = w;
            this._spaceHeight = h;
            var uiContent = this.Content;
            if (uiContent != null)
            {
                uiContent.SetLocationAndSize(x, y, w, h);
            }
        }
        public void ArrangeContent()
        {
            var uiContent = this.Content;
            if (uiContent != null)
            {
                uiContent.PerformContentLayout();
            }
        }
        public void CalculateContentSize()
        {
            _hasCalculatedSize = true;
        }
        public bool HasCalculateSize
        {
            get { return this._hasCalculatedSize; }
        }
#if DEBUG
        public override string ToString()
        {
            return "docspace:" + SpaceName + " " + base.ToString();
        }
#endif

    }
}