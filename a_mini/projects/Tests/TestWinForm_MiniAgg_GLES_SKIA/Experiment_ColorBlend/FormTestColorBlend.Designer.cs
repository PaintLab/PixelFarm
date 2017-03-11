namespace Mini
{
    partial class FormTestColorBlend
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
            this.colorCompoBox1 = new Mini.ColorCompoBox();
            this.SuspendLayout();
            // 
            // colorCompoBox1
            // 
            this.colorCompoBox1.Location = new System.Drawing.Point(12, 12);
            this.colorCompoBox1.Name = "colorCompoBox1";
            this.colorCompoBox1.Size = new System.Drawing.Size(387, 277);
            this.colorCompoBox1.TabIndex = 0;
            // 
            // FormTestColorBlend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 467);
            this.Controls.Add(this.colorCompoBox1);
            this.Name = "FormTestColorBlend";
            this.Text = "FormTestColorBlend";
            this.Load += new System.EventHandler(this.FormTestColorBlend_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ColorCompoBox colorCompoBox1;
    }
}