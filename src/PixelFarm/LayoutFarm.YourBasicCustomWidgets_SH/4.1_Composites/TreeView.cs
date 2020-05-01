//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class TreeView : AbstractControlBox
    {
        //composite           
        int _latestItemY;
        public TreeView(int width, int height)
            : base(width, height)
        {
            //panel for listview items
            NeedClipArea = true;
            this.ContentLayoutKind = BoxContentLayoutKind.VerticalStack;
            _items = new UIList<UIElement>();
        }
        public override RenderElement CurrentPrimaryRenderElement => _primElement;

        public void AddItem(TreeNode treeNode)
        {
            treeNode.SetLocation(0, _latestItemY);
            _latestItemY += treeNode.Height;
            treeNode.SetOwnerTreeView(this);
            _items.Add(this, treeNode);
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
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_primElement == null)
            {
                //first time
                var element = new CustomRenderBox(this.Width, this.Height);
                element.SetLocation(this.Left, this.Top);
                element.BackColor = _backColor;
                element.HasSpecificWidthAndHeight = true;
                //-----------------------------
                // create default layer for node content  
                //-----------------------------
                _uiNodeIcon = new ImageBox();//create with default size 
                SetupNodeIconBehaviour(_uiNodeIcon);
                element.AddChild(_uiNodeIcon);
                //-----------------------------
                _myTextRun = new CustomTextRun(10, 17);
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
                    RenderElement tnRenderElement = treeNode.GetPrimaryRenderElement();
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
            if (_isOpen && _childNodes != null)
            {
                int j = _childNodes.Count;
                for (int i = 0; i < j; ++i)
                {
                    TreeNode childNode = _childNodes[i];
                    childNode.PerformContentLayout();//manual
                                                     //set new size 
                    childNode.SetLocationAndSize(_indentWidth,
                        _newChildNodeY,
                        childNode.Width,
                        childNode.InnerHeight);
                    _newChildNodeY += childNode.InnerHeight;
                }
            }
            _desiredHeight = _newChildNodeY;
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

    }
}