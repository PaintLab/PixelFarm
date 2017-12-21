using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
 
using System.Text;
using System.Windows.Forms;

namespace Mini
{
    public partial class ColorCompoBox : UserControl
    {
        ColorsController controller;
        public ColorCompoBox()
        {
            InitializeComponent();
        }
        public void SetColor(Color c)
        {
            controller.SetColor(c);
        }
        public Color GetColor()
        {
            return controller.GetColor();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            controller.UpdateView();
            base.OnPaint(e);
        }
        private void ColorCompoBox_Load(object sender, EventArgs e)
        {
            controller = new ColorsController();
            controller.Setup2(this.label1, this.label2, this.label3, this.label4);
            controller.Setup1(this.trackR, this.trackG, this.trackB, this.trackA, this.pnlView);
        }
        class ColorsController
        {
            TrackBar trkR, trkG, trkB, trkA;
            Label lblR, lblG, lblB, lblA;
            //
            Panel view;
            Graphics viewG;
            byte R, G, B, A = 255;
            bool disableUpdateView;

            public void Setup1(TrackBar trkR, TrackBar trkG, TrackBar trkB, TrackBar trkA, Panel view)
            {
                this.view = view;
                viewG = view.CreateGraphics();

                SetupTrackBar(this.trkR = trkR, 0, 255, (s, e) => { this.R = (byte)trkR.Value; UpdateView(); });
                SetupTrackBar(this.trkG = trkG, 0, 255, (s, e) => { this.G = (byte)trkG.Value; UpdateView(); });
                SetupTrackBar(this.trkB = trkB, 0, 255, (s, e) => { this.B = (byte)trkB.Value; UpdateView(); });
                SetupTrackBar(this.trkA = trkA, 0, 255, (s, e) => { this.A = (byte)trkA.Value; UpdateView(); });
            }
            public void Setup2(Label lblR, Label lblG, Label lblB, Label lblA)
            {
                this.lblR = lblR;
                this.lblG = lblG;
                this.lblB = lblB;
                this.lblA = lblA;
            }
            public Color GetColor()
            {
                return Color.FromArgb(A, R, G, B);
            }
            public void SetColor(Color c)
            {
                disableUpdateView = true;
                trkR.Value = this.R = c.R;
                trkG.Value = this.G = c.G;
                trkB.Value = this.B = c.B;
                trkA.Value = this.A = c.A;
                disableUpdateView = false;
                UpdateView();
            }
            public void UpdateView()
            {
                if (disableUpdateView) return;
                viewG.Clear(Color.White);
                int boxW = 20, boxH = 20;
                int y = 0;
                int yposStep = 30;
                using (SolidBrush br = new SolidBrush(Color.FromArgb(255, R, 0, 0)))
                {
                    //A
                    br.Color = Color.FromArgb(255, A, A, A);
                    viewG.FillRectangle(br, new RectangleF(0, y, boxW, boxH));
                    //R
                    y += yposStep;
                    br.Color = Color.FromArgb(255, R, 0, 0);
                    viewG.FillRectangle(br, new RectangleF(0, y, boxW, boxH));

                    //G
                    y += yposStep;
                    br.Color = Color.FromArgb(255, 0, G, 0);
                    viewG.FillRectangle(br, new RectangleF(0, y, boxW, boxH));

                    //B
                    y += yposStep;
                    br.Color = Color.FromArgb(255, 0, 0, B);
                    viewG.FillRectangle(br, new RectangleF(0, y, boxW, boxH));


                    //merge all
                    y = 40;
                    br.Color = Color.FromArgb(A, R, G, B);
                    viewG.FillRectangle(br, new RectangleF(40, y, boxW * 2, boxH * 2));
                }

                lblR.Text = R.ToString();
                lblG.Text = G.ToString();
                lblB.Text = B.ToString();
                lblA.Text = A.ToString();
            }
            static void SetupTrackBar(TrackBar trk, int min, int max, EventHandler valueChanged)
            {

                trk.Minimum = min;
                trk.Maximum = max;
                trk.SmallChange = 1;
                trk.Scroll += valueChanged;
            }

        }

    }
}
