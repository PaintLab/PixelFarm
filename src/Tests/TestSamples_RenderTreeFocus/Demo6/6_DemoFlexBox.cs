//MIT, 2020, Brezza92
using System;
using PixelFarm.Drawing;
using LayoutFarm.MariusYoga;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using System.Xml;
using System.Collections.Generic;
using OpenTK.Graphics.ES20;

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
        YogaNode _root;
        YogaNode _root_child0;
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

            //Init2_1();
            Init2_2();

            _rootPanel.LayoutInstance = _root.ToLayoutInstance();
        }

        void Init1()
        {
            YogaConfig config = new YogaConfig();

            _root = new YogaNode(config)
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

            _root.Insert(0, root_child0);

            YogaNode root_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            _root.Insert(1, root_child1);

            YogaNode root_child2 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            _root.Insert(2, root_child2);

            _root.StyleDirection = YogaDirection.LeftToRight;
            _root.CalculateLayout();

            CustomWidgets.Box fb0 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = root_child0.ToLayoutInstance() };
            _rootPanel.Add(fb0);

            CustomWidgets.Box fb1 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Red, LayoutInstance = root_child1.ToLayoutInstance() };
            _rootPanel.Add(fb1);

            CustomWidgets.Box fb2 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Blue, LayoutInstance = root_child2.ToLayoutInstance() };
            _rootPanel.Add(fb2);

            _rootPanel.UpdateLayout();
        }

        void Init2()
        {
            YogaConfig config = new YogaConfig();

            _root = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            _root_child0 = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = _root.Width,
                Height = 100,
                AlignItems = YogaAlign.Center,
                AlignSelf = YogaAlign.Center,
                Flex = 1,
                FlexShrink = 1,
                StyleDirection = YogaDirection.RightToLeft
            };
            _root.Insert(0, _root_child0);

            YogaNode c1r0_child0 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            _root_child0.Insert(0, c1r0_child0);

            YogaNode c1r0_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            _root_child0.Insert(1, c1r0_child1);

            YogaNode c1r0_child2 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            _root_child0.Insert(2, c1r0_child2);

            YogaNode root_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            _root.Insert(1, root_child1);

            //YogaNode root_child2 = new YogaNode(config);
            //root_child2.Width = 100;
            //root_child2.Height = 100;
            //root_child2.FlexShrink = 1;
            //root.Insert(2, root_child2);

            _root.StyleDirection = YogaDirection.LeftToRight;
            _root.CalculateLayout();

            CustomWidgets.Box fb0 = new CustomWidgets.Box(100, 100) { BackColor = PixelFarm.Drawing.Color.Yellow, LayoutInstance = _root_child0.ToLayoutInstance() };
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
        void Init2_1()
        {
            YogaConfig config = new YogaConfig();

            _root = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            _root_child0 = _root.Append(new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = _root.Width,
                Height = 100,
                AlignItems = YogaAlign.Center,
                AlignSelf = YogaAlign.Center,
                Flex = 1,
                FlexShrink = 1,
                StyleDirection = YogaDirection.RightToLeft,
                Note = "yellow",
            });

            YogaNode c1r0_child0 = _root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1,
                Note = "blue",
            });

            YogaNode c1r0_child1 = _root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1,
                Note = "blue",
            });

            YogaNode c1r0_child2 = _root_child0.Append(new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1,
                Note = "blue",
            });


            YogaNode root_child1 = _root.Append(new YogaNode(config)
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

            _root.StyleDirection = YogaDirection.LeftToRight;
            _root.CalculateLayout();

            foreach (YogaNode child in _root)
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

        void Init2_2()
        {
            //simple, custom  :)

            string myYogaMarkup = $@"
            <column style='width:{_rootPanel.Width}; height:{_rootPanel.Height }; padding:20; align-items:stretch'>
                <row style='width:{_rootPanel.Width}; height:100; align-items:center; align-self:center; flex:1; flex-shrink:1; style-direction:right_to_left;' note='yellow'>
                       <box style='width:100; height: 100; flex-shrink:1;' note='blue'></box>
                       <box style='width:100; height: 100; margin-horizontal:20; flex-glow:1; flex-shrink:1;' note='blue'></box>
                       <box style='width:100; height: 100; flex-shrink:1;' note='blue'></box>
                </row>
                <box style='width:100; height:100; margin-horizontal:20; flex-glow:1; flex-shrink:1' note='red'>
                </box>
            </column>
            ";
            //parse this markup
            YogaConfig conf = new YogaConfig();
            _root = Parse(myYogaMarkup, conf);
            _root_child0 = _root[0];

            //------------
            _root.StyleDirection = YogaDirection.LeftToRight;
            _root.CalculateLayout();


            _rootPanel.Add(CreateBoxFromYogaNode(_root, (y_node, b_node) =>
            {
                switch (y_node.Note)
                {
                    case "yellow": b_node.BackColor = Color.Yellow; break;
                    case "blue": b_node.BackColor = Color.Blue; break;
                    case "red": b_node.BackColor = Color.Red; break;
                }
            }));


            _rootPanel.UpdateLayout();
        }
        static YogaNode Parse(string markup, YogaConfig conf)
        {
            //test-only
            markup = markup.Replace('\'', '"');
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.LoadXml(markup);
            }
            catch (Exception ex)
            {

            }
            return CreateYogaNode(xml.DocumentElement, conf);
        }

        static Dictionary<string, string> ParseStyle(string style)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] pairs = style.Split(';');
            foreach (string p in pairs)
            {
                string[] kv = p.Split(':');
                if (kv.Length == 2)
                {
                    dic.Add(kv[0].Trim(), kv[1].Trim()); ///key-value
                }
            }
            return dic;
        }

        static YogaNode CreateYogaNode(XmlElement elem, YogaConfig conf)
        {

            YogaNode node = new YogaNode(conf);
            node.Note = elem.GetAttribute("note");
            //parse style
            Dictionary<string, string> style = ParseStyle(elem.GetAttribute("style"));

            switch (elem.Name)
            {
                default: throw new NotSupportedException();
                case "box":
                    break;
                case "row":
                    node.FlexDirection = YogaFlexDirection.Row;
                    break;
                case "column":
                    node.FlexDirection = YogaFlexDirection.Column;
                    break;
            }
            foreach (var kv in style)
            {
                switch (kv.Key)
                {
                    default: throw new NotSupportedException();
                    case "width":
                        node.Width = float.Parse(kv.Value);
                        break;
                    case "height":
                        node.Height = float.Parse(kv.Value);
                        break;
                    case "padding":
                        node.Padding = float.Parse(kv.Value);
                        break;
                    case "align-items":
                        {
                            switch (kv.Value)
                            {
                                default: throw new NotSupportedException();
                                case "stretch":
                                    node.AlignItems = YogaAlign.Stretch;
                                    break;
                                case "center":
                                    node.AlignItems = YogaAlign.Center;
                                    break;
                            }
                        }
                        break;
                    case "align-self":
                        {
                            switch (kv.Value)
                            {
                                default: throw new NotSupportedException();
                                case "stretch":
                                    node.AlignSelf = YogaAlign.Stretch;
                                    break;
                                case "center":
                                    node.AlignSelf = YogaAlign.Center;
                                    break;
                            }
                        }
                        break;
                    case "margin-horizontal":
                        node.MarginHorizontal = float.Parse(kv.Value);
                        break;
                    case "flex":
                        node.Flex = float.Parse(kv.Value);
                        break;
                    case "flex-shrink":
                        node.FlexShrink = float.Parse(kv.Value);
                        break;
                    case "flex-glow":
                        node.FlexGrow = float.Parse(kv.Value);
                        break;
                    case "style-direction":
                        {
                            switch (kv.Value)
                            {
                                default: throw new NotSupportedException();
                                case "right_to_left":
                                    node.StyleDirection = YogaDirection.RightToLeft;
                                    break;
                                case "left_to_right":
                                    node.StyleDirection = YogaDirection.LeftToRight;
                                    break;
                            }
                        }
                        break;

                }

            }


            foreach (XmlNode ch in elem)
            {
                if (ch is XmlElement childElem)
                {
                    node.Append(CreateYogaNode(childElem, conf));
                }
            }
            return node;
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
            _root.Width = _rootPanel.Width;
            _root.Height = _rootPanel.Height;
            if (_root_child0 != null)
            {
                _root_child0.Width = _rootPanel.Width;
            }

            _root.CalculateLayout();
            //update data
            _rootPanel.UpdateLayout();
        }
    }
}