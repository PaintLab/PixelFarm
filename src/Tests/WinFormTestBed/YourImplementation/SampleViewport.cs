//Apache2, 2014-2018, WinterDev

using PaintLab;
using PixelFarm.Drawing;
using LayoutFarm.ContentManagers;
using LayoutFarm.UI;

namespace LayoutFarm
{
    public class SampleViewport : IViewport
    {
        ImageContentManager imageContentMan;
        LayoutFarm.UI.UISurfaceViewportControl vw;
        int primaryScreenWorkingAreaW;
        int primaryScreenWorkingAreaH;
        int _formTitleBarHeight;
        System.Windows.Forms.Form ownerForm;
        public SampleViewport(LayoutFarm.UI.UISurfaceViewportControl vw)
        {
            this.vw = vw;
            ownerForm = this.vw.FindForm();
            System.Drawing.Rectangle screenRectangle = ownerForm.RectangleToScreen(ownerForm.ClientRectangle);
            _formTitleBarHeight = screenRectangle.Top - ownerForm.Top;


            var primScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this.primaryScreenWorkingAreaW = primScreenWorkingArea.Width;
            this.primaryScreenWorkingAreaH = primScreenWorkingArea.Height;

            //--------------
            imageContentMan = new ImageContentManager();
            imageContentMan.ImageLoadingRequest += (s, e) =>
            {
                e.SetResultImage(LoadBitmap(e.ImagSource));
            };
        }
        public string OwnerFormTitle
        {
            get { return ownerForm.Text; }
            set
            {
                ownerForm.Text = value;
            }
        }
        public int OwnerFormTitleBarHeight { get { return _formTitleBarHeight; } }

        public static Image LoadBitmap(string filename)
        {
            System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(filename);
            DemoBitmap bmp = new DemoBitmap(gdiBmp.Width, gdiBmp.Height, gdiBmp);
            return bmp;
        }
        void LazyImageLoad(ImageBinder binder)
        {
            //load here as need
            imageContentMan.AddRequestImage(binder);
        }

        public int PrimaryScreenWidth
        {
            get { return this.primaryScreenWorkingAreaW; }
        }
        public int PrimaryScreenHeight
        {
            get { return this.primaryScreenWorkingAreaH; }
        }
        public void AddContent(RenderElement renderElement)
        {
            this.vw.AddContent(renderElement);
        }

        internal LayoutFarm.UI.UISurfaceViewportControl ViewportControl
        {
            get { return this.vw; }
        }
        public RootGraphic RootGfx
        {
            get { return this.vw.RootGfx; }
        }
        public ImageBinder GetImageBinder(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetLazyLoaderFunc(LazyImageLoad);
            //if use lazy img load func
            imageContentMan.AddRequestImage(clientImgBinder);
            return clientImgBinder;
        }
        public ImageBinder GetImageBinder2(string src)
        {
            ClientImageBinder clientImgBinder = new ClientImageBinder(src);
            clientImgBinder.SetImage(LoadBitmap(src));
            clientImgBinder.State = ImageBinderState.Loaded;
            return clientImgBinder;
        }

        public Image LoadImage(string imgName)
        {
            return LoadBitmap(imgName);
        }
        
        //----------------------------------------

        //UIRootElement _uiRootElement;

        //IUIRootElement IViewport.Root
        //{
        //    get
        //    {
        //        if (_uiRootElement == null)
        //        {
        //            _uiRootElement = new UIRootElement();
        //            _uiRootElement._viewport = this;
        //        }
        //        return _uiRootElement;
        //    }
        //}
        MyAppHost _myAppHost;
        IAppHost IViewport.AppHost
        {
            get
            {
                if (_myAppHost == null)
                {
                    _myAppHost = new MyAppHost();
                    _myAppHost.clientViewport = this;
                }
                return _myAppHost;
            }
        }
    }


}