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
            this.cmd_Windows_OnlyGdiPlus = new System.Windows.Forms.Button();
            this.cmd_Windows_NoGdiPlus_NoWinForms = new System.Windows.Forms.Button();
            this.cmd_Cross = new System.Windows.Forms.Button();
            this.cmdCopyNativeLibs = new System.Windows.Forms.Button();
            this.cmdForTestWithHtmlRenderer = new System.Windows.Forms.Button();
            this.cmdMinimalNetCore = new System.Windows.Forms.Button();
            this.cmdBuild_PixelFarm_Drawing = new System.Windows.Forms.Button();
            this.cmdReadSln = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.lstPreset = new System.Windows.Forms.ListBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.lstAsmReferenceList = new System.Windows.Forms.ListBox();
            this.cmdBuildSelectedMergePro = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdBuildMergePixelFarm
            // 
            this.cmdBuildMergePixelFarm.Location = new System.Drawing.Point(25, 90);
            this.cmdBuildMergePixelFarm.Name = "cmdBuildMergePixelFarm";
            this.cmdBuildMergePixelFarm.Size = new System.Drawing.Size(167, 57);
            this.cmdBuildMergePixelFarm.TabIndex = 0;
            this.cmdBuildMergePixelFarm.Text = "BuildMerge PixelFarm.One (All)";
            this.cmdBuildMergePixelFarm.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarm.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdBuildMergePixelFarmPortable
            // 
            this.cmdBuildMergePixelFarmPortable.Location = new System.Drawing.Point(198, 27);
            this.cmdBuildMergePixelFarmPortable.Name = "cmdBuildMergePixelFarmPortable";
            this.cmdBuildMergePixelFarmPortable.Size = new System.Drawing.Size(219, 57);
            this.cmdBuildMergePixelFarmPortable.TabIndex = 1;
            this.cmdBuildMergePixelFarmPortable.Text = "BuildMerge PixelFarm Portable";
            this.cmdBuildMergePixelFarmPortable.UseVisualStyleBackColor = true;
            this.cmdBuildMergePixelFarmPortable.Click += new System.EventHandler(this.cmdBuildMergePixelFarmPortable_Click);
            // 
            // cmd_Windows_OnlyGdiPlus
            // 
            this.cmd_Windows_OnlyGdiPlus.Location = new System.Drawing.Point(25, 182);
            this.cmd_Windows_OnlyGdiPlus.Name = "cmd_Windows_OnlyGdiPlus";
            this.cmd_Windows_OnlyGdiPlus.Size = new System.Drawing.Size(315, 57);
            this.cmd_Windows_OnlyGdiPlus.TabIndex = 3;
            this.cmd_Windows_OnlyGdiPlus.Text = "BuildMerge PixelFarm.One (Windows, Only GdiPlus)";
            this.cmd_Windows_OnlyGdiPlus.UseVisualStyleBackColor = true;
            this.cmd_Windows_OnlyGdiPlus.Click += new System.EventHandler(this.cmd_Windows_OnlyGdiPlus_Click);
            // 
            // cmd_Windows_NoGdiPlus_NoWinForms
            // 
            this.cmd_Windows_NoGdiPlus_NoWinForms.Location = new System.Drawing.Point(25, 257);
            this.cmd_Windows_NoGdiPlus_NoWinForms.Name = "cmd_Windows_NoGdiPlus_NoWinForms";
            this.cmd_Windows_NoGdiPlus_NoWinForms.Size = new System.Drawing.Size(360, 57);
            this.cmd_Windows_NoGdiPlus_NoWinForms.TabIndex = 4;
            this.cmd_Windows_NoGdiPlus_NoWinForms.Text = "BuildMerge PixelFarm.One (Windows,NoGdiPlus, NoWinForms)";
            this.cmd_Windows_NoGdiPlus_NoWinForms.UseVisualStyleBackColor = true;
            this.cmd_Windows_NoGdiPlus_NoWinForms.Click += new System.EventHandler(this.cmd_Windows_NoGdiPlus_NoWinForms_Click);
            // 
            // cmd_Cross
            // 
            this.cmd_Cross.Location = new System.Drawing.Point(25, 476);
            this.cmd_Cross.Name = "cmd_Cross";
            this.cmd_Cross.Size = new System.Drawing.Size(360, 57);
            this.cmd_Cross.TabIndex = 5;
            this.cmd_Cross.Text = "BuildMerge PixelFarm.One (Cross Platform, NoWinForms)";
            this.cmd_Cross.UseVisualStyleBackColor = true;
            this.cmd_Cross.Click += new System.EventHandler(this.cmd_Cross_Click);
            // 
            // cmdCopyNativeLibs
            // 
            this.cmdCopyNativeLibs.Location = new System.Drawing.Point(317, 324);
            this.cmdCopyNativeLibs.Name = "cmdCopyNativeLibs";
            this.cmdCopyNativeLibs.Size = new System.Drawing.Size(184, 57);
            this.cmdCopyNativeLibs.TabIndex = 6;
            this.cmdCopyNativeLibs.Text = "Copy NativeLibs";
            this.cmdCopyNativeLibs.UseVisualStyleBackColor = true;
            this.cmdCopyNativeLibs.Click += new System.EventHandler(this.cmdCopyNativeLibs_Click);
            // 
            // cmdForTestWithHtmlRenderer
            // 
            this.cmdForTestWithHtmlRenderer.Location = new System.Drawing.Point(25, 334);
            this.cmdForTestWithHtmlRenderer.Name = "cmdForTestWithHtmlRenderer";
            this.cmdForTestWithHtmlRenderer.Size = new System.Drawing.Size(257, 57);
            this.cmdForTestWithHtmlRenderer.TabIndex = 7;
            this.cmdForTestWithHtmlRenderer.Text = "TEST BuildMerge PixelFarm.One.HtmlRenderer";
            this.cmdForTestWithHtmlRenderer.UseVisualStyleBackColor = true;
            this.cmdForTestWithHtmlRenderer.Click += new System.EventHandler(this.cmdForTestWithHtmlRenderer_Click);
            // 
            // cmdMinimalNetCore
            // 
            this.cmdMinimalNetCore.Location = new System.Drawing.Point(229, 90);
            this.cmdMinimalNetCore.Name = "cmdMinimalNetCore";
            this.cmdMinimalNetCore.Size = new System.Drawing.Size(188, 57);
            this.cmdMinimalNetCore.TabIndex = 9;
            this.cmdMinimalNetCore.Text = "BuildMerge for MinimumNetCore";
            this.cmdMinimalNetCore.UseVisualStyleBackColor = true;
            this.cmdMinimalNetCore.Click += new System.EventHandler(this.cmdMinimalNetCore_Click);
            // 
            // cmdBuild_PixelFarm_Drawing
            // 
            this.cmdBuild_PixelFarm_Drawing.Location = new System.Drawing.Point(25, 413);
            this.cmdBuild_PixelFarm_Drawing.Name = "cmdBuild_PixelFarm_Drawing";
            this.cmdBuild_PixelFarm_Drawing.Size = new System.Drawing.Size(167, 57);
            this.cmdBuild_PixelFarm_Drawing.TabIndex = 10;
            this.cmdBuild_PixelFarm_Drawing.Text = "Build PixelFarm.Drawing";
            this.cmdBuild_PixelFarm_Drawing.UseVisualStyleBackColor = true;
            this.cmdBuild_PixelFarm_Drawing.Click += new System.EventHandler(this.cmdBuild_PixelFarm_Drawing_Click);
            // 
            // cmdReadSln
            // 
            this.cmdReadSln.Location = new System.Drawing.Point(596, 1);
            this.cmdReadSln.Name = "cmdReadSln";
            this.cmdReadSln.Size = new System.Drawing.Size(102, 37);
            this.cmdReadSln.TabIndex = 11;
            this.cmdReadSln.Text = "Read Sln";
            this.cmdReadSln.UseVisualStyleBackColor = true;
            this.cmdReadSln.Click += new System.EventHandler(this.cmdReadSln_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(862, 44);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(367, 498);
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // lstPreset
            // 
            this.lstPreset.FormattingEnabled = true;
            this.lstPreset.Location = new System.Drawing.Point(423, 438);
            this.lstPreset.Name = "lstPreset";
            this.lstPreset.Size = new System.Drawing.Size(167, 82);
            this.lstPreset.TabIndex = 13;
            // 
            // listView2
            // 
            this.listView2.FullRowSelect = true;
            this.listView2.Location = new System.Drawing.Point(596, 44);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(260, 205);
            this.listView2.TabIndex = 14;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // lstAsmReferenceList
            // 
            this.lstAsmReferenceList.FormattingEnabled = true;
            this.lstAsmReferenceList.Location = new System.Drawing.Point(596, 269);
            this.lstAsmReferenceList.Name = "lstAsmReferenceList";
            this.lstAsmReferenceList.Size = new System.Drawing.Size(260, 251);
            this.lstAsmReferenceList.TabIndex = 15;
            // 
            // cmdBuildSelectedMergePro
            // 
            this.cmdBuildSelectedMergePro.Location = new System.Drawing.Point(473, 226);
            this.cmdBuildSelectedMergePro.Name = "cmdBuildSelectedMergePro";
            this.cmdBuildSelectedMergePro.Size = new System.Drawing.Size(102, 37);
            this.cmdBuildSelectedMergePro.TabIndex = 16;
            this.cmdBuildSelectedMergePro.Text = "Build Merge";
            this.cmdBuildSelectedMergePro.UseVisualStyleBackColor = true;
            this.cmdBuildSelectedMergePro.Click += new System.EventHandler(this.cmdBuildSelectedMergePro_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1241, 674);
            this.Controls.Add(this.cmdBuildSelectedMergePro);
            this.Controls.Add(this.lstAsmReferenceList);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.lstPreset);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.cmdReadSln);
            this.Controls.Add(this.cmdBuild_PixelFarm_Drawing);
            this.Controls.Add(this.cmdMinimalNetCore);
            this.Controls.Add(this.cmdForTestWithHtmlRenderer);
            this.Controls.Add(this.cmdCopyNativeLibs);
            this.Controls.Add(this.cmd_Cross);
            this.Controls.Add(this.cmd_Windows_NoGdiPlus_NoWinForms);
            this.Controls.Add(this.cmd_Windows_OnlyGdiPlus);
            this.Controls.Add(this.cmdBuildMergePixelFarmPortable);
            this.Controls.Add(this.cmdBuildMergePixelFarm);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdBuildMergePixelFarm;
        private System.Windows.Forms.Button cmdBuildMergePixelFarmPortable;
        private System.Windows.Forms.Button cmd_Windows_OnlyGdiPlus;
        private System.Windows.Forms.Button cmd_Windows_NoGdiPlus_NoWinForms;
        private System.Windows.Forms.Button cmd_Cross;
        private System.Windows.Forms.Button cmdCopyNativeLibs;
        private System.Windows.Forms.Button cmdForTestWithHtmlRenderer;
        private System.Windows.Forms.Button cmdMinimalNetCore;
        private System.Windows.Forms.Button cmdBuild_PixelFarm_Drawing;
        private System.Windows.Forms.Button cmdReadSln;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ListBox lstPreset;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ListBox lstAsmReferenceList;
        private System.Windows.Forms.Button cmdBuildSelectedMergePro;
    }
}

