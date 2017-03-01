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
            this.canvas = new SkiaSharp.Views.Desktop.SKControl();
            this.glControl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.cmbBackEnd = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(22, 37);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(546, 385);
            this.canvas.TabIndex = 1;
            this.canvas.Text = "skControl1";
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(13, 37);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(511, 385);
            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            // 
            // cmbBackEnd
            // 
            this.cmbBackEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBackEnd.FormattingEnabled = true;
            this.cmbBackEnd.Location = new System.Drawing.Point(13, 1);
            this.cmbBackEnd.Name = "cmbBackEnd";
            this.cmbBackEnd.Size = new System.Drawing.Size(188, 21);
            this.cmbBackEnd.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 457);
            this.Controls.Add(this.cmbBackEnd);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.glControl);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private SkiaSharp.Views.Desktop.SKGLControl glControl;
        private SkiaSharp.Views.Desktop.SKControl canvas;
        private System.Windows.Forms.ComboBox cmbBackEnd;
    }
}

