//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public class DockSpacesController : NinespaceController
    {
        VerticalBoxExpansion _leftBoxVerticalExpansionFlags = VerticalBoxExpansion.Default;
        VerticalBoxExpansion _rightBoxVerticalExpansionFlags = VerticalBoxExpansion.Default;
        int _topSplitterHeight;
        int _bottomSplitterHeight;
        int _leftSplitterWidth;
        int _rightSplitterWidth;

        public event EventHandler FinishNineSpaceArrangement;
        public DockSpacesController(AbstractRectUI owner, SpaceConcept initConcept)
            : base(owner, initConcept)
        {
            _myOwner = owner;
            switch (initConcept)
            {
                case SpaceConcept.TwoSpaceHorizontal: //top-bottom
                    {
                        _spaces[L] = InitSpace(SpaceName.Left);
                        _spaces[R] = InitSpace(SpaceName.Right);
                    }
                    break;
                case SpaceConcept.TwoSpaceVertical: //left-right
                    {
                        _spaces[T] = InitSpace(SpaceName.Top);
                        _spaces[B] = InitSpace(SpaceName.Bottom);
                    }
                    break;
                case SpaceConcept.ThreeSpaceHorizontal:
                    {
                        //left-center-right 
                        _spaces[L] = InitSpace(SpaceName.Left);
                        _spaces[C] = InitSpace(SpaceName.Center);
                        _spaces[R] = InitSpace(SpaceName.Right);
                    }
                    break;
                case SpaceConcept.ThreeSpaceVertical:
                    {
                        //top-center-bottom                        
                        _spaces[T] = InitSpace(SpaceName.Top);
                        _spaces[C] = InitSpace(SpaceName.Center);
                        _spaces[B] = InitSpace(SpaceName.Bottom);
                    }
                    break;
                case SpaceConcept.FourSpace:
                    {
                        _spaces[FOURSPACE_LT] = InitSpace(SpaceName.LeftTop);
                        _spaces[FOURSPACE_RT] = InitSpace(SpaceName.RightTop);
                        _spaces[FOURSPACE_RB] = InitSpace(SpaceName.RightBottom);
                        _spaces[FOURSPACE_LB] = InitSpace(SpaceName.LeftBottom);
                    }
                    break;
                case SpaceConcept.FiveSpace:
                    {
                        _spaces[L] = InitSpace(SpaceName.Left);
                        _spaces[R] = InitSpace(SpaceName.Right);
                        _spaces[T] = InitSpace(SpaceName.Top);
                        _spaces[B] = InitSpace(SpaceName.Bottom);
                        _spaces[C] = InitSpace(SpaceName.Center);
                    }
                    break;
                case SpaceConcept.NineSpace:
                default:
                    {
                        _spaces[L] = InitSpace(SpaceName.Left);
                        _spaces[R] = InitSpace(SpaceName.Right);
                        _spaces[T] = InitSpace(SpaceName.Top);
                        _spaces[B] = InitSpace(SpaceName.Bottom);
                        _spaces[C] = InitSpace(SpaceName.Center);
                        _spaces[LT] = InitSpace(SpaceName.LeftTop);
                        _spaces[RT] = InitSpace(SpaceName.RightTop);
                        _spaces[RB] = InitSpace(SpaceName.RightBottom);
                        _spaces[LB] = InitSpace(SpaceName.LeftBottom);
                    }
                    break;
            }
        }
        public void SetSize(int w, int h)
        {
            //set controller size
            _sizeW = w;
            _sizeH = h;
            //-------------
            //arrange all space position 
            this.ArrangeAllSpaces();
        }
        public VerticalBoxExpansion LeftSpaceVerticalExpansion
        {
            get => _leftBoxVerticalExpansionFlags;
            set => _leftBoxVerticalExpansionFlags = value;
        }
        public int TopSplitterHeight
        {
            get => _topSplitterHeight;
            set => _topSplitterHeight = value;
        }
        public int BottomSplitterHeight
        {
            get => _bottomSplitterHeight;
            set => _bottomSplitterHeight = value;
        }
        public int LeftSplitterWidth
        {
            get => _leftSplitterWidth;
            set => _leftSplitterWidth = value;
        }
        public int RightSplitterWidth
        {
            get => _rightSplitterWidth;
            set => _rightSplitterWidth = value;
        }

        //-------------------------------------------------------------------
        public VerticalBoxExpansion RightSpaceVerticalExpansion
        {
            get => _rightBoxVerticalExpansionFlags;
            set => _rightBoxVerticalExpansionFlags = value;
        }


        public override void ArrangeAllSpaces()
        {
#if DEBUG

            //vinv.dbug_SetInitObject(this);
            //vinv.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.DockSpaceLayer_ArrAllDockSpaces);

#endif
            for (int i = _spaces.Length - 1; i >= 0; --i)
            {
                _spaces[i].CalculateContentSize();
            }
#if DEBUG
            //vinv.dbug_EnterLayerReArrangeContent(this);
#endif


            //-------------------------------------------------
            // this.BeginLayerGraphicUpdate(vinv);
            //------------------------------------------------- 
            int w = _sizeW;
            int h = _sizeH;
            if (_dockSpaceConcept == SpaceConcept.FourSpace)
            {
                //-------------------------------------------------
                if (!this.HasSpecificBottomSpaceHeight)
                {
                }
                if (!this.HasSpecificTopSpaceHeight)
                {
                }
                if (!this.HasSpecificRightSpaceWidth)
                {
                }
                if (!this.HasSpecificLeftSpaceWidth)
                {
                }
                //------------------------------------------------- 
                SetDockBound(_spaces[FOURSPACE_LT],
                    0,
                    0,
                    _leftSpaceWidth,
                    _topSpaceHeight);
                //------------------------------------------------------- 
                SetDockBound(_spaces[FOURSPACE_LB],
                    0,
                    _topSpaceHeight,
                    _leftSpaceWidth,
                    OwnerVisualElement.Height - (_topSpaceHeight));
                //------------------------------------------------------ 
                SetDockBound(_spaces[FOURSPACE_RT],
                    _leftSpaceWidth,
                    0,
                    w - _leftSpaceWidth,
                    _topSpaceHeight);
                //------------------------------------------------------ 
                SetDockBound(_spaces[FOURSPACE_RB],
                    _leftSpaceWidth,
                     _topSpaceHeight,
                     w - (_leftSpaceWidth),
                     h - (_topSpaceHeight));
                //------------------------------------------------------
            }
            else
            {
                //start with ninespace , the extend to proper form

                //-------------------------------------------------
                var b_space = _spaces[B];
                var t_space = _spaces[T];
                var l_space = _spaces[L];
                var r_space = _spaces[R];
                if (!this.HasSpecificBottomSpaceHeight && b_space != null)
                {
                    b_space.CalculateContentSize();
                    //if (b_space.NeedReCalculateContentSize)
                    //{
                    //    b_space.TopDownReCalculateContentSize(vinv);
                    //}
                    _bottomSpaceHeight = b_space.DesiredHeight;
                }

                if (!this.HasSpecificTopSpaceHeight && t_space != null)
                {
                    t_space.CalculateContentSize();
                    //if (t_space.NeedReCalculateContentSize)
                    //{
                    //    t_space.TopDownReCalculateContentSize(vinv);
                    //}
                    _topSpaceHeight = t_space.DesiredHeight;
                }
                if (!this.HasSpecificRightSpaceWidth && r_space != null)
                {
                    r_space.CalculateContentSize();
                    //if (r_space.NeedReCalculateContentSize)
                    //{
                    //    r_space.TopDownReCalculateContentSize(vinv);
                    //}
                    _rightSpaceWidth = r_space.DesiredWidth;
                }
                if (!this.HasSpecificLeftSpaceWidth && l_space != null)
                {
                    l_space.CalculateContentSize();
                    //if (l_space.NeedReCalculateContentSize)
                    //{
                    //    l_space.TopDownReCalculateContentSize(vinv);
                    //}
                    _leftSpaceWidth = l_space.DesiredWidth;
                }
                //-------------------------------------------------

                if (l_space != null)
                {
                    int left_y = _topSpaceHeight;
                    int left_h = h - _topSpaceHeight - _bottomSpaceHeight;
                    if ((_leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        left_y = 0;
                        left_h += _topSpaceHeight;
                    }
                    if ((_leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        left_h += _bottomSpaceHeight;
                    }
                    SetDockBound(_spaces[L],
                        0,//x
                        left_y,
                        _leftSpaceWidth,
                        left_h);
                }
                //-------------------------------------------------
                if (r_space != null)
                {
                    int right_y = _topSpaceHeight;
                    int right_h = h - _topSpaceHeight - _bottomSpaceHeight;
                    if (HasSpecificCenterSpaceWidth)
                    {
                        _rightSpaceWidth = OwnerVisualElement.Width - (_leftSpaceWidth + _centerSpaceWidth);
                    }

                    if ((_rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        right_y = 0;
                        right_h += _topSpaceHeight;
                    }
                    if ((_rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        right_h += _bottomSpaceHeight;
                    }
                    SetDockBound(_spaces[R],
                      w - _rightSpaceWidth,
                      right_y,
                      _rightSpaceWidth,
                      right_h);
                    //spaces[R].InvalidateArrangeStatus(); 
                }
                //-------------------------------------------------
                if (t_space != null)
                {
                    //top 
                    int top_x = 0;
                    int top_w = w;
                    if (_dockSpaceConcept == SpaceConcept.NineSpace)
                    {
                        top_x = _leftSpaceWidth;
                        top_w = w - (_leftSpaceWidth + _rightSpaceWidth);
                    }
                    //-------------------------------------------------------

                    if ((_leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        top_x = _leftSpaceWidth;
                        //top_w -= leftSpaceWidth;
                    }
                    if ((_rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Top) == VerticalBoxExpansion.Top)
                    {
                        //top_w -= rightSpaceWidth;
                    }
                    SetDockBound(_spaces[T],
                     top_x,
                     0,
                     top_w,
                     _topSpaceHeight);
                }
                //-------------------------------------------------
                if (b_space != null)
                {
                    int bottom_x = 0;
                    int bottom_w = w;
                    if (_dockSpaceConcept == SpaceConcept.NineSpace)
                    {
                        bottom_x = _leftSpaceWidth;
                        bottom_w = w - (_leftSpaceWidth + _rightSpaceWidth);
                    }
                    //-----------------------------------------------------

                    if ((_leftBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        bottom_x = _leftSpaceWidth;
                        //bottom_w -= leftSpaceWidth;
                    }
                    if ((_rightBoxVerticalExpansionFlags & VerticalBoxExpansion.Bottom) == VerticalBoxExpansion.Bottom)
                    {
                        //bottom_w -= rightSpaceWidth;
                    }


                    bottom_x += _leftSplitterWidth;
                    //-----------------------------------------------------
                    SetDockBound(_spaces[B],
                    bottom_x,
                    h - _bottomSpaceHeight,
                    bottom_w,
                    _bottomSpaceHeight);
                }


                //---------------------------------------------------------------------------------
                if (_spaces[C] != null)
                {
                    w = OwnerVisualElement.Width - (_rightSpaceWidth + _leftSpaceWidth) - (_leftSplitterWidth + _rightSplitterWidth);
                    h = OwnerVisualElement.Height - (_topSpaceHeight + _bottomSpaceHeight);
                    if (w < 1)
                    {
                        w = 1;
                    }
                    if (h < 1)
                    {
                        h = 1;
                    }

                    int x = _leftSpaceWidth + _leftSplitterWidth;
                    SetDockBound(_spaces[C],
                    x,
                    _topSpaceHeight,
                    w,
                    h);
                }
                if (_dockSpaceConcept == SpaceConcept.NineSpace)
                {
                    h = OwnerVisualElement.Height;
                    w = OwnerVisualElement.Width;
                    SetDockBound(_spaces[LT], 0, 0, _leftSpaceWidth, _topSpaceHeight);
                    SetDockBound(_spaces[LB], 0, h - _bottomSpaceHeight, _leftSpaceWidth, _bottomSpaceHeight);
                    SetDockBound(_spaces[RT], w - _rightSpaceWidth, 0, _rightSpaceWidth, _topSpaceHeight);
                    SetDockBound(_spaces[RB], w - _rightSpaceWidth, h - _bottomSpaceHeight, _rightSpaceWidth, _bottomSpaceHeight);
                }
                //-----------------------------
            }
            for (int i = _spaces.Length - 1; i >= 0; i--)
            {
                SpacePart dockSpace = _spaces[i];
                if (dockSpace.SpaceName == SpaceName.Left ||
                    dockSpace.SpaceName == SpaceName.Right)
                {
                }
                if (dockSpace != null)
                {
                    dockSpace.ArrangeContent();
                }
            }

            //-------------------------------------------------

            if (this.FinishNineSpaceArrangement != null)
            {
                FinishNineSpaceArrangement(this, EventArgs.Empty);
            }

#if DEBUG

            //vinv.dbug_EndLayoutTrace();

#endif
        }

        //public override void TopDownReArrangeContent()
        //{
        //    ArrangeAllDockSpaces();
        //}
    }
}