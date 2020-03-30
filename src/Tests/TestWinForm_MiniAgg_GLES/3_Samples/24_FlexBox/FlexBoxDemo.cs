using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.Drawing;
using PixelFarm.VectorMath;

using Marius.Yoga;
using Mini;

namespace PixelFarm.CpuBlit.Sample_Draw
{
    [Info(OrderCode = "24", AvailableOn = AvailableOn.GLES)]
    public class FlexBoxDemo : DemoBase
    {
        YogaNode root;
        YogaNode root_child0;
        public override void Init()
        {
            //Init1();
            Init2();
        }

        private void Init1()
        {
            if (root != null)
            {
                return;
            }
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config);
            root.FlexDirection = YogaFlexDirection.Row;
            root.Width = Width;
            root.Height = Height;
            root.Padding = 20;
            root.AlignItems = YogaAlign.Center;

            YogaNode root_child0 = new YogaNode(config);
            root_child0.Width = 100;
            root_child0.Height = 100;
            root_child0.FlexShrink = 1;

            root.Insert(0, root_child0);

            YogaNode root_child1 = new YogaNode(config);
            root_child1.Width = 100;
            root_child1.Height = 100;
            root_child1.MarginHorizontal = 20;
            root_child1.FlexGrow = 1;
            root_child1.FlexShrink = 1;
            root.Insert(1, root_child1);

            YogaNode root_child2 = new YogaNode(config);
            root_child2.Width = 100;
            root_child2.Height = 100;
            root_child2.FlexShrink = 1;
            root.Insert(2, root_child2);

            root.StyleDirection = YogaDirection.LeftToRight;
        }

        private void Init2()
        {
            if (root != null)
            {
                return;
            }
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config);
            root.FlexDirection = YogaFlexDirection.Column;
            root.Width = this.Width;
            root.Height = this.Height;
            root.Padding = 20;
            root.AlignItems = YogaAlign.Stretch;

            root_child0 = new YogaNode(config);
            root_child0.FlexDirection = YogaFlexDirection.Row;
            root_child0.Width = root.Width;
            root_child0.Height = 100;
            root_child0.AlignItems = YogaAlign.Center;
            root_child0.AlignSelf = YogaAlign.Center;
            root_child0.Flex = 1;
            root_child0.FlexShrink = 1;
            root_child0.StyleDirection = YogaDirection.RightToLeft;
            root.Insert(0, root_child0);

            YogaNode c1r0_child0 = new YogaNode(config);
            c1r0_child0.Width = 100;
            c1r0_child0.Height = 100;
            c1r0_child0.FlexShrink = 1;
            root_child0.Insert(0, c1r0_child0);

            YogaNode c1r0_child1 = new YogaNode(config);
            c1r0_child1.Width = 100;
            c1r0_child1.Height = 100;
            c1r0_child1.MarginHorizontal = 20;
            c1r0_child1.FlexGrow = 1;
            c1r0_child1.FlexShrink = 1;
            root_child0.Insert(1, c1r0_child1);

            YogaNode c1r0_child2 = new YogaNode(config);
            c1r0_child2.Width = 100;
            c1r0_child2.Height = 100;
            c1r0_child2.FlexShrink = 1;
            root_child0.Insert(2, c1r0_child2);

            YogaNode root_child1 = new YogaNode(config);
            root_child1.Width = 100;
            root_child1.Height = 100;
            root_child1.MarginHorizontal = 20;
            root_child1.FlexGrow = 1;
            root_child1.FlexShrink = 1;
            root.Insert(1, root_child1);

            root.StyleDirection = YogaDirection.LeftToRight;
        }

        public override void Draw(Painter p)
        {
            root.Width = Width;
            root.Height = Height;

            if (root_child0 != null)
            {
                root_child0.Width = Width;
            }
            root.CalculateLayout();

            DrawYogaNode(root, p);
        }

        private void DrawYogaNode(YogaNode node, Painter p)
        {
            if (node != null)
            {
                p.StrokeColor = Color.Blue;
                p.StrokeWidth = 1;
                p.DrawRect(node.LayoutX, node.LayoutY, node.LayoutWidth, node.LayoutHeight);
                if (node.Children != null)
                {
                    foreach (var ch in node.Children)
                    {
                        DrawYogaNode(ch, p);
                    }
                }
            }
        }
        [DemoAction]
        public void MinusWidth()
        {
            this.Width -= 20;
            if (this.Width < 0)
            {
                this.Width = 0;
            }
        }
        [DemoAction]
        public void AddWidth()
        {
            this.Width += 20;
        }
        [DemoAction]
        public void MinusHeight()
        {
            this.Height -= 20;
            if (this.Height < 0)
            {
                this.Height = 0;
            }
        }
        [DemoAction]
        public void AddHeight()
        {
            this.Height += 20;
        }
    }
}