namespace Mini
{
    partial class ColorCompoBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlView = new System.Windows.Forms.Panel();
            this.trackB = new System.Windows.Forms.TrackBar();
            this.trackG = new System.Windows.Forms.TrackBar();
            this.trackR = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackA = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackA)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlView
            // 
            this.pnlView.Location = new System.Drawing.Point(277, 37);
            this.pnlView.Name = "pnlView";
            this.pnlView.Size = new System.Drawing.Size(95, 207);
            this.pnlView.TabIndex = 13;
            // 
            // trackB
            // 
            this.trackB.Location = new System.Drawing.Point(28, 191);
            this.trackB.Name = "trackB";
            this.trackB.Size = new System.Drawing.Size(243, 45);
            this.trackB.TabIndex = 11;
            // 
            // trackG
            // 
            this.trackG.Location = new System.Drawing.Point(28, 145);
            this.trackG.Name = "trackG";
            this.trackG.Size = new System.Drawing.Size(243, 45);
            this.trackG.TabIndex = 10;
            // 
            // trackR
            // 
            this.trackR.Location = new System.Drawing.Point(28, 98);
            this.trackR.Name = "trackR";
            this.trackR.Size = new System.Drawing.Size(243, 45);
            this.trackR.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "R";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "G";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "B";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "A";
            // 
            // trackA
            // 
            this.trackA.Location = new System.Drawing.Point(28, 37);
            this.trackA.Name = "trackA";
            this.trackA.Size = new System.Drawing.Size(243, 45);
            this.trackA.TabIndex = 18;
            // 
            // ColorCompoBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackA);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlView);
            this.Controls.Add(this.trackB);
            this.Controls.Add(this.trackG);
            this.Controls.Add(this.trackR);
            this.Name = "ColorCompoBox";
            this.Size = new System.Drawing.Size(387, 277);
            this.Load += new System.EventHandler(this.ColorCompoBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlView;
        private System.Windows.Forms.TrackBar trackB;
        private System.Windows.Forms.TrackBar trackG;
        private System.Windows.Forms.TrackBar trackR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackA;
    }
}
