//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public enum BoxContentLayoutKind
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }

    public enum ContentStretch
    {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    public sealed class Box : AbstractBox
    {
        public Box(int w, int h)
            : base(w, h)
        {

        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "box");
            this.Describe(visitor);
            //descrube child 
            visitor.EndElement();
        }
        public void BoxSetInnerContentSize(int innerW, int innerH)
        {
            SetInnerContentSize(innerW, innerH);
        }
        public override void NotifyContentUpdate(UIElement childContent)
        {
            //set propersize

            if (childContent is ImageBox)
            {
                ImageBox imgBox = (ImageBox)childContent;
                this.SetSize(imgBox.Width, imgBox.Height);

            }
            this.InvalidateLayout();
            //this.ParentUI?.NotifyContentUpdate(this);
            this.ParentUI?.InvalidateLayout();
        }
    }
}