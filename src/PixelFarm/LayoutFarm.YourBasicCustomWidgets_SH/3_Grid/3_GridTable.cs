//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    public enum CellSizeStyle
    {
        ColumnAndRow,//depends on owner column and row
        UniformWidth,
        UniformHeight,
        UniformCell
    }

    enum CellNeighbor
    {
        Left,
        Right,
        Up,
        Down
    }



    class GridColumn
    {

        int _col_index = -1;
        int _left = 0;
        int _columnWidth = 0;
        int _calculatedWidth = 0;
        int _desiredHeight = 0;
        int _columnFlags = 0;
        //
        List<GridCell> _cells = new List<GridCell>();
        GridTable.GridColumnCollection _parentColumnCollection;
        //
        const int COLUMN_FIXED_SIZE = 1 << (1 - 1);
        const int COLUMN_HAS_CUSTOM_SIZE = 1 << (2 - 1);
        public GridColumn(int columnWidth)
        {
            _columnWidth = columnWidth;
        }
        internal void SetParentColumnCollection(GridTable.GridColumnCollection parentColumnCollection)
        {
            _parentColumnCollection = parentColumnCollection;
        }
        public int Width
        {
            get => _columnWidth;
            set => _columnWidth = value;
        }
        public int CalculatedWidth
        {
            get => _calculatedWidth;
            set => _calculatedWidth = value;
        }
        public int ColumnIndex
        {
            get => _col_index;
            internal set => _col_index = value;
        }
        //
        public int Left
        {
            get => _left;
            set => _left = value;
            //int leftdiff = _left - value;
        }
        public int Right => _columnWidth + _left;

        public bool HasCustomSize
        {
            get => (_columnFlags & COLUMN_HAS_CUSTOM_SIZE) != 0;
            set
            {
                if (value)
                {
                    _columnFlags |= COLUMN_HAS_CUSTOM_SIZE;
                }
                else
                {
                    _columnFlags &= ~COLUMN_HAS_CUSTOM_SIZE;
                }
            }
        }
        internal void RemoveRow(GridRow row)
        {
            if (_cells.Count == 0)
            {
                return;
            }
            int rowid = row.RowIndex;
            GridCell removedGridItem = _cells[rowid];
            _cells.RemoveAt(rowid);
        }

        internal void ClearAllRows()
        {
            if (_cells.Count == 0)
            {
                return;
            }
            _cells.Clear();
        }
        internal GridCell CreateGridItemForRow(GridRow row)
        {
            GridCell gridItem = new GridCell(
                this,
                row);
            _cells.Add(gridItem);
            return gridItem;
        }
        internal void AddRowRange(IEnumerable<GridRow> rows, int count)
        {
            GridCell[] newGrids = new GridCell[count];
            int i = 0;
            foreach (GridRow row in rows)
            {
                GridCell gridItem = new GridCell(this, row);
                newGrids[i] = gridItem;
                i++;
            }
            _cells.AddRange(newGrids);
        }
        internal void InsertAfter(int index, GridRow row)
        {
            GridCell gridItem = new GridCell(
                this, row);
            _cells.Insert(index + 1, gridItem);
        }

        internal void MoveRowAfter(GridRow fromRow, GridRow toRow)
        {
            int destRowIndex = toRow.RowIndex;
            if (destRowIndex > fromRow.RowIndex)
            {
                destRowIndex -= 1;
            }
            GridCell fromGridItem = _cells[fromRow.RowIndex];
            _cells.RemoveAt(fromRow.RowIndex);
            _cells.Insert(destRowIndex, fromGridItem);
        }
        public GridColumn PrevColumn
        {
            get
            {
                if (ColumnIndex > 0)
                {
                    return _parentColumnCollection[ColumnIndex - 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GridColumn NextColumn
        {
            get
            {
                if (ColumnIndex < _parentColumnCollection.Count - 1)
                {
                    return _parentColumnCollection[ColumnIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public GridCell GetCell(int rowIndex)
        {
            if (rowIndex < _cells.Count)
            {
                return _cells[rowIndex];
            }
            return null;
        }
        //
        public int CellCount => _cells.Count;

        public IEnumerable<GridCell> GetTopDownGridCellIter()
        {
            int j = _cells.Count;
            for (int i = 0; i < j; ++i)
            {
                yield return _cells[i];
            }
        }

        public int DesiredWidth
        {
            get => _calculatedWidth;
            set => _calculatedWidth = value;
        }

        public int DesiredHeight
        {
            get => _desiredHeight;
            set => _desiredHeight = value;
        }

#if DEBUG
        public override string ToString()
        {
            return "left=" + _left + ",width=" + Width;
        }
#endif
    }

    class GridRow
    {
        GridTable.GridRowCollection _parentRowCollection;
        int _row_Index = -1;
        int _top = 0;
        int _rowHeight = 0;
        int _desiredHeight;
        int _rowFlags = 0;
        const int FIXED_HEIGHT = 1 << (1 - 1);
        const int HAS_CALCULATE_HEIGHT = 1 << (2 - 1);
        public GridRow(int initRowHeight)
        {
            _rowHeight = initRowHeight;
        }

        public int RowIndex
        {
            get => _row_Index;
            internal set => _row_Index = value;
        }
        internal void SetOwnerParentRowCollection(GridTable.GridRowCollection parentRowCollection)
        {
            _parentRowCollection = parentRowCollection;
        }
        public int DesiredHeight
        {
            get
            {
                if ((_rowFlags & HAS_CALCULATE_HEIGHT) != 0)
                {
                    return _desiredHeight;
                }
                else
                {
                    return _rowHeight;
                }
            }
        }
        //
        public bool IsBoundToGrid => _parentRowCollection != null;
        //
        public int OwnerGridRowCount => _parentRowCollection.GridTable.RowCount;
        //
        public int Height
        {
            get => _rowHeight;
            set => _rowHeight = value;
        }
        public int Top
        {
            get => _top;
            set => _top = value;
        }
        public int Bottom
        {
            get => _top + _rowHeight;
        }
        public int RowId
        {
            get => RowIndex;
        }
        public GridRow PrevRow
        {
            get
            {
                return (RowIndex > 0) ?
                    _parentRowCollection[RowIndex - 1] :
                    null;
            }
        }
        public GridRow NextRow
        {
            get
            {
                if (RowIndex < _parentRowCollection.Count - 1)
                {
                    return _parentRowCollection[RowIndex + 1];
                }
                else
                {
                    return null;
                }
            }
        }
        public void AdjustBottom(int bottomPos)
        {
            //2010-09-04
            //int i = 0;
            //foreach (ArtUIGridItem affectedBox in parentRowCollection.GetCellIter(this))
            //{
            //    if (affectedBox != null)
            //    {
            //        Rectangle oldRect = affectedBox.Rect;
            //        int ydiff = bottomPos - oldRect.Bottom;
            //        if (ydiff != 0) 
            //        {
            //           
            //            affectedBox.SetRectBound(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height += ydiff);
            //            affectedBox.ReArrangeContent();  
            //            //affectedBox.PerformLayout(); 
            //        }
            //        if (i == 0)
            //        {
            //            rowHeight = affectedBox.Height;
            //        }
            //    }
            //    i++;
            //}
        }
        public void AdjustTop(int topPos)
        {
            //ArtUIGridItem[] children = parentRowCollection.GetGridItems(this);//neighbors
            if ((_rowFlags & FIXED_HEIGHT) != 0)
            {
                int i = 0;
                //foreach (ArtUIGridItem gridItem in parentRowCollection.GetCellIter(this))
                //{

                //    Rectangle oldRect = gridItem.Rect;
                //    int oldY = oldRect.Y;
                //    int ydiff = oldY - topPos;
                //    if (ydiff != 0)
                //    {
                //        gridItem.SetRectBound(oldRect.X, topPos, oldRect.Width, oldRect.Height += ydiff);
                //        
                //        gridItem.ReCalculateContentSize(); 
                //        gridItem.ReArrangeContent(); 
                //        //gridItem.PerformLayout();  
                //    }
                //    if (i == 0)
                //    {
                //        rowHeight = gridItem.Height;
                //        top = gridItem.Y;
                //    }
                //    i++;
                //}
            }
            else
            {
                int i = 0;
                //foreach (ArtUIGridItem gridItem in parentRowCollection.GetCellIter(this))
                //{
                //    Point oldPos = gridItem.Location;
                //    gridItem.SetLocation(oldPos.X, topPos);
                //    if (i == 0)
                //    {
                //        rowHeight = gridItem.Height;
                //        top = gridItem.Y;
                //    }
                //    i++;
                //}
            }

            //rowHeight = children[0].Height;
            //top = children[0].Y;
        }
        public void SetTopAndHeight(int top, int height)
        {
            _top = top;
            _rowHeight = height;
            //foreach (ArtUIGridItem cell in parentRowCollection.GetCellIter(this))
            //{
            //    Rectangle oldrect = cell.Rect;
            //    oldrect.Y = top;
            //    oldrect.Height = height;
            //    cell.SetRectBound(oldrect);
            //}
        }
        public void CalculateRowHeight()
        {
            _desiredHeight = _rowHeight;
            _rowFlags |= HAS_CALCULATE_HEIGHT;
        }
        internal void AcceptDesiredHeight(int currentTop)
        {
            if ((_rowFlags & HAS_CALCULATE_HEIGHT) != 0)
            {
                _rowHeight = _desiredHeight;
            }
            else
            {
            }
            _top = currentTop;
        }
#if DEBUG
        public override string ToString()
        {
            return "top=" + _top + ",height=" + Height;
        }
#endif

    }




    class GridCell
    {
        readonly GridRow _row;
        readonly GridColumn _column;
        RenderElement _content;

        internal GridCell(GridColumn column, GridRow row)
        {
            _row = row;
            _column = column;
        }

        public int RowIndex => _row.RowIndex;
        //
        public int ColumnIndex => _column.ColumnIndex;

        public GridRow Row => _row;
        public GridColumn Column => _column;
        //
        public Rectangle Rect => new Rectangle(_column.Left, _row.Top, _column.Width, _row.Height);
        //
        public RenderElement ContentElement
        {
            get => _content;
            set => _content = value;
        }

        //
        public int X => _column.Left;
        //
        public int Y => _row.Top;
        //
        public int Right => _column.Right;
        //
        public int Bottom => _row.Bottom;
        //
        public int Width => _column.Width;
        //
        public int Height => _row.Height;
        //
        public Point RightBottomCorner => new Point(_column.Right, _row.Bottom);
        //
        public Point RightTopCorner => new Point(_column.Right, _row.Top);
        //
        public GridCell GetNeighborGrid(CellNeighbor nb)
        {
            switch (nb)
            {
                case CellNeighbor.Left:
                    {
                        GridColumn prevColumn = _column.PrevColumn;
                        if (prevColumn != null)
                        {
                            return prevColumn.GetCell(_row.RowIndex);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Right:
                    {
                        GridColumn nextColumn = _column.NextColumn;
                        if (nextColumn != null)
                        {
                            return nextColumn.GetCell(_row.RowIndex);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Up:
                    {
                        if (_row.RowIndex > 0)
                        {
                            return _column.GetCell(_row.RowIndex - 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                case CellNeighbor.Down:
                    {
                        if (_row.RowIndex < _row.OwnerGridRowCount - 1)
                        {
                            return _column.GetCell(_row.RowIndex + 1);
                        }
                        else
                        {
                            return null;
                        }
                    }
                default:
                    {
#if DEBUG
                        throw new NotSupportedException();
#else
                            return null;
#endif
                    }
            }
        }

#if DEBUG
        public string dbugGetLinkInfo()
        {
            return "grid-link";
        }
        public override string ToString()
        {
            return _row.RowIndex.ToString() + "," + _column.ColumnIndex.ToString() + " " + base.ToString();
        }
#endif

    }


    partial class GridTable
    {
        readonly GridColumnCollection _cols;
        readonly GridRowCollection _rows;
        public GridTable()
        {
            _cols = new GridColumnCollection(this);
            _rows = new GridRowCollection(this);
        }
        public void Clear()
        {
            _cols.Clear();
            _rows.ClearAll();
        }
        //
        public IEnumerable<GridColumn> GetColumnIter() => _cols.GetColumnIter();
        //
        public IEnumerable<GridRow> GetRowIter() => _rows.GetRowIter();
        //
        public GridCell GetCell(int rowId, int columnId) => _cols[columnId].GetCell(rowId);
        //
        public int RowCount => _rows.Count;
        //
        public int ColumnCount => _cols.Count;
        //
        public GridRowCollection Rows => _rows;
        //
        public GridColumnCollection Columns => _cols;
        //
    }

    partial class GridTable
    {
        public class GridColumnCollection
        {
            readonly GridTable _table;
            readonly List<GridColumn> _cols = new List<GridColumn>();
            internal GridColumnCollection(GridTable table)
            {
                _table = table;
            }
            public void Clear() => _cols.Clear();

            public GridColumn GetColumn(int index) => _cols[index];

            public void Add(GridColumn newColumnDef)
            {
                int j = _cols.Count;
                if (j == 0)
                {
                    newColumnDef.Left = 0;
                    newColumnDef.ColumnIndex = 0;
                }
                else
                {
                    newColumnDef.Left = _cols[j - 1].Right + 1;
                    newColumnDef.ColumnIndex = j;
                }
                newColumnDef.SetParentColumnCollection(this);
                _cols.Add(newColumnDef);
#if DEBUG
                //contArrVisitor.dbug_StartLayoutTrace("GridCollection::Add(GridColumn)");
#endif

                //InvalidateGraphicAndStartBubbleUp();
#if DEBUG
                //contArrVisitor.dbug_EndLayoutTrace();
#endif

                //--------------------------------------------
            }
            //void InvalidateGraphicAndStartBubbleUp()
            //{
            //}
            public void Insert(int index, GridColumn coldef)
            {
                _cols.Insert(index, coldef);
                int j = _cols.Count;
                for (int i = index + 1; i < j; i++)
                {
                    _cols[i].ColumnIndex = i;
                }


                foreach (GridRow rowdef in _table.GetRowIter())
                {
                    coldef.CreateGridItemForRow(rowdef);
                }
                //--------------------------------------------
                //                ContentArrangementVisitor contArrVisitor = new ContentArrangementVisitor(ownerGridLayer);
                //#if DEBUG
                //                //contArrVisitor.dbug_StartLayoutTrace("GridColumnCollection::Insert");
                //                contArrVisitor.dbug_StartLayoutTrace(dbugVisualElementLayoutMsg.GridColumnCollection_Insert);
                //#endif

                //InvalidateGraphicAndStartBubbleUp();
                //OwnerGridLayer.OwnerInvalidateGraphicAndStartBubbleUp();


                //#if DEBUG
                //                contArrVisitor.dbug_EndLayoutTrace();
                //#endif
                //--------------------------------------------
            }

            public void Remove(int columnid)
            {
                GridColumn col = _cols[columnid];
                int removedColumnWidth = col.Width;
                col.ClearAllRows();
                int j = _cols.Count;
                for (int i = columnid + 1; i < j; i++)
                {
                    col = _cols[i];
                    col.ColumnIndex -= 1;
                    col.Left -= removedColumnWidth;
                }
                _cols.RemoveAt(columnid);
                OwnerInvalidateGraphicAndStartBubbleUp();
            }
            void OwnerInvalidateGraphicAndStartBubbleUp()
            {
            }
            public GridColumn First
            {
                get
                {
                    if (_cols.Count > 0)
                    {
                        return _cols[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public GridColumn Last
            {
                get
                {
                    if (_cols.Count > 0)
                    {
                        return _cols[_cols.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }


            public void MoveColumnAfter(GridColumn tobeMovedColumn, GridColumn afterColumn)
            {
                //---------------------------------------------------------

                int toTargetColumnIndex = afterColumn.ColumnIndex;
                if (tobeMovedColumn.ColumnIndex < toTargetColumnIndex)
                {
                    toTargetColumnIndex -= 1;
                }
                _cols.RemoveAt(tobeMovedColumn.ColumnIndex);
                _cols.Insert(afterColumn.ColumnIndex, tobeMovedColumn);
                UpdateColumnIndex(Math.Min(afterColumn.ColumnIndex, toTargetColumnIndex));
                this.OwnerInvalidateGraphicAndStartBubbleUp();
            }

            void UpdateColumnIndex(int startIndex)
            {
                int j = _cols.Count;
                if (startIndex < j)
                {
                    for (int i = startIndex; i < j; i++)
                    {
                        _cols[i].ColumnIndex = i;
                    }
                }
            }
            //
            public int Count => _cols.Count;
            //
            public GridColumn this[int index] => _cols[index];
            //
            public IEnumerable<GridColumn> GetColumnIter()
            {
                int j = _cols.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return _cols[i];
                }
            }
            public IEnumerable<GridColumn> GetColumnReverseIter()
            {
                for (int i = _cols.Count - 1; i > -1; --i)
                {
                    yield return _cols[i];
                }
            }
            public GridColumn GetColumnAtPosition(int x)
            {
                foreach (GridColumn coldef in _cols)
                {
                    if (coldef.Right >= x)
                    {
                        return coldef;
                    }
                }
                return null;
            }
        }

        public class GridRowCollection
        {
            GridTable _table;
            List<GridRow> _rows = new List<GridRow>();
            internal GridRowCollection(GridTable table)
            {
                _table = table;
            }
            public GridRow GetRow(int index) => _rows[index];
            //
            public GridRow this[int index] => _rows[index];
            //
            public int Count => _rows.Count;
            //
            public void MoveRowAfter(GridRow fromRow, GridRow toRow)
            {
                int toRowIndex = toRow.RowIndex;
                if (fromRow.RowIndex < toRowIndex)
                {
                    toRowIndex -= 1;
                }

                foreach (GridColumn col in _table.GetColumnIter())
                {
                    col.MoveRowAfter(fromRow, toRow);
                }

                _rows.RemoveAt(fromRow.RowIndex);
                _rows.Insert(toRowIndex, fromRow);
                UpdateRowIndex(fromRow, toRow);
            }

            void UpdateRowIndex(GridRow row1, GridRow row2)
            {
                if (row1.RowIndex < row2.RowIndex)
                {
                    int stopRowIndex = row2.RowIndex;
                    for (int i = row1.RowIndex; i <= stopRowIndex; i++)
                    {
                        _rows[i].RowIndex = i;
                    }
                }
                else
                {
                    int stopRowIndex = row1.RowIndex;
                    for (int i = row2.RowIndex; i <= stopRowIndex; i++)
                    {
                        _rows[i].RowIndex = i;
                    }
                }
            }

            public GridRow First
            {
                get
                {
                    if (_rows.Count > 0)
                    {
                        return _rows[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            public GridRow Last
            {
                get
                {
                    if (_rows.Count > 0)
                    {
                        return _rows[_rows.Count - 1];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            internal IEnumerable<GridCell> GetCellIter(GridRow rowdef)
            {
                int rowId = rowdef.RowIndex;
                if (rowId > -1 && rowId < _rows.Count)
                {
                    foreach (GridColumn coldef in _table.GetColumnIter())
                    {
                        yield return coldef.GetCell(rowId);
                    }
                }
            }
            public IEnumerable<GridRow> GetRowIter()
            {
                foreach (GridRow rowdef in _rows)
                {
                    yield return rowdef;
                }
            }

            public void Add(GridRow row)
            {
                int lastcount = _rows.Count;
                row.RowIndex = lastcount;
                if (lastcount > 0)
                {
                    row.Top = _rows[lastcount - 1].Bottom;
                }

                _rows.Add(row);
                if (!row.IsBoundToGrid)
                {
                    foreach (GridColumn column in _table.GetColumnIter())
                    {
                        column.CreateGridItemForRow(row);
                    }
                }
                row.SetOwnerParentRowCollection(this);
                OwnerInvalidateGraphicAndStartBubbleUp();
            }
            void OwnerInvalidateGraphicAndStartBubbleUp()
            {
            }
            public void Remove(int rowid)
            {
                GridRow removedRow = _rows[rowid];
                foreach (GridColumn coldef in _table.GetColumnIter())
                {
                    coldef.RemoveRow(removedRow);//
                }

                _rows.RemoveAt(rowid);
                int j = _rows.Count;
                int removeRowHeight = removedRow.Height;
                for (int i = rowid; i < j; i++)
                {
                    GridRow r = _rows[i];
                    r.RowIndex--;
                    r.Top -= removeRowHeight;
                }


                this.OwnerInvalidateGraphicAndStartBubbleUp();
            }
            public GridRow AddRow(int rowHeight)
            {
                return AddRowAfter(_rows.Count - 1, rowHeight);
            }

            public GridRow AddRowAfter(int afterRowId, int rowHeight)
            {
                int newrowId = afterRowId + 1;
                GridRow newGridRow = null;
                if (afterRowId == -1)
                {
                    newGridRow = new GridRow(rowHeight);
                    newGridRow.Top = 0;
                    Add(newGridRow);
                }
                else
                {
                    GridRow refRowDefinition = _rows[afterRowId];
                    newGridRow = new GridRow(rowHeight);
                    newGridRow.Top = refRowDefinition.Top + refRowDefinition.Height;
                    InsertAfter(afterRowId, newGridRow);
                }

                return newGridRow;
            }
            public void ClearAll()
            {
                foreach (GridColumn coldef in _table.GetColumnIter())
                {
                    coldef.ClearAllRows();
                }
                _rows.Clear();
            }

            internal void InsertAfter(int afterRowId, GridRow row)
            {
                int newRowHeight = row.Height;
                row.SetOwnerParentRowCollection(this);
                row.RowIndex = afterRowId + 1;
                _rows.Insert(afterRowId + 1, row);
                foreach (GridColumn coldef in _table.GetColumnIter())
                {
                    coldef.InsertAfter(afterRowId, row);
                }

                int j = _rows.Count;
                for (int i = afterRowId + 2; i < j; i++)
                {
                    GridRow r = _rows[i];
                    r.RowIndex = i;
                }
            }
            public void InsertAfter(GridRow afterThisRow, GridRow row)
            {
                InsertAfter(afterThisRow.RowIndex, row);
            }

            public GridRow GetRowAtPos(int y)
            {
                int j = _rows.Count;
                for (int i = 0; i < j; ++i)
                {
                    if (_rows[i].Bottom >= y)
                    {
                        return _rows[i];
                    }
                }
                return null;
            }
            //
            public GridTable GridTable => _table;

        }
    }
}