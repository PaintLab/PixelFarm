//Apache2, 2014-present, WinterDev
using System;
using System.IO;
using PixelFarm.Drawing;

using PaintLab.Svg;
using LayoutFarm.UI;
using PixelFarm.DrawingGL;
using YourImplementation;

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
        protected abstract LayoutFarm.UI.UISurfaceViewportControl GetHostSurfaceViewportControl();

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

        public ImageBinder LoadImageAndBind(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetImage(LoadImage(src));
            return clientImgBinder;
        }

        public ImageBinder CreateImageBinder(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetLazyLoaderFunc(binder =>
            {
                Image img = this.LoadImage(binder.ImageSource);
                binder.SetImage(img);
            });
            return clientImgBinder;
        }

    }



    public class WinFormAppHost : AppHost
    {
        //if ENABLE OPENGL
        //-----------------------------------
        OpenTK.MyGLControl _glControl;
        CpuBlitGLESUIElement _bridgeUI;
        bool _useBridgeUI;

        //-----------------------------------


        LayoutFarm.UI.UISurfaceViewportControl _vw;
        System.Windows.Forms.Form _ownerForm;
        public WinFormAppHost(LayoutFarm.UI.UISurfaceViewportControl vw)
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
            _bridgeUI.SetUpdateCpuBlitSurfaceDelegate(p =>
            {
                _client.DrawToThisCanvas(_bridgeUI.GetDrawBoard(), new Rectangle(0, 0, 1200, 1200));
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

////MIT, 2014-present, WinterDev
////MIT, 2018-present, WinterDev

//using System;
//using PixelFarm.DrawingGL;
//using LayoutFarm;
//using LayoutFarm.UI;
//using YourImplementation;
//namespace Mini
//{
//    class CpuBlitOnGLESAppModule
//    {
//        //hardware renderer part=> GLES
//        //software renderer part => Pure Agg

//        int _myWidth;
//        int _myHeight;
//        UISurfaceViewportControl _surfaceViewport;
//        RootGraphic _rootGfx;

//        //
//        CpuBlitGLESUIElement _bridgeUI;
//        DemoBase _demoBase;

//        OpenTK.MyGLControl _glControl;
//        public CpuBlitOnGLESAppModule() { }
//        public void BindSurface(LayoutFarm.UI.UISurfaceViewportControl surfaceViewport)
//        {
//            _myWidth = 800;
//            _myHeight = 600;

//            _surfaceViewport = surfaceViewport;
//            _rootGfx = surfaceViewport.RootGfx;
//            //----------------------
//            this._glControl = surfaceViewport.GetOpenTKControl();
//            _glControl.SetGLPaintHandler(null);

//            IntPtr hh1 = _glControl.Handle; //ensure that contrl handler is created
//            _glControl.MakeCurrent();
//        }

//        public bool WithGdiPlusDrawBoard { get; set; }

//        public void LoadExample(DemoBase demoBase)
//        {
//            _glControl.MakeCurrent();

//            this._demoBase = demoBase;
//            demoBase.Init();

//            if (WithGdiPlusDrawBoard)
//            {
//                _bridgeUI = new GdiOnGLESUIElement(_myWidth, _myHeight);
//            }
//            else
//            {
//                //pure agg's cpu blit 
//                _bridgeUI = new CpuBlitGLESUIElement(_myWidth, _myHeight);
//            }
//            _bridgeUI.SetUpdateCpuBlitSurfaceDelegate(p => _demoBase.Draw(p));

//            DemoBase.InvokePainterReady(_demoBase, _bridgeUI.GetAggPainter());
//            //
//            //use existing GLRenderSurface and GLPainter
//            //see=>UISurfaceViewportControl.InitRootGraphics()

//            GLRenderSurface glsx = _surfaceViewport.GetGLRenderSurface();
//            GLPainter glPainter = _surfaceViewport.GetGLPainter();
//            _bridgeUI.CreatePrimaryRenderElement(glsx, glPainter, _rootGfx);
//            //-----------------------------------------------
//            demoBase.SetEssentialGLHandlers(
//                () => this._glControl.SwapBuffers(),
//                () => this._glControl.GetEglDisplay(),
//                () => this._glControl.GetEglSurface()
//            );
//            //-----------------------------------------------
//            DemoBase.InvokeGLContextReady(demoBase, glsx, glPainter);
//            //Add to RenderTree
//            _rootGfx.TopWindowRenderBox.AddChild(_bridgeUI.GetPrimaryRenderElement(_rootGfx));
//            //-----------------------------------------------
//            //***
//            GeneralEventListener genEvListener = new GeneralEventListener();
//            genEvListener.MouseDown += e =>
//            {
//                _bridgeUI.ContentMayChanged = true;
//                _demoBase.MouseDown(e.X, e.Y, e.Button == UIMouseButtons.Right);
//                _bridgeUI.InvalidateGraphics();
//            };
//            genEvListener.MouseMove += e =>
//            {
//                if (e.IsDragging)
//                {
//                    _bridgeUI.InvalidateGraphics();
//                    _bridgeUI.ContentMayChanged = true;
//                    _demoBase.MouseDrag(e.X, e.Y);
//                    _bridgeUI.InvalidateGraphics();
//                }
//            };
//            genEvListener.MouseUp += e =>
//            {
//                _bridgeUI.ContentMayChanged = true;
//                _demoBase.MouseUp(e.X, e.Y);
//            };
//            //-----------------------------------------------
//            _bridgeUI.AttachExternalEventListener(genEvListener);
//        }
//        public void CloseDemo()
//        {
//            _demoBase.CloseDemo();
//        }
//    }


//}