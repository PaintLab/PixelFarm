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
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        protected override bool HasReadyRenderElement => _primElement != null;
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
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

            MouseDown?.Invoke(this, e);
        }

        protected override void OnMouseUp(UIMouseEventArgs e)
        {

            MouseUp?.Invoke(this, e);
            base.OnMouseUp(e);
        }
        //
        public override int ViewportX => _viewportX;
        public override int ViewportY => _viewportY;
        //
        public override void SetViewport(int x, int y, object reqBy)
        {
            _viewportX = x;
            _viewportY = y;
            if (this.HasReadyRenderElement)
            {
                _panel.SetViewport(x, y, this);
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
        CustomRenderBox _primElement;//bg primary render element
        CustomTextRun _myTextRun;
        Color _backColor;
        bool _isOpen = true;//test, open by default
        int _newChildNodeY = NODE_DEFAULT_HEIGHT;
        int _indentWidth = 17;
        int _desiredHeight = 0; //after layout
        List<TreeNode> _childNodes;
        TreeNode _parentNode;
        TreeView _ownerTreeView;
        //-------------------------- 
        ImageBinder _nodeIcon;
        ImageBox _uiNodeIcon;
        //--------------------------
        public TreeNode(int width, int height)
            : base(width, height)
        {
        }
        public ImageBinder NodeIconImage
        {
            get => _nodeIcon;
            set
            {
                _nodeIcon = value;
                if (_uiNodeIcon != null)
                {
                    _uiNodeIcon.ImageBinder = value;
                }
            }
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        //
        protected override bool HasReadyRenderElement => _primElement != null;
        //
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                //first time
                var element = new CustomRenderBox(rootgfx, this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = _backColor;
                element.HasSpecificWidthAndHeight = true;
                element.NeedClipArea = true;
                //-----------------------------
                // create default layer for node content 

                //-----------------------------
                _uiNodeIcon = new ImageBox(16, 16);//create with default size 
                SetupNodeIconBehaviour(_uiNodeIcon);
                element.AddChild(_uiNodeIcon);
                //-----------------------------
                _myTextRun = new CustomTextRun(rootgfx, 10, 17);
                _myTextRun.SetLocation(16, 0);
                _myTextRun.Text = "Test01";
                element.AddChild(_myTextRun);
                //-----------------------------
                _primElement = element;
            }
            return _primElement;
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
                }
            }
        }
        //------------------------------------------------
        public bool IsOpen => _isOpen;
        //
        public int ChildCount
        {
            get
            {
                if (_childNodes == null) return 0;
                return _childNodes.Count;
            }
        }
        //
        public TreeNode ParentNode => _parentNode;
        //
        public TreeView TreeView
        {
            get
            {
                if (_ownerTreeView != null)
                {
                    //top node
                    return _ownerTreeView;
                }
                else
                {
                    if (_parentNode != null)
                    {
                        //recursive
                        return _parentNode.TreeView;
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
            _ownerTreeView = ownerTreeView;
        }
        public void AddChildNode(TreeNode treeNode)
        {
            if (_childNodes == null)
            {
                _childNodes = new List<TreeNode>();
            }
            _childNodes.Add(treeNode);
            treeNode._parentNode = this;
            //---------------------------
            //add treenode presentaion
            if (_isOpen)
            {
                if (_primElement != null)
                {
                    //add child presentation 
                    //below here
                    //create layers      
                    //add to layer 
                    var tnRenderElement = treeNode.GetPrimaryRenderElement(_primElement.Root);
                    tnRenderElement.SetLocation(_indentWidth, _newChildNodeY);
                    _primElement.AddChild(tnRenderElement);
                    _newChildNodeY += tnRenderElement.Height;
                    //-----------------
                }
            }
            //---------------------------
        }
        public void Expand()
        {
            if (_isOpen) return;
            _isOpen = true;

            this.TreeView.PerformContentLayout();
        }
        public void Collapse()
        {
            if (!_isOpen) return;
            _isOpen = false;
            this.TreeView.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //if this has child
            //reset
            _desiredHeight = NODE_DEFAULT_HEIGHT;
            _newChildNodeY = NODE_DEFAULT_HEIGHT;
            if (_isOpen)
            {
                if (_childNodes != null)
                {
                    int j = _childNodes.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        var childNode = _childNodes[i];
                        childNode.PerformContentLayout();//manaul?
                        //set new size 
                        childNode.SetLocationAndSize(_indentWidth,
                            _newChildNodeY,
                            childNode.Width,
                            childNode.InnerHeight);
                        _newChildNodeY += childNode.InnerHeight;
                    }
                }
            }
            this._desiredHeight = _newChildNodeY;
        }
        //
        public override int InnerHeight => _desiredHeight;
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