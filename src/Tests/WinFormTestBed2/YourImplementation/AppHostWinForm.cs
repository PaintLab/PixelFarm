//Apache2, 2014-present, WinterDev
using System;
using System.IO;
using PixelFarm.Drawing;

using PaintLab.Svg;
using LayoutFarm.UI;
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
        bool _useBridgeUI;
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


            //essential***
            _bridgeUI.SetUpdateCpuBlitSurfaceDelegate((p, area) =>
            {
                _client.DrawToThisCanvas(_bridgeUI.GetDrawBoard(), area);
            });
            //DemoBase.InvokePainterReady(_demoBase, _bridgeUI.GetAggPainter()); 
            GLRenderSurface glsx = _vw.GetGLRenderSurface();
            GLPainter glPainter = _vw.GetGLPainter();

            RootGraphic rootGfx = _vw.RootGfx;
            _bridgeUI.CreatePrimaryRenderElement(glsx, glPainter, rootGfx);

            _useBridgeUI = true;
            //demoBase.SetEssentialGLHandlers(
            //    () => this._glControl.SwapBuffers(),
            //    () => this._glControl.GetEglDisplay(),
            //    () => this._glControl.GetEglSurface()
            //);
            //------------------------------------------------
            //***
            //this will do as a proxy for transfer event from e1 to e2 

            //GeneralEventListener evListener = new GeneralEventListener();
            //evListener.MouseDown += e =>
            //{
            //    _bridgeUI.ContentMayChanged = true;
            //    _bridgeUI.InvalidateGraphics();
            //};
            //evListener.MouseMove += e =>
            //{
            //    if (e.IsDragging)
            //    {
            //        _bridgeUI.InvalidateGraphics();
            //        _bridgeUI.ContentMayChanged = true;
            //    }
            //};
            //evListener.MouseUp += e =>
            //{

            //    _bridgeUI.ContentMayChanged = true;
            //    _bridgeUI.InvalidateGraphics();
            //};
            //_bridgeUI.AttachExternalEventListener(evListener);

            rootGfx.TopWindowRenderBox.AddChild(_bridgeUI.GetPrimaryRenderElement(rootGfx));
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


        //TODO: review primary render element here again!
        //so special root elem switch?
        RenderElement _client;
        public override void AddChild(RenderElement renderElement)
        {
            if (_useBridgeUI)
            {
                _client = renderElement;
                _bridgeUI.CurrentPrimaryRenderElement.AddChild(renderElement);

            }
            else
            {
                this._vw.AddChild(renderElement);
            }
        }
        public override void AddChild(RenderElement renderElement, object owner)
        {
            if (_useBridgeUI)
            {
                _client = renderElement;
                _bridgeUI.CurrentPrimaryRenderElement.AddChild(renderElement);
            }
            else
            {
                this._vw.AddChild(renderElement, owner);
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
                        VgRenderVx vgRenderVx = ReadSvgFile(imgName);
                        return CreateBitmap(vgRenderVx, reqW, reqH);

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
        //
        PixelFarm.CpuBlit.ActualBitmap CreateBitmap(VgRenderVx renderVx, int reqW, int reqH)
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
            using (VgPainterArgsPool.Borrow(painter, out VgPaintArgs paintArgs))
            {
                renderVx._renderE.Paint(paintArgs);
            }
            painter.StrokeWidth = prevStrokeW;//restore 
            return backimg;
        }
    }
}