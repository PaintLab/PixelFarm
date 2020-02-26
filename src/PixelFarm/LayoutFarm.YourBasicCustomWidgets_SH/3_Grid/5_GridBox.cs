//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    class GridViewRenderBox : CustomRenderBox
    {
        GridLayer _gridLayer;
        public GridViewRenderBox(RootGraphic rootgfx, int w, int h)
            : base(rootgfx, w, h)
        {
            //this.LayoutHint = BoxContentLayoutKind.VerticalStack;
        }
        //public override void AddChild(RenderElement renderE)
        //{
        //    base.AddChild(renderE);
        //}
        public void BuildGrid(GridTable gridTable, CellSizeStyle cellSizeStyle)
        {
            _gridLayer = new GridLayer(this, cellSizeStyle, gridTable);
        }
        //
        public GridLayer GridLayer => _gridLayer;
        //
        public void SetContent(int r, int c, RenderElement re)
        {
            _gridLayer.GetCell(r, c).ContentElement = re;
        }
        public void SetContent(int r, int c, UIElement ui)
        {
            _gridLayer.GetCell(r, c).ContentElement = ui.GetPrimaryRenderElement(this.Root);
        }
        protected override void RenderClientContent(DrawBoard d, Rectangle updateArea)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{
            //}
#endif
            //sample bg  
            //TODO: review here again


            if (this.MayHasViewport)
            {
                d.FillRectangle(BackColor, ViewportLeft, ViewportTop, this.Width, this.Height);
            }
            else
            {
                d.FillRectangle(BackColor, 0, 0, this.Width, this.Height);
            }

            _gridLayer.DrawChildContent(d, updateArea);
            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(d, ref updateArea);
            }
#if DEBUG
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }
    }

    enum GridSelectionStyle : byte
    {
        RectBox,
        FlowBox
    }

    class GridSelectionSession
    {
        public GridCell _latestHitCell;
        public GridCell _beginSelectedCell;
        public Box _bodyBox; //used in RectBox mode and FlowBox mode
        public Box _headBox;
        public Box _tailBox;
        GridSelectionStyle _gridSelectionStyle;
        bool _moreThan1Cell;


        GridView _targetGridView;
        public GridSelectionSession()
        {

            _gridSelectionStyle = GridSelectionStyle.RectBox;
        }
        public void SetTargetGridView(GridView targetGridView)
        {
            _targetGridView = targetGridView;

            _bodyBox = SetupHighlightBox();

            //----
            targetGridView.AddChild(_bodyBox);

            if (_gridSelectionStyle == GridSelectionStyle.FlowBox)
            {
                //prepare 3 box
                //head, body, tail
                _headBox = SetupHighlightBox();
                _tailBox = SetupHighlightBox();
                // 
                targetGridView.AddChild(_headBox);
                targetGridView.AddChild(_tailBox);
            }

        }

        static Box SetupHighlightBox()
        {
            var box = new Box(10, 10);
            box.BackColor = new Color(100, 255, 0, 0);
            box.Visible = false;
            box.TransparentAllMouseEvents = true;
            return box;
        }
        public GridSelectionStyle GridSelectionStyle
        {
            get => _gridSelectionStyle;
            set => _gridSelectionStyle = value;
        }
        public void StartAt(GridCell hitCell)
        {

            _moreThan1Cell = false;
            _beginSelectedCell = _latestHitCell = hitCell;


            _bodyBox.SetLocationAndSize(hitCell.X, hitCell.Y, hitCell.Width, hitCell.Height);
            _bodyBox.Visible = true;
            Started = true;
        }

        public bool Started { get; private set; }
        void SetLatestHit_RectBoxModel(GridCell hitCell)
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
        void SetLatestHit_FlowBoxModel(GridCell hitCell)
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
                                GridCell lastCellOfHeadRow = _targetGridView.GetCell(hitCell.RowIndex, _targetGridView.ColumnCount - 1);
                                _headBox.SetLocation(hitCell.X, hitCell.Y);
                                _headBox.SetSize(lastCellOfHeadRow.Right - hitCell.X, lastCellOfHeadRow.Height);
                            }
                            //-----
                            {
                                GridCell lastCellOfTailRow = _targetGridView.GetCell(_beginSelectedCell.RowIndex, 0);
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
                                GridCell lastCellOfHeadRow = _targetGridView.GetCell(hitCell.RowIndex, _targetGridView.ColumnCount - 1);
                                _headBox.SetLocation(hitCell.X, hitCell.Y);
                                _headBox.SetSize(lastCellOfHeadRow.Right - _headBox.Left, lastCellOfHeadRow.Height);
                            }
                            //-----
                            //in between


                            //-----
                            {
                                GridCell lastCellOfTailRow = _targetGridView.GetCell(_beginSelectedCell.RowIndex, 0);
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
        public void SetLatestHit(GridCell hitCell)
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


    public class GridView : AbstractBox
    {
        GridViewRenderBox _gridViewRenderE;
        CellSizeStyle _cellSizeStyle;
        GridTable _gridTable;
        GridSelectionSession _gridSelectionSession;
        Color _gridBorderColor;

        public struct GridCellInfo
        {
            public readonly int Row;
            public readonly int Column;
            public GridCellInfo(int column, int row)
            {
                Column = column;
                Row = row;
            }

            public bool IsEmpty
            {
                get { return Row < 0; }
            }

#if DEBUG
            public override string ToString()
            {
                return Column + "," + Row;
            }
#endif
        }

        public GridView(int width, int height)
            : base(width, height)
        {
            //has special grid layer
            _gridTable = new GridTable();
            EnableGridCellSelection = true;
            ClearSelectionWhenLostFocus = true;
            AcceptKeyboardFocus = true;

            _gridBorderColor = Color.Black; //default//TODO: impl Theme classes...
        }
        public override void PerformContentLayout()
        {
            //calculate grid width
            var cols = _gridTable.Columns;
            int ncols = cols.Count;
            int widthSum = 0;
            for (int n = 0; n < ncols; ++n)
            {
                widthSum += cols[n].Width;
            }
            //2. create rows
            var rows = _gridTable.Rows;
            int heightSum = 0;
            int nrows = rows.Count;
            for (int n = 0; n < nrows; ++n)
            {
                heightSum += rows[n].Height;
            }

            base.PerformContentLayout();

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

            //1. create cols
            var cols = _gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width 
                cols.Add(new GridColumn(eachColumnWidth));
            }
            //2. create rows
            var rows = _gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {
                rows.Add(new GridRow(eachRowHeight));
            }

            //***
            this.InvalidateLayout();
        }

        public void BuildGrid(int ncols, int nrows, CellSizeStyle cellSizeStyle)
        {
            _cellSizeStyle = cellSizeStyle;
            //1. create cols
            var cols = _gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                GridColumn col = new GridColumn(1);
                cols.Add(col);
            }
            //2. create rows
            var rows = _gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {
                rows.Add(new GridRow(1));
            }
        }
        //
        public int RowCount => _gridTable.RowCount;
        public int ColumnCount => _gridTable.ColumnCount;
        //
        internal GridCell GetCell(int row, int col) => _gridTable.GetCell(row, col);

        public GridCellInfo GetCellInfoByMousePosition(int x, int y)
        {
            GridLayer layer = _gridViewRenderE.GridLayer;
            GridCell hitCell = layer.GetCellByPosition(x, y);
            if (hitCell != null)
            {
                return new GridCellInfo(hitCell.ColumnIndex, hitCell.RowIndex);
            }
            else
            {
                return new GridCellInfo(-1, -1);
            }
        }
        public Color GridBorderColor
        {
            get => _gridBorderColor;
            set
            {
                _gridBorderColor = value;
                if (_gridViewRenderE != null)
                {
                    _gridViewRenderE.GridLayer.GridBorderColor = value;
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
                    GridCell cell = _gridTable.GetCell(r, c);
                    RenderElement cellContent = cell.ContentElement as RenderElement;
                    if (cellContent != null)
                    {
                        if (cellContent.HasParent)
                        {
                            cellContent.RemoveSelf();
                        }
                        //
                        cell.ContentElement = null;
                    }
                }
            }

            _gridViewRenderE.InvalidateGraphics();
        }

        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            int cur_vwLeft = this.ViewportLeft;
            int cur_vwTop = this.ViewportTop;
            int newVwLeft = (int)(cur_vwTop - (e.Delta * 10f / 120f));
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
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            //System.Console.WriteLine(e.X + "," + e.Y);
            if (e.IsDragging)
            {
                if (this.EnableGridCellSelection)
                {
                    GridLayer layer = _gridViewRenderE.GridLayer;
                    GridCell hitCell = layer.GetCellByPosition(e.X, e.Y);
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
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            GridLayer layer = _gridViewRenderE.GridLayer;
            GridCell hitCell = layer.GetCellByPosition(e.X, e.Y);
            if (hitCell != null)
            {
                var box = hitCell.ContentElement as RenderBoxBase;
                if (box != null)
                {
                    if (box.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y))
                    {
                        IUIEventListener evenListener = box.GetController() as IUIEventListener;
                        if (evenListener != null)
                        {
                            int tmpX = e.X;
                            int tmpY = e.Y;
                            e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                            evenListener.ListenMouseUp(e);
                            e.SetLocation(tmpX, tmpY);
                        }
                    }
                }
                ////
                ////move _dragController to the selected cell? 
                ////
                //if (EnableGridCellSelection)
                //{
                //    //--------
                //    if (_gridSelectionSession == null)
                //    {
                //        _gridSelectionSession = new GridSelectionSession();
                //        _gridSelectionSession.SetTargetGridView(this);
                //    }
                //    _gridSelectionSession.StartAt(hitCell);
                //}
            }
            base.OnMouseUp(e);
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            //check if cell content
            //find grid item 

            //System.Console.WriteLine(e.X + "," + e.Y);

            GridLayer layer = _gridViewRenderE.GridLayer;
            GridCell hitCell = layer.GetCellByPosition(e.X, e.Y);
            if (hitCell != null)
            {
                var box = hitCell.ContentElement as RenderBoxBase;
                if (box != null)
                {
                    if (box.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y))
                    {
                        IUIEventListener evenListener = box.GetController() as IUIEventListener;
                        if (evenListener != null)
                        {
                            int tmpX = e.X;
                            int tmpY = e.Y;
                            e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                            evenListener.ListenMouseDown(e);
                            e.SetLocation(tmpX, tmpY);
                        }
                    }
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


            var gridLayer = _gridViewRenderE.GridLayer;
            colLeft = 0;
            for (int n = 0; n < ncols; ++n)
            {
                var col = gridLayer.GetColumn(n);
                col.Width = eachColWidth;
                col.Left = colLeft;
                colLeft += eachColWidth;
            }
            rowTop = 0;
            for (int n = 0; n < nrows; ++n)
            {
                var row = gridLayer.GetRow(n);
                row.Height = eachRowHeight;
                row.Top = rowTop; ;
                rowTop += eachRowHeight;
            }
        }
        public void SetCellContent(UIElement ui, int rowIndex, int colIndex)
        {
            if (rowIndex < _gridTable.RowCount && colIndex < _gridTable.ColumnCount)
            {
                _gridTable.GetCell(rowIndex, colIndex).ContentElement = ui;
                if (_gridViewRenderE != null)
                {

                    RenderElement re = ui.GetPrimaryRenderElement(_gridViewRenderE.Root);
                    _gridViewRenderE.SetContent(rowIndex, colIndex, re);


                    GridLayer layer = _gridViewRenderE.GridLayer;
                    GridCell gridCell = layer.GetCell(rowIndex, colIndex);

                    GridCellParentLink parentLink = new GridCellParentLink(gridCell, _gridViewRenderE);
                    RenderElement.SetParentLink(re, parentLink);
                }
            }
        }




        class GridCellParentLink : RenderBoxes.IParentLink
        {
            RenderElement _parentRenderE;
            GridCell _gridCell;
            public GridCellParentLink(GridCell gridCell, RenderElement parentRenderE)
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

            public RenderElement FindOverlapedChildElementAtPoint(RenderElement afterThisChild, Point point)
            {
                return null;
            }
        }


        protected override void OnLostMouseFocus(UIMouseEventArgs e)
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
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_gridViewRenderE == null)
            {
                var myGridBox = new GridViewRenderBox(rootgfx, this.Width, this.Height);
                myGridBox.HasSpecificWidthAndHeight = true;//***
                myGridBox.NeedClipArea = this.NeedClipArea;

                myGridBox.SetLocation(this.Left, this.Top);
                myGridBox.SetController(this);
                myGridBox.BackColor = KnownColors.FromKnownColor(KnownColor.LightGray);
                this.SetPrimaryRenderElement(myGridBox);
                _gridViewRenderE = myGridBox;
                //create layers
                int nrows = _gridTable.RowCount;
                int ncols = _gridTable.ColumnCount;
                //----------------------------------------        


                myGridBox.BuildGrid(_gridTable, this.CellSizeStyle);
                //add grid content

                GridLayer layer = _gridViewRenderE.GridLayer;
                for (int c = 0; c < ncols; ++c)
                {
                    for (int r = 0; r < nrows; ++r)
                    {
                        GridCell gridCell = _gridTable.GetCell(r, c);
                        var content = gridCell.ContentElement as UIElement;
                        if (content != null)
                        {
                            myGridBox.SetContent(r, c, content);
                            RenderElement uiRenderE = content.GetPrimaryRenderElement(rootgfx);
                            GridCellParentLink parentLink = new GridCellParentLink(gridCell, _gridViewRenderE);
                            RenderElement.SetParentLink(uiRenderE, parentLink);
                        }
                    }
                }

                if (ChildCount > 0)
                {
                    foreach (UIElement ui in GetChildIter())
                    {
                        _gridViewRenderE.AddChild(ui);

                    }
                }

                _gridViewRenderE.GridLayer.GridBorderColor = _gridBorderColor;
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