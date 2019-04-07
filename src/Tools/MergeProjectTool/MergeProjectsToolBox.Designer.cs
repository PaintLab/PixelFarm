namespace BuildMergeProject
{
    partial class MergeProjectsToolBox
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
            this.listView2 = new System.Windows.Forms.ListView();
            this.lstAsmReferenceList = new System.Windows.Forms.ListBox();
            this.lstPreset = new System.Windows.Forms.ListBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.cmbNetStd = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView2
            // 
            this.listView2.FullRowSelect = true;
            this.listView2.Location = new System.Drawing.Point(3, 3);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(606, 205);
            this.listView2.TabIndex = 23;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // lstAsmReferenceList
            // 
            this.lstAsmReferenceList.FormattingEnabled = true;
            this.lstAsmReferenceList.Location = new System.Drawing.Point(3, 214);
            this.lstAsmReferenceList.Name = "lstAsmReferenceList";
            this.lstAsmReferenceList.Size = new System.Drawing.Size(606, 251);
            this.lstAsmReferenceList.TabIndex = 21;
            // 
            // lstPreset
            // 
            this.lstPreset.FormattingEnabled = true;
            this.lstPreset.Location = new System.Drawing.Point(3, 471);
            this.lstPreset.Name = "lstPreset";
            this.lstPreset.Size = new System.Drawing.Size(167, 82);
            this.lstPreset.TabIndex = 20;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(615, 10);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(367, 498);
            this.listView1.TabIndex = 19;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // cmbNetStd
            // 
            this.cmbNetStd.FormattingEnabled = true;
            this.cmbNetStd.Location = new System.Drawing.Point(268, 487);
            this.cmbNetStd.Name = "cmbNetStd";
            this.cmbNetStd.Size = new System.Drawing.Size(194, 21);
            this.cmbNetStd.TabIndex = 25;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(175, 485);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 37);
            this.button1.TabIndex = 27;
            this.button1.Text = "Build Merge";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.cmdBuildMerge_Click);
            // 
            // MergeProjectsToolBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbNetStd);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.lstAsmReferenceList);
            this.Controls.Add(this.lstPreset);
            this.Controls.Add(this.listView1);
            this.Name = "MergeProjectsToolBox";
            this.Size = new System.Drawing.Size(1003, 567);
            this.ResumeLayout(false);

        }


        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ListBox lstAsmReferenceList;
        private System.Windows.Forms.ListBox lstPreset;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ComboBox cmbNetStd;
        private System.Windows.Forms.Button button1;
    }
}
