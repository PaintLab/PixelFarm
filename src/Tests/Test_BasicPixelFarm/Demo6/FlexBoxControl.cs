//MIT, 2020, Brezza92
using LayoutFarm.MariusYoga;
using System.Collections.Generic;

namespace LayoutFarm.CustomWidgets
{
    public class FlexBox : AbstractBox
    {
        public FlexBox(int w,int h): base(w, h)
        {
            
        }
        private YogaNode _yoga;
        public YogaNode YogaNode
        {
            get => _yoga;
            set
            {
                _yoga = value;
                Update();
            }
        }

        public void Update()
        {
            if (YogaNode == null)
            {
                return;
            }
            int width = (int)YogaNode.LayoutWidth;
            int height = (int)YogaNode.LayoutHeight;
            SetSize(width, height);

            SetLocation((int)YogaNode.LayoutX, (int)YogaNode.LayoutY);

            if (ChildCount > 0)
            {
                foreach (var ui in this.GetChildIter())
                {
                    if (ui is FlexBox fb)
                        fb.Update();
                }
            }
        }
    }
     
}