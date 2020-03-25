//MIT, 2018, Tomáš Pažourek, https://github.com/tompazourek/Colourful 

namespace PaintLab.Colourful
{
    /// <summary>
    /// Color represented as a vector in its color space
    /// </summary>
    public interface IColorVector
    {
        /// <summary>
        /// Vector
        /// </summary>
        Vector Vector { get; }
    }
}