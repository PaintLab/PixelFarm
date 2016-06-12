using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mini
{
    public partial class FormGdiTest : Form
    {
        bool isMouseDown;
        List<Point> points = new List<Point>();
        Graphics g;
        public FormGdiTest()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            g = this.CreateGraphics();

        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            points.Clear();
            points.Add(new Point(e.X, e.Y));
            isMouseDown = true;
            DrawLines(g);
            base.OnMouseDown(e);
        }
        void DrawLines(Graphics g)
        {
            if (points.Count > 1)
            {
                g.DrawLines(Pens.Black, points.ToArray());
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            points.Add(new Point(e.X, e.Y));
            DrawLines(g);
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                points.Add(new Point(e.X, e.Y));
            }
            base.OnMouseMove(e);
            DrawLines(g);

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (points != null && points.Count > 1)
            {
                DrawLines(e.Graphics);
            }
            base.OnPaint(e);
        }
    }
}
