//Apache2, 2014-present, WinterDev
using System;
using System.IO;
using PaintLab.Svg;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

namespace LayoutFarm
{
    public class AppHostWithRootGfx : AppHost
    {
        RootGraphic _rootgfx;
        public AppHostWithRootGfx() { }
        public void Setup(AppHostConfig appHostConfig)
        {   
            
            _rootgfx = appHostConfig.RootGfx;
            _primaryScreenWorkingAreaW = appHostConfig.ScreenW;
            _primaryScreenWorkingAreaH = appHostConfig.ScreenH;
        }

        public override RootGraphic RootGfx => _rootgfx;
        //
        public override void AddChild(RenderElement renderElement)
        {
            _rootgfx.AddChild(renderElement);
        }
        public override void AddChild(RenderElement renderElement, object owner)
        {
            _rootgfx.AddChild(renderElement);
        }
        public override Image LoadImage(byte[] rawImgFile, string imgTypeHint)
        {
            try
            {
                using (System.IO.MemoryStream ms = new MemoryStream(rawImgFile))
                {
                    ms.Position = 0;
                    //TODO: review here again
                    using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(ms))
                    {
                        PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(gdiBmp.Width, gdiBmp.Height);
                        //#if DEBUG
                        //                        memBmp._dbugNote = "img;
                        //#endif
                        
                        PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(gdiBmp, memBmp);
                        return memBmp;
                    }
                }
            }
            catch (System.Exception ex)
            {
                //return error img
                return null;
            }
        }
        public override Image LoadImage(string imgName, int reqW, int reqH)
        {
            if (!File.Exists(imgName)) //resolve to actual img 
            {
                return null;
            }

            //we support svg as src of img
            //...
            //THIS version => just check an extension of the request file
            string ext = System.IO.Path.GetExtension(imgName).ToLower();
            switch (ext)
            {
                default: return null;
                case ".svg":
                    try
                    {
                        string svg_str = File.ReadAllText(imgName);
                        VgVisualElement vgVisElem = VgVisualDocHelper.CreateVgVisualDocFromFile(imgName).VgRootElem;
                        return CreateBitmap(vgVisElem, reqW, reqH);

                    }
                    catch (System.Exception ex)
                    {
                        return null;
                    }
                case ".png":
                case ".jpg":
                    {
                        try
                        {

                            using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName))
                            {
                                MemBitmap memBmp = new MemBitmap(gdiBmp.Width, gdiBmp.Height);
#if DEBUG
                                memBmp._dbugNote = "img" + imgName;
#endif
                               BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(gdiBmp, memBmp);
                                return memBmp;
                            }

                        }
                        catch (System.Exception ex)
                        {
                            //return error img
                            return null;
                        }
                    }
            }
        }


        PixelFarm.CpuBlit.MemBitmap CreateBitmap(VgVisualElement renderVx, int reqW, int reqH)
        {

            RectD bound = renderVx.GetRectBounds();
            //create
            MemBitmap backingBmp = new MemBitmap((int)bound.Width + 10, (int)bound.Height + 10);
#if DEBUG
            backingBmp._dbugNote = "renderVx";
#endif
            //PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backingBmp);

            using (Tools.BorrowAggPainter(backingBmp, out var painter))
            using (Tools.More.BorrowVgPaintArgs(painter, out var paintArgs))
            {
                double prevStrokeW = painter.StrokeWidth;

                renderVx.Paint(paintArgs);
                painter.StrokeWidth = prevStrokeW;//restore 
            }

            return backingBmp;
        }
    }
}