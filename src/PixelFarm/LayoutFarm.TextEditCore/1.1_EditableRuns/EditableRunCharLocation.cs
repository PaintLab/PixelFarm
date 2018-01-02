//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.Text
{
    public struct EditableRunCharLocation
    {

        public readonly int pixelOffset;
        /// <summary>
        /// begin selected charIndex =-1
        /// </summary>
        public readonly int charIndex;

        public EditableRunCharLocation(int pixelOffset, int charIndex)
        {
            this.pixelOffset = pixelOffset;
            this.charIndex = charIndex;
        }
    }
}