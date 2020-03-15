namespace Mini
{
    partial class FormTestBitmapAtlas
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmdBuildBmpAtlas = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lbl_src = new System.Windows.Forms.Label();
            this.cmdReadBmpAtlas = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 29);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(157, 199);
            this.listBox1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pictureBox1.Location = new System.Drawing.Point(172, 29);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(111, 92);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // cmdBuildBmpAtlas
            // 
            this.cmdBuildBmpAtlas.Location = new System.Drawing.Point(172, 127);
            this.cmdBuildBmpAtlas.Name = "cmdBuildBmpAtlas";
            this.cmdBuildBmpAtlas.Size = new System.Drawing.Size(111, 44);
            this.cmdBuildBmpAtlas.TabIndex = 3;
            this.cmdBuildBmpAtlas.Text = "Build BmpAtlas";
            this.cmdBuildBmpAtlas.UseVisualStyleBackColor = true;
            this.cmdBuildBmpAtlas.Click += new System.EventHandler(this.cmdBuildBmpAtlas_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Silver;
            this.pictureBox2.Location = new System.Drawing.Point(289, 29);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(553, 324);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // lbl_src
            // 
            this.lbl_src.AutoSize = true;
            this.lbl_src.Location = new System.Drawing.Point(8, 13);
            this.lbl_src.Name = "lbl_src";
            this.lbl_src.Size = new System.Drawing.Size(24, 13);
            this.lbl_src.TabIndex = 5;
            this.lbl_src.Text = "src:";
            // 
            // cmdReadBmpAtlas
            // 
            this.cmdReadBmpAtlas.Location = new System.Drawing.Point(848, 29);
            this.cmdReadBmpAtlas.Name = "cmdReadBmpAtlas";
            this.cmdReadBmpAtlas.Size = new System.Drawing.Size(117, 44);
            this.cmdReadBmpAtlas.TabIndex = 6;
            this.cmdReadBmpAtlas.Text = "Read BmpAtlas";
            this.cmdReadBmpAtlas.UseVisualStyleBackColor = true;
            this.cmdReadBmpAtlas.Click += new System.EventHandler(this.cmdReadBmpAtlas_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(848, 79);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(206, 277);
            this.listBox2.TabIndex = 7;
            // 
            // FormTestBitmapAtlas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 721);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.cmdReadBmpAtlas);
            this.Controls.Add(this.lbl_src);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.cmdBuildBmpAtlas);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Name = "FormTestBitmapAtlas";
            this.Text = "FormTestBitmapAtlas";
            this.Load += new System.EventHandler(this.FormTestBitmapAtlas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button cmdBuildBmpAtlas;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label lbl_src;
        private System.Windows.Forms.Button cmdReadBmpAtlas;
        private System.Windows.Forms.ListBox listBox2;
    }
}