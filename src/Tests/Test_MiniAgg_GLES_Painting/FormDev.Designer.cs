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
            this.lstExamples = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.chkGdiAntiAlias = new System.Windows.Forms.CheckBox();
            this.lstBackEndRenderer = new System.Windows.Forms.ListBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button1 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.cmdTestBitmapAtlas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstExamples
            // 
            this.lstExamples.FormattingEnabled = true;
            this.lstExamples.Location = new System.Drawing.Point(244, 141);
            this.lstExamples.Name = "lstExamples";
            this.lstExamples.Size = new System.Drawing.Size(234, 433);
            this.lstExamples.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(244, 628);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(150, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Test Filter";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
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
            // lstBackEndRenderer
            // 
            this.lstBackEndRenderer.FormattingEnabled = true;
            this.lstBackEndRenderer.Location = new System.Drawing.Point(244, 14);
            this.lstBackEndRenderer.Name = "lstBackEndRenderer";
            this.lstBackEndRenderer.Size = new System.Drawing.Size(234, 121);
            this.lstBackEndRenderer.TabIndex = 16;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 14);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(226, 572);
            this.treeView1.TabIndex = 19;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(400, 628);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 32);
            this.button1.TabIndex = 20;
            this.button1.Text = "TestPaintFx";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(88, 666);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(150, 32);
            this.button4.TabIndex = 22;
            this.button4.Text = "TestMsdfGen";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(244, 666);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(150, 32);
            this.button5.TabIndex = 24;
            this.button5.Text = "TestImgResampling";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // cmdTestBitmapAtlas
            // 
            this.cmdTestBitmapAtlas.Location = new System.Drawing.Point(88, 628);
            this.cmdTestBitmapAtlas.Name = "cmdTestBitmapAtlas";
            this.cmdTestBitmapAtlas.Size = new System.Drawing.Size(150, 32);
            this.cmdTestBitmapAtlas.TabIndex = 25;
            this.cmdTestBitmapAtlas.Text = "TestBitmapAtlas";
            this.cmdTestBitmapAtlas.UseVisualStyleBackColor = true;
            this.cmdTestBitmapAtlas.Click += new System.EventHandler(this.cmdTestBitmapAtlas_Click);
            // 
            // FormDev
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 748);
            this.Controls.Add(this.cmdTestBitmapAtlas);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.lstBackEndRenderer);
            this.Controls.Add(this.chkGdiAntiAlias);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lstExamples);
            this.Name = "FormDev";
            this.Text = "DevForm";
            this.Load += new System.EventHandler(this.FormDev_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.ListBox lstExamples;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkGdiAntiAlias;
        private System.Windows.Forms.ListBox lstBackEndRenderer;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button cmdTestBitmapAtlas;
    }
}