//Apache2, 2014-2018, WinterDev

namespace LayoutFarm.Text
{
    public struct EditableRunCharLocation
    {
        /// <summary>
        /// pixel offset relative to current run
        /// </summary>
        public readonly int pixelOffset;
        /// <summary>
        /// character offset relative to current run
        /// </summary>
        public readonly int RunCharIndex;

        public EditableRunCharLocation(int pixelOffset, int charIndex)
        {
            if (charIndex == -1)
            {

            }
            //charIndex less than 0 ==> invalid pos 
            this.pixelOffset = pixelOffset;
            this.RunCharIndex = charIndex;
        }
    }
}