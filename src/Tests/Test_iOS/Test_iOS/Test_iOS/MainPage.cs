//MIT, 2018, WinterDev
using System;
using Xamarin.Forms;
//
using SkiaSharp;
using SkiaSharp.Views.Forms;
//
using PixelFarm.CpuBlit;

namespace Test_iOS
{
    public partial class MainPage : ContentPage
    {
        SKCanvasView canvasView;

        SKBitmap _myCanvasBmp;

        //PixelFarm
        ActualBitmap _actualBmp;//
        AggPainter _painter;
        bool _needContentUpdate;

        public MainPage()
        {
           
            canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            Content = canvasView;

            //1. create bitmap backend for our canvas
            _myCanvasBmp = new SKBitmap(800, 600);
            //2. create agg render surface and painter

            unsafe
            {
                int* ptr = (int*)_myCanvasBmp.GetAddr(0, 0);
                int w = _myCanvasBmp.Width;
                int h = _myCanvasBmp.Height;

                _actualBmp = new ActualBitmap(w, h, (IntPtr)ptr);
                _painter = new AggPainter(new AggRenderSurface(_actualBmp));

                _needContentUpdate = true;
            }
        }
        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            //get image buffer from info 
            if (_needContentUpdate)
            {
                _painter.Clear(PixelFarm.Drawing.Color.Yellow);
                _painter.StrokeColor = PixelFarm.Drawing.Color.Red;
                _painter.StrokeWidth = 2;
                _painter.DrawLine(0, 0, 100, 100);
                _needContentUpdate = false;
            }
            //draw entire bmp or draw some part of it
            canvas.DrawBitmap(_myCanvasBmp, 0, 0);
        }
    }
}
