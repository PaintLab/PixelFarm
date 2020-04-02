namespace Mini
{
    partial class FormImgResampling
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
            this.button3 = new System.Windows.Forms.Button();
            this.cmdFreeTransform = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(32, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 32);
            this.button3.TabIndex = 23;
            this.button3.Text = "ImgScaling";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cmdFreeTransform
            // 
            this.cmdFreeTransform.Location = new System.Drawing.Point(188, 12);
            this.cmdFreeTransform.Name = "cmdFreeTransform";
            this.cmdFreeTransform.Size = new System.Drawing.Size(150, 32);
            this.cmdFreeTransform.TabIndex = 22;
            this.cmdFreeTransform.Text = "ImgTransform";
            this.cmdFreeTransform.UseVisualStyleBackColor = true;
            this.cmdFreeTransform.Click += new System.EventHandler(this.cmdFreeTransform_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(32, 51);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(657, 492);
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // FormImgResampling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 683);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.cmdFreeTransform);
            this.Name = "FormImgResampling";
            this.Text = "FormImgResampling";
            this.Load += new System.EventHandler(this.FormImgResampling_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button cmdFreeTransform;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}