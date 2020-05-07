//Apache2, 2014-present, WinterDev
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutFarm.UI;
namespace LayoutFarm.Dev
{
    public partial class FormDemoList : Form
    {


        public FormDemoList()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            //------
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GLES);
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GdiPlus);           
            //this.lstPlatformSelectors.Items.Add(InnerViewportKind.Skia);
            //this.lstPlatformSelectors.Items.Add(InnerViewportKind.AggOnGLES);
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GdiPlusOnGLES);
            this.lstPlatformSelectors.SelectedIndex = 0;//set default
        }
        //
        public TreeView SamplesTreeView => _samplesTreeView;
        //
        void Form1_Load(object sender, EventArgs e)
        {
            this.lstDemoList.DoubleClick += (s1, e1) => RunSelectedDemo();
            this.lstPlatformSelectors.DoubleClick += (s1, e1) => RunSelectedDemo();

        }
        void RunSelectedDemo()
        {
            //load demo sample
            if (this.lstDemoList.SelectedItem is DemoInfo selectedDemoInfo)
            {
                App selectedDemo = (App)Activator.CreateInstance(selectedDemoInfo.DemoType);
                RunDemo(selectedDemo);
            }
        }


        GraphicsViewRoot _viewroot;
        Form _latest_formCanvas;
        public void RunDemo(App app)
        {
            //1. create blank form
            YourImplementation.DemoFormCreatorHelper.CreateReadyForm(
                (InnerViewportKind)lstPlatformSelectors.SelectedItem,
                out _viewroot,
                out _latest_formCanvas);

            AppHost appHost = new AppHost();
            AppHostConfig config = new AppHostConfig();
            YourImplementation.UISurfaceViewportSetupHelper.SetUISurfaceViewportControl(config, _viewroot);
            appHost.Setup(config);


            _latest_formCanvas.FormClosed += (s, e) =>
            {
                //when owner form is disposed
                //we should clear our resource?
                app.OnClosing();
                app.OnClosed();
                _latest_formCanvas = null;
                _viewroot = null;
            };


            //2. create app host 
            appHost.StartApp(app);
            //_viewroot.TopDownRecalculateContent();
            _viewroot.PaintToOutputWindow();


            //==================================================  
            if (this.chkShowLayoutInspector.Checked)
            {
                YourImplementation.LayoutInspectorUtils.ShowFormLayoutInspector(_viewroot);
            }

            if (this.chkShowFormPrint.Checked)
            {
                ShowFormPrint(_viewroot);
            }
        }

        static void ShowFormPrint(GraphicsViewRoot viewport)
        {

            var formPrint = new LayoutFarm.Dev.FormPrint();
            formPrint.Show();

            formPrint.FormClosed += (s, e2) =>
            {
                formPrint = null;
            };
            formPrint.Connect(viewport);
        }

        public void ClearDemoList()
        {
            this.lstDemoList.Items.Clear();
        }

        public void LoadDemoList(System.Reflection.Assembly asm)
        {
            Type demoBaseType = typeof(App);            
            List<DemoInfo> demoInfoList = new List<DemoInfo>();
            foreach (Type t in asm.GetTypes())
            {
                if (demoBaseType.IsAssignableFrom(t) && t != demoBaseType && !t.IsAbstract)
                {
                    string demoTypeName = t.Name;
                    object[] notes = t.GetCustomAttributes(typeof(DemoNoteAttribute), false);
                    string noteMsg = null;
                    if (notes != null && notes.Length > 0)
                    {
                        //get one only
                        if (notes[0] is DemoNoteAttribute note)
                        {
                            noteMsg = note.Message;
                        }
                    }
                    demoInfoList.Add(new DemoInfo(t, noteMsg));
                }
            }
            demoInfoList.Sort((d1, d2) =>
            {
                if (d1.DemoNote != null && d2.DemoNote != null)
                {
                    return d1.DemoNote.CompareTo(d2.DemoNote);
                }
                else
                {
                    return d1.DemoType.Name.CompareTo(d2.DemoType.Name);
                }
            });
            foreach (var demo in demoInfoList)
            {
                this.lstDemoList.Items.Add(demo);
            }

        }

        private void chkShowLayoutInspector_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkShowFormPrint_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {



            int w = _viewroot.Width;
            int h = _viewroot.Height;

            //create target gdi+ bmp 
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h))
            {
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                //
                _viewroot.PaintToPixelBuffer(bmpData.Scan0);
                //
                bmp.UnlockBits(bmpData);
                bmp.Save("001.png");
            }

        }
    }
}
