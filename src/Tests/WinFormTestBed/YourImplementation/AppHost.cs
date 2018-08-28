//Apache2, 2014-present, WinterDev

using System.IO;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm
{


    public abstract class AppHost : IAppHost
    {
        protected ImageContentManager imageContentMan;
        protected int _primaryScreenWorkingAreaW;
        protected int _primaryScreenWorkingAreaH;
        protected int _formTitleBarHeight;

        public AppHost()
        {

            //--------------
            imageContentMan = new ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadImage(e.ImagSource));
            };
            //------- 
        }
        public abstract string OwnerFormTitle { get; set; }
        public int OwnerFormTitleBarHeight { get { return _formTitleBarHeight; } }
        public Image LoadImage(string imgName)
        {
            if (File.Exists(imgName)) //resolve to actual img 
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
            return null;
        }


        public abstract System.IO.Stream GetReadStream(string src);

        void LazyImageLoad(ImageBinder binder)
        {
            //load here as need
            imageContentMan.AddRequestImage(binder);
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
        public abstract RootGraphic RootGfx { get; }

        public ImageBinder GetImageBinder(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetLazyLoaderFunc(LazyImageLoad);
            //if use lazy img load func
            //imageContentMan.AddRequestImage(clientImgBinder);
            return clientImgBinder;
        }
        public ImageBinder GetImageBinder2(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetImage(LoadImage(src));
            clientImgBinder.State = BinderState.Loaded;
            return clientImgBinder;
        }
    }



    public class WinFormAppHost : AppHost
    {

        LayoutFarm.UI.UISurfaceViewportControl vw;
        System.Windows.Forms.Form ownerForm;
        public WinFormAppHost(LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            //---------------------------------------
            //this specific for WindowForm viewport
            //---------------------------------------
            this.vw = vw;
            ownerForm = this.vw.FindForm();
            System.Drawing.Rectangle screenRectangle = ownerForm.RectangleToScreen(ownerForm.ClientRectangle);
            _formTitleBarHeight = screenRectangle.Top - ownerForm.Top;


            var primScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this._primaryScreenWorkingAreaW = primScreenWorkingArea.Width;
            this._primaryScreenWorkingAreaH = primScreenWorkingArea.Height;

            //--------------
            imageContentMan = new ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadImage(e.ImagSource));
            };
            //------- 
        }
        public override string OwnerFormTitle
        {
            get { return ownerForm.Text; }
            set
            {
                ownerForm.Text = value;
            }
        }
        internal LayoutFarm.UI.UISurfaceViewportControl ViewportControl
        {
            get { return this.vw; }
        }

        public override RootGraphic RootGfx
        {
            get { return this.vw.RootGfx; }
        }
        void LazyImageLoad(ImageBinder binder)
        {
            //load here as need
            imageContentMan.AddRequestImage(binder);
        }
        public override Stream GetReadStream(string src)
        {
            return null;
        }
        public override void AddChild(RenderElement renderElement)
        {
            this.vw.AddChild(renderElement);
        }
    }


}