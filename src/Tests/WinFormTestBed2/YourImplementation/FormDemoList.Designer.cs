namespace LayoutFarm.Dev
{
    partial class FormDemoList
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.chkShowLayoutInspector = new System.Windows.Forms.CheckBox();
            this.lstHtmlTestFiles = new System.Windows.Forms.ListBox();
            this.lstDemoList = new System.Windows.Forms.ListBox();
            this._samplesTreeView = new System.Windows.Forms.TreeView();
            this.chkShowFormPrint = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lstPlatformSelectors = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chkShowLayoutInspector
            // 
            this.chkShowLayoutInspector.AutoSize = true;
            this.chkShowLayoutInspector.Location = new System.Drawing.Point(12, 5);
            this.chkShowLayoutInspector.Name = "chkShowLayoutInspector";
            this.chkShowLayoutInspector.Size = new System.Drawing.Size(153, 17);
            this.chkShowLayoutInspector.TabIndex = 5;
            this.chkShowLayoutInspector.Text = "Also show LayoutInspector";
            this.chkShowLayoutInspector.UseVisualStyleBackColor = true;
            this.chkShowLayoutInspector.CheckedChanged += new System.EventHandler(this.chkShowLayoutInspector_CheckedChanged);
            // 
            // lstHtmlTestFiles
            // 
            this.lstHtmlTestFiles.Location = new System.Drawing.Point(558, 448);
            this.lstHtmlTestFiles.Name = "lstHtmlTestFiles";
            this.lstHtmlTestFiles.Size = new System.Drawing.Size(261, 69);
            this.lstHtmlTestFiles.TabIndex = 11;
            // 
            // lstDemoList
            // 
            this.lstDemoList.FormattingEnabled = true;
            this.lstDemoList.Location = new System.Drawing.Point(136, 43);
            this.lstDemoList.Name = "lstDemoList";
            this.lstDemoList.Size = new System.Drawing.Size(416, 472);
            this.lstDemoList.TabIndex = 15;
            // 
            // _samplesTreeView
            // 
            this._samplesTreeView.Location = new System.Drawing.Point(558, 43);
            this._samplesTreeView.Name = "_samplesTreeView";
            this._samplesTreeView.Size = new System.Drawing.Size(261, 399);
            this._samplesTreeView.TabIndex = 17;
            // 
            // chkShowFormPrint
            // 
            this.chkShowFormPrint.AutoSize = true;
            this.chkShowFormPrint.Location = new System.Drawing.Point(180, 5);
            this.chkShowFormPrint.Name = "chkShowFormPrint";
            this.chkShowFormPrint.Size = new System.Drawing.Size(100, 17);
            this.chkShowFormPrint.TabIndex = 18;
            this.chkShowFormPrint.Text = "Show FormPrint";
            this.chkShowFormPrint.UseVisualStyleBackColor = true;
            this.chkShowFormPrint.CheckedChanged += new System.EventHandler(this.chkShowFormPrint_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(587, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 35);
            this.button1.TabIndex = 21;
            this.button1.Text = "Save To Image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lstPlatformSelectors
            // 
            this.lstPlatformSelectors.FormattingEnabled = true;
            this.lstPlatformSelectors.Location = new System.Drawing.Point(12, 43);
            this.lstPlatformSelectors.Name = "lstPlatformSelectors";
            this.lstPlatformSelectors.Size = new System.Drawing.Size(115, 173);
            this.lstPlatformSelectors.TabIndex = 22;
            // 
            // FormDemoList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 529);
            this.Controls.Add(this.lstPlatformSelectors);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkShowFormPrint);
            this.Controls.Add(this._samplesTreeView);
            this.Controls.Add(this.lstDemoList);
            this.Controls.Add(this.lstHtmlTestFiles);
            this.Controls.Add(this.chkShowLayoutInspector);
            this.Name = "FormDemoList";
            this.Text = "TestGraphicPackage2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private System.Windows.Forms.CheckBox chkShowLayoutInspector;
        private System.Windows.Forms.ListBox lstHtmlTestFiles;
        private System.Windows.Forms.ListBox lstDemoList;
        private System.Windows.Forms.TreeView _samplesTreeView;
        private System.Windows.Forms.CheckBox chkShowFormPrint;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox lstPlatformSelectors;
    }
}

