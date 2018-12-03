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
            _ownerDockspaceController = ownerDockspaceController;
            _spaceWidth = spaceWidth;
            _spaceHeight = spaceHeight;
            _spaceName = docSpacename;
        }
        public NinespaceController ParentSpaceSet => _ownerDockspaceController;
        public AbstractRectUI Content
        {
            get => _spaceContent;
            set => _spaceContent = value;
        }
        public SpaceName SpaceName => _spaceName;

        public NamedSpaceContainerOverlapMode OverlapMode
        {
            get => _overlapMode;
            set => _overlapMode = value;
        }
        //
        public bool Visible => !_hidden;
        //
        public int X => _spaceX;
        public int Y => _spaceY;
        //
        public int Width => _spaceWidth;
        public int Height => _spaceHeight;

        public int Right => _spaceX + _spaceWidth;
        public int Bottom => _spaceY + _spaceHeight;
        // 
        public int DesiredHeight => Height; //temp
        public int DesiredWidth => Width;


        public void SetSize(int w, int h)
        {
            _spaceWidth = w;
            _spaceHeight = h;
        }
        public void SetLocation(int x, int y)
        {
            _spaceX = x;
            _spaceY = y;
        }
        public void SetBound(int x, int y, int w, int h)
        {
            _spaceX = x;
            _spaceY = y;
            _spaceWidth = w;
            _spaceHeight = h;
            this.Content?.SetLocationAndSize(x, y, w, h);
        }
        public void ArrangeContent()
        {
            this.Content?.PerformContentLayout();
        }
        public void CalculateContentSize()
        {
            _hasCalculatedSize = true;
        }
        public bool HasCalculateSize => _hasCalculatedSize;
#if DEBUG
        public override string ToString()
        {
            return "docspace:" + SpaceName + " " + base.ToString();
        }
#endif

    }
}