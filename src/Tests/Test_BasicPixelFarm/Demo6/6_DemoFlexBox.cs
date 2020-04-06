//MIT, 2020, Brezza92
using Marius.Yoga;
using System;

namespace LayoutFarm
{
    [DemoNote("6 FlexBox")]
    class Demo_FlexBox : App
    {
        YogaNode root;
        YogaNode root_child0;
        CustomWidgets.FlexBox _rootPanel;
        protected override void OnStart(AppHost host)
        {
            int halfW = host.PrimaryScreenWidth / 2;
            int halfH = host.PrimaryScreenHeight / 2;
            _rootPanel = new LayoutFarm.CustomWidgets.FlexBox(halfW, halfH);
            _rootPanel.SetLocation(0, 0);
            host.AddChild(_rootPanel);

            var addWidthBtn = new LayoutFarm.CustomWidgets.Box(100, 50);
            addWidthBtn.MouseDown += AddWidthBtn_MouseDown;
            addWidthBtn.SetLocation(0, halfH + 50);
            host.AddChild(addWidthBtn);

            var minusWidthBtn = new LayoutFarm.CustomWidgets.Box(100, 50);
            minusWidthBtn.MouseDown += MinusWidthBtn_MouseDown;
            minusWidthBtn.SetLocation(120, halfH + 50);
            host.AddChild(minusWidthBtn);

            //Init1();
            Init2();
            _rootPanel.YogaNode = root;
        }

        private void Init1()
        {
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config);
            root.FlexDirection = YogaFlexDirection.Row;
            root.Width = _rootPanel.Width;
            root.Height = _rootPanel.Height;
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
            root.CalculateLayout();

            CustomWidgets.FlexBox fb0 = new CustomWidgets.FlexBox(100, 100) 
            { BackColor = PixelFarm.Drawing.Color.Blue, YogaNode = root_child0 };
            _rootPanel.Add(fb0);

            CustomWidgets.FlexBox fb1 = new CustomWidgets.FlexBox(100, 100) 
            { BackColor = PixelFarm.Drawing.Color.Red, YogaNode = root_child1 };
            _rootPanel.Add(fb1);

            CustomWidgets.FlexBox fb2 = new CustomWidgets.FlexBox(100, 100) 
            { BackColor = PixelFarm.Drawing.Color.Blue, YogaNode = root_child2 };
            _rootPanel.Add(fb2);
        }

        private void Init2()
        {
            YogaConfig config = new YogaConfig();

            root = new YogaNode(config);
            root.FlexDirection = YogaFlexDirection.Column;
            root.Width = _rootPanel.Width;
            root.Height = _rootPanel.Height;
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

            //YogaNode root_child2 = new YogaNode(config);
            //root_child2.Width = 100;
            //root_child2.Height = 100;
            //root_child2.FlexShrink = 1;
            //root.Insert(2, root_child2);

            root.StyleDirection = YogaDirection.LeftToRight;
            root.CalculateLayout();

            CustomWidgets.FlexBox fb0 = new CustomWidgets.FlexBox(100, 100)
            { BackColor = PixelFarm.Drawing.Color.Yellow, YogaNode = root_child0 };
            _rootPanel.Add(fb0);

            CustomWidgets.FlexBox fb0_child0 = new CustomWidgets.FlexBox(100, 100)
            { BackColor = PixelFarm.Drawing.Color.Blue, YogaNode = c1r0_child0 };
            fb0.Add(fb0_child0);

            CustomWidgets.FlexBox fb0_child1 = new CustomWidgets.FlexBox(100, 100)
            { BackColor = PixelFarm.Drawing.Color.Blue, YogaNode = c1r0_child1 };
            fb0.Add(fb0_child1);

            CustomWidgets.FlexBox fb0_child2 = new CustomWidgets.FlexBox(100, 100)
            { BackColor = PixelFarm.Drawing.Color.Blue, YogaNode = c1r0_child2 };
            fb0.Add(fb0_child2);

            CustomWidgets.FlexBox fb1 = new CustomWidgets.FlexBox(100, 100)
            { BackColor = PixelFarm.Drawing.Color.Red, YogaNode = root_child1 };
            _rootPanel.Add(fb1);

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

            _rootPanel.Update();
        }
    }
}