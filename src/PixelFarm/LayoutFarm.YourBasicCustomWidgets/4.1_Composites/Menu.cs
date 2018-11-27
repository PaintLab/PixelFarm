//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class MenuItem : AbstractRectUI
    {
        CustomRenderBox _primElement;//background 
        Color _backColor = Color.LightGray;
        bool _thisMenuOpened;
        //1. land part
        AbstractRectUI _landPart;
        //2. float part   
        MenuBox _floatPart;
        CustomRenderBox _floatPartRenderElement;
        HingeFloatPartStyle _floatPartStyle;
        List<MenuItem> _childItems;
        public MenuItem(int width, int height)
            : base(width, height)
        {
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
                renderE.HasSpecificWidthAndHeight = true;
                renderE.SetController(this);
                //------------------------------------------------
                //create visual layer                 

                if (this._landPart != null)
                {
                    renderE.AddChild(this._landPart);
                }
                if (this._floatPart != null)
                {
                }

                //---------------------------------
                _primElement = renderE;
            }
            return _primElement;
        }
        //----------------------------------------------------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            if (this.MouseDown != null)
            {
                this.MouseDown(this, e);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (this.MouseUp != null)
            {
                MouseUp(this, e);
            }
            base.OnMouseUp(e);
        }

        //---------------------------------------------------- 
        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        //----------------------------------------------------  
        public AbstractRectUI LandPart
        {
            get { return this._landPart; }
            set
            {
                this._landPart = value;
                if (value != null)
                {
                    //if new value not null
                    //check existing land part
                    if (this._landPart != null)
                    {
                        //remove existing landpart
                    }

                    if (_primElement != null)
                    {
                        //add 
                        _primElement.AddChild(value);
                    }
                }
                else
                {
                    if (this._landPart != null)
                    {
                        //remove existing landpart

                    }
                }
            }
        }
        public MenuBox FloatPart
        {
            get { return this._floatPart; }
            set
            {
                this._floatPart = value;
                if (value != null)
                {
                    //attach float part 
                }
            }
        }
        //---------------------------------------------------- 
        public bool IsOpened
        {
            get { return this._thisMenuOpened; }
        }
        public void Open()
        {
            if (_thisMenuOpened) return;
            this._thisMenuOpened = true;
            //-----------------------------------
            if (this._primElement == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                case HingeFloatPartStyle.Popup:
                    {
                        //add float part to top window layer
                        var topRenderBox = _primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            Point globalLocation = _primElement.GetGlobalLocation();
                            _floatPart.SetLocation(globalLocation.X, globalLocation.Y + _primElement.Height);
                            this._floatPartRenderElement = this._floatPart.GetPrimaryRenderElement(_primElement.Root) as CustomRenderBox;
                            topRenderBox.AddChild(_floatPartRenderElement);
                            //temp here

                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void Close()
        {
            if (!_thisMenuOpened) return;
            this._thisMenuOpened = false;
            if (this._primElement == null) return;
            if (_floatPart == null) return;
            switch (_floatPartStyle)
            {
                default:
                    {
                    }
                    break;
                case HingeFloatPartStyle.Popup:
                    {
                        var topRenderBox = _primElement.GetTopWindowRenderBox();
                        if (topRenderBox != null)
                        {
                            if (this._floatPartRenderElement != null)
                            {
                                topRenderBox.RemoveChild(_floatPartRenderElement);
                            }
                        }
                    }
                    break;
                case HingeFloatPartStyle.Embeded:
                    {
                    }
                    break;
            }
        }
        public void MaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = true;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public void UnmaintenanceParentOpenState()
        {
            if (this.ParentMenuItem != null)
            {
                this.ParentMenuItem.MaintenceOpenState = false;
                this.ParentMenuItem.MaintenanceParentOpenState();
            }
        }
        public bool MaintenceOpenState
        {
            get;
            private set;
        }
        public void CloseRecursiveUp()
        {
            this.Close();
            if (this.ParentMenuItem != null &&
               !this.ParentMenuItem.MaintenceOpenState)
            {
                this.ParentMenuItem.CloseRecursiveUp();
            }
        }
        public MenuItem ParentMenuItem
        {
            get;
            private set;
        }
        public HingeFloatPartStyle FloatPartStyle
        {
            get { return this._floatPartStyle; }
            set
            {
                this._floatPartStyle = value;
            }
        }
        public void AddSubMenuItem(MenuItem childItem)
        {
            if (_childItems == null)
            {
                _childItems = new List<MenuItem>();
            }
            this._childItems.Add(childItem);
            _floatPart.AddChild(childItem);
            childItem.ParentMenuItem = this;
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "menuitem");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }


    public class MenuBox : AbstractBox
    {
        bool _showing;
        RenderBoxBase _topWindow;
        RenderElement _myRenderE;
        public MenuBox(int w, int h)
            : base(w, h)
        {
        }
        public void ShowMenu(RootGraphic rootgfx)
        {
            //add to topmost box 
            if (!_showing)
            {

                rootgfx.AddChild(this._myRenderE = this.GetPrimaryRenderElement(_topWindow.Root));
                _showing = true;
            }
        }
        public void HideMenu()
        {
            if (_showing)
            {
                //remove from top 
                _showing = false;
                if (this._topWindow != null && this._myRenderE != null)
                {
                    _topWindow.RemoveChild(this._myRenderE);
                }
            }
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "menubox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}