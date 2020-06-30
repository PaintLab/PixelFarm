//Apache2, 2014-present, WinterDev

using System;

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.UI.ForImplementator;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.CustomWidgets
{
    class GridViewRenderBox : CustomRenderBox
    {
        GridLayer _gridLayer;
        public GridViewRenderBox(int w, int h)
            : base(w, h)
        {

        }
        public override bool HasCustomHitTest => true;
        protected override bool CustomHitTest(HitChain hitChain)
        {
            hitChain.AddHitObject(this);
            if (_gridLayer != null && _gridLayer.HitTestCore(hitChain))
            {
                return true;
            }
            int before = hitChain.Count;
            ChildrenHitTestCore(hitChain);
            return before < hitChain.Count;
        }

        public GridLayer GridLayer
        {
            get => _gridLayer;
            set
            {
                _gridLayer = value;
            }
        }

        public UI.GridColumn GetColumn(int col) => _gridLayer.GetColumn(col);
        public UI.GridRow GetRow(int row) => _gridLayer.GetRow(row);

        public void SetContent(int r, int c, RenderElement re)
        {
            _gridLayer.GetCell(r, c).ContentElement = re;
        }
        public void SetContent(int r, int c, UIElement ui)
        {
            _gridLayer.GetCell(r, c).ContentElement = ui.GetPrimaryRenderElement();
        }


        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{
            //}
#endif

            //sample bg  
            //this render element dose not have child node, so
            //if WaitForStartRenderElement == true,
            //then we skip rendering its content
            //else if this renderElement has more child, we need to walk down) 


            if (!WaitForStartRenderElement)
            {
                if (BackColor.A > 0)
                {
                    d.FillRectangle(BackColor, _viewportLeft, _viewportTop, this.Width, this.Height); //TODO : review drawing background color
                    d.TextBackgroundColorHint = BackColor;
                }
            }

            _gridLayer.DrawChildContent(d, updateArea);

            System.Collections.Generic.IEnumerable<RenderElement> drawingIter = GetDrawingIter();
            if (drawingIter != null)
            {
                RenderElemHelper.DrawChildContent(
                  HitTestHint.Custom,
                  drawingIter,
                  d, updateArea);
            }



            //selection layer
        }

        public GridCell GetCellInfoByMousePosition(int x, int y)
        {

            UI.GridCell hitCell = _gridLayer.GetCellByPosition(x, y);
            if (hitCell != null)
            {
                return new GridCell(hitCell.ColumnIndex, hitCell.RowIndex);
            }
            else
            {
                return new GridCell(-1, -1);
            }
        }
        public UI.GridCell GetCellByMousePosition(int x, int y) => _gridLayer.GetCellByPosition(x, y);
        public UI.GridCell GetCell(int row, int column) => _gridLayer.GetCell(row, column);
        public Color GridBorderColor
        {
            get => _gridLayer.GridBorderColor;
            set => _gridLayer.GridBorderColor = value;
        }
    }

    enum GridSelectionStyle : byte
    {
        RectBox,
        FlowBox
    }

    class GridSelectionSession
    {
        public UI.GridCell _latestHitCell;
        public UI.GridCell _beginSelectedCell;
        public Box _bodyBox; //used in RectBox mode and FlowBox mode
        public Box _headBox;
        public Box _tailBox;
        GridSelectionStyle _gridSelectionStyle;
        bool _moreThan1Cell;


        GridBox _targetGridView;
        public GridSelectionSession()
        {

            _gridSelectionStyle = GridSelectionStyle.RectBox;
        }
        public void SetTargetGridView(GridBox targetGridView)
        {
            _targetGridView = targetGridView;

            _bodyBox = SetupHighlightBox();

            //----
            targetGridView.Add(_bodyBox);

            if (_gridSelectionStyle == GridSelectionStyle.FlowBox)
            {
                //prepare 3 box
                //head, body, tail
                _headBox = SetupHighlightBox();
                _tailBox = SetupHighlightBox();
                // 
                targetGridView.Add(_headBox);
                targetGridView.Add(_tailBox);
            }

        }

        static Box SetupHighlightBox()
        {
            var box = new Box(10, 10);
            box.BackColor = new Color(100, 255, 0, 0);
            box.Visible = false;
            box.TransparentForMouseEvents = true;
            return box;
        }
        public GridSelectionStyle GridSelectionStyle
        {
            get => _gridSelectionStyle;
            set => _gridSelectionStyle = value;
        }
        public void StartAt(UI.GridCell hitCell)
        {

            _moreThan1Cell = false;
            _beginSelectedCell = _latestHitCell = hitCell;


            _bodyBox.SetLocationAndSize(hitCell.X, hitCell.Y, hitCell.Width, hitCell.Height);
            _bodyBox.Visible = true;
            Started = true;
        }

        public bool Started { get; private set; }
        void SetLatestHit_RectBoxModel(UI.GridCell hitCell)
        {

            if (hitCell != _beginSelectedCell)
            {

                _moreThan1Cell = true;

                if (hitCell.ColumnIndex > _beginSelectedCell.ColumnIndex)
                {
                    //select next to right side of the begin
                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);


                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper
                        _bodyBox.SetLocation(_beginSelectedCell.X, hitCell.Y);
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, _beginSelectedCell.Bottom - hitCell.Y);


                    }
                    else
                    {
                        //move to lower
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);
                    }

                }
                else if (hitCell.ColumnIndex < _beginSelectedCell.ColumnIndex)
                {
                    //select to left side
                    //move  
                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row
                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - _beginSelectedCell.Y);

                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper

                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - hitCell.Y);


                    }
                    else
                    {
                        //select to lower
                        _bodyBox.SetLocation(hitCell.X, _beginSelectedCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, hitCell.Bottom - _beginSelectedCell.Y);

                    }
                }
                else
                {
                    //same column 

                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row

                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper 
                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - hitCell.Y);
                    }
                    else
                    {
                        //select to lower
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);
                    }
                }
            }
            else
            {

                if (_moreThan1Cell)
                {
                    _bodyBox.SetSize(hitCell.Width, hitCell.Height);
                    _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                    _moreThan1Cell = false;
                    //_endSelectedCell = hitCell;
                }

            }
            _latestHitCell = hitCell;
        }
        void SetLatestHit_FlowBoxModel(UI.GridCell hitCell)
        {

            if (hitCell != _beginSelectedCell)
            {

                _moreThan1Cell = true;

                if (hitCell.ColumnIndex > _beginSelectedCell.ColumnIndex)
                {
                    //select next to right side of the begin
                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);
                        _bodyBox.Visible = true;
                        _headBox.Visible = _tailBox.Visible = false;
                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper
                        int rowDiff = _beginSelectedCell.RowIndex - hitCell.RowIndex;
                        if (rowDiff == 1)
                        {
                            //just head and tail 
                            //select to end of the row 
                            int colCount = _targetGridView.ColumnCount;
                            {
                                //
                                UI.GridCell lastCellOfHeadRow = _targetGridView.GetCell(hitCell.RowIndex, _targetGridView.ColumnCount - 1);
                                _headBox.SetLocation(hitCell.X, hitCell.Y);
                                _headBox.SetSize(lastCellOfHeadRow.Right - hitCell.X, lastCellOfHeadRow.Height);
                            }
                            //-----
                            {
                                UI.GridCell lastCellOfTailRow = _targetGridView.GetCell(_beginSelectedCell.RowIndex, 0);
                                _tailBox.SetLocation(0, _beginSelectedCell.Y);
                                _tailBox.SetSize(hitCell.Right, lastCellOfTailRow.Height);
                            }


                            _headBox.Visible = _tailBox.Visible = true;
                            _bodyBox.Visible = false;
                        }
                        else
                        {
                            //more than 2 row
                            //so set head-body-tail

                            int colCount = _targetGridView.ColumnCount;
                            {
                                //
                                UI.GridCell lastCellOfHeadRow = _targetGridView.GetCell(hitCell.RowIndex, _targetGridView.ColumnCount - 1);
                                _headBox.SetLocation(hitCell.X, hitCell.Y);
                                _headBox.SetSize(lastCellOfHeadRow.Right - _headBox.Left, lastCellOfHeadRow.Height);
                            }
                            //-----
                            //in between


                            //-----
                            {
                                UI.GridCell lastCellOfTailRow = _targetGridView.GetCell(_beginSelectedCell.RowIndex, 0);
                                _headBox.SetLocation(0, _beginSelectedCell.Y);
                                _headBox.SetSize(_beginSelectedCell.X, lastCellOfTailRow.Height);
                            }


                            _headBox.Visible = _tailBox.Visible = true;
                            _bodyBox.Visible = false;
                        }
                    }
                    else
                    {
                        //move to lower
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);
                    }

                }
                else if (hitCell.ColumnIndex < _beginSelectedCell.ColumnIndex)
                {
                    //select to left side
                    //move  
                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row
                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - _beginSelectedCell.Y);

                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper

                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - hitCell.Y);


                    }
                    else
                    {
                        //select to lower
                        _bodyBox.SetLocation(hitCell.X, _beginSelectedCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, hitCell.Bottom - _beginSelectedCell.Y);

                    }
                }
                else
                {
                    //same column 

                    if (hitCell.RowIndex == _beginSelectedCell.RowIndex)
                    {
                        //same row

                    }
                    else if (hitCell.RowIndex < _beginSelectedCell.RowIndex)
                    {
                        //move upper 
                        _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                        _bodyBox.SetSize(_beginSelectedCell.Right - hitCell.X, _beginSelectedCell.Bottom - hitCell.Y);
                    }
                    else
                    {
                        //select to lower
                        _bodyBox.SetSize(hitCell.Right - _beginSelectedCell.X, hitCell.Bottom - _beginSelectedCell.Y);
                    }
                }
            }
            else
            {

                if (_moreThan1Cell)
                {
                    _bodyBox.SetSize(hitCell.Width, hitCell.Height);
                    _bodyBox.SetLocation(hitCell.X, hitCell.Y);
                    _moreThan1Cell = false;
                    //_endSelectedCell = hitCell;
                }

            }
            _latestHitCell = hitCell;

        }
        public void SetLatestHit(UI.GridCell hitCell)
        {
            switch (_gridSelectionStyle)
            {
                default:
                    SetLatestHit_RectBoxModel(hitCell);
                    break;
                case GridSelectionStyle.FlowBox:
                    SetLatestHit_FlowBoxModel(hitCell);
                    break;
            }
        }
        public void ClearSelection()
        {
            _bodyBox.Visible = false;
            if (_headBox != null) _headBox.Visible = false;
            if (_tailBox != null) _tailBox.Visible = false;
            _moreThan1Cell = false;
            _latestHitCell = _beginSelectedCell = null;
            Started = false;
        }

        public void MoveRight()
        {
            //check if we can move to right side
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell
                if (_latestHitCell != null && _latestHitCell.ColumnIndex < _targetGridView.ColumnCount - 1)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex, _latestHitCell.ColumnIndex + 1));
                }

            }
        }
        public void MoveLeft()
        {
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell
                if (_latestHitCell != null && _latestHitCell.ColumnIndex > 0)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex, _latestHitCell.ColumnIndex - 1));
                }
            }
        }
        public void MoveUp()
        {
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell
                if (_latestHitCell != null && _latestHitCell.RowIndex > 0)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex - 1, _latestHitCell.ColumnIndex));
                }
            }
        }
        public void MoveDown()
        {
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell
                if (_latestHitCell != null && _latestHitCell.RowIndex < _targetGridView.RowCount - 1)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex + 1, _latestHitCell.ColumnIndex));
                }
            }
        }
        public void MoveHome()
        {
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell
                if (_latestHitCell != null)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex, 0));
                }
            }
        }
        public void MoveEnd()
        {
            if (_moreThan1Cell)
            {

            }
            else
            {
                //single cell 
                if (_latestHitCell != null)
                {
                    StartAt(_targetGridView.GetCell(_latestHitCell.RowIndex, _targetGridView.ColumnCount - 1));
                }

            }
        }
        public void SelectAll()
        {

            StartAt(_targetGridView.GetCell(0, 0));
            SetLatestHit_RectBoxModel(_targetGridView.GetCell(_targetGridView.RowCount - 1, _targetGridView.ColumnCount - 1));
        }
    }

    public readonly struct GridColumn
    {
        readonly UI.GridColumn _col;
        readonly GridBox _gridbox;
        internal GridColumn(GridBox gridbox, UI.GridColumn col)
        {
            _gridbox = gridbox;
            _col = col;
        }
        public int Width
        {
            get => _col.Width;
            set
            {
                //check style of gridbox
                if (_gridbox.CellSizeStyle != CellSizeStyle.UniformWidth)
                {
                    _col.Width = value;
                    _gridbox.InvalidateLayout();
                }
            }
        }
    }

    public readonly struct GridRow
    {
        readonly UI.GridRow _row;
        readonly GridBox _gridbox;
        internal GridRow(GridBox gridbox, UI.GridRow row)
        {
            _gridbox = gridbox;
            _row = row;
        }
        public int Height
        {
            get => _row.Height;
            set
            {
                //check style of gridbox
                if (_gridbox.CellSizeStyle != CellSizeStyle.UniformWidth)
                {
                    _row.Height = value;
                    _gridbox.InvalidateLayout();
                }
            }
        }

    }
    public readonly struct GridCell
    {
        public readonly int Row;
        public readonly int Column;
        public GridCell(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public bool IsEmpty => Row < 0;


#if DEBUG
        public override string ToString()
        {
            return Column + "," + Row;
        }
#endif
    }

    public static class BoxExtensions
    {

        static BoxExtensions()
        {
            Temp<LayoutUpdateArgs>.SetNewHandler(
                () => new LayoutUpdateArgs(),
                null);
        }
        public static void PerformContentLayout(this AbstractBox box)
        {
            using (Temp<LayoutUpdateArgs>.Borrow(out LayoutUpdateArgs args))
            {
                box.PerformContentLayout(args);
            }
        }
    }

    public class GridBox : AbstractBox
    {
        GridViewRenderBox _gridViewRenderE;
        CellSizeStyle _cellSizeStyle;

        GridTable _gridTable;
        GridLayer _gridLayer;

        GridSelectionSession _gridSelectionSession;
        Color _gridBorderColor;

        UIList<UIElement> _children;
        public GridBox(int width, int height)
            : base(width, height)
        {
            //has special grid layer

            EnableGridCellSelection = true;
            ClearSelectionWhenLostFocus = true;
            AcceptKeyboardFocus = true;

            _gridBorderColor = Color.Black; //default//TODO: impl Theme classes...
        }
        protected override IUICollection<UIElement> GetDefaultChildrenIter() => _children;

        public void Add(UIElement ui)
        {
            if (_children == null)
            {
                _children = new UIList<UIElement>();
            }
            _children.Add(this, ui);
            if (_gridViewRenderE != null)
            {
                _gridViewRenderE.AddChild(ui.GetPrimaryRenderElement());
            }
        }

        public override void PerformContentLayout(LayoutUpdateArgs args)
        {
            //calculate grid width
            var cols = _gridTable.Columns;
            int ncols = cols.Count;

            int left = 0;
            for (int n = 0; n < ncols; ++n)
            {
                UI.GridColumn col = cols[n];
                col.Left = left;
                left += col.Width;
            }
            int widthSum = left;
            var rows = _gridTable.Rows;
            int nrows = rows.Count;

            int top = 0;
            for (int n = 0; n < nrows; ++n)
            {
                UI.GridRow row = rows[n];
                row.Top = top;
                top += row.Height;
            }

            int heightSum = top;

            //other layer
            base.PerformContentLayout(args);

            int finW = System.Math.Max(InnerWidth, widthSum);
            int finH = System.Math.Max(InnerHeight, heightSum);

            //if (finW != InnerWidth ||
            //    finH != InnerHeight)
            //{
            //    this.InvalidateGraphics();
            //    SetInnerContentSize(finW, finH);
            //    this.InvalidateGraphics();
            //}
            //else
            //{
            SetInnerContentSize(finW, finH);
            //}



            RaiseLayoutFinished();
        }



        public void BuildGrid(int ncols, int eachColumnWidth, int nrows, int eachRowHeight)
        {
            _cellSizeStyle = CellSizeStyle.ColumnAndRow;
            _gridTable = new GridTable();


            //1. create cols
            var cols = _gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width 
                cols.Add(new UI.GridColumn(eachColumnWidth));
            }
            //2. create rows
            var rows = _gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {
                rows.Add(new UI.GridRow(eachRowHeight));
            }

            _gridLayer = new GridLayer(this.Width, this.Height, _cellSizeStyle, _gridTable);

            if (_gridViewRenderE != null)
            {
                _gridViewRenderE.GridLayer = _gridLayer;
            }
            this.InvalidateLayout();
        }

        public void BuildGrid(int ncols, int nrows, CellSizeStyle cellSizeStyle)
        {
            _cellSizeStyle = cellSizeStyle;
            //1. create cols
            _gridTable = new GridTable();
            var cols = _gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                UI.GridColumn col = new UI.GridColumn(1);
                cols.Add(col);
            }
            //2. create rows
            var rows = _gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {
                rows.Add(new UI.GridRow(1));
            }
            //***
            _gridLayer = new GridLayer(this.Width, this.Height, _cellSizeStyle, _gridTable);

            if (_gridViewRenderE != null)
            {
                _gridViewRenderE.GridLayer = _gridLayer;
            }
        }

        //
        public int RowCount => _gridTable.RowCount;
        public int ColumnCount => _gridTable.ColumnCount;
        //
        internal UI.GridCell GetCell(int row, int col) => _gridTable.GetCell(row, col);

        public GridCell GetCellInfoByMousePosition(int x, int y) => _gridViewRenderE.GetCellInfoByMousePosition(x, y);

        public GridRow GetRow(int row) => new GridRow(this, _gridTable.Rows[row]);
        public GridColumn GetColumn(int col) => new GridColumn(this, _gridTable.Columns[col]);

        public Color GridBorderColor
        {
            get => _gridBorderColor;
            set
            {
                _gridBorderColor = value;
                if (_gridViewRenderE != null)
                {
                    _gridViewRenderE.GridBorderColor = value;
                }
            }
        }
        /// <summary>
        /// clear ui content in each cell
        /// </summary>
        public void ClearAllCellContent()
        {

            //not clear grid structure
            //just clear content in each cell  
            int rowCount = _gridTable.RowCount;
            int colCount = _gridTable.ColumnCount;
            for (int r = 0; r < rowCount; ++r)
            {
                for (int c = 0; c < colCount; ++c)
                {
                    UI.GridCell cell = _gridTable.GetCell(r, c);
                    RenderElement content = cell.ContentElement;
                    if (content != null)
                    {
                        if (content.GetController() is UIElement ui)
                        {
                            if (ui.ParentUI != null)
                            {
                                ui.ParentUI = null;
                                UIElement.UnsafeRemoveLinkedNode(ui);
                            }
                            else
                            {
                                //??
                            }

                        }
                        RenderElement.RemoveParentLink(content);
                        cell.ContentElement = null;
                    }

                }
            }

            _gridViewRenderE.InvalidateGraphics();
        }

        protected override void OnMouseWheel(UIMouseWheelEventArgs e)
        {
            int cur_vwLeft = this.ViewportLeft;
            int cur_vwTop = this.ViewportTop;
            int newVwLeft = (int)(cur_vwTop - (e.Delta * 10f / 120f));
            //TODO: review this
            if (newVwLeft > -1 && newVwLeft < (this.InnerHeight - this.Height + 17))
            {
                this.SetViewport(cur_vwLeft, newVwLeft);

            }
            else if (newVwLeft < 0)
            {
                this.SetViewport(cur_vwLeft, 0);
            }
            base.OnMouseWheel(e);
            RaiseViewportChanged();
        }

        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            //System.Console.WriteLine(e.X + "," + e.Y);
            if (e.IsDragging)
            {
                if (this.EnableGridCellSelection)
                {

                    UI.GridCell hitCell = _gridViewRenderE.GetCellByMousePosition(e.X, e.Y);
                    if (_gridSelectionSession != null)
                    {
                        _gridSelectionSession.SetLatestHit(hitCell);
                    }
                }
                else
                {
                    int cur_vwL = this.ViewportLeft;
                    int cur_vwT = this.ViewportTop;

                    int newVwX = (int)(cur_vwL - e.XDiff);
                    int newVwY = (int)(cur_vwT + e.YDiff);

                    if (newVwX < 0)
                    {
                        newVwX = 0;
                        //clamp!
                        this.SetViewport(newVwX, cur_vwT);
                        //gridHeader.SetViewport(newVwX, 0);
                        RaiseViewportChanged();
                    }
                    else if (newVwX > -1 && newVwX < (this.InnerWidth - this.Width))
                    {

                        //clamp!
                        this.SetViewport(newVwX, cur_vwT);

                        RaiseViewportChanged();
                        //gridHeader.SetViewport(newVwX, 0);
                    }
                    else
                    {
                        newVwX = this.InnerWidth - this.Width;
                        this.SetViewport(newVwX, cur_vwT);
                        RaiseViewportChanged();
                        //gridHeader.SetViewport(newVwX, 0);
                    }
                }
            }
            else
            {
                //not draging 

            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {

            UI.GridCell hitCell = _gridViewRenderE.GetCellByMousePosition(e.X, e.Y);

            if (hitCell != null &&
                hitCell.ContentElement is RenderElement box &&
                box.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y) &&
                box.GetController() is IUIEventListener evenListener)
            {

                int tmpX = e.X;
                int tmpY = e.Y;
                e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                evenListener.ListenMouseUp(e);
                e.SetLocation(tmpX, tmpY);

            }


            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            //check if cell content
            //find grid item 

            UI.GridCell hitCell = _gridViewRenderE.GetCellByMousePosition(e.X, e.Y);
            if (hitCell != null)
            {
                if (hitCell.ContentElement is RenderElement box &&
                    box.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y) &&
                    box.GetController() is IUIEventListener evenListener)
                {

                    int tmpX = e.X;
                    int tmpY = e.Y;
                    e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                    evenListener.ListenMouseDown(e);
                    e.SetLocation(tmpX, tmpY);

                }
                //
                //move _dragController to the selected cell? 
                //
                if (EnableGridCellSelection)
                {
                    //--------
                    if (_gridSelectionSession == null)
                    {
                        _gridSelectionSession = new GridSelectionSession();
                        _gridSelectionSession.SetTargetGridView(this);
                    }
                    _gridSelectionSession.StartAt(hitCell);
                }
            }
            else
            {

            }
            base.OnMouseDown(e);
        }
        public override void SetSize(int width, int height)
        {
            //readjust cellsize
            base.SetSize(width, height);
            //----------------------------------
            var cols = _gridTable.Columns;
            int ncols = cols.Count;
            //each col width
            int eachColWidth = width / ncols;
            int colLeft = 0;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                var col = cols[n];
                col.Width = eachColWidth;
                col.Left = colLeft;
                colLeft += eachColWidth;
            }

            var rows = _gridTable.Rows;
            int nrows = rows.Count;
            int eachRowHeight = height / nrows;
            int rowTop = 0;
            for (int n = 0; n < nrows; ++n)
            {
                var row = rows[n];
                row.Height = eachRowHeight;
                row.Top = rowTop; ;
                rowTop += eachRowHeight;
            }
            //----------------------------------
            if (_gridViewRenderE == null) { return; }



            colLeft = 0;
            for (int n = 0; n < ncols; ++n)
            {
                UI.GridColumn col = _gridViewRenderE.GetColumn(n);
                col.Width = eachColWidth;
                col.Left = colLeft;
                colLeft += eachColWidth;
            }
            rowTop = 0;
            for (int n = 0; n < nrows; ++n)
            {
                UI.GridRow row = _gridViewRenderE.GetRow(n);
                row.Height = eachRowHeight;
                row.Top = rowTop; ;
                rowTop += eachRowHeight;
            }
        }
        public void SetCellContent(UIElement ui, int rowIndex, int colIndex)
        {
            if (rowIndex < _gridTable.RowCount && colIndex < _gridTable.ColumnCount)
            {
                if (ui.ParentUI != null)
                {
                    throw new NotSupportedException();
                }
                ui.ParentUI = this;

                UI.GridCell gridCell = _gridLayer.GetCell(rowIndex, colIndex);
                RenderElement re = ui.GetPrimaryRenderElement();
                gridCell.ContentElement = re;
                if (_gridViewRenderE != null)
                {
                    //create parent link  
                    RenderElement.SetParentLink(re, new GridCellParentLink(gridCell, _gridViewRenderE));
                }
                else
                {
                    //create parent link later
                }

            }
        }


        class GridCellParentLink : RenderBoxes.IParentLink
        {
            readonly RenderElement _parentRenderE;
            readonly UI.GridCell _gridCell;
            public GridCellParentLink(UI.GridCell gridCell, RenderElement parentRenderE)
            {
                _parentRenderE = parentRenderE;
                _gridCell = gridCell;
            }
            public RenderElement ParentRenderElement => _parentRenderE;

            public void AdjustLocation(ref int px, ref int py)
            {
                px += _gridCell.X;
                py += _gridCell.Y;
            }

#if DEBUG
            public string dbugGetLinkInfo()
            {
                return "";
            }
#endif            
        }
        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            //check if 
            if (ClearSelectionWhenLostFocus)
            {
                ClearSelection();
            }

            base.OnLostMouseFocus(e);
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            if (_gridSelectionSession != null && _gridSelectionSession.Started)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.Home:
                        _gridSelectionSession.MoveHome();
                        break;
                    case UIKeys.End:
                        _gridSelectionSession.MoveEnd();
                        break;
                    case UIKeys.A:
                        if (e.Ctrl)
                        {
                            //ctrl+a
                            _gridSelectionSession.SelectAll();
                        }
                        break;
                }
            }
            base.OnKeyDown(e);
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (_gridSelectionSession != null && _gridSelectionSession.Started)
            {
                switch (e.KeyCode)
                {
                    case UIKeys.Left:
                        _gridSelectionSession.MoveLeft();
                        break;
                    case UIKeys.Right:
                        _gridSelectionSession.MoveRight();
                        break;
                    case UIKeys.Up:
                        _gridSelectionSession.MoveUp();
                        break;
                    case UIKeys.Down:
                        _gridSelectionSession.MoveDown();
                        break;
                }
            }
            return false;
        }
        //
        public CellSizeStyle CellSizeStyle
        {
            get => _cellSizeStyle;
            set => _cellSizeStyle = value;
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _gridViewRenderE;
        //
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_gridViewRenderE == null)
            {
                var gridRenderE = new GridViewRenderBox(this.Width, this.Height);
                gridRenderE.NeedClipArea = this.NeedClipArea;

                gridRenderE.SetLocation(this.Left, this.Top);
                gridRenderE.SetController(this);

                gridRenderE.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);//TODO: use theme

                this.SetPrimaryRenderElement(gridRenderE);

                _gridViewRenderE = gridRenderE;
                _primElement = _gridViewRenderE;

                gridRenderE.GridLayer = _gridLayer;
                _gridLayer.UpdateParentLink(gridRenderE);

                //we need to update parent link between
                //grid cell and its content


                if (_children != null && _children.Count > 0)
                {
                    foreach (UIElement ui in _children.GetIter())
                    {
                        _gridViewRenderE.AddChild(ui);
                    }
                }

                _gridViewRenderE.GridBorderColor = _gridBorderColor;
            }
            return _gridViewRenderE;
        }


        //--------------------------------------------------
        //selection
        public bool EnableGridCellSelection { get; set; }
        public bool ClearSelectionWhenLostFocus { get; set; }
        public void ClearSelection() => _gridSelectionSession?.ClearSelection();

    }






}