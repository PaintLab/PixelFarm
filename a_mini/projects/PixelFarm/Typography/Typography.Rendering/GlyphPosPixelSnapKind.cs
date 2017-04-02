using System;
namespace Typography.Rendering
{
    /// <summary>
    /// how to pos a glyph on specific point
    /// </summary>
    public enum GlyphPosPixelSnapKind : byte
    {
        Integer,//default
        Half,
        None
    }
}