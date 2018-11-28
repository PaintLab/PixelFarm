//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class TreeView : AbstractRectUI
    {
        //composite          
        CustomRenderBox _primElement;//background
        Color _backColor = Color.LightGray;
        int _viewportX, _viewportY;
        UICollection _uiList;
        int _latestItemY;
        Box _panel; //panel 

        public TreeView(int width, int height)
            : base(width, height)
        {
            //panel for listview items
            this._panel = new Box(width, height);
            _panel.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            _panel.BackColor = Color.LightGray;
            _panel.NeedClipArea = true;
            _uiList = new UICollection(this);
            _uiList.AddUI(_panel);
        }

        protected override bool HasReadyRenderElement
        {
            get { return this._primElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this._primElement; }
        }
        public Color BackColor
        {
            get { return this._backColor; }
            set
            {
                this._backColor = value;
                if (HasReadyRenderElement)
                {
                    this._primElement.BackColor = value;
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.BackColor = _backColor;
                renderE.SetController(this);
                renderE.HasSpecificWidthAndHeight = true;
                //------------------------------------------------
                //create visual layer 
                int n = this._uiList.Count;
                for (int m = 0; m < n; ++m)
                {
                    renderE.AddChild(_uiList.GetElement(m));
                }

                //---------------------------------
                _primElement = renderE;
            }
            return _primElement;
        }
        public void AddItem(TreeNode treeNode)
        {
            treeNode.SetLocation(0, _latestItemY);
            _latestItemY += treeNode.Height;
            treeNode.SetOwnerTreeView(this);
            _panel.AddChild(treeNode);
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
        }

        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }


        public override int ViewportX
        {
            get { return this._viewportX; }
        }
        public override int ViewportY
        {
            get { return this._viewportY; }
        }
        public override void SetViewport(int x, int y, object reqBy)
        {
            this._viewportX = x;
            this._viewportY = y;
            if (this.HasReadyRenderElement)
            {
                this._panel.SetViewport(x, y, this);
            }
        }
        //----------------------------------------------------

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public override void PerformContentLayout()
        {
            //manually perform layout of its content 
            //here: arrange item in panel
            this._panel.PerformContentLayout();
        }
        //----------------------------------------------------   
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "treeview");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }

    public class TreeNode : AbstractRectUI
    {
        const int NODE_DEFAULT_HEIGHT = 17;
        CustomRenderBox primElement;//bg primary render element
        CustomTextRun myTextRun;
        Color backColor;
        bool isOpen = true;//test, open by default
        int newChildNodeY = NODE_DEFAULT_HEIGHT;
        int indentWidth = 17;
        int desiredHeight = 0; //after layout
        List<TreeNode> childNodes;
        TreeNode parentNode;
        TreeView ownerTreeView;
        //-------------------------- 
        ImageBinder nodeIcon;
        ImageBox uiNodeIcon;
        //--------------------------
        public TreeNode(int width, int height)
            : base(width, height)
        {

        }
        public ImageBinder NodeIconImage
        {
            get { return this.nodeIcon; }
            set
            {
                this.nodeIcon = value;
                if (uiNodeIcon != null)
                {
                    uiNodeIcon.ImageBinder = value;
                }
            }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.primElement; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return primElement != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (primElement == null)
            {
                //first time
                var element = new CustomRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = this.backColor;
                element.HasSpecificWidthAndHeight = true;
                element.NeedClipArea = true;
                //-----------------------------
                // create default layer for node content


                //-----------------------------
                uiNodeIcon = new ImageBox(16, 16);//create with default size 
                SetupNodeIconBehaviour(uiNodeIcon);
                element.AddChild(uiNodeIcon);
                //-----------------------------
                myTextRun = new CustomTextRun(rootgfx, 10, 17);
                myTextRun.SetLocation(16, 0);
                myTextRun.Text = "Test01";
                element.AddChild(myTextRun);
                //-----------------------------
                this.primElement = element;
            }
            return primElement;
        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (HasReadyRenderElement)
                {
                    this.primElement.BackColor = value;
                }
            }
        }
        //------------------------------------------------
        public bool IsOpen
        {
            get { return this.isOpen; }
        }
        public int ChildCount
        {
            get
            {
                if (childNodes == null) return 0;
                return childNodes.Count;
            }
        }
        public TreeNode ParentNode
        {
            get { return this.parentNode; }
        }
        public TreeView TreeView
        {
            get
            {
                if (this.ownerTreeView != null)
                {
                    //top node
                    return this.ownerTreeView;
                }
                else
                {
                    if (this.parentNode != null)
                    {
                        //recursive
                        return this.parentNode.TreeView;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        internal void SetOwnerTreeView(TreeView ownerTreeView)
        {
            this.ownerTreeView = ownerTreeView;
        }
        public void AddChildNode(TreeNode treeNode)
        {
            if (childNodes == null)
            {
                childNodes = new List<TreeNode>();
            }
            this.childNodes.Add(treeNode);
            treeNode.parentNode = this;
            //---------------------------
            //add treenode presentaion
            if (this.isOpen)
            {
                if (this.primElement != null)
                {
                    //add child presentation 
                    //below here
                    //create layers                    

                    //add to layer

                    var tnRenderElement = treeNode.GetPrimaryRenderElement(primElement.Root);
                    tnRenderElement.SetLocation(indentWidth, newChildNodeY);
                    primElement.AddChild(tnRenderElement);
                    newChildNodeY += tnRenderElement.Height;
                    //-----------------
                }
            }
            //---------------------------
        }
        public void Expand()
        {
            if (this.isOpen) return;
            this.isOpen = true;

            this.TreeView.PerformContentLayout();
        }
        public void Collapse()
        {
            if (!this.isOpen) return;
            this.isOpen = false;
            this.TreeView.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //if this has child
            //reset
            this.desiredHeight = NODE_DEFAULT_HEIGHT;
            this.newChildNodeY = NODE_DEFAULT_HEIGHT;
            if (this.isOpen)
            {
                if (childNodes != null)
                {
                    int j = childNodes.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        var childNode = childNodes[i];
                        childNode.PerformContentLayout();//manaul?
                        //set new size 
                        childNode.SetLocationAndSize(indentWidth,
                            newChildNodeY,
                            childNode.Width,
                            childNode.InnerHeight);
                        newChildNodeY += childNode.InnerHeight;
                    }
                }
            }
            this.desiredHeight = newChildNodeY;
        }
        public override int InnerHeight
        {
            get
            {
                return this.desiredHeight;
            }
        }
        //------------------------------------------------
        void SetupNodeIconBehaviour(ImageBox uiNodeIcon)
        {
            uiNodeIcon.MouseDown += (s, e) =>
            {
                if (this.IsOpen)
                {
                    //then close
                    this.Collapse();
                }
                else
                {
                    this.Expand();
                }
            };
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "treenode");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}