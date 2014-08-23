namespace Mini
{
    partial class FormRasterImage
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
            this.button1 = new System.Windows.Forms.Button();
            this.cmdBicubicInterpolation = new System.Windows.Forms.Button();
            this.cmdBilinearInterpolation = new System.Windows.Forms.Button();
            this.cmdRotate30Bilinear = new System.Windows.Forms.Button();
            this.cmdRotate30Bicubic = new System.Windows.Forms.Button();
            this.cmdLinearEq = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdBicubicInterpolation
            // 
            this.cmdBicubicInterpolation.Location = new System.Drawing.Point(231, 94);
            this.cmdBicubicInterpolation.Name = "cmdBicubicInterpolation";
            this.cmdBicubicInterpolation.Size = new System.Drawing.Size(98, 35);
            this.cmdBicubicInterpolation.TabIndex = 1;
            this.cmdBicubicInterpolation.Text = "Bicubic";
            this.cmdBicubicInterpolation.UseVisualStyleBackColor = true;
            this.cmdBicubicInterpolation.Click += new System.EventHandler(this.cmdBicubicInterpolation_Click);
            // 
            // cmdBilinearInterpolation
            // 
            this.cmdBilinearInterpolation.Location = new System.Drawing.Point(231, 53);
            this.cmdBilinearInterpolation.Name = "cmdBilinearInterpolation";
            this.cmdBilinearInterpolation.Size = new System.Drawing.Size(98, 35);
            this.cmdBilinearInterpolation.TabIndex = 2;
            this.cmdBilinearInterpolation.Text = "Bilinear";
            this.cmdBilinearInterpolation.UseVisualStyleBackColor = true;
            this.cmdBilinearInterpolation.Click += new System.EventHandler(this.cmdBilinearInterpolation_Click);
            // 
            // cmdRotate30Bilinear
            // 
            this.cmdRotate30Bilinear.Location = new System.Drawing.Point(24, 213);
            this.cmdRotate30Bilinear.Name = "cmdRotate30Bilinear";
            this.cmdRotate30Bilinear.Size = new System.Drawing.Size(176, 35);
            this.cmdRotate30Bilinear.TabIndex = 3;
            this.cmdRotate30Bilinear.Text = "Rotate_30_Bilinear";
            this.cmdRotate30Bilinear.UseVisualStyleBackColor = true;
            this.cmdRotate30Bilinear.Click += new System.EventHandler(this.cmdRotate30Bilinear_Click);
            // 
            // cmdRotate30Bicubic
            // 
            this.cmdRotate30Bicubic.Location = new System.Drawing.Point(24, 254);
            this.cmdRotate30Bicubic.Name = "cmdRotate30Bicubic";
            this.cmdRotate30Bicubic.Size = new System.Drawing.Size(176, 35);
            this.cmdRotate30Bicubic.TabIndex = 4;
            this.cmdRotate30Bicubic.Text = "Rotate_30_Bicubic";
            this.cmdRotate30Bicubic.UseVisualStyleBackColor = true;
            this.cmdRotate30Bicubic.Click += new System.EventHandler(this.cmdRotate30Bicubic_Click);
            // 
            // cmdLinearEq
            // 
            this.cmdLinearEq.Location = new System.Drawing.Point(24, 385);
            this.cmdLinearEq.Name = "cmdLinearEq";
            this.cmdLinearEq.Size = new System.Drawing.Size(176, 35);
            this.cmdLinearEq.TabIndex = 5;
            this.cmdLinearEq.Text = "LinearEq";
            this.cmdLinearEq.UseVisualStyleBackColor = true;
            this.cmdLinearEq.Click += new System.EventHandler(this.cmdLinearEq_Click);
            // 
            // FormRasterImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 468);
            this.Controls.Add(this.cmdLinearEq);
            this.Controls.Add(this.cmdRotate30Bicubic);
            this.Controls.Add(this.cmdRotate30Bilinear);
            this.Controls.Add(this.cmdBilinearInterpolation);
            this.Controls.Add(this.cmdBicubicInterpolation);
            this.Controls.Add(this.button1);
            this.Name = "FormRasterImage";
            this.Text = "FormRasterImage";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cmdBicubicInterpolation;
        private System.Windows.Forms.Button cmdBilinearInterpolation;
        private System.Windows.Forms.Button cmdRotate30Bilinear;
        private System.Windows.Forms.Button cmdRotate30Bicubic;
        private System.Windows.Forms.Button cmdLinearEq;

    }
}