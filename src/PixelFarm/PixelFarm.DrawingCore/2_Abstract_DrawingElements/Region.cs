//MIT, 2014-present, WinterDev

namespace PixelFarm.Drawing
{
    public abstract class Region : System.IDisposable
    {
        public abstract void Dispose();
        public abstract object InnerRegion { get; }

        /// <summary>
        /// Region to contain the portion of the specified Region that does not intersect with this Region.
        /// </summary>
        /// <param name="another"></param>
        /// <returns></returns>
        public abstract Region CreateComplement(Region another);
        public abstract Region CreateExclude(Region another);
        public abstract Region CreateUnion(Region another);
        public abstract Region CreateIntersect(Region another);
    }



}