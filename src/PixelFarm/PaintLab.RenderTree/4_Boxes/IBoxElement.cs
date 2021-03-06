﻿//Apache2, 2014-present, WinterDev



namespace LayoutFarm.UI
{
    public interface IBoxElement
    {
        //for css layout  
        void ChangeElementSize(int w, int h);
        int MinHeight { get; }
    }

    public interface IAbstractRect
    {
        ushort MarginLeft { get; }
        ushort MarginTop { get; }
        ushort MarginRight { get; }
        ushort MarginBottom { get; }

        int Left { get; }
        int Top { get; }
        int Height { get; }
        int Width { get; }
        void SetLocation(int left, int top);
        void SetSize(int w, int h);         
        void SetLocationAndSize(int left, int top, int width, int height);
        RenderElement GetPrimaryRenderElement();
        RectUIAlignment HorizontalAlignment { get; }
        VerticalAlignment VerticalAlignment { get; }
        bool HasSpecificWidth { get; }
        bool HasSpecificHeight { get; }
    }
}