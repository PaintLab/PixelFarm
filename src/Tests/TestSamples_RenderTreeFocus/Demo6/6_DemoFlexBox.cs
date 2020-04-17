//MIT, 2020, Brezza92
using System;
using PixelFarm.Drawing;
using LayoutFarm.MariusYoga;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;

namespace LayoutFarm
{

    public class YogaLayoutInstance : LayoutInstance
    {
        YogaNode _yogaNode;
        public YogaLayoutInstance(YogaNode yogaNode)
        {
            _yogaNode = yogaNode;
        }

        public override RectangleF GetResultBounds() => new RectangleF(_yogaNode.LayoutX, _yogaNode.LayoutY, _yogaNode.LayoutWidth, _yogaNode.LayoutHeight);
    }

    public delegate void YogaDecor<T>(T t);

    public static partial class YogaNodeExtensions
    {
        public static YogaNode Append(this YogaNode node, YogaNode child)
        {
            return node.Insert(node.Count, child);
        }
        public static void Append(this YogaNode node, YogaNode child, YogaDecor<YogaNode> childDecor)
        {
            childDecor(child);
            node.Insert(node.Count, child);
        }
    }


    public static partial class YogaNodeExtensions
    {
        public static YogaLayoutInstance ToLayoutInstance(this YogaNode node) => new YogaLayoutInstance(node);
    }


    [DemoNote("6 FlexBox")]
    public class Demo_FlexBox : App
    {
        YogaNode root;
        YogaNode root_child0;
        CustomWidgets.Box _rootPanel;
        protected override void OnStart(AppHost host)
        {
            int halfW = host.PrimaryScreenWidth / 2;
            int halfH = host.PrimaryScreenHeight / 2;
            _rootPanel = new Box(halfW, halfH);
            _rootPanel.SetLocation(0, 0);
            host.AddChild(_rootPanel);

            var addWidthBtn = new Box(100, 50);
            var label1 = new Label();
            label1.Text = "+";
            label1.TransparentForMouseEvents = true;
            addWidthBtn.Add(label1);
            addWidthBtn.MouseDown += AddWidthBtn_MouseDown;
            addWidthBtn.SetLocation(0, halfH + 50);
            host.AddChild(addWidthBtn);

            var minusWidthBtn = new Box(100, 50);

            var label2 = new Label();
            label2.Text = "-";
            label2.TransparentForMouseEvents = true;
            minusWidthBtn.Add(label2);
            minusWidthBtn.MouseDown += MinusWidthBtn_MouseDown;
            minusWidthBtn.SetLocation(120, halfH + 50);
            host.AddChild(minusWidthBtn);

            //Init1();
            //Init2();
            Init2_1();
            //Init3();

            _rootPanel.LayoutInstance = root.ToLayoutInstance();
        }

        private void Init1()
        {
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Center
            };

            YogaNode root_child0 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };

            root.Insert(0, root_child0);

            YogaNode root_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            root.Insert(1, root_child1);

            YogaNode root_child2 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            root.Insert(2, root_child2);

            root.StyleDirection = YogaDirection.LeftToRight;
            root.CalculateLayout();

            CustomWidgets.Box fb0 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = root_child0.ToLayoutInstance() };
            _rootPanel.Add(fb0);

            CustomWidgets.Box fb1 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Red, LayoutInstance = root_child1.ToLayoutInstance() };
            _rootPanel.Add(fb1);

            CustomWidgets.Box fb2 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = root_child2.ToLayoutInstance() };
            _rootPanel.Add(fb2);

            _rootPanel.UpdateLayout();
        }

        private void Init2()
        {
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            root_child0 = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = root.Width,
                Height = 100,
                AlignItems = YogaAlign.Center,
                AlignSelf = YogaAlign.Center,
                Flex = 1,
                FlexShrink = 1,
                StyleDirection = YogaDirection.RightToLeft
            };
            root.Insert(0, root_child0);

            YogaNode c1r0_child0 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            root_child0.Insert(0, c1r0_child0);

            YogaNode c1r0_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            root_child0.Insert(1, c1r0_child1);

            YogaNode c1r0_child2 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            root_child0.Insert(2, c1r0_child2);

            YogaNode root_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            root.Insert(1, root_child1);

            //YogaNode root_child2 = new YogaNode(config);
            //root_child2.Width = 100;
            //root_child2.Height = 100;
            //root_child2.FlexShrink = 1;
            //root.Insert(2, root_child2);

            root.StyleDirection = YogaDirection.LeftToRight;
            root.CalculateLayout();

            CustomWidgets.Box fb0 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Yellow, LayoutInstance = root_child0.ToLayoutInstance() };
            _rootPanel.Add(fb0);

            CustomWidgets.Box fb0_child0 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = c1r0_child0.ToLayoutInstance() };
            fb0.Add(fb0_child0);

            CustomWidgets.Box fb0_child1 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = c1r0_child1.ToLayoutInstance() };
            fb0.Add(fb0_child1);

            CustomWidgets.Box fb0_child2 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = c1r0_child2.ToLayoutInstance() };
            fb0.Add(fb0_child2);

            CustomWidgets.Box fb1 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Red, LayoutInstance = root_child1.ToLayoutInstance() };
            _rootPanel.Add(fb1);

            _rootPanel.UpdateLayout();
        }
        private void Init2_1()
        {
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            root_child0 = root.Append(new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = root.Width,
                Height = 100,
                AlignItems = YogaAlign.Center,
                AlignSelf = YogaAlign.Center,
                Flex = 1,
                FlexShrink = 1,
                StyleDirection = YogaDirection.RightToLeft,
                Note = "yellow",
            });

            YogaNode c1r0_child0 = root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1,
                Note = "blue",
            });

            YogaNode c1r0_child1 = root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1,
                Note = "blue",
            });

            YogaNode c1r0_child2 = root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1,
                Note = "blue",
            });


            YogaNode root_child1 = root.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1,
                Note = "red",
            });

            //YogaNode root_child2 = new YogaNode(config);
            //root_child2.Width = 100;
            //root_child2.Height = 100;
            //root_child2.FlexShrink = 1;
            //root.Insert(2, root_child2);

            root.StyleDirection = YogaDirection.LeftToRight;
            root.CalculateLayout();

            foreach (YogaNode child in root)
            {
                _rootPanel.Add(CreateBoxFromYogaNode(child, (y_node, b_node) =>
                {
                    switch (y_node.Note)
                    {
                        case "yellow": b_node.BackColor = Color.Yellow; break;
                        case "blue": b_node.BackColor = Color.Blue; break;
                        case "red": b_node.BackColor = Color.Red; break;
                    }
                }));
            }

            _rootPanel.UpdateLayout();
        }

        static Box CreateBoxFromYogaNode(YogaNode node, Action<YogaNode, Box> decor)
        {
            CustomWidgets.Box fb0 = new CustomWidgets.Box(100, 100) { LayoutInstance = node.ToLayoutInstance() };
            decor(node, fb0);

            foreach (YogaNode child in node)
            {
                fb0.Add(CreateBoxFromYogaNode(child, decor));
            }
            return fb0;
        }

        private void MinusWidthBtn_MouseDown(object sender, UI.UIMouseDownEventArgs e)
        {
            _rootPanel.SetSize(_rootPanel.Width - 20, _rootPanel.Height);
            Update();
        }

        private void AddWidthBtn_MouseDown(object sender, UI.UIMouseDownEventArgs e)
        {
            _rootPanel.SetSize(_rootPanel.Width + 20, _rootPanel.Height);
            Update();
        }

        private void Update()
        {
            root.Width = _rootPanel.Width;
            root.Height = _rootPanel.Height;
            if (root_child0 != null)
                root_child0.Width = _rootPanel.Width;

            root.CalculateLayout();
            //update data
            _rootPanel.UpdateLayout();
        }
    }
}