//Apache2, 2014-2018, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    class GridBoxRenderElement : CustomRenderBox
    {
        GridLayer gridLayer;
        public GridBoxRenderElement(RootGraphic rootgfx, int w, int h)
            : base(rootgfx, w, h)
        {
        }
        public void BuildGrid(GridTable gridTable, CellSizeStyle cellSizeStyle)
        {
            this.gridLayer = new GridLayer(this, cellSizeStyle, gridTable);
        }
        public GridLayer GridLayer
        {
            get { return this.gridLayer; }
        }
        public void SetContent(int r, int c, RenderElement re)
        {
            gridLayer.GetCell(r, c).ContentElement = re;
        }
        public void SetContent(int r, int c, UIElement ui)
        {
            gridLayer.GetCell(r, c).ContentElement = ui.GetPrimaryRenderElement(this.Root);
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
#if DEBUG
            //if (this.dbugBreak)
            //{
            //}
#endif
            //sample bg   
            //canvas.FillRectangle(BackColor, updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height);
            canvas.FillRectangle(BackColor, 0, 0, this.Width, this.Height);
            gridLayer.DrawChildContent(canvas, updateArea);
            if (this.HasDefaultLayer)
            {
                this.DrawDefaultLayer(canvas, ref updateArea);
            }
#if DEBUG
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }
    }

    public class GridBox : EaseBox
    {
        GridBoxRenderElement gridBoxRenderE;

        CellSizeStyle cellSizeStyle;
        SimpleBox _dragController;
        GridTable gridTable;
        public GridBox(int width, int height)
            : base(width, height)
        {
            //has special grid layer
            gridTable = new GridTable();

        }
        public void BuildGrid(int ncols, int nrows, CellSizeStyle cellSizeStyle)
        {
            this.cellSizeStyle = cellSizeStyle;
            //1. create cols
            var cols = gridTable.Columns;
            for (int n = 0; n < ncols; ++n)
            {
                //create with defatul width
                GridColumn col = new GridColumn(1);
                cols.Add(col);
            }
            //2. create rows
            var rows = gridTable.Rows;
            for (int n = 0; n < nrows; ++n)
            {
                rows.Add(new GridRow(1));
            }
        }

        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {

            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            //check if cell content
            //find grid item
            int nrows = gridTable.RowCount;
            int colCount = gridTable.ColumnCount;
            var rows = gridTable.Rows;
            GridLayer layer = gridBoxRenderE.GridLayer;

            GridCell hitCell = layer.GetGridItemByPosition(e.X, e.Y);
            if (hitCell != null)
            {
                var box = hitCell.ContentElement as RenderBoxBase;
                if (box != null)
                {
                    if (box.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y))
                    {
                        IEventListener evenListener = box.GetController() as IEventListener;
                        if (evenListener != null)
                        {
                            int tmpX = e.X;
                            int tmpY = e.Y;
                            e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                            evenListener.ListenMouseDown(e);
                            e.SetLocation(tmpX, tmpY);
                        } 
                    }
                    //var re = content.GetPrimaryRenderElement(gridBoxRenderE.Root);
                    //if (re.ContainPoint(e.X - hitCell.X, e.Y - hitCell.Y))
                    //{
                    //    //hit on this elem too!
                    //    IEventListener evenListener = (IEventListener)content;

                    //    int tmpX = e.X;
                    //    int tmpY = e.Y;
                    //    e.SetLocation(tmpX - hitCell.X, tmpY - hitCell.Y);
                    //    evenListener.ListenMouseDown(e);
                    //    e.SetLocation(tmpX, tmpY);
                    //}
                }
            }

            //bool found = false;
            //for (int n = 0; !found && n < nrows; ++n)
            //{
            //    for (int cc = 0; !found && cc < colCount; ++cc)
            //    {

            //        GridCell gridCell = gridTable.GetCell(n, cc);
            //        if (gridCell != null && gridCell.ContentElement != null)
            //        {
            //            //check if the content is on the hit
            //            GridCell gridCell1 = layer.GetCell(n, cc);
            //            var content = gridCell.ContentElement as UIElement;
            //            if (content != null)
            //            {
            //                var re = content.GetPrimaryRenderElement(gridBoxRenderE.Root);
            //                if (re.ContainPoint(e.X - gridCell1.X, e.Y - gridCell1.Y))
            //                {
            //                    //hit on this elem too!
            //                    IEventListener evenListener = (IEventListener)content;

            //                    int tmpX = e.X;
            //                    int tmpY = e.Y;
            //                    e.SetLocation(tmpX - gridCell1.X, tmpY - gridCell1.Y);
            //                    evenListener.ListenMouseDown(e);
            //                    e.SetLocation(tmpX, tmpY);
            //                    found = true;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}

            base.OnMouseDown(e);
        }
        public override void SetSize(int width, int height)
        {
            //readjust cellsize
            base.SetSize(width, height);
            //----------------------------------
            var cols = gridTable.Columns;
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

            var rows = gridTable.Rows;
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
            if (this.gridBoxRenderE == null) { return; }


            var gridLayer = gridBoxRenderE.GridLayer;
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
        public void AddUI(UIElement ui, int rowIndex, int colIndex)
        {
            if (rowIndex < gridTable.RowCount && colIndex < gridTable.ColumnCount)
            {
                gridTable.GetCell(rowIndex, colIndex).ContentElement = ui;
                if (this.gridBoxRenderE != null)
                {
                    RenderElement re = ui.GetPrimaryRenderElement(gridBoxRenderE.Root);
                    gridBoxRenderE.SetContent(rowIndex, colIndex, re);


                    GridLayer layer = gridBoxRenderE.GridLayer;
                    GridCell gridCell = layer.GetCell(rowIndex, colIndex);

                    GridCellParentLink parentLink = new GridCellParentLink(gridCell, gridBoxRenderE);
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
            public RenderElement ParentRenderElement
            {
                get
                {
                    return _parentRenderE;
                }
            }

            public void AdjustLocation(ref Point p)
            {
                p.X += _gridCell.X;
                p.Y += _gridCell.Y;
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


        public CellSizeStyle CellSizeStyle
        {
            get { return this.cellSizeStyle; }
            set { this.cellSizeStyle = value; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.gridBoxRenderE; }
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.gridBoxRenderE != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (gridBoxRenderE == null)
            {
                var myGridBox = new GridBoxRenderElement(rootgfx, this.Width, this.Height);
                myGridBox.SetLocation(this.Left, this.Top);
                myGridBox.SetController(this);
                this.SetPrimaryRenderElement(myGridBox);
                this.gridBoxRenderE = myGridBox;
                //create layers
                int nrows = this.gridTable.RowCount;
                int ncols = this.gridTable.ColumnCount;
                //----------------------------------------        

                myGridBox.BuildGrid(gridTable, this.CellSizeStyle);
                //add grid content
                for (int c = 0; c < ncols; ++c)
                {
                    for (int r = 0; r < nrows; ++r)
                    {
                        var gridCell = gridTable.GetCell(r, c);
                        var content = gridCell.ContentElement as UIElement;
                        if (content != null)
                        {
                            myGridBox.SetContent(r, c, content);
                        }
                    }
                }

                _dragController = new SimpleBox(10, 10);

            }
            return gridBoxRenderE;
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "gridbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}