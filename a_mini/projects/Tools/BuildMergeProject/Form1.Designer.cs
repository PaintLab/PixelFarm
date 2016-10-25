namespace BuildMergeProject
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
            this.cmdBuildMergePixelFarm = new System.Windows.Forms.Button();
            this.cmdBuildMergePixelFarmPortable = new System.Windows.Forms.Button();
            this.cmdBuildMergePixelFarmMiniAgg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdBuildMergePixelFarm
            // 
            this.cmdBuildMergePixelFarm.Location = new System.Drawing.Point(25, 251);
            this.cmdBuildMergePixelFarm.Name = "cmdBuildMergePixelFarm";
            this.cmdBuildMergePixelFarm.Size = new System.Drawing.Size(167, 57);
            this.cmdBuildMergePixelFarm.TabIndex = 0;
            this.cmdBuildMergePixelFarm.Text = "BuildMerge PixelFarm.One";
            this.cmdBuildMergePixelFarm.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarm.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdBuildMergePixelFarmPortable
            // 
            this.cmdBuildMergePixelFarmPortable.Location = new System.Drawing.Point(327, 22);
            this.cmdBuildMergePixelFarmPortable.Name = "cmdBuildMergePixelFarmPortable";
            this.cmdBuildMergePixelFarmPortable.Size = new System.Drawing.Size(226, 57);
            this.cmdBuildMergePixelFarmPortable.TabIndex = 1;
            this.cmdBuildMergePixelFarmPortable.Text = "BuildMerge PixelFarm Portable";
            this.cmdBuildMergePixelFarmPortable.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarmPortable.Click += new System.EventHandler(this.cmdBuildMergePixelFarmPortable_Click);
            // 
            // cmdBuildMergePixelFarmMiniAgg
            // 
            this.cmdBuildMergePixelFarmMiniAgg.Location = new System.Drawing.Point(25, 156);
            this.cmdBuildMergePixelFarmMiniAgg.Name = "cmdBuildMergePixelFarmMiniAgg";
            this.cmdBuildMergePixelFarmMiniAgg.Size = new System.Drawing.Size(167, 57);
            this.cmdBuildMergePixelFarmMiniAgg.TabIndex = 2;
            this.cmdBuildMergePixelFarmMiniAgg.Text = "BuildMerge PixelFarm.MiniAgg";
            this.cmdBuildMergePixelFarmMiniAgg.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarmMiniAgg.Click += new System.EventHandler(this.cmdMergePixelFarmMiniAgg_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 378);
            this.Controls.Add(this.cmdBuildMergePixelFarmMiniAgg);
            this.Controls.Add(this.cmdBuildMergePixelFarmPortable);
            this.Controls.Add(this.cmdBuildMergePixelFarm);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildMergePixelFarm;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmPortable;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmMiniAgg;
    }
}

