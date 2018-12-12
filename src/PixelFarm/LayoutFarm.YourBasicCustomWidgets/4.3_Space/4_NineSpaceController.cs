//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

namespace LayoutFarm.UI
{
    public abstract class NinespaceController
    {
        //----------------------------
        //for five and nine space
        protected const int C = 0;
        protected const int L = 1;
        protected const int R = 2;
        protected const int T = 3;
        protected const int B = 4;
        protected const int LT = 5;
        protected const int LB = 6;
        protected const int RT = 7;
        protected const int RB = 8;
        //----------------------------
        protected const int FOURSPACE_LT = 0;
        protected const int FOURSPACE_LB = 1;
        protected const int FOURSPACE_RT = 2;
        protected const int FOURSPACE_RB = 3;
        protected SpacePart[] _spaces;
        //from user intention value
        protected int _topSpaceHeight = 1;
        protected int _bottomSpaceHeight = 1;
        protected int _leftSpaceWidth = 1;
        protected int _rightSpaceWidth = 1;
        protected int _centerSpaceWidth = -1;
        protected int _sizeW;
        protected int _sizeH;
        protected SpaceConcept _dockSpaceConcept = SpaceConcept.FiveSpace;
        protected AbstractRectUI _myOwner;

        public NinespaceController(AbstractRectUI owner, SpaceConcept initConcept)
        {
            _myOwner = owner;
            _dockSpaceConcept = initConcept;
            switch (initConcept)
            {
                case SpaceConcept.NineSpaceFree:
                case SpaceConcept.NineSpace:
                    {
                        _spaces = new SpacePart[9];
                    }
                    break;
                default:
                    {
                        _spaces = new SpacePart[5];
                    }
                    break;
            }
            _sizeH = owner.Height;
            _sizeW = owner.Width;
        }

        protected SpacePart InitSpace(SpaceName name)
        {
            //only call from ctor?
            return new SpacePart(this, 10, 10, name);
        }
        //
        public AbstractRectUI Owner => _myOwner;
        //
        public bool ControlChildPosition => true;
        //
#if DEBUG
        public string dbugGetLinkInfo()
        {
            return this.ToString();
        }
#endif
        //
        internal SpacePart[] GetAllSpaces() => _spaces;
        //
        public IEnumerable<UIElement> GetVisualElementIter()
        {
            for (int i = _spaces.Length - 1; i >= 0; --i)
            {
                SpacePart sp = _spaces[i];
                if (sp != null)
                {
                    yield return sp.Content;
                }
            }
        }
        public IEnumerable<UIElement> GetVisualElementReverseIter()
        {
            for (int i = _spaces.Length - 1; i >= 0; --i)
            {
                SpacePart sp = _spaces[i];
                if (sp != null)
                {
                    yield return sp.Content;
                }
            }
        }
        public SpaceConcept SpaceConcept
        {
            get => _dockSpaceConcept;
            set => _dockSpaceConcept = value;
        }

        /// <summary>
        /// left area of 2,3,5,9 space
        /// </summary>
        public SpacePart LeftSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceHorizontal:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return _spaces[L];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// right area of 2,3,5,9 space
        /// </summary>
        public SpacePart RightSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceHorizontal:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return _spaces[R];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// top area of 2,3,5,9 space
        /// </summary>
        public SpacePart TopSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceVertical:
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return _spaces[T];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// bottom area of 2,3,5,9 space
        /// </summary>
        public SpacePart BottomSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.TwoSpaceVertical:
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return _spaces[B];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// center area of  3,5,9 space
        /// </summary>
        public SpacePart CenterSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.ThreeSpaceVertical:
                    case SpaceConcept.ThreeSpaceHorizontal:
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.FiveSpace:
                        {
                            return _spaces[C];
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart LeftTopSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:
                        {
                            return _spaces[FOURSPACE_LT];
                        }
                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:
                        {
                            return _spaces[LT];
                        }
                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart RightTopSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:

                        return _spaces[FOURSPACE_RT];
                    //
                    case SpaceConcept.NineSpaceFree:
                    case SpaceConcept.NineSpace:

                        return _spaces[RT];

                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart LeftBottomSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:

                        return _spaces[FOURSPACE_LB];

                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:

                        return _spaces[LB];

                }
                return null;
            }
        }
        /// <summary>
        /// corner area of  4,9 space
        /// </summary>
        public SpacePart RightBottomSpacePart
        {
            get
            {
                switch (_dockSpaceConcept)
                {
                    case SpaceConcept.FourSpace:

                        return _spaces[FOURSPACE_RB];

                    case SpaceConcept.NineSpace:
                    case SpaceConcept.NineSpaceFree:

                        return _spaces[RB];

                }
                return null;
            }
        }
        //
        public int TopSpaceHeight => _topSpaceHeight;
        //
        protected AbstractRectUI OwnerVisualElement => _myOwner;
        //
        public void SetTopSpaceHeight(int value)
        {
            if (_dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Height - 2)
                {
                    value = OwnerVisualElement.Height - 2;
                }
                _topSpaceHeight = value;
                if (_spaces[C] == null)
                {
                    _bottomSpaceHeight = OwnerVisualElement.Height - _topSpaceHeight;
                }
                else
                {
                    if (_topSpaceHeight >= OwnerVisualElement.Height - _bottomSpaceHeight - 1)
                    {
                        _bottomSpaceHeight = OwnerVisualElement.Height - _topSpaceHeight - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Height - 1)
                {
                    value = OwnerVisualElement.Height - 1;
                }
                _topSpaceHeight = value;
                _bottomSpaceHeight = OwnerVisualElement.Height - _topSpaceHeight;
            }
            this.HasSpecificTopSpaceHeight = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }
        //
        public int BottomSpaceHeight => _bottomSpaceHeight;
        //
        public void SetBottomSpaceHeight(int value)
        {
            if (_dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Height - 2)
                {
                    value = OwnerVisualElement.Height - 2;
                }
                _bottomSpaceHeight = value;
                if (_spaces[C] == null)
                {
                    _topSpaceHeight = OwnerVisualElement.Height - _bottomSpaceHeight;
                }
                else
                {
                    if (_topSpaceHeight >= OwnerVisualElement.Height - _bottomSpaceHeight - 1)
                    {
                        _topSpaceHeight = OwnerVisualElement.Height - _bottomSpaceHeight - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Height - 1)
                {
                    value = OwnerVisualElement.Height - 1;
                }
                _bottomSpaceHeight = value;
                _topSpaceHeight = OwnerVisualElement.Height - _bottomSpaceHeight;
            }
            this.HasSpecificBottomSpaceHeight = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }
        //
        public bool HasSpecificLeftSpaceWidth { get; set; }
        public bool HasSpecificCenterSpaceWidth { get; set; }
        public bool HasSpecificRightSpaceWidth { get; set; }
        public bool HasSpecificTopSpaceHeight { get; set; }
        public bool HasSpecificBottomSpaceHeight { get; set; }
        //
        public int LeftSpaceWidth => _leftSpaceWidth;
        //
        public void SetLeftSpaceWidth(int value)
        {
            if (_dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value > OwnerVisualElement.Width - 2)
                {
                    value = OwnerVisualElement.Width - 2;
                }
                _leftSpaceWidth = value;
                if (_spaces[C] == null)
                {
                    _rightSpaceWidth = OwnerVisualElement.Width - _leftSpaceWidth;
                }
                else
                {
                    if (_leftSpaceWidth >= OwnerVisualElement.Width - _rightSpaceWidth - 1)
                    {
                        _rightSpaceWidth = OwnerVisualElement.Width - _leftSpaceWidth - 1;
                    }
                }
            }
            else
            {
                if (value > OwnerVisualElement.Width - 1)
                {
                    value = OwnerVisualElement.Width - 1;
                }
                _leftSpaceWidth = value;
                _rightSpaceWidth = OwnerVisualElement.Width - _leftSpaceWidth;
            }

#if DEBUG

            //this.dbugVRoot.dbug_PushLayoutTraceMessage("^Set LeftSpaceWidth=" + value);
#endif
            this.HasSpecificLeftSpaceWidth = true;
            this.InvalidateArrangementInAllDockSpaces();
            this.ArrangeAllSpaces();
        }

        public int RightSpaceWidth => _rightSpaceWidth;
        public void SetRightSpaceWidth(int value)
        {
            if (_dockSpaceConcept != SpaceConcept.FourSpace)
            {
                if (value >= OwnerVisualElement.Width - 2)
                {
                    value = OwnerVisualElement.Width - 2;
                }
                _rightSpaceWidth = value;
                if (_spaces[C] == null)
                {
                    //if no centerspace ,then use right space width
                    _leftSpaceWidth = OwnerVisualElement.Width - _rightSpaceWidth;
                }
                else
                {
                    if (_leftSpaceWidth > OwnerVisualElement.Width - _rightSpaceWidth - 1)
                    {
                        _leftSpaceWidth = OwnerVisualElement.Width - _rightSpaceWidth - 1;
                    }
                }
            }
            else
            {
                if (value >= OwnerVisualElement.Width - 1)
                {
                    value = OwnerVisualElement.Width - 1;
                }

                _rightSpaceWidth = value;
                _leftSpaceWidth = OwnerVisualElement.Width - _rightSpaceWidth;
            }

            this.HasSpecificRightSpaceWidth = true;
            this.InvalidateArrangementInAllDockSpaces();
            ArrangeAllSpaces();
        }

        public int CenterSpaceWidth => _centerSpaceWidth;

        public void SetCenterSpaceWidth(int value)
        {
            _centerSpaceWidth = value;
            if (value > -1)
            {
                this.HasSpecificCenterSpaceWidth = true;
                this.InvalidateArrangementInAllDockSpaces();
                ArrangeAllSpaces();
            }
        }

        public virtual void TopDownReCalculateContentSize()
        {
#if DEBUG

#endif

            for (int i = _spaces.Length - 1; i > -1; i--)
            {
                var docspace = _spaces[i];
                if (docspace != null)
                {
                    if (!docspace.HasCalculateSize)
                    {
                        docspace.CalculateContentSize();
                    }
                    else
                    {
                        //contentArrVisitor.dbug_WriteInfo("SKIP " + docspace.dbug_FullElementDescription);
#if DEBUG
                        //vinv.dbug_WriteInfo(dbugVisitorMessage.SKIP, docspace);
#endif
                    }
                }
            }
            //---------------------------------------------------------

            if (_dockSpaceConcept == SpaceConcept.NineSpaceFree)
            {
                int maxWidth = CalculateTotalFreeSpacesDesiredWidth();
                int maxHeight = CalculateTotalFreeSpacesDesiredHeight();
            }
            else
            {
                int maxWidth = CalculateTotalDockSpaceDesiredWidth();
                int maxHeight = CalculateTotalDockSpaceDesiredHeight();
            }
            //---------------------------------------------------------
#if DEBUG
            //vinv.dbug_ExitLayerReCalculateContent();
#endif

        }

        int CalculateTotalFreeSpacesDesiredWidth()
        {
            int maxWidth = 0;
            int w = CalculateTotalFreeSpacesDesiredWidth(_spaces[LT], _spaces[T], _spaces[RT]);
            maxWidth = w;
            w = CalculateTotalFreeSpacesDesiredWidth(_spaces[L], _spaces[C], _spaces[R]);
            if (w > maxWidth)
            {
                maxWidth = w;
            }
            w = CalculateTotalFreeSpacesDesiredWidth(_spaces[LB], _spaces[B], _spaces[RB]);
            if (w > maxWidth)
            {
                maxWidth = w;
            }
            return maxWidth;
        }
        int CalculateTotalDockSpaceDesiredWidth()
        {
            int maxWidth = 0;
            switch (_dockSpaceConcept)
            {
                case SpaceConcept.FiveSpace:
                    {
                        int w = _spaces[T].DesiredWidth;
                        maxWidth = w;
                        w = CalculateTotalDockSpaceDesiredWidth(_spaces[L], _spaces[C], _spaces[R]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        w = _spaces[B].DesiredWidth;
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        return maxWidth;
                    }
                case SpaceConcept.FourSpace:
                    {
                        throw new NotImplementedException();
                    }
                default:
                    {
                        int w = CalculateTotalDockSpaceDesiredWidth(_spaces[LT], _spaces[T], _spaces[RT]);
                        maxWidth = w;
                        w = CalculateTotalDockSpaceDesiredWidth(_spaces[L], _spaces[C], _spaces[R]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        w = CalculateTotalDockSpaceDesiredWidth(_spaces[LB], _spaces[B], _spaces[RB]);
                        if (w > maxWidth)
                        {
                            maxWidth = w;
                        }
                        return maxWidth;
                    }
            }
        }
        int CalculateTotalDockSpaceDesiredHeight()
        {
            switch (_dockSpaceConcept)
            {
                case SpaceConcept.FiveSpace:
                    {
                        int maxHeight = 0;
                        int h = _spaces[L].DesiredHeight;
                        maxHeight = h;
                        h = CalculateTotalDockSpaceDesiredHeight(_spaces[T], _spaces[C], _spaces[B]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        h = _spaces[R].DesiredHeight;
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        return maxHeight;
                    }
                default:
                    {
                        int maxHeight = 0;
                        int h = CalculateTotalDockSpaceDesiredHeight(_spaces[LT], _spaces[L], _spaces[LB]);
                        maxHeight = h;
                        h = CalculateTotalDockSpaceDesiredHeight(_spaces[T], _spaces[C], _spaces[B]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        h = CalculateTotalDockSpaceDesiredHeight(_spaces[RT], _spaces[R], _spaces[RB]);
                        if (h > maxHeight)
                        {
                            maxHeight = h;
                        }
                        return maxHeight;
                    }
            }
        }
        static int CalculateTotalDockSpaceDesiredWidth(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int total = 0;
            if (bx1 != null)
            {
                total += bx1.DesiredWidth;
            }
            if (bx2 != null)
            {
                total += bx2.DesiredWidth;
            }
            if (bx3 != null)
            {
                total += bx3.DesiredWidth;
            }
            return total;
        }
        static int CalculateTotalDockSpaceDesiredHeight(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int total = 0;
            if (bx1 != null)
            {
                total += bx1.DesiredHeight;
            }
            if (bx2 != null)
            {
                total += bx2.DesiredHeight;
            }
            if (bx3 != null)
            {
                total += bx3.DesiredHeight;
            }
            return total;
        }
        int CalculateTotalFreeSpacesDesiredHeight()
        {
            int maxHeight = 0;
            int h = CalculateTotalFreeSpacesDesiredHeight(_spaces[LT], _spaces[T], _spaces[RT]);
            maxHeight = h;
            h = CalculateTotalFreeSpacesDesiredHeight(_spaces[L], _spaces[C], _spaces[R]);
            if (h > maxHeight)
            {
                maxHeight = h;
            }
            h = CalculateTotalFreeSpacesDesiredHeight(_spaces[LB], _spaces[B], _spaces[RB]);
            if (h > maxHeight)
            {
                maxHeight = h;
            }
            return maxHeight;
        }
        static int CalculateTotalFreeSpacesDesiredWidth(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int totalWidth = 0;
            if (bx1 != null)
            {
                switch (bx1.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalWidth += (bx1.DesiredWidth / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalWidth += bx1.DesiredWidth;
                        }
                        break;
                }
            }
            if (bx2 != null)
            {
                //center not care overlapping
                totalWidth += bx2.DesiredWidth;
            }
            if (bx3 != null)
            {
                switch (bx3.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalWidth += (bx3.DesiredWidth / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalWidth += bx3.DesiredWidth;
                        }
                        break;
                }
            }
            return totalWidth;
        }
        static int CalculateTotalFreeSpacesDesiredHeight(SpacePart bx1, SpacePart bx2, SpacePart bx3)
        {
            int totalHeight = 0;
            if (bx1 != null)
            {
                switch (bx1.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalHeight += (bx1.DesiredHeight / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalHeight += bx1.DesiredHeight;
                        }
                        break;
                }
            }
            if (bx2 != null)
            {
                //center not care overlapping
                totalHeight += bx2.DesiredHeight;
            }
            if (bx3 != null)
            {
                switch (bx3.OverlapMode)
                {
                    case NamedSpaceContainerOverlapMode.Middle:
                        {
                            totalHeight += (bx3.DesiredHeight / 2);
                        }
                        break;
                    case NamedSpaceContainerOverlapMode.Outer:
                        {
                            totalHeight += bx3.DesiredHeight;
                        }
                        break;
                }
            }
            return totalHeight;
        }

        protected void InvalidateArrangementInAllDockSpaces()
        {
            //int j = this.spaces.Length;
            //for (int i = this.spaces.Length - 1; i >= 0; --i)
            //{
            //    spaces[i].InvalidateArrangeStatus();
            //}
        }

        public abstract void ArrangeAllSpaces();
#if DEBUG
        //public override string ToString()
        //{
        //    //if (dockSpaceConcept == SpaceConcept.NineSpaceFree)
        //    //{
        //    //    return "FREE_NINE_LAY (L" + dbug_layer_id + this.dbugLayerState + "):" + this.PostCalculateContentSize.ToString() + " of " + ownerVisualElement.ToString();
        //    //}
        //    //else
        //    //{
        //    //    return "dock layer (L" + dbug_layer_id + this.dbugLayerState + "):" + this.PostCalculateContentSize.ToString() + " of " + ownerVisualElement.ToString();
        //    //}
        //}
#endif

        protected static void SetDockBound(SpacePart dock, int x, int y, int newWidth, int newHeight)
        {
            if (dock == null)
            {
                return;
            }
            dock.SetBound(x, y, newWidth, newHeight);
        }

        //public virtual void TopDownReArrangeContent()
        //{
        //}
    }
}