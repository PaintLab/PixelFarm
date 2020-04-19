//MIT, 2020, Brezza92
using System;
using PixelFarm.Drawing;
using LayoutFarm.MariusYoga;
using LayoutFarm.CustomWidgets;
using LayoutFarm.UI;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;

namespace LayoutFarm
{

    public class YogaSpec
    {
        public YogaFlexDirection FlexDirection;
        public YogaValue Width;
        public YogaValue Height;
        public YogaAlign AlignItems;
        public YogaAlign AlignSelf;
        public float? FlexShrink;
        public float? FlexGlow;
        public float? Flex;
        public YogaDirection StyleDirection;
        public YogaValue MarginHorizontal;
        public YogaValue Padding;
    }

    public class YogaLayoutInstance : LayoutInstance
    {
        YogaNode _yogaNode;
        YogaSpec _spec;
        public YogaLayoutInstance(YogaNode yogaNode)
        {
            _yogaNode = yogaNode;

        }
        public YogaLayoutInstance(YogaSpec spec)
        {
            _spec = spec;
        }

        public void SetNode(YogaNode value)
        {
            if (value == null)
            {
                //clear _yoganode
                _yogaNode = null;
                return;
            }

            if (_spec != null)
            {
                value.FlexDirection = _spec.FlexDirection;
                //apply spec
                value.Width = _spec.Width;
                value.Height = _spec.Height;
                value.AlignItems = _spec.AlignItems;
                value.AlignSelf = _spec.AlignSelf;

                value.Flex = _spec.Flex;
                value.FlexGrow = _spec.FlexGlow;
                value.FlexShrink = _spec.FlexShrink;

                value.StyleDirection = _spec.StyleDirection;
                value.MarginHorizontal = _spec.MarginHorizontal;
                value.Padding = _spec.Padding;
            }
            _yogaNode = value;
        }
        public YogaNode YogaNode => _yogaNode;

        public override bool GetResultBounds(out RectangleF rects)
        {
            if (_yogaNode != null)
            {
                rects = new RectangleF(_yogaNode.LayoutX, _yogaNode.LayoutY, _yogaNode.LayoutWidth, _yogaNode.LayoutHeight);
                return true;
            }
            else
            {
                rects = RectangleF.Empty;
                return false;
            }
        }
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
        public static YogaLayoutInstance ToLayoutInstance(this YogaSpec spec) => new YogaLayoutInstance(spec);
    }


    [DemoNote("6 FlexBox")]
    public class Demo_FlexBox : App
    {
        YogaNode _rootNode;
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
            //Init2_3();

            _rootPanel.LayoutInstance = _rootNode.ToLayoutInstance();
        }

        void Init1()
        {
            YogaConfig config = new YogaConfig();

            _rootNode = new YogaNode(config)
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

            _rootNode.Insert(0, root_child0);

            YogaNode root_child1 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                MarginHorizontal = 20,
                FlexGrow = 1,
                FlexShrink = 1
            };
            _rootNode.Insert(1, root_child1);

            YogaNode root_child2 = new YogaNode(config)
            {
                Width = 100,
                Height = 100,
                FlexShrink = 1
            };
            _rootNode.Insert(2, root_child2);

            _rootNode.StyleDirection = YogaDirection.LeftToRight;
            _rootNode.CalculateLayout();

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

            _rootNode = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            var _root_child0 = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = _rootNode.Width,
                Height = 100,
                AlignItems = YogaAlign.Center,
                AlignSelf = YogaAlign.Center,
                Flex = 1,
                FlexShrink = 1,
                StyleDirection = YogaDirection.RightToLeft
            };
            _rootNode.Insert(0, _root_child0);

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
            _rootNode.Insert(1, root_child1);

            //YogaNode root_child2 = new YogaNode(config);
            //root_child2.Width = 100;
            //root_child2.Height = 100;
            //root_child2.FlexShrink = 1;
            //root.Insert(2, root_child2);

            _rootNode.StyleDirection = YogaDirection.LeftToRight;
            _rootNode.CalculateLayout();

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

            _rootNode = new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Column,
                Width = _rootPanel.Width,
                Height = _rootPanel.Height,
                Padding = 20,
                AlignItems = YogaAlign.Stretch
            };

            YogaNode root_child0 = _rootNode.Append(new YogaNode(config)
            {
                FlexDirection = YogaFlexDirection.Row,
                Width = _rootNode.Width,
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


            YogaNode root_child1 = _rootNode.Append(new YogaNode(config)
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

            _rootNode.StyleDirection = YogaDirection.LeftToRight;
            _rootNode.CalculateLayout();

            foreach (YogaNode child in _rootNode)
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

        /// <summary>
        /// create yoga tree first then create boxes
        /// </summary>
        void Init2_2()
        {


            //1. markup
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

            //2. parse to yoga tree
            YogaConfig conf = new YogaConfig();
            _rootNode = Parse(myYogaMarkup, conf);
            YogaNode root_child0 = _rootNode[0];

            //------------
            _rootNode.StyleDirection = YogaDirection.LeftToRight;
            _rootNode.CalculateLayout();


            //3. create box from yoga tree
            _rootPanel.Add(CreateBoxFromYogaNode(_rootNode, (y_node, b_node) =>
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

        static YogaJustify GetJustify(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "flex_start": return YogaJustify.FlexStart;
                case "center": return YogaJustify.Center;
                case "flex_end": return YogaJustify.FlexEnd;
                case "space_between": return YogaJustify.SpaceBetween;
                case "space_around": return YogaJustify.SpaceAround;
                case "space_evently": return YogaJustify.SpaceEvenly;
            }
        }
        static YogaWrap GetYogaWrap(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "no_wrap": return YogaWrap.NoWrap;
                case "wrap": return YogaWrap.Wrap;
                case "wrap_reverse": return YogaWrap.WrapReverse;
            }
        }
        static YogaDisplay GetDisplay(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "flex": return YogaDisplay.Flex;
                case "none": return YogaDisplay.None;
            }
        }
        static YogaAlign GetYogaAlign(string value)
        {
            switch (value)
            {
                case "auto": return YogaAlign.Auto;
                case "flex_start": return YogaAlign.FlexStart;
                case "center": return YogaAlign.Center;
                case "flex_end": return YogaAlign.FlexEnd;
                case "stretch": return YogaAlign.Stretch;
                case "baseline": return YogaAlign.Baseline;
                case "space_between": return YogaAlign.SpaceBetween;
                case "space_around": return YogaAlign.SpaceBetween;
                default: throw new NotSupportedException();
            }
        }
        static YogaOverflow GetOverflow(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "visible": return YogaOverflow.Visible;
                case "hidden": return YogaOverflow.Hidden;
                case "scroll": return YogaOverflow.Scroll;
            }
        }
        static YogaDirection GetYogaDirection(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "right_to_left":
                    return YogaDirection.RightToLeft;
                case "left_to_right":
                    return YogaDirection.LeftToRight;
                case "inherit":
                    return YogaDirection.Inherit;
            }
        }
        static YogaPositionType GetYogaPosition(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "relative": return YogaPositionType.Relative;
                case "absolute": return YogaPositionType.Absolute;
            }
        }
        static YogaFlexDirection GetFlexDirection(string value)
        {
            switch (value)
            {
                default: throw new NotSupportedException();
                case "column": return YogaFlexDirection.Column;
                case "column_reverse": return YogaFlexDirection.ColumnReverse;
                case "row": return YogaFlexDirection.Row;
                case "row_reverse": return YogaFlexDirection.RowReverse;
            }
        }

        static YogaValue ParseYogaValue(string value)
        {
            if (value.EndsWith("%"))
            {
                return YogaValue.Percent(float.Parse(value.Substring(0, value.Length - 1)));
            }
            else if (value.EndsWith("pt"))
            {
                return YogaValue.Point(float.Parse(value.Substring(0, value.Length - 2)));
            }
            else if (float.TryParse(value, out float result))
            {
                return YogaValue.Point(result);
            }
            else if (value == "auto")
            {
                return YogaValue.Auto;
            }
            else
            {
                return YogaValue.Undefined;
            }
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
                        node.Width = ParseYogaValue(kv.Value);
                        break;
                    case "height":
                        node.Height = ParseYogaValue(kv.Value);
                        break;
                    case "padding":
                        node.Padding = ParseYogaValue(kv.Value);
                        break;
                    case "align-items":
                        node.AlignItems = GetYogaAlign(kv.Value);
                        break;
                    case "align-self":
                        node.AlignSelf = GetYogaAlign(kv.Value);
                        break;
                    case "align-content":
                        node.AlignContent = GetYogaAlign(kv.Value);
                        break;
                    case "margin-horizontal":
                        node.MarginHorizontal = ParseYogaValue(kv.Value);
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

                    case "max-width":
                        node.MaxWidth = ParseYogaValue(kv.Value);
                        break;
                    case "max-height":
                        node.MaxHeight = ParseYogaValue(kv.Value);
                        break;
                    case "min-width":
                        node.MinWidth = ParseYogaValue(kv.Value);
                        break;
                    case "min-height":
                        node.MinHeight = ParseYogaValue(kv.Value);
                        break;
                    case "aspect-ratio":
                        node.MinHeight = float.Parse(kv.Value);
                        break;
                    case "overflow":
                        node.Overflow = GetOverflow(kv.Value);
                        break;
                    case "flex-basis":
                        node.FlexBasis = ParseYogaValue(kv.Value);
                        break;
                    case "wrap":
                        node.Wrap = GetYogaWrap(kv.Value);
                        break;
                    case "position-type":
                        node.PositionType = GetYogaPosition(kv.Value);
                        break;
                    case "justify-content":
                        node.JustifyContent = GetJustify(kv.Value);
                        break;
                    case "flex-direction":
                        node.FlexDirection = GetFlexDirection(kv.Value);
                        break;
                    case "style-direction":
                        node.StyleDirection = GetYogaDirection(kv.Value);
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

        YogaNode CreateYogaNode(Box ui, YogaConfig config, bool updateMode)
        {
            LayoutInstance instance = ui.LayoutInstance;
            if (instance is YogaLayoutInstance inst)
            {

                if (inst.YogaNode == null)
                {
                    //create yoga node 
                    YogaNode node = new YogaNode(config);
                    inst.SetNode(node);
                }

                //already has node
                foreach (UIElement childUI in ui.GetChildIter())
                {
                    if (childUI is Box childBox)
                    {
                        inst.YogaNode.Append(CreateYogaNode(childBox, config, updateMode));
                    }
                }
                return inst.YogaNode;
            }
            return null;

        }
        /// <summary>
        /// create boxes first and assign yoga spec then arrange the boxes
        /// </summary>
        void Init2_3()
        {
            //if the yoga-node is expensive=> use yoga-spec and reusable yoga-node

            ////1. markup
            //string myYogaMarkup = $@"
            //<column style='width:{_rootPanel.Width}; height:{_rootPanel.Height }; padding:20; align-items:stretch'>
            //    <row style='width:{_rootPanel.Width}; height:100; align-items:center; align-self:center; flex:1; flex-shrink:1; style-direction:right_to_left;' note='yellow'>
            //           <box style='width:100; height: 100; flex-shrink:1;' note='blue'></box>
            //           <box style='width:100; height: 100; margin-horizontal:20; flex-glow:1; flex-shrink:1;' note='blue'></box>
            //           <box style='width:100; height: 100; flex-shrink:1;' note='blue'></box>
            //    </row>
            //    <box style='width:100; height:100; margin-horizontal:20; flex-glow:1; flex-shrink:1' note='red'>
            //    </box>
            //</column>
            //";


            Box columnBox = new Box(100, 100)
            {
                LayoutInstance = (new YogaSpec()
                {
                    FlexDirection = YogaFlexDirection.Column,
                    Width = _rootPanel.Width,
                    Height = _rootPanel.Height,
                    Padding = 20,
                    AlignItems = YogaAlign.Stretch
                }).ToLayoutInstance()
            };
            Box row = new Box(100, 100)
            {
                BackColor = Color.Yellow,
                LayoutInstance = (new YogaSpec()
                {
                    FlexDirection = YogaFlexDirection.Row,
                    Width = _rootPanel.Width,
                    Height = 100,
                    AlignItems = YogaAlign.Center,
                    AlignSelf = YogaAlign.Center,
                    Flex = 1,
                    FlexShrink = 1,
                    StyleDirection = YogaDirection.RightToLeft
                }).ToLayoutInstance()
            };

            columnBox.Add(row);
            {
                Box b1 = new Box(100, 100)
                {
                    BackColor = Color.Blue,
                    LayoutInstance = (new YogaSpec() { Width = 100, Height = 100, FlexShrink = 1 }).ToLayoutInstance()
                };
                Box b2 = new Box(100, 100)
                {
                    BackColor = Color.Blue,
                    LayoutInstance = (new YogaSpec() { Width = 100, Height = 100, MarginHorizontal = 20, FlexGlow = 1, FlexShrink = 1, }).ToLayoutInstance()
                };
                Box b3 = new Box(100, 100)
                {
                    BackColor = Color.Blue,
                    LayoutInstance = (new YogaSpec() { Width = 100, Height = 100, FlexShrink = 1 }).ToLayoutInstance()
                };
                row.Add(b1);
                row.Add(b2);
                row.Add(b3);
            }
            Box b4 = new Box(100, 100)
            {
                BackColor = Color.Red,
                LayoutInstance = (new YogaSpec() { Width = 100, Height = 100, MarginHorizontal = 20, FlexGlow = 1, FlexShrink = 1 }).ToLayoutInstance()
            };
            columnBox.Add(b4);
            _rootPanel.Add(columnBox);


            //------------------------
            //now create yoga node from YogaSpec
            //------------------------ 
            ////2. parse to yoga tree
            YogaConfig conf = new YogaConfig();
            _rootNode = CreateYogaNode(columnBox, conf, false);
            _rootNode.StyleDirection = YogaDirection.LeftToRight;
            //------------             
            _rootNode.CalculateLayout();
            _rootPanel.UpdateLayout();
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
            _rootNode.Width = _rootPanel.Width;
            _rootNode.Height = _rootPanel.Height;
            var root_child0 = _rootNode[0];
            if (root_child0 != null)
            {
                root_child0.Width = _rootPanel.Width;
            }

            _rootNode.CalculateLayout();
            //update data
            _rootPanel.UpdateLayout();


        }
    }
}