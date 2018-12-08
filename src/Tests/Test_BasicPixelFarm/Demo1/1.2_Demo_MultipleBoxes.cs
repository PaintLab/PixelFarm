//Apache2, 2014-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.2 MultpleBox")]
    class Demo_MultipleBox : App
    {
        LayoutFarm.CustomWidgets.CheckBox _currentSingleCheckedBox;
        protected override void OnStart(AppHost host)
        {
            SetupImageList(host);
            for (int i = 1; i < 5; ++i)
            {
                var textbox = new LayoutFarm.CustomWidgets.Box(30, 30);
                textbox.SetLocation(i * 40, i * 40);
                host.AddChild(textbox);
            }
            //--------------------
            //image box
            //load bitmap with gdi+           
            ImageBinder imgBinder = host.LoadImageAndBind("../Data/imgs/favorites32.png");

            var imgBox = new CustomWidgets.ImageBox(imgBinder.Width, imgBinder.Height);
            imgBox.ImageBinder = imgBinder;
            host.AddChild(imgBox);
            //--------------------
            //checked box
            int boxHeight = 20;
            int boxY = 50;
            //multiple select
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.CustomWidgets.CheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                host.AddChild(statedBox);
            }
            //-------------------------------------------------------------------------
            //single select 
            boxY += 50;
            for (int i = 0; i < 4; ++i)
            {
                var statedBox = new LayoutFarm.CustomWidgets.CheckBox(20, boxHeight);
                statedBox.SetLocation(10, boxY);
                boxY += boxHeight + 5;
                host.AddChild(statedBox);
                statedBox.WhenChecked += (s, e) =>
                {
                    var selectedBox = (LayoutFarm.CustomWidgets.CheckBox)s;
                    if (selectedBox != _currentSingleCheckedBox)
                    {
                        if (_currentSingleCheckedBox != null)
                        {
                            _currentSingleCheckedBox.Checked = false;
                        }
                        _currentSingleCheckedBox = selectedBox;
                    }
                };
            }
            //-------------------------------------------------------------------
            //test canvas
            var canvasBox = new MyDrawingCanvas(300, 300);
            canvasBox.SetLocation(400, 150);
            host.AddChild(canvasBox);
            //-------------------------------------------------------------------

        }


        class MyDrawingCanvas : LayoutFarm.CustomWidgets.MiniAggCanvasBox
        {
            int _lastX;
            int _lastY;
            List<Point> _pointList = new List<Point>();
            public MyDrawingCanvas(int w, int h)
                : base(w, h)
            {
            }
            protected override void OnMouseDown(UIMouseEventArgs e)
            {
                ////test only!!!         
                _lastX = e.X;
                _lastY = e.Y;
                _pointList.Add(new Point(_lastX, _lastY));
            }
            protected override void OnMouseMove(UIMouseEventArgs e)
            {
                //test
                //draw on this canvas
                if (!e.IsDragging)
                {
                    return;
                }
                _lastX = e.X;
                _lastY = e.Y;
                //temp fix here -> need converter
                var p = this.Painter;
                p.Clear(PixelFarm.Drawing.Color.White);
                _pointList.Add(new Point(_lastX, _lastY));
                //clear and render again
                int j = _pointList.Count;
                for (int i = 1; i < j; ++i)
                {
                    var p0 = _pointList[i - 1];
                    var p1 = _pointList[i];
                    p.DrawLine(
                        p0.X, p0.Y,
                        p1.X, p1.Y);
                }

                this.InvalidateCanvasContent();
            }
            protected override void OnMouseUp(UIMouseEventArgs e)
            {
            }
        }

        static void SetupImageList(AppHost host)
        {
            if (!LayoutFarm.CustomWidgets.ResImageList.HasImages)
            {
                //set imagelists
                var imgdic = new Dictionary<CustomWidgets.ImageName, Image>();
                imgdic[CustomWidgets.ImageName.CheckBoxUnChecked] = host.LoadImage("../Data/imgs/arrow_close.png");
                imgdic[CustomWidgets.ImageName.CheckBoxChecked] = host.LoadImage("../Data/imgs/arrow_open.png");
                LayoutFarm.CustomWidgets.ResImageList.SetImageList(imgdic);
            }
        }
        //static Bitmap LoadBitmap(string filename)
        //{
        //    System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
        //    Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
        //    return bmp;
        //}
        //static ImageBinder LoadImage(string filename)
        //{
        //    System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
        //    Bitmap bmp = new Bitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
        //    ImageBinder binder = new ClientImageBinder(null);
        //    binder.SetImage(bmp);
        //    binder.State = ImageBinderState.Loaded;
        //    return binder;
        //}
    }
}