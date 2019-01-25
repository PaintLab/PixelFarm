//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;


namespace PaintLab.Svg
{
    public abstract class VgVisitorBase
    {
        public VgVisualElement Current;
        public ICoordTransformer _currentTx;
        internal VgVisitorBase() { }
        internal virtual void Reset()
        {
            _currentTx = null;
            Current = null;
        }
    }

    public class VgVisitorArgs : VgVisitorBase
    {
        internal VgVisitorArgs() { }
        public Action<VertexStore, VgVisitorArgs> VgElemenVisitHandler;

        /// <summary>
        /// use for finding vg boundaries
        /// </summary>
        public float TempCurrentStrokeWidth { get; internal set; }

        internal override void Reset()
        {
            base.Reset();//*** reset base class fiels too

            //-------
            TempCurrentStrokeWidth = 1;
            VgElemenVisitHandler = null;
        }
    }
    public class VgPaintArgs : VgVisitorBase
    {
        float _opacity;
        bool _maskMode;
        internal VgPaintArgs()
        {
            Opacity = 1;
        }
        public Painter P { get; internal set; }
        public Action<VertexStore, VgPaintArgs> PaintVisitHandler;
        public bool HasOpacity => _opacity < 1;
        public float Opacity
        {
            get => _opacity;
            set
            {
                if (value < 0)
                {
                    _opacity = 0;
                }
                else if (value > 1)
                {
                    _opacity = 1;//not use this opacity
                }
                else
                {
                    _opacity = value;
                }
            }
        }
        internal override void Reset()
        {
            base.Reset();//*** reset base class fields too
            //-------
            Opacity = 2;
            P = null;
            PaintVisitHandler = null;
        }
        public bool MaskMode
        {
            get => _maskMode;
            set
            {
                _maskMode = value;
            }
        }
    }

    public static class VgPainterArgsPool
    {

        public static TempContext<VgPaintArgs> Borrow(Painter painter, out VgPaintArgs paintArgs)
        {
            if (!Temp<VgPaintArgs>.IsInit())
            {
                Temp<VgPaintArgs>.SetNewHandler(
                    () => new VgPaintArgs(),
                    p => p.Reset());//when relese back
            }

            var context = Temp<VgPaintArgs>.Borrow(out paintArgs);
            paintArgs.P = painter;
            return context;
        }
    }
    public static class VgVistorArgsPool
    {

        public static TempContext<VgVisitorArgs> Borrow(out VgVisitorArgs visitor)
        {
            if (!Temp<VgVisitorArgs>.IsInit())
            {
                Temp<VgVisitorArgs>.SetNewHandler(
                    () => new VgVisitorArgs(),
                    p => p.Reset());//when relese back
            }

            return Temp<VgVisitorArgs>.Borrow(out visitor);
        }
    }

}