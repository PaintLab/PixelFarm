//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.RenderBoxes
{
    /// <summary>
    /// a group of render elements
    /// </summary>
    public abstract class RenderElementLayer
    {
#if DEBUG
        public int dbug_layer_id;
        static int dbug_layer_id_count = 0;
        public int dbug_InvalidateCount = 0;
        public int dbug_ValidateCount = 0;
#endif


        protected const int IS_LAYER_HIDDEN = 1 << (14 - 1);
        protected const int IS_GROUND_LAYER = 1 << (15 - 1);
        protected const int MAY_HAS_OTHER_OVERLAP_CHILD = 1 << (16 - 1);
        protected const int DOUBLE_BACKCANVAS_WIDTH = 1 << (18 - 1);
        protected const int DOUBLE_BACKCANVAS_HEIGHT = 1 << (19 - 1);
        protected const int CONTENT_DRAWING = 1 << (22 - 1);
        protected const int ARRANGEMENT_VALID = 1 << (23 - 1);
        protected const int HAS_CALCULATE_SIZE = 1 << (24 - 1);
        protected const int FLOWLAYER_HAS_MULTILINE = 1 << (25 - 1);
        //

        protected int _layerFlags;
        protected RenderElement _owner;
        int _postCalculateContentWidth;
        int _postCalculateContentHeight;

        public RenderElementLayer(RenderElement owner)
        {
            _owner = owner;
#if DEBUG
            this.dbug_layer_id = dbug_layer_id_count;
            ++dbug_layer_id_count;
#endif
        }
        public RenderElement OwnerRenderElement => _owner;
        //
        public RootGraphic Root => _owner.Root;
        //
        public abstract void Clear();

        public bool Visible
        {
            get => (_layerFlags & IS_LAYER_HIDDEN) == 0;

            set
            {
                _layerFlags = value ?
                    _layerFlags & ~IS_LAYER_HIDDEN :
                    _layerFlags | IS_LAYER_HIDDEN;
            }
        }
        //
        public Size PostCalculateContentSize => new Size(_postCalculateContentWidth, _postCalculateContentHeight);
        //
        protected void OwnerInvalidateGraphic()
        {
            _owner?.InvalidateGraphics();
        }

        protected void BeginDrawingChildContent()
        {
            _layerFlags |= CONTENT_DRAWING;
        }
        protected void FinishDrawingChildContent()
        {
            _layerFlags &= ~CONTENT_DRAWING;
        }

        public bool DoubleBackCanvasWidth
        {
            get => (_layerFlags & DOUBLE_BACKCANVAS_WIDTH) != 0;

            set
            {
                _layerFlags = value ?
                    _layerFlags | DOUBLE_BACKCANVAS_WIDTH :
                    _layerFlags & ~DOUBLE_BACKCANVAS_WIDTH;
            }
        }

        public bool DoubleBackCanvasHeight
        {
            get => (_layerFlags & DOUBLE_BACKCANVAS_HEIGHT) != 0;

            set
            {
                _layerFlags = value ?
                    _layerFlags | DOUBLE_BACKCANVAS_HEIGHT :
                    _layerFlags & ~DOUBLE_BACKCANVAS_HEIGHT;
            }
        }
        protected void SetDoubleCanvas(bool useWithWidth, bool useWithHeight)
        {
            DoubleBackCanvasWidth = useWithWidth;
            DoubleBackCanvasHeight = useWithHeight;
        }
        protected void SetPostCalculateLayerContentSize(int width, int height)
        {
            ValidateCalculateContentSize();
            _postCalculateContentWidth = width;
            _postCalculateContentHeight = height;
        }

        protected void SetPostCalculateLayerContentSize(Size s)
        {
            ValidateCalculateContentSize();
            _postCalculateContentWidth = s.Width;
            _postCalculateContentHeight = s.Height;
        }

        public abstract bool HitTestCore(HitChain hitChain);
        public abstract void TopDownReCalculateContentSize();
        public abstract void TopDownReArrangeContent();
        public abstract IEnumerable<RenderElement> GetRenderElementIter();
        public abstract IEnumerable<RenderElement> GetRenderElementReverseIter();
        public abstract void DrawChildContent(DrawBoard canvasPage, Rectangle updateArea);
        protected void ValidateArrangement()
        {
#if DEBUG
            this.dbug_ValidateCount++;
#endif

            _layerFlags |= ARRANGEMENT_VALID;
        }

        //
        bool NeedReArrangeContent => (_layerFlags & ARRANGEMENT_VALID) == 0;
        //
        void ValidateCalculateContentSize()
        {
            _layerFlags |= HAS_CALCULATE_SIZE;
        }
#if DEBUG
        public RootGraphic dbugVRoot
        {
            get
            {
                return LayoutFarm.RootGraphic.dbugCurrentGlobalVRoot;
            }
        }
        public string dbugLayerState
        {
            get
            {
                if (NeedReArrangeContent)
                {
                    return "[A]";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public abstract void dbug_DumpElementProps(dbugLayoutMsgWriter writer);
        static dbugVisualLayoutTracer dbugGetLayoutTracer()
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return null;
            }
            else
            {
                return visualroot.dbug_GetLastestVisualLayoutTracer();
            }
        }
        protected static void vinv_dbug_EnterLayerReCalculateContent(RenderElementLayer layer)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushLayerElement(layer);
            debugVisualLay.WriteInfo("..>L_RECAL_TOPDOWN :" + layer.ToString());
        }
        protected static void vinv_dbug_ExitLayerReCalculateContent()
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            RenderElementLayer layer = (RenderElementLayer)debugVisualLay.PeekElement();
            debugVisualLay.WriteInfo("<..L_RECAL_TOPDOWN  :" + layer.ToString());
            debugVisualLay.PopLayerElement();
        }
        protected static void vinv_dbug_BeginSetElementBound(RenderElement ve)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.BeginNewContext();
            debugVisualLay.WriteInfo(dbugVisitorMessage.WITH_0.text, ve);
        }
        protected static void vinv_dbug_EndSetElementBound(RenderElement ve)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo(dbugVisitorMessage.WITH_1.text, ve);
            debugVisualLay.EndCurrentContext();
        }
        protected static void vinv_dbug_EnterLayerReArrangeContent(RenderElementLayer layer)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushLayerElement(layer);
            debugVisualLay.WriteInfo("..>LAYER_ARR :" + layer.ToString());
        }
        protected static void vinv_dbug_ExitLayerReArrangeContent()
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            RenderElementLayer layer = (RenderElementLayer)debugVisualLay.PeekElement();
            debugVisualLay.WriteInfo("<..LAYER_ARR :" + layer.ToString());
            debugVisualLay.PopLayerElement();
        }
        protected static void vinv_dbug_WriteInfo(dbugVisitorMessage msg, object o)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo(msg.text);
        }
#endif

    }
}