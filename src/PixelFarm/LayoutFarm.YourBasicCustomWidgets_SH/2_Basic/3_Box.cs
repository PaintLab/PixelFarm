//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
   
   
    public enum ContentStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    public class Box : AbstractBox
    {
        public Box(int w, int h)
            : base(w, h)
        {

        }
        public override void NotifyContentUpdate(UIElement childContent)
        {
            //set propersize

            //if (childContent is ImageBox)
            //{
            //    ImageBox imgBox = (ImageBox)childContent;
            //    this.SetSize(imgBox.Width, imgBox.Height); 
            //}

            this.InvalidateLayout();
            //this.ParentUI?.NotifyContentUpdate(this);
            this.ParentUI?.InvalidateLayout();
        }
    }


}