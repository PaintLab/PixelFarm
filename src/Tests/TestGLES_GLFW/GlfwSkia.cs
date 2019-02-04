////MIT, 2016-present, WinterDev
//using System;
////using SkiaSharp;

//using Pencil.Gaming;
//using PixelFarm;
//using PixelFarm.Forms;
//using OpenTK.Graphics.ES20;
//using OpenTkEssTest;

//using Typography.FontManagement;
//using PixelFarm.CpuBlit;
//using System.IO;

//namespace TestGlfw
//{
//    //class GlfwSkia : GlfwAppBase
//    //{
//    //    static PixelFarm.DrawingGL.GLPainterContext _pcx;
//    //    static PixelFarm.CpuBlit.MemBitmap myImg;
//    //    public GlfwSkia()
//    //    {
//    //        int ww_w = 800;
//    //        int ww_h = 600;
//    //        int max = Math.Max(ww_w, ww_h);
//    //        _pcx = PixelFarm.DrawingGL.GLPainterContext.Create(max, max, ww_w, ww_h, true);
//    //    }
//    //    public override void UpdateViewContent(FormRenderUpdateEventArgs formRenderUpdateEventArgs)
//    //    {
//    //        //1. create platform bitmap 
//    //        // create the surface
//    //        int w = 800;
//    //        int h = 600;

//    //        if (myImg == null)
//    //        {

//    //            myImg = new PixelFarm.CpuBlit.MemBitmap(w, h);
//    //            //test1
//    //            // create the surface
//    //            var info = new SKImageInfo(w, h, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
//    //            using (var surface = SKSurface.Create(info, PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(myImg).Ptr, myImg.Stride))
//    //            {
//    //                // start drawing
//    //                SKCanvas canvas = surface.Canvas;
//    //                DrawWithSkia(canvas);
//    //                surface.Canvas.Flush();
//    //            }
//    //        }

//    //        var glBmp = new PixelFarm.DrawingGL.GLBitmap(myImg);
//    //        _pcx.DrawImage(glBmp, 0, 600);
//    //        glBmp.Dispose();
//    //    }
//    //    static void DrawWithSkia(SKCanvas canvas)
//    //    {
//    //        canvas.Clear(new SKColor(255, 255, 255, 255));
//    //        using (SKPaint p = new SKPaint())
//    //        {
//    //            p.TextSize = 36.0f;
//    //            p.Color = (SKColor)0xFF4281A4;
//    //            p.StrokeWidth = 2;
//    //            p.IsAntialias = true;
//    //            canvas.DrawLine(0, 0, 100, 100, p);
//    //            p.Color = SKColors.Red;
//    //            canvas.DrawText("Hello!", 20, 100, p);
//    //        }
//    //    }

//    //    static PixelFarm.CpuBlit.MemBitmap LoadImage(string filename)
//    //    {
//    //        ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
//    //        using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
//    //        {
//    //            //TODO: review img loading, we should not use only its extension
//    //            //
//    //            string fileExt = System.IO.Path.GetExtension(filename).ToLower();
//    //            switch (fileExt)
//    //            {
//    //                case ".png":
//    //                    {
//    //                        var decoder = new ImageTools.IO.Png.PngDecoder();
//    //                        extendedImg.Load(fs, decoder);
//    //                    }
//    //                    break;
//    //                case ".jpg":
//    //                    {
//    //                        var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
//    //                        extendedImg.Load(fs, decoder);
//    //                    }
//    //                    break;
//    //                default:
//    //                    throw new System.NotSupportedException();

//    //            }
//    //            //var decoder = new ImageTools.IO.Png.PngDecoder();

//    //        }
//    //        //assume 32 bit 

//    //        PixelFarm.CpuBlit.MemBitmap memBmp = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(
//    //            extendedImg.PixelWidth,
//    //            extendedImg.PixelHeight,
//    //            extendedImg.Pixels32
//    //            );
//    //        //the imgtools load data as BigEndian
//    //        memBmp.IsBigEndian = true;
//    //        return memBmp;
//    //    }
//    //}

//}