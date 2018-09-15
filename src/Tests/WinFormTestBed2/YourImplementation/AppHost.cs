//Apache2, 2014-present, WinterDev

using System.IO;
using PixelFarm.Drawing;
using LayoutFarm.Svg;
using PaintLab.Svg;

namespace LayoutFarm
{
    public abstract class AppHost
    {

        protected int _primaryScreenWorkingAreaW;
        protected int _primaryScreenWorkingAreaH;
        protected int _formTitleBarHeight;

        public AppHost()
        {


        }
        public abstract string OwnerFormTitle { get; set; }
        public abstract Image LoadImage(string imgName, int reqW, int reqH);
        public Image LoadImage(string imgName)
        {
            return LoadImage(imgName, 0, 0);
        }
        public int OwnerFormTitleBarHeight { get { return _formTitleBarHeight; } }


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


        public int PrimaryScreenWidth
        {
            get { return this._primaryScreenWorkingAreaW; }
        }
        public int PrimaryScreenHeight
        {
            get { return this._primaryScreenWorkingAreaH; }
        }
        public abstract void AddChild(RenderElement renderElement);
        public abstract void AddChild(RenderElement renderElement, object owner);

        public abstract RootGraphic RootGfx { get; }
    }



    public class WinFormAppHost : AppHost
    {

        LayoutFarm.UI.UISurfaceViewportControl _vw;
        System.Windows.Forms.Form _ownerForm;
        public WinFormAppHost(LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            this._vw = vw;
            _ownerForm = this._vw.FindForm();
            System.Drawing.Rectangle screenRectangle = _ownerForm.RectangleToScreen(_ownerForm.ClientRectangle);
            _formTitleBarHeight = screenRectangle.Top - _ownerForm.Top;


            System.Drawing.Rectangle primScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this._primaryScreenWorkingAreaW = primScreenWorkingArea.Width;
            this._primaryScreenWorkingAreaH = primScreenWorkingArea.Height;


        }

        public override string OwnerFormTitle
        {
            get { return _ownerForm.Text; }
            set
            {
                _ownerForm.Text = value;
            }
        }
        internal LayoutFarm.UI.UISurfaceViewportControl ViewportControl
        {
            get { return this._vw; }
        }

        public override RootGraphic RootGfx
        {
            get { return this._vw.RootGfx; }
        }

        public override void AddChild(RenderElement renderElement)
        {
            this._vw.AddChild(renderElement);
        }
        public override void AddChild(RenderElement renderElement, object owner)
        {
            this._vw.AddChild(renderElement, owner);
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
                        VgRenderVx vgRenderVx = ReadSvgFile(imgName);
                        return CreateBitmap(vgRenderVx);

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

                            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName);
                            GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
                            return bmp;
                        }
                        catch (System.Exception ex)
                        {
                            //return error img
                            return null;
                        }
                    }

            }

        }

        VgRenderVx ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            SvgDocBuilder docBuidler = new SvgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);//***
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //TODO: review this step again
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();
            return builder.CreateRenderVx(docBuidler.ResultDocument, svgElem =>
            {
                //**
                //TODO: review here

            });
        }
        PixelFarm.CpuBlit.ActualBitmap CreateBitmap(VgRenderVx renderVx)
        {

            PixelFarm.CpuBlit.RectD bound = renderVx.GetBounds();
            //create
            PixelFarm.CpuBlit.ActualBitmap backimg = new PixelFarm.CpuBlit.ActualBitmap((int)bound.Width + 10, (int)bound.Height + 10);
            PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backimg);
            ////TODO: review here
            ////temp fix
            //if (s_openfontTextService == null)
            //{
            //    s_openfontTextService = new OpenFontTextService();
            //}


            ////
            double prevStrokeW = painter.StrokeWidth;
            VgPainterArgsPool.GetFreePainterArgs(painter, out VgPaintArgs paintArgs);
            renderVx._renderE.Paint(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
            painter.StrokeWidth = prevStrokeW;//restore


            return backimg;
        }



    }


}