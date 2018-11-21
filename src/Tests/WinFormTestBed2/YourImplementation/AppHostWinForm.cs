//Apache2, 2014-present, WinterDev
using System;
using System.IO;

using PaintLab.Svg;
using LayoutFarm.UI;
using PixelFarm.Drawing;
//
using PixelFarm.DrawingGL;
using YourImplementation;

namespace LayoutFarm
{
    public class AppHostWinForm : AppHost
    {
        //if ENABLE OPENGL
        //-----------------------------------
        OpenTK.MyGLControl _glControl;
        CpuBlitGLESUIElement _bridgeUI;

        //-----------------------------------


        LayoutFarm.UI.UISurfaceViewportControl _vw;
        System.Windows.Forms.Form _ownerForm;
        public AppHostWinForm(LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            _vw = vw;

            _ownerForm = this._vw.FindForm();
            System.Drawing.Rectangle screenRectangle = _ownerForm.RectangleToScreen(_ownerForm.ClientRectangle);
            _formTitleBarHeight = screenRectangle.Top - _ownerForm.Top;


            System.Drawing.Rectangle primScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            _primaryScreenWorkingAreaW = primScreenWorkingArea.Width;
            _primaryScreenWorkingAreaH = primScreenWorkingArea.Height;


            // 
            switch (vw.InnerViewportKind)
            {
                case InnerViewportKind.GdiPlusOnGLES:
                case InnerViewportKind.AggOnGLES:
                    SetUpGLSurface(vw.GetOpenTKControl());
                    break;
            }
        }
        void SetUpGLSurface(OpenTK.MyGLControl glControl)
        {
            if (glControl == null) return;
            //TODO: review here
            //Temp: 
            _glControl = glControl;
            _glControl.SetGLPaintHandler(null);
            //
            IntPtr hh1 = _glControl.Handle; //ensure that contrl handler is created
            _glControl.MakeCurrent();

            if (_vw.InnerViewportKind == InnerViewportKind.GdiPlusOnGLES)
            {
                _bridgeUI = new GdiOnGLESUIElement(glControl.Width, glControl.Height);
            }
            else
            {
                //pure agg's cpu blit 
                _bridgeUI = new CpuBlitGLESUIElement(glControl.Width, glControl.Height);
            }


            //optional***
            //_bridgeUI.SetUpdateCpuBlitSurfaceDelegate((p, area) =>
            //{
            //    _client.DrawToThisCanvas(_bridgeUI.GetDrawBoard(), area);
            //});


            GLRenderSurface glsx = _vw.GetGLRenderSurface();
            GLPainter glPainter = _vw.GetGLPainter();

            RootGraphic rootGfx = _vw.RootGfx;
            _bridgeUI.CreatePrimaryRenderElement(glsx, glPainter, rootGfx);



            //*****
            RenderBoxBase renderE = (RenderBoxBase)_bridgeUI.GetPrimaryRenderElement(rootGfx);
            rootGfx.AddChild(renderE);
            rootGfx.SetPrimaryContainerElement(renderE);
            //***
        }

        //
        protected override UISurfaceViewportControl GetHostSurfaceViewportControl()
        {
            return _vw;
        }

        public override string OwnerFormTitle
        {
            get { return _ownerForm.Text; }
            set
            {
                _ownerForm.Text = value;
            }
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
                        VgVisualElement vgVisElem = ReadSvgFile(imgName).VgRootElem;
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

                            //System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName);
                            //GdiPlusBitmap bmp = new GdiPlusBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
                            //return bmp;


                            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(imgName);
                            PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(gdiBmp.Width, gdiBmp.Height);
                            PixelFarm.CpuBlit.Imaging.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(
                                gdiBmp, memBmp);
                            return memBmp;
                        }
                        catch (System.Exception ex)
                        {
                            //return error img
                            return null;
                        }
                    }

            }

        }

        VgDocRoot ReadSvgFile(string filename)
        {

            string svgContent = System.IO.File.ReadAllText(filename);
            SvgDocBuilder docBuidler = new SvgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);//***
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);
            //TODO: review this step again
            VgDocBuilder builder = new VgDocBuilder();
            return builder.CreateVgVisualElem(docBuidler.ResultDocument, svgElem =>
            {
                //**
                //TODO: review here

            });
        }
        //
        PixelFarm.CpuBlit.MemBitmap CreateBitmap(VgVisualElement renderVx, int reqW, int reqH)
        {

            PixelFarm.CpuBlit.RectD bound = renderVx.GetRectBounds();
            //create
            PixelFarm.CpuBlit.MemBitmap backingBmp = new PixelFarm.CpuBlit.MemBitmap((int)bound.Width + 10, (int)bound.Height + 10);
            PixelFarm.CpuBlit.AggPainter painter = PixelFarm.CpuBlit.AggPainter.Create(backingBmp);
            ////TODO: review here
            ////temp fix
            //if (s_openfontTextService == null)
            //{
            //    s_openfontTextService = new OpenFontTextService();
            //}


            ////
            double prevStrokeW = painter.StrokeWidth;
            using (VgPainterArgsPool.Borrow(painter, out VgPaintArgs paintArgs))
            {
                renderVx.Paint(paintArgs);
            }
            painter.StrokeWidth = prevStrokeW;//restore 
            return backingBmp;
        }
    }
}