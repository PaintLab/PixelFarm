//from github.com/vvvv/svg 
//license : Microsoft Public License (MS-PL) 



namespace LayoutFarm.Svg.Transforms
{
    /// <summary>
    /// The class which applies custom transform to this Matrix
    /// </summary>
    public sealed class SvgTransformMatrix : SvgTransform
    {
        float[] _elements;
        public float[] Elements
        {
            get { return this._elements; }
            set { this._elements = value; }
        }
        public SvgTransformMatrix(float[] elements)
        {
            this._elements = elements;
        }
    }
}