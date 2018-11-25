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

            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GdiPlus);
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GLES);
            //this.lstPlatformSelectors.Items.Add(InnerViewportKind.Skia);
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.AggOnGLES);
            this.lstPlatformSelectors.Items.Add(InnerViewportKind.GdiPlusOnGLES);
            this.lstPlatformSelectors.SelectedIndex = 0;//set default


        }
        public TreeView SamplesTreeView
        {
            get { return this._samplesTreeView; }
        }
        void Form1_Load(object sender, EventArgs e)
        {
            this.lstDemoList.DoubleClick += (s1, e1) => RunSelectedDemo();
            this.lstPlatformSelectors.DoubleClick += (s1, e1) => RunSelectedDemo();

        }
        void RunSelectedDemo()
        {
            //load demo sample
            DemoInfo selectedDemoInfo = this.lstDemoList.SelectedItem as DemoInfo;
            if (selectedDemoInfo == null) return;

            App selectedDemo = (App)Activator.CreateInstance(selectedDemoInfo.DemoType);
            RunDemo(selectedDemo);

        }


        LayoutFarm.UI.UISurfaceViewportControl _latestviewport;
        Form _latest_formCanvas;
        public void RunDemo(App app)
        {
            //1. create blank form
            YourImplementation.DemoFormCreatorHelper.CreateReadyForm(
                (InnerViewportKind)lstPlatformSelectors.SelectedItem,
                out _latestviewport, out _latest_formCanvas);

            _latest_formCanvas.FormClosed += (s, e) =>
            {
                //when owner form is disposed
                //we should clear our resource?
                _latest_formCanvas = null;
                _latestviewport = null;
            };
            //2. create app host


            app.Start(new AppHostWinForm(_latestviewport));
            _latestviewport.TopDownRecalculateContent();
            _latestviewport.PaintMe();


            //==================================================  
            if (this.chkShowLayoutInspector.Checked)
            {
                YourImplementation.LayoutInspectorUtils.ShowFormLayoutInspector(_latestviewport);
            }

            if (this.chkShowFormPrint.Checked)
            {
                ShowFormPrint(_latestviewport);
            }
        }

        static void ShowFormPrint(LayoutFarm.UI.UISurfaceViewportControl viewport)
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

        public void LoadDemoList(Type sampleAssemblySpecificType)
        {
            Type demoBaseType = typeof(App);


            var thisAssem = System.Reflection.Assembly.GetAssembly(sampleAssemblySpecificType);
            List<DemoInfo> demoInfoList = new List<DemoInfo>();

            foreach (var t in thisAssem.GetTypes())
            {
                if (demoBaseType.IsAssignableFrom(t) && t != demoBaseType && !t.IsAbstract)
                {
                    string demoTypeName = t.Name;
                    object[] notes = t.GetCustomAttributes(typeof(DemoNoteAttribute), false);
                    string noteMsg = null;
                    if (notes != null && notes.Length > 0)
                    {
                        //get one only
                        DemoNoteAttribute note = notes[0] as DemoNoteAttribute;
                        if (note != null)
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



            int w = _latestviewport.Width;
            int h = _latestviewport.Height;

            //create target gdi+ bmp 
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(w, h))
            {
                System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, w, h),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                //
                this._latestviewport.PaintToPixelBuffer(bmpData.Scan0);
                //
                bmp.UnlockBits(bmpData);
                bmp.Save("d:\\WImageTest\\001.png");
            }

        }
    }
}
