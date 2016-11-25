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
            this.cmdBuildMergePixelFarmMiniAggOne = new System.Windows.Forms.Button();
            this.cmd_Windows_OnlyGdiPlus = new System.Windows.Forms.Button();
            this.cmd_Windows_NoGdiPlus_NoWinForms = new System.Windows.Forms.Button();
            this.cmd_Cross = new System.Windows.Forms.Button();
            this.cmdCopyNativeLibs = new System.Windows.Forms.Button();
            this.cmdForTestWithHtmlRenderer = new System.Windows.Forms.Button();
            this.cmdBuildMergePixelFarmMiniAgg = new System.Windows.Forms.Button();
            this.cmdMinimalNetCore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdBuildMergePixelFarm
            // 
            this.cmdBuildMergePixelFarm.Location = new System.Drawing.Point(25, 251);
            this.cmdBuildMergePixelFarm.Name = "cmdBuildMergePixelFarm";
            this.cmdBuildMergePixelFarm.Size = new System.Drawing.Size(360, 57);
            this.cmdBuildMergePixelFarm.TabIndex = 0;
            this.cmdBuildMergePixelFarm.Text = "BuildMerge PixelFarm.One (All)";
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
            // cmdBuildMergePixelFarmMiniAggOne
            // 
            this.cmdBuildMergePixelFarmMiniAggOne.Location = new System.Drawing.Point(25, 156);
            this.cmdBuildMergePixelFarmMiniAggOne.Name = "cmdBuildMergePixelFarmMiniAggOne";
            this.cmdBuildMergePixelFarmMiniAggOne.Size = new System.Drawing.Size(167, 57);
            this.cmdBuildMergePixelFarmMiniAggOne.TabIndex = 2;
            this.cmdBuildMergePixelFarmMiniAggOne.Text = "BuildMerge PixelFarm.MiniAgg.One";
            this.cmdBuildMergePixelFarmMiniAggOne.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarmMiniAggOne.Click += new System.EventHandler(this.cmdMergePixelFarmMiniAggOne_Click);
            // 
            // cmd_Windows_OnlyGdiPlus
            // 
            this.cmd_Windows_OnlyGdiPlus.Location = new System.Drawing.Point(25, 314);
            this.cmd_Windows_OnlyGdiPlus.Name = "cmd_Windows_OnlyGdiPlus";
            this.cmd_Windows_OnlyGdiPlus.Size = new System.Drawing.Size(360, 57);
            this.cmd_Windows_OnlyGdiPlus.TabIndex = 3;
            this.cmd_Windows_OnlyGdiPlus.Text = "BuildMerge PixelFarm.One (Windows, Only GdiPlus)";
            this.cmd_Windows_OnlyGdiPlus.UseVisualStyleBackColor = true;
            this.cmd_Windows_OnlyGdiPlus.Click += new System.EventHandler(this.cmd_Windows_OnlyGdiPlus_Click);
            // 
            // cmd_Windows_NoGdiPlus_NoWinForms
            // 
            this.cmd_Windows_NoGdiPlus_NoWinForms.Location = new System.Drawing.Point(25, 377);
            this.cmd_Windows_NoGdiPlus_NoWinForms.Name = "cmd_Windows_NoGdiPlus_NoWinForms";
            this.cmd_Windows_NoGdiPlus_NoWinForms.Size = new System.Drawing.Size(360, 57);
            this.cmd_Windows_NoGdiPlus_NoWinForms.TabIndex = 4;
            this.cmd_Windows_NoGdiPlus_NoWinForms.Text = "BuildMerge PixelFarm.One (Windows,NoGdiPlus, NoWinForms)";
            this.cmd_Windows_NoGdiPlus_NoWinForms.UseVisualStyleBackColor = true;
            this.cmd_Windows_NoGdiPlus_NoWinForms.Click += new System.EventHandler(this.cmd_Windows_NoGdiPlus_NoWinForms_Click);
            // 
            // cmd_Cross
            // 
            this.cmd_Cross.Location = new System.Drawing.Point(25, 440);
            this.cmd_Cross.Name = "cmd_Cross";
            this.cmd_Cross.Size = new System.Drawing.Size(360, 57);
            this.cmd_Cross.TabIndex = 5;
            this.cmd_Cross.Text = "BuildMerge PixelFarm.One (Cross Platform, NoWinForms)";
            this.cmd_Cross.UseVisualStyleBackColor = true;
            this.cmd_Cross.Click += new System.EventHandler(this.cmd_Cross_Click);
            // 
            // cmdCopyNativeLibs
            // 
            this.cmdCopyNativeLibs.Location = new System.Drawing.Point(500, 251);
            this.cmdCopyNativeLibs.Name = "cmdCopyNativeLibs";
            this.cmdCopyNativeLibs.Size = new System.Drawing.Size(184, 57);
            this.cmdCopyNativeLibs.TabIndex = 6;
            this.cmdCopyNativeLibs.Text = "Copy NativeLibs";
            this.cmdCopyNativeLibs.UseVisualStyleBackColor = true;
            this.cmdCopyNativeLibs.Click += new System.EventHandler(this.cmdCopyNativeLibs_Click);
            // 
            // cmdForTestWithHtmlRenderer
            // 
            this.cmdForTestWithHtmlRenderer.Location = new System.Drawing.Point(25, 503);
            this.cmdForTestWithHtmlRenderer.Name = "cmdForTestWithHtmlRenderer";
            this.cmdForTestWithHtmlRenderer.Size = new System.Drawing.Size(360, 57);
            this.cmdForTestWithHtmlRenderer.TabIndex = 7;
            this.cmdForTestWithHtmlRenderer.Text = "TEST BuildMerge PixelFarm.One.HtmlRenderer";
            this.cmdForTestWithHtmlRenderer.UseVisualStyleBackColor = true;
            this.cmdForTestWithHtmlRenderer.Click += new System.EventHandler(this.cmdForTestWithHtmlRenderer_Click);
            // 
            // cmdBuildMergePixelFarmMiniAgg
            // 
            this.cmdBuildMergePixelFarmMiniAgg.Location = new System.Drawing.Point(500, 440);
            this.cmdBuildMergePixelFarmMiniAgg.Name = "cmdBuildMergePixelFarmMiniAgg";
            this.cmdBuildMergePixelFarmMiniAgg.Size = new System.Drawing.Size(167, 57);
            this.cmdBuildMergePixelFarmMiniAgg.TabIndex = 8;
            this.cmdBuildMergePixelFarmMiniAgg.Text = "BuildMerge PixelFarm.MiniAgg";
            this.cmdBuildMergePixelFarmMiniAgg.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarmMiniAgg.Click += new System.EventHandler(this.cmdBuildMergePixelFarmMiniAgg_Click);
            // 
            // cmdMinimalNetCore
            // 
            this.cmdMinimalNetCore.Location = new System.Drawing.Point(218, 156);
            this.cmdMinimalNetCore.Name = "cmdMinimalNetCore";
            this.cmdMinimalNetCore.Size = new System.Drawing.Size(188, 57);
            this.cmdMinimalNetCore.TabIndex = 9;
            this.cmdMinimalNetCore.Text = "BuildMerge for MinimumNetCore";
            this.cmdMinimalNetCore.UseVisualStyleBackColor = true;
            this.cmdMinimalNetCore.Click += new System.EventHandler(this.cmdMinimalNetCore_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 565);
            this.Controls.Add(this.cmdMinimalNetCore);
            this.Controls.Add(this.cmdBuildMergePixelFarmMiniAgg);
            this.Controls.Add(this.cmdForTestWithHtmlRenderer);
            this.Controls.Add(this.cmdCopyNativeLibs);
            this.Controls.Add(this.cmd_Cross);
            this.Controls.Add(this.cmd_Windows_NoGdiPlus_NoWinForms);
            this.Controls.Add(this.cmd_Windows_OnlyGdiPlus);
            this.Controls.Add(this.cmdBuildMergePixelFarmMiniAggOne);
            this.Controls.Add(this.cmdBuildMergePixelFarmPortable);
            this.Controls.Add(this.cmdBuildMergePixelFarm);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildMergePixelFarm;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmPortable;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmMiniAggOne;
        private System.Windows.Forms.Button cmd_Windows_OnlyGdiPlus;
        private System.Windows.Forms.Button cmd_Windows_NoGdiPlus_NoWinForms;
        private System.Windows.Forms.Button cmd_Cross;
        private System.Windows.Forms.Button cmdCopyNativeLibs;
        private System.Windows.Forms.Button cmdForTestWithHtmlRenderer;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmMiniAgg;
        private System.Windows.Forms.Button cmdMinimalNetCore;
    }
}

