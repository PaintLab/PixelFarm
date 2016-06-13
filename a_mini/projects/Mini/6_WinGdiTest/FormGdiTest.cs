using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.Agg.Image;
using System.Text;
using System.Windows.Forms;
using Mini.WinForms;
namespace Mini
{
    public partial class FormGdiTest : Form
    {
        Graphics _g;
        List<PixelToolController> pixelToolControllers;
        PixelToolController _currentTool;
        bool _isMouseDown;
        BufferedGraphics myBuffer;
        public FormGdiTest()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // This example assumes the existence of a form called Form1.
            // Gets a reference to the current BufferedGraphicsContext
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            // Creates a BufferedGraphics instance associated with Form1, and with 
            // dimensions the same size as the drawing surface of Form1.
            myBuffer = currentContext.Allocate(this.CreateGraphics(),
               this.DisplayRectangle);
            _g = myBuffer.Graphics;
            //
            //_g.SmoothingMode = SmoothingMode.AntiAlias;

            pixelToolControllers = new List<PixelToolController>();
            //create pixel tool controller name list 

            var tools = new PixelToolControllerFactory[]
             {
                new PixelToolControllerFactory<MyDrawingBrushController>("DrawingBrush"),
                new PixelToolControllerFactory<MyCuttingBrushController>("CuttingBrush"),
                new PixelToolControllerFactory<MyShapePickupTool>("ShapePickupTool") {CreateOnce= true },
                new PixelToolControllerFactory<MyLionSpriteTool>("Lion"){CreateOnce= true }
             };
            this.cmbPixelTools.Items.AddRange(tools);
            cmbPixelTools.SelectedIndex = 0;
            UpdateOutput(_g);
        }
        void UpdateOutput(Graphics g)
        {
            g.Clear(Color.White);
            //-------------------
            //draw to output 
            int j = pixelToolControllers.Count;
            for (int i = 0; i < j; ++i)
            {
                pixelToolControllers[i].Draw(g);
            }

            if (_currentTool != null && !_currentTool.IsDrawingTool)
            {
                _currentTool.Draw(g);
            }

            myBuffer.Render();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //clear
            _isMouseDown = true;
            var selectedFactory = cmbPixelTools.SelectedItem as PixelToolControllerFactory;
            if (selectedFactory == null)
            {
                return;
            }
            //--------------------------------------------------           

            //create new tools
            _currentTool = selectedFactory.CreateNewTool();
            _currentTool.SetPreviousPixelControllerObjects(pixelToolControllers);
            if (_currentTool.IsDrawingTool)
            {
                pixelToolControllers.Add(_currentTool);
            }

            //test
            //switch to high speed
            _g.SmoothingMode = SmoothingMode.HighSpeed;
            _currentTool.InvokeMouseDown(e.X, e.Y);
            UpdateOutput(_g);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                _currentTool.InvokeMouseUp(e.X, e.Y);
                _currentTool = null;
            }

            //test
            //switch back to anti alias
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            UpdateOutput(_g);
            base.OnMouseUp(e);
            _isMouseDown = false;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_currentTool != null)
            {
                _currentTool.InvokeMouseMove(e.X, e.Y);
            }

            if (_isMouseDown)
            {
                UpdateOutput(_g);
            }
            base.OnMouseMove(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateOutput(e.Graphics);
            base.OnPaint(e);
        }
    }
}
