//Apache2, 2014-present, WinterDev
using System;
using System.IO;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PaintLab.Svg;

namespace LayoutFarm
{
    public class AppHost
    {

        protected int _primaryScreenWorkingAreaW;
        protected int _primaryScreenWorkingAreaH;
        public AppHost()
        {
        }
        //override this to get exact executable path
        public virtual string ExecutablePath => System.IO.Directory.GetCurrentDirectory();

        public void StartApp(App app)
        {
            if (PreviewApp(app))
            {
                app.StartApp(this);
            }
        }
        protected virtual bool PreviewApp(App app)
        {
            return true;
        }



        public virtual Image LoadImage(byte[] rawImgFile, string imgTypeHint)
        {
            try
            {
                using (System.IO.MemoryStream ms = new MemoryStream(rawImgFile))
                {
                    ms.Position = 0;
                    return PixelFarm.Platforms.ImageIOPortal.ReadImageDataFromMemStream(ms, imgTypeHint);
                }
            }
            catch (System.Exception ex)
            {
                //return error img
                return null;
            }
        }
        public virtual Image LoadImage(string imgName, int reqW, int reqH)
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
                            byte[] rawImgBuff = File.ReadAllBytes(imgName);
                            using (MemoryStream ms = new MemoryStream(rawImgBuff))
                            {
                                ms.Position = 0;
                                return PixelFarm.Platforms.ImageIOPortal.ReadImageDataFromMemStream(ms, ext);
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

            var bound = renderVx.GetRectBounds();
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
        public Image LoadImage(string imgName)
        {
            return LoadImage(imgName, 0, 0);
        }
        public virtual System.IO.Stream GetReadStream(string src)
        {
            return App.ReadStreamS(src);
        }
        public virtual System.IO.Stream GetWriteStream(string dest)
        {
            return App.GetWriteStream(dest);
        }
        public virtual bool UploadStream(string url, Stream stream)
        {
            return App.UploadStream(url, stream);
        }
        //
        public int PrimaryScreenWidth => _primaryScreenWorkingAreaW;
        public int PrimaryScreenHeight => _primaryScreenWorkingAreaH;
        //
        public void AddChild(RenderElement renderElement)
        {
            _rootgfx.AddChild(renderElement);
        }
        public void AddChild(RenderElement renderElement, object owner)
        {
            _rootgfx.AddChild(renderElement);
        }

        public RootGraphic RootGfx => _rootgfx;

        public virtual ImageBinder LoadImageAndBind(string src)
        {
            ImageBinder clientImgBinder = new ImageBinder(src);
            clientImgBinder.SetLocalImage(LoadImage(src));
            return clientImgBinder;
        }

        public ImageBinder CreateImageBinder(string src)
        {
            ImageBinder clientImgBinder = new ImageBinder(src);
            clientImgBinder.SetImageLoader(binder =>
            {
                Image img = this.LoadImage(binder.ImageSource);
                binder.SetLocalImage(img);
            });
            return clientImgBinder;
        }
        public virtual void CustomContentRequest(object customContentReq) { }

        RootGraphic _rootgfx;
        public void Setup(AppHostConfig appHostConfig)
        {

            _rootgfx = appHostConfig.RootGfx;
            _primaryScreenWorkingAreaW = appHostConfig.ScreenW;
            _primaryScreenWorkingAreaH = appHostConfig.ScreenH;
        }
    }

    public class AppHostConfig
    {
        public RootGraphic RootGfx;
        public int ScreenW;
        public int ScreenH;

    }
}
