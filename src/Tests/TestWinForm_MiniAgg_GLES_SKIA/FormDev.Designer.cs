namespace Mini
{
    partial class FormDev
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


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button5 = new System.Windows.Forms.Button();
            this.chkGdiAntiAlias = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.cmdSignedDistance = new System.Windows.Forms.Button();
            this.lstBackEndRenderer = new System.Windows.Forms.ListBox();
            this.cmdFreeTransform = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(234, 433);
            this.listBox1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(417, 413);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Test Filter";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(252, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(429, 317);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(573, 455);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 32);
            this.button5.TabIndex = 6;
            this.button5.Text = "TestGdiPlus";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // chkGdiAntiAlias
            // 
            this.chkGdiAntiAlias.AutoSize = true;
            this.chkGdiAntiAlias.Location = new System.Drawing.Point(596, 14);
            this.chkGdiAntiAlias.Name = "chkGdiAntiAlias";
            this.chkGdiAntiAlias.Size = new System.Drawing.Size(85, 17);
            this.chkGdiAntiAlias.TabIndex = 8;
            this.chkGdiAntiAlias.Text = "Gdi AntiAlias";
            this.chkGdiAntiAlias.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(573, 413);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(150, 32);
            this.button6.TabIndex = 9;
            this.button6.Text = "TestGLES2";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // cmdSignedDistance
            // 
            this.cmdSignedDistance.Location = new System.Drawing.Point(417, 489);
            this.cmdSignedDistance.Name = "cmdSignedDistance";
            this.cmdSignedDistance.Size = new System.Drawing.Size(150, 32);
            this.cmdSignedDistance.TabIndex = 14;
            this.cmdSignedDistance.Text = "Test SignedDistance";
            this.cmdSignedDistance.UseVisualStyleBackColor = true;
            this.cmdSignedDistance.Click += new System.EventHandler(this.cmdSignedDistance_Click);
            // 
            // lstBackEndRenderer
            // 
            this.lstBackEndRenderer.FormattingEnabled = true;
            this.lstBackEndRenderer.Location = new System.Drawing.Point(12, 455);
            this.lstBackEndRenderer.Name = "lstBackEndRenderer";
            this.lstBackEndRenderer.Size = new System.Drawing.Size(234, 121);
            this.lstBackEndRenderer.TabIndex = 16;
            // 
            // cmdFreeTransform
            // 
            this.cmdFreeTransform.Location = new System.Drawing.Point(417, 451);
            this.cmdFreeTransform.Name = "cmdFreeTransform";
            this.cmdFreeTransform.Size = new System.Drawing.Size(150, 32);
            this.cmdFreeTransform.TabIndex = 18;
            this.cmdFreeTransform.Text = "ImgTransform";
            this.cmdFreeTransform.UseVisualStyleBackColor = true;
            this.cmdFreeTransform.Click += new System.EventHandler(this.cmdFreeTransform_Click);
            // 
            // FormDev
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 588);
            this.Controls.Add(this.cmdFreeTransform);
            this.Controls.Add(this.lstBackEndRenderer);
            this.Controls.Add(this.cmdSignedDistance);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.chkGdiAntiAlias);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Name = "FormDev";
            this.Text = "DevForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox chkGdiAntiAlias;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button cmdSignedDistance;
        private System.Windows.Forms.ListBox lstBackEndRenderer;
        private System.Windows.Forms.Button cmdFreeTransform;
    }
}