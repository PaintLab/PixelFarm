namespace Mini
{
    partial class FormGdiTest
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
            this.cmbPixelTools = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmbPixelTools
            // 
            this.cmbPixelTools.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPixelTools.FormattingEnabled = true;
            this.cmbPixelTools.Location = new System.Drawing.Point(13, 13);
            this.cmbPixelTools.Name = "cmbPixelTools";
            this.cmbPixelTools.Size = new System.Drawing.Size(168, 21);
            this.cmbPixelTools.TabIndex = 0;
            // 
            // FormGdiTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 623);
            this.Controls.Add(this.cmbPixelTools);
            this.Name = "FormGdiTest";
            this.Text = "FormGdiTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPixelTools;
    }
}