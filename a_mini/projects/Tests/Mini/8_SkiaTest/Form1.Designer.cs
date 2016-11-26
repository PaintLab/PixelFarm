namespace TestSkia1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.canvas = new SkiaSharp.Views.Desktop.SKControl();
            this.SuspendLayout();
            // 
            // skglControl1
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(13, 13);
            this.glControl.Name = "skglControl1";
            this.glControl.Size = new System.Drawing.Size(511, 385);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            // 
            // skControl1
            // 
            this.canvas.Location = new System.Drawing.Point(22, 13);
            this.canvas.Name = "skControl1";
            this.canvas.Size = new System.Drawing.Size(546, 385);
            this.canvas.TabIndex = 1;
            this.canvas.Text = "skControl1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 457);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.glControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SkiaSharp.Views.Desktop.SKGLControl glControl;
        private SkiaSharp.Views.Desktop.SKControl canvas;
    }
}

