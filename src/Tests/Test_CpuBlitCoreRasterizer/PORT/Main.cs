//MIT, 2020, WinterDev
using System;
using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.Rasterization;
using PixelFarm.CpuBlit.PixelProcessing;


public static class Program
{
    public static void Main()
    {
        //TEST
        //this is low-level scanline rasterizer
        //1. create vertex store
        VertexStore vxs = new VertexStore();
        vxs.AddMoveTo(10, 10);
        vxs.AddLineTo(50, 10);
        vxs.AddLineTo(50, 50);
        vxs.AddLineTo(10, 50);
        vxs.AddCloseFigure();

        //2. create scanline rasterizer
        ScanlineRasterizer sclineRas = new ScanlineRasterizer();
        sclineRas.AddPath(vxs);

        //3. create destination bitmap
        DestBitmapRasterizer destBmpRasterizer = new DestBitmapRasterizer();

        //4. create 32bit rgba bitmap blender

        MyBitmapBlender myBitmapBlender = new MyBitmapBlender();

        //5. create output bitmap
        using (MemBitmap membitmap = new MemBitmap(800, 600))
        {
            //6. attach target bitmap to bitmap blender
            myBitmapBlender.Attach(membitmap);

            //7. rasterizer sends the vector content inside sclineRas
            //   to the bitmap blender and  

            destBmpRasterizer.RenderWithColor(myBitmapBlender, //blender+ output 
                sclineRas, //with vectors input inside
                new ScanlinePacked8(),
                Color.Red);

            //8. the content inside membitmap is just color image buffer
            //   you can copy it to other image object (eg SkImage, Gdi+ image etc)

        }

    }

    class MyBitmapBlender : BitmapBlenderBase
    {

        //custom implementation of BitmapBlender

        public override void WriteBuffer(int[] newbuffer)
        {

        }
    }

}